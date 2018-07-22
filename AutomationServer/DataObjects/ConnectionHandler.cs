using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using AutomationTestCommon;

namespace AutomationTestServer
{
    internal class ConnectionHandler
    {
        private TcpClient mClient;
        private VMInstance mVMInstance;

        private const int mHeaderBufferSize = 8;
        private byte[] mHeaderData = new byte[mHeaderBufferSize];

        internal ConnectionHandler(TcpClient client, VMInstance vmInstance)
        {
            mClient = client;
            mVMInstance = vmInstance;
        }

        internal void Start()
        {
            ReceiveHeader();
        }

        internal void Stop()
        {
            if (mClient != null)
            {
                mClient.Close();
                mClient = null;
            }
        }

        private void ReceiveHeader()
        {
            try
            {
                mClient.Client.BeginReceive(mHeaderData, 0, mHeaderBufferSize, SocketFlags.None, HandleHeader, null);
            }
            catch (Exception ex)
            {
                Log("Error while reading data from client " + mVMInstance.VMName + ": " + ex);

                // Close the connection
                mVMInstance.HandleConnectionDone();
                mVMInstance = null;
            }
        }

        // This is the main method that receives a command, handles it, and closes the connection.
        private void HandleHeader(IAsyncResult ar)
        {
            int read = 0;
            try
            {
                read = mClient.Client.EndReceive(ar);
                if (read > 0)
                {
                    AutomationTestMessageID messageID = (AutomationTestMessageID)BitConverter.ToInt16(mHeaderData, 0);
                    short version = BitConverter.ToInt16(mHeaderData, 2);
                    int dataLength = BitConverter.ToInt32(mHeaderData, 4);
                    HandleMessageID(messageID, dataLength);
                }
                else
                {
                    Log("Read 0 bytes.");
                }
            }
            catch
            {
                // ignore
            }

            // Close the connection
            mVMInstance.HandleConnectionDone();

            // Delete reference to parent object
            mVMInstance = null;
        }

        private void HandleMessageID(AutomationTestMessageID messageID, int dataLength)
        {
            if (messageID == AutomationTestMessageID.StartMessageRequest)
            {
                HandleStartMessage(dataLength);
            }
            else if (messageID == AutomationTestMessageID.InitialMessageRequest)
            {
                HandleInitialMessage();
            }
            if (messageID == AutomationTestMessageID.GetCommandRequest)
            {
                HandleGetCommand(dataLength);
            }
            else if (messageID == AutomationTestMessageID.CommandDoneRequest)
            {
                HandleCommandDone(dataLength);
            }
            else if (messageID == AutomationTestMessageID.HeartBeatRequest)
            {
                HandleHeartbeat(dataLength);
            }
            else if (messageID == AutomationTestMessageID.LogIndication)
            {
                HandleLog(dataLength);
            }
        }

        private void HandleStartMessage(int dataLength)
        {
            var response = mVMInstance.HandleStartMessage();
            byte[] data = JsonHelper.ToByteStream<StartClientResponse>(response);
            SendData(AutomationTestMessageID.StartMessageResponse, data);
        }

        private void HandleInitialMessage()
        {
            var response = mVMInstance.HandleInitialMessage();
            byte[] data = JsonHelper.ToByteStream<InitialMessageResponse>(response);
            SendData(AutomationTestMessageID.InitialMessageResponse, data);
        }

        private void HandleHeartbeat(int dataLength)
        {
            byte[] data = ReadBuffer(dataLength);
            if (data != null)
            {
                HeartbeatRequest request = JsonHelper.ToObject<HeartbeatRequest>(data);
                var response = mVMInstance.HandleHeartbeat(request);
                data = JsonHelper.ToByteStream<HeartbeatResponse>(response);
                SendData(AutomationTestMessageID.HeartBeatResponse, data);
            }
            else
            {
                mVMInstance.HandleHeartbeat(null);
                SendData(AutomationTestMessageID.HeartBeatResponse, null);
            }
        }

        private void HandleGetCommand(int dataLength)
        {
            byte[] data = ReadBuffer(dataLength);
            ATCommandRequest request = JsonHelper.ToObject<ATCommandRequest>(data);

            try
            {
                var command = mVMInstance.HandleGetCommand(request);
                if (command == null)
                {
                    // no need to respond
                }
                else
                {
                    var item = command.Item1;
                    ATCommandResponse response = new ATCommandResponse()
                    {
                        TestSuiteID = item.TestSuiteID,
                        TestJobID = item.TestJobID,
                        TestCommandID = item.TestCommandID,
                        TestCommand = item.TestCommandString,
                        TimeoutMinutes = item.TimeoutMinutes,
                        TestPackageDirectory = item.TestPackageDirectory,
                        TestSuiteDirectory = item.TestSuiteDirectory,
                        Version = item.Version,
                        LicenseKey = item.LicenseKey,
                        DownloadLink = item.DownloadLink,
                        Snapshot = command.Item2,
                        MaxExecutionOrder = command.Item1.MaxExecutionOrder,
                        ExecutionOrder = command.Item1.ExecutionOrder,
                        UserName = command.Item1.UserName,
                        RunCount = command.Item1.RunCount
                    };
                    data = JsonHelper.ToByteStream<ATCommandResponse>(response);
                    SendData(AutomationTestMessageID.GetCommandResponse, data);
                }
            }
            catch (Exception ex)
            {
                Log("Exception: " + ex);
            }
        }

        private void HandleCommandDone(int dataLength)
        {
            byte[] buffer = ReadBuffer(dataLength);
            ATCommandDoneRequest indication = JsonHelper.ToObject<ATCommandDoneRequest>(buffer);
            mVMInstance.HandleCommandDone(indication);

            // Send response
            SendData(AutomationTestMessageID.CommandDoneResponse, null);
        }

        private byte[] ReadBuffer(int dataLength)
        {
            if (dataLength <= 0)
            {
                return null;
            }

            byte[] buffer = new byte[dataLength];
            int read = 0;
            try
            {
                mClient.Client.ReceiveTimeout = 1000;  // ms
                read = mClient.Client.Receive(buffer, dataLength, SocketFlags.None);
            }
            catch (Exception ex)
            {
                Log("Error when reading from socket: " + ex.Message);
                throw new Exception("Error while reading from socket.");
            }

            if (read == 0)
            {
                throw new Exception("No data read from socket.");
            }

            return buffer;
        }

        private void SendData(AutomationTestMessageID messageID, byte[] data)
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
            BitConverter.GetBytes((Int16)78).CopyTo(buffer, 2);
            BitConverter.GetBytes((Int32)buffer.Length - mHeaderBufferSize).CopyTo(buffer, 4);

            if (mClient != null)
            {
                mClient.Client.Send(buffer);
            }
        }

        private void HandleLog(int dataLength)
        {
            byte[] buffer = ReadBuffer(dataLength);
            LogMessageIndication indication = JsonHelper.ToObject<LogMessageIndication>(buffer);
            Log(indication.Message);
        }

        private void Log(string msg)
        {
            if (mVMInstance != null)
            {
                mVMInstance.Log(msg);
            }
        }
    }
}
