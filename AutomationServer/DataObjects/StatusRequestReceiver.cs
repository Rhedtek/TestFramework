using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using AutomationTestCommon;

namespace AutomationTestServer.DataObjects
{
    internal class StatusRequestReceiver
    {
        TestServer mTestServer;
        TcpListener mListener;

        internal StatusRequestReceiver(TestServer testServer)
        {
            mTestServer = testServer;
        }

        internal void Start()
        {
            var ip = APPHelper.GetLocalIP();
            if (ip != null)
            {
                Log("Starting StatusRequest listener at local IP: " + ip.ToString(), true);

                mListener = new TcpListener(APPHelper.GetLocalIP(), Definitions.mTestServerControlPortNumber);
                mListener.Start();
                BeginAcceptConnection();
            }
            else
            {
                Log("Failed to get local IP");
            }
        }

        private void AcceptConnection(IAsyncResult ar)
        {
            var client = mListener.EndAcceptTcpClient(ar);

            // Process request
            StatusRequestHandler handler = new StatusRequestHandler(client, mTestServer);
            
            // Asynchronously accept new connections,
            BeginAcceptConnection();

            // before handling the request synchronously.
            handler.Start();
        }

        private void BeginAcceptConnection()
        {
            try
            {
                if (mListener != null)
                {
                    mListener.BeginAcceptTcpClient(AcceptConnection, null);
                }
            }
            catch (Exception ex)
            {
                Log("Exception when begin accepting connection: " + ex);
            }
        }

        internal void Stop()
        {
            try
            {
                Log("Stopping StatusRequest receiver");
                mListener.Stop();
                mListener = null;
            }
            catch
            {
                // nothing
            }
        }

        private void Log(string txt, bool stdOut = false)
        {
            mTestServer.Log(txt, stdOut);
        }
    }
}
