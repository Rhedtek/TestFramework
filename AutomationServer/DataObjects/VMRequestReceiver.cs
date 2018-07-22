using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using AutomationTestCommon;

namespace AutomationTestServer.DataObjects
{
    internal class VMRequestReceiver
    {
        private TcpListener mListener = null;
        private TestServer mTestServer;
        private List<VMInstance> mVMInstanceData = new List<VMInstance>();
        private NetworkCredential mNetworkCredentials = null;

        internal VMRequestReceiver(TestServer testServer)
        {
            mTestServer = testServer;
            mNetworkCredentials = new NetworkCredential("user", "password");
        }

        internal void Start()
        {
            Initialize();

            var ip = APPHelper.GetLocalIP();
            if (ip != null)
            {
                Log("Starting VM Request listener at local IP: " + ip.ToString(), true);

                try
                {
                    mListener = new TcpListener(ip, Definitions.mPortNumber);
                    mListener.Start();
                    BeginAcceptConnection();
                }
                catch (Exception ex)
                {
                    Log("Exception: " + ex);
                }
            }
            else
            {
                Log("Failed to get local IP");
            }
        }

        internal void Stop()
        {
            // Stop the listener socket
            try
            {
                Log("Stopping VMRequest receiver");

                if (mListener != null)
                {
                    mListener.Stop();
                    mListener = null;
                }
            }
            catch { }

            // Wait for all existing connections to clear up
            while (true)
            {
                Log("Checking for active connections.", true);

                bool busy = false;
                foreach (var vmInstance in mVMInstanceData)
                {
                    busy = busy || vmInstance.Busy();
                }

                if (busy == false)
                {
                    break;
                }

                Thread.Sleep(5000);
            }

        }

        private void Initialize()
        {
            Log("Loading VM data.", true);

            var vms = VMInstanceData.Select();
            foreach (var vm in vms)
            {
                Log("Adding " + vm.VMName + " @ " + vm.IPAddress, true);
                var vmInstance = new VMInstance(vm, mTestServer, mNetworkCredentials);
                mVMInstanceData.Add(vmInstance);
            }
        }

        private void AcceptConnection(IAsyncResult ar)
        {
            try
            {
                TcpClient client = mListener.EndAcceptTcpClient(ar);
                IPEndPoint endpoint = client.Client.RemoteEndPoint as IPEndPoint;
                if (endpoint != null)
                {
                    var ipAddress = endpoint.Address.ToString();
                    var vmInstance = GetVMInstanceData(ipAddress);
                    if (vmInstance != null)
                    {
                        vmInstance.Start(client);
                    }
                    else
                    {
                        Log("Could not match IP " + ipAddress + " to a valid VM");
                        client.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Log("Exception when accepting new connection: " + ex);
            }

            BeginAcceptConnection();
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

        private VMInstance GetVMInstanceData(string ipAddress)
        {
            var vmInstance = mVMInstanceData.FirstOrDefault(e => e.IPAddress == ipAddress);
            if (vmInstance == null)
            {
                // This is old code, so you should never get here, since all
                // VMs are loaded at start up
                Log("Fetching information for IP " + ipAddress, true);
                var vmInstanceData = VMInstanceData.SelectByIP(ipAddress);
                if (vmInstanceData != null)
                {
                    Log("Adding information for IP " + ipAddress, true);
                    vmInstance = new VMInstance(vmInstanceData, mTestServer, mNetworkCredentials);
                    mVMInstanceData.Add(vmInstance);
                }
            }
            return vmInstance;
        }

        private void Log(string txt, bool stdOut = false)
        {
            mTestServer.Log(txt, stdOut);
        }
    }
}
