using FlashPay_CardManagement.ViewModels.Shared;
using FlashPay_CardManagement.ViewModels.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace FlashPay_CardManagement.Filters
{
    public class LoginFilter : IAuthorizationFilter
    {
        private readonly UserDataKeepManage<UserDataKeepViewModel> manage;

        public LoginFilter(UserDataKeepManage<UserDataKeepViewModel> manage)
        {
            this.manage = manage;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            /*
             尚未填入狀態碼
             錯誤訊息先寫死，後面需改成使用函式取得
             */

            BaseViewModel<String> model = new BaseViewModel<String>();
            model.Message = "No login system";
            try
            {
                ControllerActionDescriptor controller = (ControllerActionDescriptor)context.ActionDescriptor;
                String ControllerName = controller.ControllerName;
                if ("login".Equals(ControllerName.ToLower()))
                {
                    return;
                }

                String token = context.HttpContext.Request.Headers["x-token"];
                if (token != null)
                {
                    manage.UserDataDecrypt(token);
                    /* 若要實現特定時間自動登出，可加入最後呼叫時間來判斷 */
                    if (manage.data != null)
                    {
                        return;
                    }
                }

                context.Result = new JsonResult(model);
            }
            catch (Exception)
            {
                model.Message = "An error occurred";

                context.HttpContext.Response.StatusCode = 404;
                context.Result = new JsonResult(model);
            }
        }
    }
}
