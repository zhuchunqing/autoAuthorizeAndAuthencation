using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Common
{
    public class ReturnMes<T>
    {
        public ReturnMes(string code,string messages,bool success) 
        {
            this.Code = code;
            this.Messages = messages;
            this.Success = success;
        }
        public string Code { get; set; }
        public string Messages { get;set; }
        public bool Success { get; set; }
        public T Value { get; set; }
    }
}
