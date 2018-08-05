using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using DAO;
using DAO.Impl;
using EF;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineCustomerService.Middleware;
using Service;
using Service.Impl;
using StackExchange.Redis;

namespace OnlineCustomerService
{
    public class Startup
    {
        private readonly IConfiguration config;

        public Startup(IConfiguration config)
        {
            this.config = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            
            /* 相關 DB 物件 */
            services.AddDbContext<DbContext, OnlineCustomerServiceContext>(options => options.UseSqlServer(config.GetConnectionString("MainDatabase")));
            services.AddScoped(_ => {
                return ConnectionMultiplexer.Connect(config.GetConnectionString("MainRedis"));
            });

            /* 相關 DAO 物件 */
            services.AddScoped<IAdminDAO, AdminDAOImpl>();
            services.AddScoped<IDialogueRecordDAO, DialogueRecordDAOImpl>();
            services.AddScoped<IDialogueRecordCacheDAO, DialogueRecordCacheDAOImpl>();

            /* 相關 Service 物件 */
            services.AddScoped<IClientWebSocket, ClientWebSocketImpl>();
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
            app.UseWebSockets();
            app.UseWebSocketNotify();

            //app.Use(async (context, next) =>
            //{
            //    if (context.Request.Path == "/ws")
            //    {
            //        if (context.WebSockets.IsWebSocketRequest)
            //        {
            //            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
            //            await Echo(context, webSocket);
            //        }
            //        else
            //        {
            //            context.Response.StatusCode = 400;
            //        }
            //    }
            //    else
            //    {
            //        await next();
            //    }

            //});
        }

        private async Task Echo(HttpContext context, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            var acceptStr = System.Text.Encoding.UTF8.GetString(buffer).Trim(char.MinValue);

            while (!result.CloseStatus.HasValue)
            {
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }
}
