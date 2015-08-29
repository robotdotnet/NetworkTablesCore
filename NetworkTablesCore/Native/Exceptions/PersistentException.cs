using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkTablesCore.Native.Exceptions
{
    public class PersistentException : IOException
    {
        public PersistentException(string message) : base(message)
        {
            
        }
    }
}
