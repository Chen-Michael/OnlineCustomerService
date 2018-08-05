using System;
using System.Collections.Generic;
using System.Linq;
using DAO.Request;
using DAO.Response;
using DAO.Shared;
using EF;
using Microsoft.EntityFrameworkCore;
using Utils.Expansion;

namespace DAO.Impl
{
    public class DialogueRecordDAOImpl : BaseDAO, IDialogueRecordDAO
    {
        public DialogueRecordDAOImpl(DbContext context) : base(context)
        {

        }

        private DialogueRecord Mapper(DialogueRecordRequest request)
        {
            DialogueRecord model = new DialogueRecord();
            model.Id = request.Id;
            model.AdminId = request.AdminId;
            model.UserId = request.UserId;
            model.UserName = request.UserName;
            model.Mail = request.Mail;
            model.Phone = request.Phone;
            model.Question = request.Question;
            model.Record = request.Record;

            return model;
        }

        private DialogueRecordResponse Mapper(DialogueRecord model)
        {
            if (model == null)
            {
                return null;
            }

            DialogueRecordResponse response = new DialogueRecordResponse();
            response.Id = model.Id;
            response.AdminId = model.AdminId;
            response.UserId = model.UserId;
            response.UserName = model.UserName;
            response.Mail = model.Mail;
            response.Phone = model.Phone;
            response.Question = model.Question;
            response.Record = model.Record;
            response.CreateTime = model.CreateTime;

            return response;
        }

        public void Add(BaseModel<String> model, DialogueRecordRequest request)
        {
            if (onlineCustomerServiceContext == null)
            {
                return;
            }

            DialogueRecord dialogueRecord = Mapper(request);

            onlineCustomerServiceContext.DialogueRecord.Add(dialogueRecord);
            onlineCustomerServiceContext.SaveChanges();

            model.Success = true;
            model.Result = Convert.ToString(dialogueRecord.Id);
        }

        public void Detail(BaseModel<DialogueRecordResponse> model, DialogueRecordRequest request)
        {
            if (onlineCustomerServiceContext == null)
            {
                return;
            }

            if (request.Id < 1)
            {
                return;
            }

            IQueryable<DialogueRecord> queryable = onlineCustomerServiceContext.DialogueRecord.Where(record => record.Id == request.Id);

            model.Success = true;
            model.Result = Mapper(queryable.First());
        }

        public void Get(BaseModel<List<DialogueRecordResponse>> model, DialogueRecordRequest request)
        {
            if (onlineCustomerServiceContext == null)
            {
                return;
            }

            IQueryable<DialogueRecord> queryable = onlineCustomerServiceContext.DialogueRecord.Where(
                record => 
                    (request.AdminId < 1 || record.AdminId == request.AdminId) &&
                    (request.UserId < 1 || record.UserId == request.UserId)
            ).Select(column => new DialogueRecord() { Id = column.Id, UserId = column.UserId, UserName = column.UserName, Question = column.Question, CreateTime = column.CreateTime });

            Int32 pageCount = queryable.Count();
            List<DialogueRecordResponse> result = new List<DialogueRecordResponse>();
            foreach (DialogueRecord record in queryable.Pagination(model.PageIndex, model.PageSize))
            {
                result.Add(Mapper(record));
            }

            model.Success = true;
            model.PageCount = pageCount;
            model.Result = result;
        }
    }
}
