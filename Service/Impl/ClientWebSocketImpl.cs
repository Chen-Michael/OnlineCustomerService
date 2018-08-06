using DAO;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Service.Impl
{
    public class ClientWebSocketImpl : IClientWebSocket
    {
        public WebSocket WebSocket { set; get; }

        private readonly IDialogueRecordCacheDAO dialogueRecordCacheDAO;

        public ClientWebSocketImpl(IDialogueRecordCacheDAO dialogueRecordCacheDAO)
        {
            this.dialogueRecordCacheDAO = dialogueRecordCacheDAO;
        }

        public async Task Start()
        {
            var buffer = new Byte[1024 * 4];
            WebSocketReceiveResult result = await WebSocket.ReceiveAsync(new ArraySegment<Byte>(buffer), CancellationToken.None);

            //var acceptStr = System.Text.Encoding.UTF8.GetString(buffer).Trim(char.MinValue);

            while (!result.CloseStatus.HasValue)
            {
                await WebSocket.SendAsync(new ArraySegment<Byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);

                result = await WebSocket.ReceiveAsync(new ArraySegment<Byte>(buffer), CancellationToken.None);
            }

            await WebSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }
}
