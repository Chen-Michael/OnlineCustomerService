using DAO.Request;
using DAO.Response;
using DAO.Shared;
using EF;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils.Expansion;

namespace DAO.Impl
{
    public class AdminDAOImpl : BaseDAO, IAdminDAO
    {
        public AdminDAOImpl(DbContext context) : base(context)
        {

        }

        public AdminResponse Mapper(Admin admin)
        {
            AdminResponse response = new AdminResponse();
            response.Id = admin.Id;
            response.Account = admin.Account;
            response.Password = admin.Password;
            response.Name = admin.Name;
            response.CreateTime = admin.CreateTime;

            return response;
        }

        public void Get(BaseModel<List<AdminResponse>> model, AdminRequest request)
        {
            if (onlineCustomerServiceContext == null)
            {
                return;
            }

            IQueryable<Admin> queryable = onlineCustomerServiceContext.Admin.Where(
                admin => 
                    (String.IsNullOrWhiteSpace(request.Account) || admin.Account.Equals(request.Account))
            );

            Int32 pageCount = queryable.Count();
            List<AdminResponse> result = new List<AdminResponse>();
            foreach (Admin admin in queryable.Pagination(model.PageIndex, model.PageSize))
            {
                result.Add(Mapper(admin));
            }

            model.Success = true;
            model.PageCount = pageCount;
            model.Result = result;
        }
    }
}
