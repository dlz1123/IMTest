using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WP8Test.MessageModel
{
    public class NewMessageModel:ModelBase
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public bool IsSelfSend { get; set; }
    }
}
