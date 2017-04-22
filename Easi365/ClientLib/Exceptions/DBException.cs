using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientLib.Exceptions
{
    public class DBException : Exception
    {
        public DBException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
