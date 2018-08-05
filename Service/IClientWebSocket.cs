using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Service
{
    public interface IClientWebSocket
    {
        WebSocket WebSocket { set; get; }
        Task Start();
    }
}
