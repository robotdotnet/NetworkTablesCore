using System.IO;

namespace NetworkTables.Native.Exceptions
{
    public class PersistentException : IOException
    {
        public PersistentException(string message) : base(message)
        {
            
        }
    }
}
