using System;

namespace NetworkTables.Native.Exceptions
{
    public class TableKeyNotDefinedException : InvalidOperationException
    {
        public TableKeyNotDefinedException(string message) : base($"Unknown Table Key: {message}")
        {
            
        }
    }
}
