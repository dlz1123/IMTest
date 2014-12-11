using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using WebSocket4Net;
using WP8Test;

namespace WebSocketService
{
    

    public class SocketClient
    {
        private static SocketClient _instance = null;
        private static readonly object lockHelper = new object();
        private WebSocket4Net.WebSocket webSocketClient;
        public event Action<object, MessageReceivedEventArgs> MessageReceived;
        public Action<object, DataReceivedEventArgs> DataReceived;
        public Action<string> WebSocketOpenOrCloseAction;
        //private List<MessageItem> sendWaitCallBackQueue;
        private Dictionary<WebSocketMessage, Timer> sendQueue;//发送的消息作为主键，timer超时控制
        //private Queue<MessageReceivedEventArgs> receivedQueue;
        private static int count=1000;
        private AutoResetEvent autoResetEvent = new AutoResetEvent(false);
        private static object lockObj = new object();
        private SocketClient() { }
        #region 公开方法
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

        /// <summary>
        /// 初始化iwebsocket
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public void InitWebSocketClient(string ip, string port)
        {
            if (sendQueue == null)
                sendQueue = new Dictionary<WebSocketMessage, Timer>();
            //if (sendWaitCallBackQueue == null)
            //    sendWaitCallBackQueue = new List<MessageItem>();
            //if(receivedQueue==null)
            //    receivedQueue = new Queue<MessageReceivedEventArgs>();
            webSocketClient = new WebSocket(string.Format("ws://{0}:{1}/chat&token=10102", ip, port));

            //webSocketClient.AllowUnstrustedCertificate = true;
            webSocketClient.Opened += new EventHandler(webSocketClient_Opened);
            webSocketClient.Closed += new EventHandler(webSocketClient_Closed);
            webSocketClient.DataReceived += (s, m) =>
            {
                if (DataReceived != null)
                    DataReceived(s, m);
                // webSocketClient.Send("Received Message" + m.Message);
            };
            webSocketClient.AutoSendPingInterval = 60;
            webSocketClient.EnableAutoSendPing = true;
            webSocketClient.Open();
            webSocketClient.MessageReceived += (s, m) =>
            {
                if (MessageReceived != null)
                    MessageReceived(s, m);
            };
           
        }

        public WebSocketState GetState()
        {
            if (this.webSocketClient != null)
                return this.webSocketClient.State;
            else
            {
                return WebSocketState.None;
            }
        }
        public bool SendMsg(WebSocketMessage msg)
        {
            try
            {
                lock (lockObj)
                {
                    int timerOut = 1000*60;//默认一分钟超时
                    if (msg.RequestModel.TimeOut != 0)
                        timerOut = 1000*msg.RequestModel.TimeOut;
                    sendQueue.Add(msg, new System.Threading.Timer(new System.Threading.TimerCallback(RequestTimeOut), msg, timerOut, 0));
                    autoResetEvent.Set();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
        #region 私有方法

        

        protected void webSocketClient_Closed(object sender, EventArgs e)
        {
            if (WebSocketOpenOrCloseAction != null)
                WebSocketOpenOrCloseAction("closed");
        }

        protected void webSocketClient_Opened(object sender, EventArgs e)
        {
            if (WebSocketOpenOrCloseAction != null)
                WebSocketOpenOrCloseAction("opened");
            SendMsgThread();
            ReceivedMsgThread();
        }
        /// <summary>
        /// 发送线程
        /// </summary>
        public  void SendMsgThread()
        {
            Thread checktheloginuser = new Thread(SendMsgToServer);
            checktheloginuser.Start();
        }
        /// <summary>
        /// 接收线程
        /// </summary>
        public void ReceivedMsgThread()
        {
            //Thread checktheloginuser = new Thread(new ParameterizedThreadStart(ReceivedMsgFromServer));
            //checktheloginuser.Start(o);
            Thread checktheloginuser = new Thread(ReceivedMsgFromServer);
            checktheloginuser.Start();
        }

        private void ReceivedMsgFromServer(object o)
        {
            //Debug.WriteLine("is time to Received");
            //while (autoResetEvent.WaitOne())
            //{
            //    while (receivedQueue.Count > 0)
            //    {
            //        lock (lockObj)
            //        {

            //            var msg = receivedQueue.Dequeue();
            //            //msg.CallBackAction()
            //            Debug.WriteLine(msg.Message.ToString());
            //            //sendWaitCallBackQueue.Remove(msg);

            //        }

            //    }
            //}
            
        }
        /// <summary>
        /// 发送消息到服务器
        /// </summary>
        /// <param name="o"></param>
        private void SendMsgToServer(object o)
        {
            Debug.WriteLine("is time to send");
            while (autoResetEvent.WaitOne())
            {
                while (sendQueue.Count > 0)
                {
                    if (webSocketClient.State == WebSocketState.Open)
                    {
                        var msgItem = sendQueue.First();
                        try
                        {
                            lock (lockObj)
                            {

                                webSocketClient.Send(WebSocketMessage.Serialization(msgItem.Key));
                                sendQueue.Remove(msgItem.Key);
                            }
                        }
                        catch (Exception ex)
                        {
                            lock (lockObj)
                            {
                                if (!sendQueue.ContainsKey(msgItem.Key))
                                {
                                    sendQueue.Add(msgItem.Key,msgItem.Value);
                                }
                            }
                        }
                    }
                }
            }
        }

       

        private static void RequestTimeOut(object o)
        {
            //UserPassport up=new UserPassport();

            Thread checktheloginuser = new Thread(new ParameterizedThreadStart(TimeOutCallBack));
            checktheloginuser.Start(o);
        }

        private static void TimeOutCallBack(object o)
        {
            ModelBase socket = o as ModelBase;
            if (socket != null)
            {
                socket.CallBackAction.Invoke(new MessageReceivedEventArgs("timeout"));
            }

        }

        #endregion
    }
}
