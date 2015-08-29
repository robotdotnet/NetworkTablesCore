using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkTablesCore.Tables
{
    public interface IRemote
    {
        void AddConnectionListener(IRemoteConnectionListener listener, bool immediateNotify);

        void RemoveConnectionListener(IRemoteConnectionListener listener);

        bool IsConnected();
        bool IsServer();
    }
}
