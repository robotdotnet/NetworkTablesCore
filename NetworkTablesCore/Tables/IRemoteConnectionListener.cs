using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkTablesCore.Tables
{
    public interface IRemoteConnectionListener
    {
        void Connected(IRemote remote);
        void Disconnected(IRemote remote);
    }
}
