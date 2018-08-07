using System;
using System.Collections.Generic;
using DAO;
using Service.Request;
using Service.Response;
using Service.Shared;
using Utils.Encryption;

namespace Service.Impl
{
    public class AdminServiceImpl : IAdminService
    {
        private IAdminDAO adminDAO;

        public AdminServiceImpl(IAdminDAO adminDAO, SHA SHA)
        {
            this.adminDAO = adminDAO;
        }

        public AdminResponse Mapper(DAO.Response.AdminResponse model)
        {
            AdminResponse response = new AdminResponse();
            response.Id = model.Id;
            response.Account = model.Account;
            response.Name = model.Name;
            response.CreateTime = model.CreateTime;

            return response;
        }

        public void Login(BaseModel<AdminResponse> model, AdminRequest request)
        {
            DAO.Request.AdminRequest request2 = new DAO.Request.AdminRequest();
            request2.Account = request.Account;

            String password = SHA.SHA1(request.Password);

            DAO.Shared.BaseModel<List<DAO.Response.AdminResponse>> model2 = new DAO.Shared.BaseModel<List<DAO.Response.AdminResponse>>();

            /* 獲取資料 */
            adminDAO.Get(model2, request2);

            if (!model2.Success || model2.Result.Count != 1 || !password.Equals(model2.Result[0].Password))
            {
                return;
            }

            model.Success = true;
            model.Result = Mapper(model2.Result[0]);
        }
    }
}
