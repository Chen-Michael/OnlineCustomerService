using DAO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Service;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineCustomerService.Middleware
{
    /// <summary>
    /// WebSocket攔截器
    /// </summary>
    public class WebSocketMiddleware
    {
        private readonly RequestDelegate next;

        public WebSocketMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                return;
            }

            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();

            IClientWebSocket clientWebSocket = (IClientWebSocket)context.RequestServices.GetService(typeof(IClientWebSocket));
            clientWebSocket.WebSocket = webSocket;
            await clientWebSocket.Start();
        }
    }

    public static class WebSocketMiddlewareExtensions
    {  
        public static IApplicationBuilder UseWebSocketNotify(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<WebSocketMiddleware>();
        }
    }
}
