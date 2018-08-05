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

namespace OnlineCustomerService.Controllers
{
    public class HomeController : Controller
    {
        private readonly OnlineCustomerServiceContext context;
        private readonly ConnectionMultiplexer connection;
        private readonly IAdminDAO adminDAO;

        public HomeController(DbContext context, ConnectionMultiplexer connection, IAdminDAO adminDAO)
        {
            this.context = (OnlineCustomerServiceContext)context;
            this.connection = connection;
            this.adminDAO = adminDAO;
        }

        public JsonResult Index()
        {
            var redisDatabase = connection.GetDatabase();

            redisDatabase.StringSet("kkk", "12345679++");

            return Json(context.Admin.Pagination(1,10).ToList());
        }
    }
}