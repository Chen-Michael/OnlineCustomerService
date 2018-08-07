using System;

namespace OnlineCustomerService.Filters
{
    public interface UserDataKeepManage<T>
    {
        T data { set; get; }

        String UserDataEncrypt();

        void UserDataDecrypt(String token);
    }
}
