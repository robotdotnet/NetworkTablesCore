using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkTablesCore
{
    public class ConnectionInfo
    {
        public string RemoteId { get; }
        public string RemoteName { get; }
        public int RemotePort { get; }
        public long LastUpdate { get; }
        public int ProtocolVersion { get; }

        public ConnectionInfo(string rId, string rName, int rPort, long lastUpdate, int protocolVersion)
        {
            this.RemoteId = rId;
            this.RemoteName = rName;
            this.RemotePort = rPort;
            this.LastUpdate = lastUpdate;
            this.ProtocolVersion = protocolVersion;
        }
    }
}
