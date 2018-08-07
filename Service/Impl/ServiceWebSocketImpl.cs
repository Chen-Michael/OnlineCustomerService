using DAO;
using DAO.Request;
using DAO.Shared;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Service.Impl
{
    public class ServiceWebSocketImpl : IServiceWebSocket
    {
        public Int32 AdminId { set; get; }
        public String WebSocketId { set; get; }
        public String WebSocketClientId { set; get; }
        public WebSocket WebSocket { set; get; }

        private readonly IDialogueRecordCacheDAO dialogueRecordCacheDAO;
        private readonly IDialogueRecordDAO dialogueRecordDAO;
        private readonly IConnection mqConnection;
        private readonly IWebSocketManage webSocketManage;

        private IModel channel;
        private Dictionary<String, String> data;
        private Boolean work;
        private String clientQuestion;
        private String clientName;
        private String clientMail;
        private String clientPhone;

        public ServiceWebSocketImpl(IDialogueRecordCacheDAO dialogueRecordCacheDAO, IDialogueRecordDAO dialogueRecordDAO, IConnection mqConnection, IWebSocketManage webSocketManage)
        {
            this.dialogueRecordCacheDAO = dialogueRecordCacheDAO;
            this.dialogueRecordDAO = dialogueRecordDAO;
            this.mqConnection = mqConnection;
            this.webSocketManage = webSocketManage;

            WebSocketId = Guid.NewGuid().ToString();
            work = false;
        }

        public void Init()
        {
            /* 向管理器註冊本體 */
            if (WebSocket != null)
            {
                while(!webSocketManage.RegisterService(WebSocketId, this)){
                    WebSocketId = Guid.NewGuid().ToString();
                }
            }
            
            /* 初始化 Message Queue */
            channel = mqConnection.CreateModel();

            channel.QueueDeclare(
                queue: "task_queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                /* 如果有在服務用戶，拒絕此次要求 */
                if (work)
                {
                    channel.BasicReject(8, true);
                    return;
                }

                String body = Encoding.UTF8.GetString(ea.Body);

                try
                {
                    data = JsonConvert.DeserializeObject<Dictionary<String, String>>(body);

                    WebSocketClientId = data["websocketId"];
                    clientQuestion = data["question"];
                    clientName = data["name"];
                    clientMail = data["mail"];
                    clientPhone = data["phone"];

                    if (!webSocketManage.SetWebSocketServiceId(WebSocketClientId, WebSocketId))
                    {
                        return;
                    }

                    webSocketManage.SendToService(WebSocketId, body);

                    webSocketManage.SendToClient(WebSocketClientId, JsonConvert.SerializeObject(new { Start = true }));

                    AddDialogue(body);

                    work = true;
                }
                catch (Exception)
                {

                }
                finally
                {
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
            };

            channel.BasicConsume(queue: "task_queue", autoAck: false, consumer: consumer);
        }

        public void Reset()
        {
            webSocketManage.SendToService(WebSocketId, JsonConvert.SerializeObject(new { End = true }));
            SaveDialogue();
            work = false;
        }

        private void AddDialogue(String message)
        {
            try
            {
                BaseModel<String> model = new BaseModel<String>();

                DialogueRecordCacheRequest request = new DialogueRecordCacheRequest();
                request.Id = WebSocketClientId;
                request.Value = message;
                request.CreateTime = DateTime.Now;

                dialogueRecordCacheDAO.Add(model, request);
            }
            catch (Exception)
            {
                
            }
        }

        private void SaveDialogue()
        {
            if (!work)
            {
                return;
            }

            /* 取出快取對話紀錄 */
            BaseModel<List<String>> model = new BaseModel<List<String>>();

            DialogueRecordCacheRequest request = new DialogueRecordCacheRequest();
            request.Id = WebSocketClientId;

            dialogueRecordCacheDAO.Get(model, request);

            if (!model.Success)
            {
                return;
            }

            /* 保存對話至資料庫 */
            BaseModel<String> model2 = new BaseModel<String>();

            DialogueRecordRequest request2 = new DialogueRecordRequest();
            request2.AdminId = AdminId;
            request2.UserName = clientName;
            request2.Mail = clientMail;
            request2.Phone = clientPhone;
            request2.Question = clientQuestion;
            request2.Record = JsonConvert.SerializeObject(model.Result);

            dialogueRecordDAO.Add(model2, request2);

            if (!model2.Success)
            {
                return;
            }

            /* 刪除快取對話紀錄 */
            BaseModel<String> model3 = new BaseModel<String>();

            dialogueRecordCacheDAO.Delete(model3, request);

            if (!model3.Success)
            {
                return;
            }
        }

        public async Task Start()
        {
            try
            {
                Init();

                using (mqConnection)
                using (channel)
                {
                    ArraySegment<Byte> buffer = new ArraySegment<byte>(new Byte[8192]);
                    
                    WebSocketReceiveResult result = await WebSocket.ReceiveAsync(buffer, CancellationToken.None);
                    
                    while (!result.CloseStatus.HasValue)
                    {
                        /* 接收訊息 */
                        using (MemoryStream ms = new MemoryStream())
                        {
                            ms.Write(buffer.Array, buffer.Offset, result.Count);

                            while (!result.EndOfMessage)
                            {
                                ms.Write(buffer.Array, buffer.Offset, result.Count);

                                result = await WebSocket.ReceiveAsync(buffer, CancellationToken.None);
                            }

                            ms.Seek(0, SeekOrigin.Begin);

                            using (StreamReader reader = new StreamReader(ms, Encoding.UTF8))
                            {
                                String message = reader.ReadToEnd();

                                webSocketManage.SendToClient(WebSocketClientId, message);

                                AddDialogue(message);
                            }
                        }

                        result = await WebSocket.ReceiveAsync(buffer, CancellationToken.None);
                    }

                    await WebSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                }
            }
            catch (Exception)
            {
                mqConnection.Dispose();
                channel.Dispose();
            }

            /* 向管理器反註冊本體 */
            webSocketManage.UnRegisterService(WebSocketId);

            /* 保存資料 */
            SaveDialogue();
        }
    }
}
