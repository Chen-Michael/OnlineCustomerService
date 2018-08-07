using System;

namespace Service.Shared
{
    public class BaseModel<T>
    {
        public String Code { get; set; }
        public Boolean Success { get; set; }
        public String Message { get; set; }

        public Int32 PageIndex { set; get; } = 1;
        public Int32 PageSize { set; get; } = Int32.MaxValue;
        public Int32 PageCount { set; get; }

        public T Result { set; get; }
    }
}
