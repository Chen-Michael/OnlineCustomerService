using DAO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using OnlineCustomerService.Filters;
using OnlineCustomerService.ViewModels;
using Service;
using System;
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

        public async Task Invoke(HttpContext context, UserDataKeepManage<UserDataKeepViewModel> manage)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                return;
            }

            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();

            if ("/service".Equals(context.Request.Path))
            {
                String token = context.Request.Query["token"];
                manage.UserDataDecrypt(token ?? "");
                if (manage.data != null)
                {
                    IServiceWebSocket serviceWebSocket = (IServiceWebSocket)context.RequestServices.GetService(typeof(IServiceWebSocket));
                    serviceWebSocket.WebSocket = webSocket;
                    serviceWebSocket.AdminId = manage.data.Id;

                    await serviceWebSocket.Start();
                }
            }
            else if ("/client".Equals(context.Request.Path))
            {
                String name = context.Request.Query["name"];
                String mail = context.Request.Query["mail"];
                String phone = context.Request.Query["phone"];
                String question = context.Request.Query["question"];

                IClientWebSocket clientWebSocket = (IClientWebSocket)context.RequestServices.GetService(typeof(IClientWebSocket));
                clientWebSocket.WebSocket = webSocket;
                clientWebSocket.Name = name;
                clientWebSocket.Mail = mail;
                clientWebSocket.Phone = phone;
                clientWebSocket.Question = question;

                await clientWebSocket.Start();
            }
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
