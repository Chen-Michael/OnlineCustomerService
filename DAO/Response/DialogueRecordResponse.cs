using System;

namespace DAO.Response
{
    public class DialogueRecordResponse
    {
        public Int32 Id { get; set; }
        public Int32 AdminId { get; set; }
        public Int32? UserId { get; set; }
        public String UserName { get; set; }
        public String Mail { get; set; }
        public String Phone { get; set; }
        public String Question { get; set; }
        public String Record { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
