using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocket4Net;

namespace WP8Test
{
    public abstract class ModelBase
    {
        public string MethodName
        {
            get; set;
        }

        public Action<object> CallBackAction
        {
            get;
            set;
        }

        public DateTime DateTime { get; set; }
        public int TimeOut { get; set; }
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

    }
}
