using DAO;
using DAO.Request;
using DAO.Shared;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Service.Impl
{
    public class ClientWebSocketImpl : IClientWebSocket
    {
        public String Name { set; get; }
        public String Mail { set; get; }
        public String Phone { set; get; }
        public String Question { set; get; }

        public String WebSocketId { set; get; }
        public String WebSocketServiceId { set; get; }
        public WebSocket WebSocket { set; get; }

        private readonly IDialogueRecordCacheDAO dialogueRecordCacheDAO;
        private readonly IConnection mqConnection;
        private readonly IWebSocketManage webSocketManage;

        private IModel channel;

        public ClientWebSocketImpl(IDialogueRecordCacheDAO dialogueRecordCacheDAO, IConnection mqConnection, IWebSocketManage webSocketManage)
        {
            this.dialogueRecordCacheDAO = dialogueRecordCacheDAO;
            this.mqConnection = mqConnection;
            this.webSocketManage = webSocketManage;

            WebSocketId = Guid.NewGuid().ToString();
        }

        public void Init()
        {
            /* 向管理器註冊本體 */
            if (WebSocket != null)
            {
                while (!webSocketManage.RegisterClient(WebSocketId, this))
                {
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

            IBasicProperties properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            String message = JsonConvert.SerializeObject(new {
                name = Name,
                mail = Mail,
                phone = Phone,
                question = Question,
                websocketId = WebSocketId
            });

            channel.BasicPublish(
                exchange: "",
                routingKey: "task_queue",
                basicProperties: properties,
                body: Encoding.UTF8.GetBytes(message)
            );
        }

        public void Reset()
        {
            webSocketManage.SendToClient(WebSocketId, JsonConvert.SerializeObject(new { End = true }));
            WebSocketServiceId = "";
        }

        private void AddDialogue(String message)
        {
            try
            {
                BaseModel<String> model = new BaseModel<String>();

                DialogueRecordCacheRequest request = new DialogueRecordCacheRequest();
                request.Id = WebSocketId;
                request.Value = message;
                request.CreateTime = DateTime.Now;

                dialogueRecordCacheDAO.Add(model, request);
            }
            catch (Exception)
            {

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
                        if (!String.IsNullOrWhiteSpace(WebSocketServiceId))
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

                                    webSocketManage.SendToService(WebSocketServiceId, message);

                                    AddDialogue(message);
                                }
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
            webSocketManage.UnRegisterClient(WebSocketId);
        }
    }
}
