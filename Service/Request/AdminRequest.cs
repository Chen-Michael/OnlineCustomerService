using System;

namespace Service.Request
{
    public class AdminRequest
    {
        public Int32 Id { get; set; }
        public String Account { get; set; }
        public String Password { get; set; }
        public String Name { get; set; }
    }
}
