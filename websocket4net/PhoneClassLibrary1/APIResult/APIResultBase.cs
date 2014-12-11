using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WP8Test;

namespace WebSocketService
{
    public abstract class APIResultBase
    {
        //public abstract MessageItem GetMessageItem(object o);
        public abstract void methodCallBack();
        public abstract void trigger(int port);
        public abstract void saveToDB();
        protected ModelBase MsgItem { get; set; }
    }
}
