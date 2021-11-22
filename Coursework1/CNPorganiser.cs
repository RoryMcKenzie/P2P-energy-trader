using System;
using System.Collections.Generic;
using System.Linq;
using ActressMas;

namespace Coursework1
{
    class CNPorganiser : Agent
    {
        private List<string> sellerList;
        //add buyerlist

        private Dictionary<string,int> proposals;
        private int i = 0;
        private int j = 0;

        public override void Setup()
        {
            sellerList = new List<string>();
            //add buyerlist
            proposals = new Dictionary<string,int>();
        }

        //have actdefault with variable that decrements and then if it's 0 do the thing
        
        public override void Act(Message message)
        {
            message.Parse(out string action, out List<String> parameters);
            switch (action)
            {
                case "seller":
                    sellerList.Add(message.Sender);
                    //workaround, to be changed
                    i++;
                    if (i == 10)
                    {
                        SendCallsForProposals();
                        Console.WriteLine("cfpturn " + Environment.Memory["Turn"]);
                    } 
                    break;
                case "buyer":
                    //workaround, to be changed
                    //add line of code adding the name to a buyers list, which can then be requested by each seller
                    i++;
                    if (i == 10)
                    {
                        SendCallsForProposals();
                        Console.WriteLine("cfpturn " + Environment.Memory["Turn"]);
                    } 
                    break;
                case "propose":
                    j++;
                    //Console.WriteLine(message.Sender + " proposal received " + Environment.Memory["Turn"]);
                    proposals.Add(message.Sender, Int32.Parse(parameters[0]));
                    if (j == sellerList.Count)
                    {
                        EvaluateProposals();
                    }
                    break;
                case "nopropose":
                    j++;
                    if (j == sellerList.Count)
                    {
                        EvaluateProposals();
                    } 
                    break;
                case "reset":
                    proposals.Clear();
                    SendCallsForProposals();
                    j = 0;
                    break;
            }
        }

        private void SendCallsForProposals()
        {
            foreach (string seller in sellerList)
            {
                Send(seller, "organisercfp");
            }
        }

        private void EvaluateProposals()
        {
            if (proposals.Count != 0)
            {

                Console.WriteLine("organiser evaluated proposals " + Environment.Memory["Turn"]);

                //figure out how this works with bids which are the same, maybe change it 
                var highest = proposals.OrderByDescending(x => x.Value).FirstOrDefault();

                Console.WriteLine("organiser accepted " + highest.Key + " " + Environment.Memory["Turn"]);

                foreach (string seller in sellerList)
                {
                    if (seller == highest.Key)
                    {
                        Send(seller, "accepted");
                    }
                }
            }
            else
            {
                Console.WriteLine("no sellers left");
                Broadcast("auctionend");
            }
        }
    }
}
