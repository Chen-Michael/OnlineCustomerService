using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EF;
using DAO.Shared;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using Utils.Expansion;
using Microsoft.EntityFrameworkCore;
using DAO;
using Service;
using Service.Request;
using Service.Response;

namespace OnlineCustomerService.Controllers
{
    public class HomeController : Controller
    {
        private readonly OnlineCustomerServiceContext context;
        private readonly ConnectionMultiplexer connection;
        private readonly IAdminDAO adminDAO;
        private readonly IAdminService adminService;

        public HomeController(DbContext context, ConnectionMultiplexer connection, IAdminDAO adminDAO, IAdminService adminService)
        {
            this.context = (OnlineCustomerServiceContext)context;
            this.connection = connection;
            this.adminDAO = adminDAO;
            this.adminService = adminService;
        }

        public JsonResult Index()
        {
            //var redisDatabase = connection.GetDatabase();

            //redisDatabase.StringSet("kkk", "12345679++");

            //return Json(context.Admin.Pagination(1,10).ToList());

            AdminRequest request = new AdminRequest();
            request.Account = "admin01";
            request.Password = "000";
            Service.Shared.BaseModel<AdminResponse> model = new Service.Shared.BaseModel<AdminResponse>();

            //adminService.Login(model, request);

            return Json(model);
        }
    }
}