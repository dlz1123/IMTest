using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocket4Net;
using WebSocketService;
using WP8Test.MessageModel;

namespace WP8Test.Server
{
    public class EventProxy
    {
        private static EventProxy _instance;
        private static readonly object lockHelper = new object();
        public event Action<MessageReceivedEventArgs> ReceivedSystemMessageEvent;
        private Dictionary<int, WebSocketMessage> MessageDic;
        public static int count = 1000;
        private EventProxy()
        {

        }

        private int GetSequenceNum()
        {
            return count++;
        }
        public static EventProxy GetInstance()
        {
            if (_instance == null)
            {
                lock (lockHelper)
                {
                    if (_instance == null)
                        _instance = new EventProxy();
                }

            }
            return _instance;
        }

        public void SendMessage(NewMessageModel msg)
        {
            var socketMsg = new WebSocketMessage();
            socketMsg.Name = "NewMessage";
            socketMsg.RequestParams =new Dictionary<string, string>();
            socketMsg.RequestParams.Add("Name","dlz");
            socketMsg.RequestParams.Add("Content", msg.Content);
            socketMsg.RequestModel = msg;
            socketMsg.Body = Newtonsoft.Json.JsonConvert.SerializeObject(socketMsg.RequestParams);
            socketMsg.Type = "Json";
            socketMsg.Sequence = GetSequenceNum();
            MessageDic.Add(socketMsg.Sequence,socketMsg);
            SocketClient.GetInstance().SendMsg(socketMsg);
        }
        public void Init()
        {
            if (MessageDic == null)
                MessageDic = new Dictionary<int, WebSocketMessage>();
            SocketClient.GetInstance().MessageReceived += EventToUI_MessageReceived;
        }

        void EventToUI_MessageReceived(object arg1, WebSocket4Net.MessageReceivedEventArgs arg2)
        {
            //if (MessageReceivedEvent != null)
            //    MessageReceivedEvent(arg2);
            var msg = WebSocketMessage.Deserialization(arg2.Message);

            //如果MessageDic不存在，就是服务器主动推送的消息
            if (MessageDic.ContainsKey(msg.Sequence))
            {
                if (MessageDic[msg.Sequence].RequestModel.CallBackAction != null)
                    MessageDic[msg.Sequence].RequestModel.CallBackAction.Invoke(msg.ResponeModel);
                MessageDic.Remove(msg.Sequence);
            }
            else
            {
                if (ReceivedSystemMessageEvent != null)
                    ReceivedSystemMessageEvent(arg2);
            }


        }
    }
}
