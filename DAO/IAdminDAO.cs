using DAO.Request;
using DAO.Response;
using DAO.Shared;
using System;
using System.Collections.Generic;

namespace DAO
{
    public interface IAdminDAO
    {
        void Get(BaseModel<List<AdminResponse>> model, AdminRequest request);
    }
}
