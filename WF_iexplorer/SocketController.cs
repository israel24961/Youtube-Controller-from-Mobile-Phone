using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace WF_iexplorer
{
    public class MessageEventController
    {
        /*
         * Following Event Pattern
         */
        //Object that inherits from EventArgs, carries the data
        public class MessageSend_EventArgs : EventArgs
        {
            public readonly string message;
            public MessageSend_EventArgs(string message)
            {
                this.message = message;
            }
        }
        public class MessageConnect_EventArgs : EventArgs
        {
            public readonly IPEndPoint RemoteIP;
            public MessageConnect_EventArgs(IPEndPoint RemoteIP)
            {
                this.RemoteIP = RemoteIP;
            }
        }
        //Event Handler, which will call the functions added to its self
        public event EventHandler<MessageSend_EventArgs> ReceivedEventHandler;
        public event EventHandler<MessageConnect_EventArgs> ConnectedEventHandler;

        //Method for firing the event
        public virtual void onReceive(MessageSend_EventArgs msg)
        {
            ReceivedEventHandler?.Invoke(this, msg);
        }
        public virtual void onConnect(MessageConnect_EventArgs msg)
        {
            ConnectedEventHandler?.Invoke(this, msg);
        }


    }

    public class SocketController
    {
        CustomDebug DebugTag;
        Socket socketServer;
        MessageEventController EventMessage;

        public void AddOnReceive(EventHandler<MessageEventController.MessageSend_EventArgs> Eh)
        {
            EventMessage.ReceivedEventHandler += Eh;
        }

        public void AddOnConnect(EventHandler<MessageEventController.MessageConnect_EventArgs> Eh)
        {
            EventMessage.ConnectedEventHandler += Eh;
        }

        public SocketController(IPAddress ip, int port, string DebugTAG)
        {
            DebugTag = new CustomDebug(DebugTAG);
            DebugTag.WriteLine(
                $"Created socket with IP: \n\t" +
                    $"IP:{Dns.GetHostName()}\n\t" +
                    $"Port:{port}");
            socketServer = new Socket(AddressFamily.InterNetwork,
                                          SocketType.Stream,
                                          ProtocolType.Tcp);
            socketServer.Bind(new System.Net.IPEndPoint(ip, port));//34197

            EventMessage = new MessageEventController();
        }
        public void StartListening(int numOfConnections)
        {
            socketServer?.Listen(numOfConnections);
            socketServer.BeginAccept(new AsyncCallback(AsyncMethodSocket), null);
        }
        private void AsyncMethodSocket(IAsyncResult ar)
        {
            //Once Connected, gets the socket connected
            Socket handler = socketServer.EndAccept(ar);
            Task.Run(() =>
            {
                EventMessage.onConnect(
                    new MessageEventController.MessageConnect_EventArgs(
                        (IPEndPoint)handler.RemoteEndPoint)
                    );
            });
            //Printing ips and ports for debugging
            var ipOfRemote = handler.RemoteEndPoint as IPEndPoint;
            DebugTag.WriteLine(
                $"Remote Socket connected:\n\t " +
                $"IP: {ipOfRemote?.Address}\n\t " +
                    $"PORT:{ipOfRemote?.Port}");
            var ipOfLocal = handler.LocalEndPoint as IPEndPoint;
            DebugTag.WriteLine(
                $"Local Socket connected:\n\t" +
                    $"IP: {ipOfLocal?.Address}\n\t " +
                    $"PORT:{ipOfLocal?.Port}");
            this.PreparateReceive(handler);
        }
        private void ReceivedFromSocket(IAsyncResult ar)
        {
            //object passed
            var dynObject = (dynamic)ar.AsyncState;
            var handler = (Socket)dynObject.socketHandler;

            //EndTheAsyncCall
            SocketError socketError = SocketError.Success;
            int? read = handler?.EndReceive(ar, out socketError);
            if (socketError != SocketError.Success)
            {
                handler.Close();
                socketServer.BeginAccept(new AsyncCallback(AsyncMethodSocket), null);
                return;
            }
            this.PreparateReceive(handler);
            var ipOfRemote = handler.RemoteEndPoint as IPEndPoint;

            var buffer = (byte[])dynObject.buffer;


            DebugTag.WriteLine(
                $"Been sent by Remote Socket:\n\t" +
                    $"IP: {ipOfRemote?.Address}\n\t " +
                    $"PORT:{ipOfRemote?.Port}\n\t" +
                    $"Nº Bytes:{read}");

            if (read == null || read <= 0)
            {
                handler.Close();
                return;
            }

            Task.Run(() =>
            EventMessage
                .onReceive(new MessageEventController.MessageSend_EventArgs(
                    System.Text.Encoding.ASCII
                    .GetString(buffer, 0, read.Value)))
                );

        }
        private void PreparateReceive(Socket handler)
        {
            var buffer = new byte[127];
            handler.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, this.ReceivedFromSocket, new { buffer = buffer, socketHandler = handler });
        }
        public static IPAddress getLocalIP()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            return host
                .AddressList
                .Where(ip =>
                {
                    return ip.ToString().StartsWith("192");
                }).FirstOrDefault();

        }
        class CustomDebug
        {
            string TAG;
            public CustomDebug(string tag)
            {
                TAG = tag;
            }
            public void WriteLine(string msg)
            {
                Debug.WriteLine(TAG + msg);
            }
        }
    }
}
