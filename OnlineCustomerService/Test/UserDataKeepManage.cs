using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlashPay_CardManagement.Filters
{
    public interface UserDataKeepManage<T>
    {
        T data { set; get; }

        String UserDataEncrypt();

        void UserDataDecrypt(String key);
    }
}
