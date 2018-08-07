using DAO.Shared;
using System;

namespace OnlineCustomerService.ViewModels
{
    public class AdminViewModel<T> : BaseModel<T>
    {
        public String Account { get; set; }
        public String Password { get; set; }
    }
}
