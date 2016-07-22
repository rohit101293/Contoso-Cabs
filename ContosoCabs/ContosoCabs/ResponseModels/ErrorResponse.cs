using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContosoCabs.ResponseModels
{
    public class ErrorResponse
    {
        public int Code;
        public string Message;
        public ErrorResponse(int code, string message)
        {
            Code = code;
            Message = message;
        }
    }
}
