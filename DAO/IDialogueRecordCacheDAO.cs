using DAO.Request;
using DAO.Shared;
using System;

namespace DAO
{
    public interface IDialogueRecordCacheDAO
    {
        void Add(BaseModel<String> model, DialogueRecordCacheRequest request);
    }
}
