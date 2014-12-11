using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocketService
{
    public class ReceivedMsgOperationContainer
    {
        private static ReceivedMsgOperationContainer _instance;
        public static ReceivedMsgOperationContainer GetInstance()
        {
            if (_instance == null)
            {
                _instance = new ReceivedMsgOperationContainer();
            }
            return _instance;
        }

        private ReceivedMsgOperationContainer()
        {
            
        }

        public void OperationMsg()
        {
            
        }
    }
}
