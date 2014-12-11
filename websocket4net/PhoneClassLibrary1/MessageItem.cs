using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocket4Net;

namespace WP8Test
{
    //public class MessageItem
    //{
    //    public MessageItem(string name, string content, bool isSelfSend, Action<MessageReceivedEventArgs> callBackAction,int timeout=0)
    //    {
    //        this.Content = content;
    //        this.Name = name;
    //        this.IsSelfSend = isSelfSend;
    //        this.CallBackAction = callBackAction;
    //        this.TimeOut = timeout;
    //        count++;
    //    }
    //    public int TimeOut { get; set; }
    //    private static int count = 0;

    //    public int Count
    //    {
    //        get { return count; }
    //    }

    //    public Action<MessageReceivedEventArgs> CallBackAction
    //    {
    //        get; set;
    //    }
    //    private MessageItem()
    //    {
            
    //    }

    //    public int Id { get; set; }
    //    private string _name;
    //    /// <summary>
    //    /// 聊天消息ID
    //    /// </summary>
    //    public string Name
    //    {
    //        get { return _name; }
    //        set
    //        {
    //            _name = value;
    //            NotifyPropertyChanged("Name");
    //        }
    //    }


    //    private string _content;
    //    /// <summary>
    //    /// 聊天内容
    //    /// </summary>
    //    public string Content
    //    {
    //        get { return _content; }
    //        set
    //        {
    //            _content = value;
    //            NotifyPropertyChanged("Content");
    //        }
    //    }


    //    private bool _isSelfSend;
    //    /// <summary>
    //    /// 聊天内容
    //    /// </summary>
    //    public bool IsSelfSend
    //    {
    //        get { return _isSelfSend; }
    //        set
    //        {
    //            _isSelfSend = value;
    //            NotifyPropertyChanged("IsSelfSend");
    //        }
    //    }

    //    public event PropertyChangedEventHandler PropertyChanged;
    //    public void NotifyPropertyChanged(string propertyName)
    //    {
    //        System.Windows.Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
    //        {
    //            if (PropertyChanged != null)
    //            {
    //                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    //            }
    //        }));
    //    }
    //}
}
