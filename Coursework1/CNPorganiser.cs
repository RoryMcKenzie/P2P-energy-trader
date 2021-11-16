using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActressMas;

namespace Coursework1
{
    class CNPorganiser : Agent
    {
        List<string> sellerList;
        List<string> buyerList;

        public override void Setup()
        {
            sellerList = new List<string>();
            buyerList = new List<string>();

        }

        public override void Act(Message message)
        {
            message.Parse(out string action, out List<String> parameters);
            switch (action)
            {
                case "buyer":
                    buyerList.Add(message.Sender);
                    break;
                case "seller":
                    sellerList.Add(message.Sender);
                    break;
            }
        }

    }
}
