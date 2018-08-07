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
using OnlineCustomerService.Filters;
using OnlineCustomerService.Filters.impl;
using OnlineCustomerService.Middleware;
using OnlineCustomerService.ViewModels;
using RabbitMQ.Client;
using Service;
using Service.Impl;
using StackExchange.Redis;
using Utils.Encryption;

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
            services.AddMvc(options =>
            {
                //options.Filters.Add(new LoginFilter());
            });

            /* 相關 DB 物件 */
            services.AddDbContext<DbContext, OnlineCustomerServiceContext>(options => options.UseSqlServer(config.GetConnectionString("MainDatabase")));
            services.AddScoped(_ => {
                return ConnectionMultiplexer.Connect(config.GetConnectionString("MainRedis"));
            });

            /* 相關 MQ 物件 */
            ConnectionFactory factory = new ConnectionFactory() { HostName = config.GetConnectionString("MainMQ") };
            services.AddScoped(_ => {
                return factory.CreateConnection();
            });

            /* 相關 JWT 物件 */
            String key = config.GetValue<String>("JWT-Key");
            services.AddScoped<UserDataKeepManage<UserDataKeepViewModel>>(_ => {
                return new UserDataKeepManageImpl<UserDataKeepViewModel>(key);
            });

            /* 相關 WebSocket 物件 */
            services.AddSingleton<IWebSocketManage, WebSocketManageImpl>();
            services.AddScoped<IServiceWebSocket, ServiceWebSocketImpl>();
            services.AddScoped<IClientWebSocket, ClientWebSocketImpl>();

            /* 相關 DAO 物件 */
            services.AddScoped<IAdminDAO, AdminDAOImpl>();
            services.AddScoped<IDialogueRecordDAO, DialogueRecordDAOImpl>();
            services.AddScoped<IDialogueRecordCacheDAO, DialogueRecordCacheDAOImpl>();

            /* 相關 Service 物件 */
            services.AddScoped<IAdminService, AdminServiceImpl>();

            /* 相關 Utils 物件 */
            services.AddScoped<SHA>();
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
        }
    }
}
