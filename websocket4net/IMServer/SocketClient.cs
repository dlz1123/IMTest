using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using WebSocket4Net;

namespace IMServer
{
    public class MessageItem
    {
        public MessageItem(int id,string content)
        {
            this.Id = id;
            this.Content = content;
        }

        private MessageItem ()
        {
        }

        public int Id { get; set; }
        public string Content { get; set; }
    }
    public class SocketClient
    {
        private static SocketClient _instance = null;
        private static readonly object lockHelper = new object();
        private WebSocket4Net.WebSocket webSocketClient;
        private string serverIP { get; set; }
        private string serverPort { get; set; }
        private SocketClient() { }
        public Action<object,MessageReceivedEventArgs> MessageReceived;
        public Action<object,DataReceivedEventArgs> DataReceived;
        public Action<string> WebSocketOpenOrCloseAction;

        private Queue<MessageItem> sendQueue;
        private int count;
        public static SocketClient GetInstance()
        {
            if (_instance == null)
            {
                lock (lockHelper)
                {
                    if (_instance == null)
                        _instance = new SocketClient();
                }
            }
            return _instance;
        }
        

        void InitWebSocketClient(string ip,string port)
        {
            webSocketClient = new WebSocket4Net.WebSocket(string.Format("ws://{0}:{1}/chat&token=10102", ip, port));
            
            //webSocketClient.AllowUnstrustedCertificate = true;
            webSocketClient.Opened += new EventHandler(webSocketClient_Opened);
            webSocketClient.Closed += new EventHandler(webSocketClient_Closed);
            webSocketClient.DataReceived += (s, m) =>
            {
                if (DataReceived != null)
                    DataReceived(s,m);
                // webSocketClient.Send("Received Message" + m.Message);
            };
            webSocketClient.AutoSendPingInterval = 60;
            webSocketClient.EnableAutoSendPing = true;
            webSocketClient.Open();
            webSocketClient.MessageReceived += (s, m) =>
            {
                if (MessageReceived != null)
                    MessageReceived(s,m);
            };
            if(sendQueue==null)
                sendQueue=new Queue<MessageItem>();
        }

        public WebSocketState GetState()
        {
            if (this.webSocketClient!=null)
                return this.webSocketClient.State;
            else
            {
                return WebSocketState.None;
            }
        }
        protected void webSocketClient_Closed(object sender, EventArgs e)
        {
            if (WebSocketOpenOrCloseAction != null)
                WebSocketOpenOrCloseAction("closed");
        }

        protected void webSocketClient_Opened(object sender, EventArgs e)
        {
            if (WebSocketOpenOrCloseAction != null)
                WebSocketOpenOrCloseAction("opened");

            var timer = new System.Threading.Timer(new System.Threading.TimerCallback(SendMsgThread), null, 1000, 0);


        }
        public static void SendMsgThread(object o)
        {
        }

        public  void CloseSession(object o)
        {
        }  

        public bool SendMsg(string msg)
        {
            try
            {
                sendQueue.Enqueue(new MessageItem(count++,msg));
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
