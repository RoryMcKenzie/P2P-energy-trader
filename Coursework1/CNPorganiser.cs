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
        private Dictionary<string,int> sellerList;
        private Dictionary<string,int> buyerList;

        private Dictionary<string,int> proposals;
        int i = 0;

        public override void Setup()
        {
            sellerList = new Dictionary<string,int>();
            buyerList = new Dictionary<string,int>();
            proposals = new Dictionary<string,int>();
            //SendCallsForProposals();
        }

        public override void ActDefault()
        {
          /* if (Environment.Memory["Turn"] == 3)
            {
                SendCallsForProposals();
            } */
           if (Environment.Memory["Turn"] == 6)
            {
                EvaluateProposals();
            } 
        }

        public override void Act(Message message)
        {
            message.Parse(out string action, out List<String> parameters);
            switch (action)
            {
                case "seller":
                    sellerList.Add(message.Sender, Int32.Parse(parameters[0]));
                    //Appalling workaround, has to be changed
                    i++;
                    if (i == 10)
                    {
                        SendCallsForProposals();
                        Console.WriteLine("cfpturn " + Environment.Memory["Turn"]);
                    } 
                    break;
                case "buyer":
                    buyerList.Add(message.Sender, Int32.Parse(parameters[1]));
                    //Appalling workaround, has to be changed
                    i++;
                    if (i == 10)
                    {
                        SendCallsForProposals();
                        Console.WriteLine("cfpturn " + Environment.Memory["Turn"]);
                    } 
                    break;
                case "propose":
                    Console.WriteLine(message.Sender + " proposal received " + Environment.Memory["Turn"]);
                    proposals.Add(message.Sender, Int32.Parse(parameters[0]));
                    break;
            }
        }

        private void SendCallsForProposals()
        {
            foreach (string seller in sellerList.Keys)
            {
                Send(seller, "organisercfp");
            }
        }

        private void EvaluateProposals()
        {
            Console.WriteLine("evaluated proposals " + Environment.Memory["Turn"]);

            var highest = proposals.OrderByDescending(x => x.Value).FirstOrDefault();

            Console.WriteLine("accepted " + highest.Key + " " + Environment.Memory["Turn"]);

            foreach (string seller in sellerList.Keys)
            {
                if (seller == highest.Key)
                {
                    Send(seller, "accepted");
                }
                else
                {
                    Send(seller, "rejected");
                }
            }
        }
    }
}
