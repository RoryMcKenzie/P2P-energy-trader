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
        private int sellerMessagesReceived = 0;
        private int householdsFinished = 0;

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
                //Received when a seller tells the organiser their status
                case "seller":
                    sellerList.Add(message.Sender);
                    if (buyerList.Count + sellerList.Count == Globals.householdAgentNo)
                    {
                        SendCallsForProposals();
                    } 
                    break;

                //Received when a seller tells the organiser their status
                case "buyer":
                    buyerList.Add(message.Sender);
                    if (buyerList.Count + sellerList.Count == Globals.householdAgentNo)
                    {
                        SendCallsForProposals();
                    } 
                    break;

                //Received when a seller submits a proposal
                case "Seller_Propose":
                    sellerMessagesReceived++;
                    proposals.Add(message.Sender, Int32.Parse(parameters[0]));
                    if (sellerMessagesReceived == sellerList.Count)
                    {
                        EvaluateProposals();
                    }
                    break;

                //Received when a seller submits a rejection
                case "Seller_Reject":
                    sellerMessagesReceived++;
                    if (sellerMessagesReceived == sellerList.Count)
                    {
                        EvaluateProposals();
                    } 
                    break;

                //Received when seller requests the BuyerList
                case "Request_BuyerList":
                    string buyerliststring = "";
                    foreach (string buyer in buyerList)
                    {
                        buyerliststring += (" " + buyer);
                    }
                    Send(message.Sender, "BuyerList" + buyerliststring);
                    Globals.messageCount++;
                    break;

                //Received once an auction has ended and next seller is to be chosen
                case "NextAuction":
                    proposals.Clear();
                    SendCallsForProposals();
                    sellerMessagesReceived = 0;
                    break;
                //Received once all HouseholdAgents have bought and sold everything
                case "Done":
                    householdsFinished++;
                    if(householdsFinished == 10)
                    {
                        Console.WriteLine("\nFinal totals: \n");
                        Broadcast("final");
                    }
                    break;
            }   
        }

        private void SendCallsForProposals()
        {
            foreach (string seller in sellerList)
            {
                Send(seller, "CFP_Sellers");
                Globals.messageCount++;
            }
        }

        private void EvaluateProposals()
        {
            if (proposals.Count != 0)
            {
                var highest = proposals.OrderByDescending(x => x.Value).FirstOrDefault();
                
                foreach (string seller in sellerList)
                {
                    if (seller == highest.Key)
                    {
                        Send(seller, "Seller_BidAccepted");
                        Globals.messageCount++;
                    }
                }
            }
            else
            {
                Console.WriteLine("\nThere are no remaining sellers.");
                Broadcast("AuctionEnd");
                Console.WriteLine();
                Globals.messageCount += Globals.broadcastNo;
            }
        }
    }
}
