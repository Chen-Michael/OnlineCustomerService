using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Newtonsoft.Json;
using System;

namespace FlashPay_CardManagement.Filters.impl
{
    public class UserDataKeepManageImpl<T> : UserDataKeepManage<T>
    {
        public T data { set; get; }
        private String key;
        private IJsonSerializer serializer = new JsonNetSerializer();
        private IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();

        public UserDataKeepManageImpl(String key)
        {
            this.key = key;
        }

        public void UserDataDecrypt(String token)
        {
            IDateTimeProvider provider = new UtcDateTimeProvider();
            IJwtValidator validator = new JwtValidator(serializer, provider);
            IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);
            data = JsonConvert.DeserializeObject<T>(decoder.Decode(token, key, verify: true));
        }

        public String UserDataEncrypt()
        {
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
            var token = encoder.Encode(data, key);
            return token;
        }
    }
}
