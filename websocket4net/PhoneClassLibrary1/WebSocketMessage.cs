using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WP8Test;
using WP8Test.MessageModel;

namespace WebSocketService
{
    public class WebSocketMessage
    {
        public string Name { get; set; }
        public int Sequence { get; set; }
        public string Type { get; set; }
        public string Body { get; set; }
        public int StatusCode { get; set; }
        public Dictionary<string,string> RequestParams { get; set; }
        public ModelBase RequestModel { get; set; }
        public ModelBase ResponeModel { get; set; }

        public static string Serialization(WebSocketMessage msg)
        {
            return string.Format("N:{0}\nS:{1}\nT:{2}\n\n{3}",msg.Name,msg.Sequence,msg.Type,msg.Body);
        }
        public static WebSocketMessage Deserialization(string str)
        {
            try
            {
                WebSocketMessage message = new WebSocketMessage();
                var array = str.Split(new string[]{"\n"},StringSplitOptions.None);
                message.Name = array[0].Split(':')[1].ToString();
                int sequence = 0;
                int.TryParse(array[1].Split(':')[1], out sequence);
                message.Sequence = sequence;
                message.Type = array[2].Split(':')[1].ToString();
                int statueCode = 0;
                int.TryParse(array[3].Split(':')[1],out statueCode);
                message.StatusCode = statueCode;
                message.Body = array[4];
                message.ResponeModel = GetModel(message.Name, message.Body);
                return message;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static ModelBase GetModel(string methodName,string str)
        {
            ModelBase model=null;
            switch (methodName)
            {
                case "NewMessage":
                   model= Newtonsoft.Json.JsonConvert.DeserializeObject<NewMessageModel>(str);
                    break;
            }
            return model;
        }
    }
}
