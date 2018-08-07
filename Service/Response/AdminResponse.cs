using System;

namespace Service.Response
{
    public class AdminResponse
    {
        public Int32 Id { get; set; }
        public String Account { get; set; }
        public String Password { get; set; }
        public String Name { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
