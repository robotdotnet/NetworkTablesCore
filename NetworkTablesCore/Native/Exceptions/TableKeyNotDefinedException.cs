using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkTablesCore.Native.Exceptions
{
    public class TableKeyNotDefinedException : InvalidOperationException
    {
        public TableKeyNotDefinedException(string message) : base($"Unknown Table Key: {message}")
        {
            
        }
    }
}
