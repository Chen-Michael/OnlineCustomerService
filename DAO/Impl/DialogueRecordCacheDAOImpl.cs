using DAO.Request;
using DAO.Shared;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;

namespace DAO.Impl
{
    public class DialogueRecordCacheDAOImpl : BaseDAO, IDialogueRecordCacheDAO
    {
        public DialogueRecordCacheDAOImpl(ConnectionMultiplexer redisConnection) : base(redisConnection)
        {

        }

        public void Add(BaseModel<String> model, DialogueRecordCacheRequest request)
        {
            if (redisConnection == null)
            {
                return;
            }

            IDatabase database = redisConnection.GetDatabase();
            database.ListRightPush(request.Id, JsonConvert.SerializeObject(request));

            model.Success = true;
        }
    }
}
