using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using WebSocketService;
using WP8Test;
using WP8Test.MessageModel;

namespace Fleck.Samples.ConsoleApp
{


    class Server
    {
        private static Dictionary<IWebSocketConnection, Timer> allSockets;
        private static int timeOut = 1000*60;
        static void Main()
        {
            FleckLog.Level = LogLevel.Debug;
            allSockets = new Dictionary<IWebSocketConnection, Timer>();
            var server = new WebSocketServer("ws://192.9.145.139:8181");
            
            server.Start(socket =>
                {
                    socket.OnOpen = () =>
                        {
                            Console.WriteLine(string.Format("{0}--{1}--{2}",DateTime.Now,socket.ConnectionInfo.ClientIpAddress,"Open!"));
                            allSockets.Add(socket, new System.Threading.Timer(new System.Threading.TimerCallback(CheckState), socket, timeOut, 0));

                        };
                    socket.OnClose = () =>
                        {
                            Console.WriteLine("Close!");
                            allSockets.Remove(socket);
                        };
                    socket.OnMessage = message =>
                        {
                            allSockets[socket].Change(timeOut*3, 0);
                            if (message.ToLower() == "ping")
                            {
                                socket.SendPong( Encoding.UTF8.GetBytes("Pong"));
                                socket.Send("pong");
                            }
                            Message msg = new Message();
                            try
                            {
                                msg = Message.Deserialization(message);
                            }
                            catch (Exception ex)
                            {
                                
                            }
                            msg.StatusCode = 200;
                            switch (msg.Name)
                            {
                                case"NewMessage":
                                {
                                    var newMessageItem = new NewMessageModel();
                                    newMessageItem.Name = "server";
                                    newMessageItem.Content = "收到消息---" + msg.RequestParams["Content"];
                                    msg.Body = Newtonsoft.Json.JsonConvert.SerializeObject(newMessageItem);
                                    
                                    break;
                                }
                            }
                            //var socketItem=allSockets.ToList().Find(p=>p.ConnectionInfo.ClientIpAddress==message)
                            //allSockets.ToList().ForEach(s => s.Send("Echo: " + message));
                            for (int i = 0; i < 1000; i++)
                            {
                                socket.Send(Message.Serialization(msg));
                            }
                            
                        };
                });



            var input = Console.ReadLine();
            while (input != "exit")
            {
                foreach (var socket in allSockets.ToList())
                {
                    socket.Key.Send(input);
                }
                input = Console.ReadLine();
            }

        }

        //启动监视"已登录用户通信情况"的线程
        public static void CheckState(object o)
        {
            //UserPassport up=new UserPassport();

            Thread checktheloginuser = new Thread(new ParameterizedThreadStart(CloseSession));
            checktheloginuser.Start(o);
        }

        public static void CloseSession(object o)
        {
            IWebSocketConnection socket = o as IWebSocketConnection;
            if (socket!=null)
            {
                //socket.Close();
                //allSockets.Remove(socket);
                //Console.WriteLine(string.Format("{0}--{1}--{2}", DateTime.Now, socket.ConnectionInfo.ClientIpAddress, "Close!"));
            }
            
            Console.WriteLine(DateTime.Now+"all Scoket Count"+allSockets.Count);
        }  //定时检查在线人目前状态

        private static double DateDiff(DateTime DateTime1, DateTime DateTime2)
        {
            TimeSpan span = (TimeSpan)(DateTime1 - DateTime2);
            return span.TotalSeconds;
        }
        public class Message
        {
            public string Name { get; set; }
            public int Sequence { get; set; }
            public string Type { get; set; }
            public string Body { get; set; }
            public int StatusCode { get; set; }
            public Dictionary<string, string> RequestParams { get; set; }
            public static string Serialization(Message msg)
            {
                return string.Format("N:{0}\nS:{1}\nT:{2}\nC:{3}\n{4}", msg.Name, msg.Sequence, msg.Type, msg.StatusCode, msg.Body);
            }
            public static Message Deserialization(string str)
            {
                try
                {
                    Message message = new Message();
                    var array = str.Split(new string[] { "\n" }, StringSplitOptions.None);
                    message.Name = array[0].Split(':')[1].ToString();
                    int sequence = 0;
                    int.TryParse(array[1].Split(':')[1], out sequence);
                    message.Sequence = sequence;
                    message.Type = array[2].Split(':')[1].ToString();
                    message.Body = array[4];
                    message.RequestParams =
                        Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(message.Body);
                    return message;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
       
    }
}
