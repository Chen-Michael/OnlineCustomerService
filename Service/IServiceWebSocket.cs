using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Service
{
    public interface IServiceWebSocket
    {
        Int32 AdminId { set; get; }
        String WebSocketId { set; get; }
        String WebSocketClientId { set; get; }
        WebSocket WebSocket { set; get; }
        void Reset();
        Task Start();
    }
}
