using Service.Request;
using Service.Response;
using Service.Shared;

namespace Service
{
    public interface IAdminService
    {
        void Login(BaseModel<AdminResponse> model, AdminRequest request);
    }
}
