using System;
using System.Collections.Generic;
using System.Linq;
using ActressMas;

namespace Coursework1
{
    class CNPorganiser : Agent
    {
        private List<string> sellerList;
        private List<string> buyerList;

        private Dictionary<string,int> proposals;
        private int j = 0;

        public override void Setup()
        {
            sellerList = new List<string>();
            buyerList = new List<string>();
            proposals = new Dictionary<string,int>();
        }
        
        public override void Act(Message message)
        {
            message.Parse(out string action, out List<String> parameters);
            switch (action)
            {
                case "seller":
                    sellerList.Add(message.Sender);
                    if (buyerList.Count + sellerList.Count == Globals.householdAgentNo)
                    {
                        SendCallsForProposals();
                    } 
                    break;
                case "buyer":
                    buyerList.Add(message.Sender);
                    if (buyerList.Count + sellerList.Count == Globals.householdAgentNo)
                    {
                        SendCallsForProposals();
                    } 
                    break;
                case "propose":
                    j++;
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
                case "buyerlistrequest":
                    string buyerliststring = "";
                    foreach(string buyer in buyerList)
                    {
                        buyerliststring += (" " + buyer);
                    }                    
                    Send(message.Sender, "buyerlist" + buyerliststring);
                    Globals.messageCount++;
                    break;
            }
        }

        private void SendCallsForProposals()
        {
            foreach (string seller in sellerList)
            {
                Send(seller, "organisercfp");
                Globals.messageCount++;
            }
        }

        private void EvaluateProposals()
        {
            if (proposals.Count != 0)
            {

               // Console.WriteLine("organiser evaluated proposals " + Environment.Memory["Turn"]);

                //maybe make seller send utilitySellPrice instead and the lowest wins, so that the remaining sellers will get decent money 

                //change to OrderBy if thats the case
                var highest = proposals.OrderByDescending(x => x.Value).FirstOrDefault();

               // Console.WriteLine("organiser accepted " + highest.Key + " " + Environment.Memory["Turn"]);

                foreach (string seller in sellerList)
                {
                    if (seller == highest.Key)
                    {
                        Send(seller, "accepted");
                        Globals.messageCount++;
                    }
                }
            }
            else
            {
                //Console.WriteLine("no sellers left");
                Broadcast("auctionend");
                Globals.messageCount += Globals.broadcastNo;

            }
        }
    }
}
