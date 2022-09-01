using System;
using System.Net;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System.Windows.Threading;

namespace SARecorder.Automation
{
    public delegate void ScanerEventHandler(object sender, ScanerEventArgs e);

    public struct UdpState : IDisposable
    {
        public UdpClient client;
        public IPEndPoint endpoint;

        public async Task SendUdpMessage(byte[] message, IPEndPoint targetIp)
        {
            await client.SendAsync(message, message.Length, targetIp);
        }

        public async Task SendUdpMessage(byte[] message, string targetIp, int targetPort = 65003)
        {
            await SendUdpMessage(message, new IPEndPoint(IPAddress.Parse(targetIp), targetPort));
        }

        public void Dispose()
        {
            EndPoint currentEndPoint = client.Client.LocalEndPoint;

            client.Close();
            client.Dispose();

            Debug.WriteLine($"{currentEndPoint} is closed");
        }
    }

    public class ScanerFilter : IDisposable
    {
        public event ScanerEventHandler OnScanerDataReceived;

        public static string SCANER_GATEWAY_IP = "192.168.0.100";
        public static int SCANER_GATEWAY_PORT = 65003;

        private readonly Dispatcher eventInvoker;
        private UdpState currentState;

        public double SAGATState = 0;
        public List<double> OtherSignals = new List<double>();

        public bool IsReady => currentState.client != null;
        public bool IsConnected { get; private set; }

        public ScanerFilter(Dispatcher dispatcher)
        {
            eventInvoker = dispatcher;
        }

        public void Connect()
        {
            if (IsReady == false)
            {
                currentState = ConnectScaner();
            }
        }

        private UdpState ConnectScaner()
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(SCANER_GATEWAY_IP), SCANER_GATEWAY_PORT);
            UdpState state = new UdpState()
            {
                client = new UdpClient(endPoint),
                endpoint = endPoint
            };
            _ = state.client.BeginReceive(OnReceiveMessage, state);
            Debug.WriteLine($"Activated...[{state.client.Client.LocalEndPoint}]");

            currentState = state;

            return state;
        }

        private void OnReceiveMessage(IAsyncResult ar)
        {
            if (IsConnected == false)
            {
                IsConnected = true;
            }

            if (ar.AsyncState != null)
            {
                UdpState state = (UdpState)ar.AsyncState;

                IPEndPoint remoteIP = null;
                byte[] messageByte = null;
                try
                {
                    messageByte = state.client.EndReceive(ar, ref remoteIP);
                }
                catch(ObjectDisposedException)
                {
                    Debug.WriteLine("Socket is already disposed. Stop receiving.");
                    return;
                }

                // Parsing
                int dataCount = messageByte.Length / 8;
                for (int i = 0; i < dataCount; i++)
                {
                    double val = BitConverter.ToDouble(messageByte, i * 8);
                    switch (i)
                    {
                        case 0:
                            SAGATState = val;
                            break;
                        default:
                            if (i - 1 < OtherSignals.Count)
                            {
                                OtherSignals[i - 1] = val;
                            }
                            else
                            {
                                OtherSignals.Add(val);
                            }
                            break;
                    }
                }

                eventInvoker.BeginInvoke(() =>
                {
                    OnScanerDataReceived?.Invoke(this, new ScanerEventArgs(SAGATState >= 0.5 && SAGATState < 1.5, OtherSignals.ToArray()));
                });
                _ = state.client.BeginReceive(OnReceiveMessage, ar.AsyncState);
            }
            else
            {
                Debug.WriteLine("UDP state is null: Socket will be close");
                currentState.Dispose();
            }
        }

        public void Dispose()
        {
            currentState.Dispose();
        }
    }
}
