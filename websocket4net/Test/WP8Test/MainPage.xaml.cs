using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using WebSocket4Net;
using WebSocketService;
using WP8Test.MessageModel;
using WP8Test.Resources;
using Microsoft.Phone.Testing;
using WP8Test.Server;

namespace WP8Test
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            this.Loaded += MainPage_Loaded;
            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
            this.MsgListSelector.ItemsSource = this.MessageList;
        }

        private ObservableCollection<ModelBase> _messageList = new ObservableCollection<ModelBase>();
        public ObservableCollection<ModelBase> MessageList
        {
            get { return _messageList; }
            set
            {
                _messageList = value;
                NotifyPropertyChanged("Images");
            }
        }

        private WebSocket webSocketClient;
        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {


            var socket = SocketClient.GetInstance();
            socket.InitWebSocketClient("192.9.145.139", "8181");
            socket.WebSocketOpenOrCloseAction = new Action<string>((p) => { Debug.WriteLine(p); });
            EventProxy.GetInstance().Init();
            EventProxy.GetInstance().ReceivedSystemMessageEvent += MainPage_MessageReceivedEvent;

        }

        void MainPage_MessageReceivedEvent(MessageReceivedEventArgs obj)
        {
            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    var item =new NewMessageModel();
                    item.IsSelfSend = false;
                    item.Name = "系统消息";
                    item.Content = obj.Message;
                    MessageList.Add(item);
                }));
        }

        protected void webSocketClient_Closed(object sender, EventArgs e)
        {
            //m_CloseEvent.Set();
        }

        protected void webSocketClient_Opened(object sender, EventArgs e)
        {
            //m_OpenedEvent.Set();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }));
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            NewMessageModel msg = new NewMessageModel();
            msg.Name = "dlz";
            msg.Content = this.MsgTextBox.Text;
            msg.MethodName = "NewMessage";
            //msg.TimeOut = 0;
            msg.CallBackAction = (p) =>
            {
                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    var item = p as NewMessageModel;
                    if (item != null)
                    {
                        MessageList.Add(item);
                    }
                }));
            };
            msg.IsSelfSend = true;
            EventProxy.GetInstance().SendMessage(msg);
            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    MessageList.Add(msg);
                    this.MsgTextBox.Text = "";
                }));
        }
    }
}