using DAO.Request;
using DAO.Shared;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;

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

        public void Delete(BaseModel<String> model, DialogueRecordCacheRequest request)
        {
            if (redisConnection == null)
            {
                return;
            }

            IDatabase database = redisConnection.GetDatabase();

            database.KeyDelete(request.Id);

            model.Success = true;
        }

        public void Get(BaseModel<List<String>> model, DialogueRecordCacheRequest request)
        {
            if (redisConnection == null)
            {
                return;
            }

            IDatabase database = redisConnection.GetDatabase();

            Int64 len = database.ListLength(request.Id);

            List<String> result = new List<String>();

            for (Int32 i = 0; i < len; i++)
            {
                result.Add(database.ListGetByIndex(request.Id, i));
            }

            model.Success = true;
            model.Result = result;
        }
    }
}
