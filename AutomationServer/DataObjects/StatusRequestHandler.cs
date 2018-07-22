using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.Net.Sockets;
using AutomationTestCommon;

namespace AutomationTestServer.DataObjects
{
    internal class StatusRequestHandler
    {
        private TcpClient mClient;
        private TestServer mTestServer;
        private const int mHeaderBufferSize = 8;

        internal StatusRequestHandler(TcpClient client, TestServer testServer)
        {
            mClient = client;
            mTestServer = testServer;
        }

        internal void Start()
        {
            Log("StatusRequestHandler::Start");

            try
            {
                byte[] buffer = new byte[mHeaderBufferSize];
                mClient.Client.ReceiveTimeout = 100;   // ms timeout
                int read = mClient.Client.Receive(buffer, mHeaderBufferSize, SocketFlags.None);
                if (read == 8)
                {
                    TestServerMessageID messageID = (TestServerMessageID)BitConverter.ToInt16(buffer, 0);
                    short version = BitConverter.ToInt16(buffer, 2);
                    int dataLength = BitConverter.ToInt32(buffer, 4);
                    HandleMessageID(messageID, dataLength);

                    Log("MessageID: " + messageID + "; data length: " + dataLength);
                }
            }
            catch (Exception ex)
            {
                Log("StatusRequestHandler exception: " + ex);
            }

            mClient.Close();
            mClient = null;
            mTestServer = null;
        }

        private void HandleMessageID(TestServerMessageID messageID, int dataLength)
        {
            if (messageID == TestServerMessageID.TestServerStatusReq)
            {
                TestServerStatusRsp rsp = new TestServerStatusRsp()
                {
                    Status = mTestServer.TestServerState.ToString()
                };

                byte[] data = JsonHelper.ToByteStream<TestServerStatusRsp>(rsp);
                Send(TestServerMessageID.TestServerStatusRsp, data);

                Log("Send status: " + rsp.Status);
            }
            else if (messageID == TestServerMessageID.TestServerStopReq)
            {
                // This call could take a while
                mTestServer.Stop();

                Send(TestServerMessageID.TestServerStopRsp, null);

                Log("Send StopRsp");
            }
        }

        private void Send(TestServerMessageID messageID, byte[] data)
        {
            byte[] buffer;

            if (data != null)
            {
                buffer = new byte[data.Length + mHeaderBufferSize];
                data.CopyTo(buffer, 8);
            }
            else
            {
                buffer = new byte[mHeaderBufferSize];
            }

            BitConverter.GetBytes((Int16)messageID).CopyTo(buffer, 0);
            BitConverter.GetBytes((Int16)1).CopyTo(buffer, 2); // Version, not used right now
            BitConverter.GetBytes((Int32)buffer.Length - mHeaderBufferSize).CopyTo(buffer, 4);

            mClient.Client.Send(buffer);
        }

        private void Log(string txt, bool stdOut = false)
        {
            mTestServer.Log(txt, stdOut);
        }
    }
}
