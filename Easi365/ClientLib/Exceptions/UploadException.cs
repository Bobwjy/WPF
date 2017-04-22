using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientLib.Exceptions
{
    class UploadException : Exception
    {
        public UploadException(string message)
            : base(message)
        {
        }
    }
}
