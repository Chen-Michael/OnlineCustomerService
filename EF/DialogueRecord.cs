using System;
using System.Collections.Generic;

namespace EF
{
    public partial class DialogueRecord
    {
        public int Id { get; set; }
        public int AdminId { get; set; }
        public int? UserId { get; set; }
        public string UserName { get; set; }
        public string Mail { get; set; }
        public string Phone { get; set; }
        public string Question { get; set; }
        public string Record { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
