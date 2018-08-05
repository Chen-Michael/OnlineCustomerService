using DAO.Request;
using DAO.Response;
using DAO.Shared;
using System;
using System.Collections.Generic;

namespace DAO
{
    public interface IDialogueRecordDAO
    {
        void Add(BaseModel<String> model, DialogueRecordRequest request);
        void Get(BaseModel<List<DialogueRecordResponse>> model, DialogueRecordRequest request);
        void Detail(BaseModel<DialogueRecordResponse> model, DialogueRecordRequest request);
    }
}
