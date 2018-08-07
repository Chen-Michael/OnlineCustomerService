using DAO.Request;
using DAO.Shared;
using System;
using System.Collections.Generic;

namespace DAO
{
    public interface IDialogueRecordCacheDAO
    {
        void Add(BaseModel<String> model, DialogueRecordCacheRequest request);
        void Delete(BaseModel<String> model, DialogueRecordCacheRequest request);
        void Get(BaseModel<List<String>> model, DialogueRecordCacheRequest request);
    }
}
