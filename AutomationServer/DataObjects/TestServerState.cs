using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationTestServer.DataObjects
{
    public enum TestServerState
    {
        Unknown,
        Initializing,
        Running,
        Stopping,
        Stopped     // which of course you'll never see
    }
}
