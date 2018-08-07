using System;
using Microsoft.AspNetCore.Mvc;
using OnlineCustomerService.Filters;
using OnlineCustomerService.ViewModels;
using Service;
using Service.Request;
using Service.Response;
using Service.Shared;

namespace OnlineCustomerService.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminService adminService;
        private readonly UserDataKeepManage<UserDataKeepViewModel> manage;

        public AdminController(IAdminService adminService, UserDataKeepManage<UserDataKeepViewModel> manage)
        {
            this.adminService = adminService;
            this.manage = manage;
        }

        public JsonResult Login([FromBody] AdminViewModel<String> data)
        {
            BaseModel<AdminResponse> model = new BaseModel<AdminResponse>();
            AdminRequest request = new AdminRequest();
            request.Account = data.Account;
            request.Password = data.Password;

            adminService.Login(model, request);

            if (model.Success)
            {
                UserDataKeepViewModel userData = new UserDataKeepViewModel();
                userData.Id = model.Result.Id;
                userData.Name = model.Result.Name;

                manage.data = userData;

                data.Success = true;
                data.Result = manage.UserDataEncrypt();
            }

            return Json(data);
        }
    }
}