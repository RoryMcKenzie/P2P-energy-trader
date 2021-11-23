using System;
using System.Collections.Generic;
using System.Linq;
using ActressMas;

namespace Coursework1
{
    class HouseholdAgent : Agent
    {
        private int dailyNeed;
        private int dailyGenerate;
        private int utilityBuyPrice;
        private int utilitySellPrice;
        private string status;
        private int amountToBuy = 0;
        private int amountToSell = 0;
        private int money = 0;

        private Dictionary<string, int> proposals;
        private List<string> buyerList;

        private int j = 0;

        public override void Setup()
        {
            proposals = new Dictionary<string, int>();
            buyerList = new List<string>();
            Send("environment", "start");
        }

        public override void Act(Message message)
        {
            message.Parse(out string action, out List<String> parameters);
            switch (action)
            {
                case "inform":
                    dailyNeed = Int32.Parse(parameters[0]);
                    dailyGenerate = Int32.Parse(parameters[1]);
                    utilityBuyPrice = Int32.Parse(parameters[2]);
                    utilitySellPrice = Int32.Parse(parameters[3]);
                    if (dailyGenerate > dailyNeed)
                    {
                        status = "seller";
                        amountToSell = dailyGenerate - dailyNeed;
                    }
                    else
                    {
                        status = "buyer";
                        amountToBuy = dailyNeed - dailyGenerate;
                    }                    
                    string content = $"{status} {amountToSell} {amountToBuy}";
                    Console.WriteLine(this.Name + " - Needs:" + dailyNeed.ToString() + "kWh, Generates: " + dailyGenerate.ToString() + "kWh, BuyPrice: £" + utilityBuyPrice.ToString() + ", SellPrice: £" + utilitySellPrice.ToString() + " " + status);
                    Send("organiser", content);
                    Console.WriteLine("inform " + Environment.Memory["Turn"]);
                    break;
                case "organisercfp":
                    if (amountToSell > 0)
                    {
                        Console.WriteLine(this.Name + " cfp received " + Environment.Memory["Turn"]);
                        SellerPropose();
                    }
                    else
                    {
                        Send("organiser", "nopropose");
                    }
                    break;
                case "accepted":
                    CallForProposals();
                    break;
                //buyer receives seller cfp
                case "sellercfp":
                    if (amountToBuy > 0)
                    {
                        Send(message.Sender, $"propose {utilityBuyPrice - 1}");
                        Console.WriteLine("buyerpropose " + Environment.Memory["Turn"]);
                    }
                    else
                    {
                        Send(message.Sender, "nopropose");
                    }
                    break;
                //seller receives proposal from buyer
                case "propose":
                    j++;
                    proposals.Add(message.Sender, Int32.Parse(parameters[0]));
                    Console.WriteLine("receiveproposal " + Environment.Memory["Turn"]);
                    if (j == buyerList.Count)
                    {
                        EvaluateProposals();
                    }                    
                    break;
                case "nopropose":
                    j++;
                    if (j == buyerList.Count)
                    {
                        EvaluateProposals();
                    }
                    break;
                case "bidaccepted":
                    money -= Int32.Parse(parameters[0]);
                    amountToBuy--;
                    break;
                case "auctionend":
                    BuyAndSellFromProvider();
                    break;
                case "buyerlist":
                    foreach(string buyer in parameters)
                    {
                        buyerList.Add(buyer);
                    }
                    CallForProposals();
                    break;
            }
        }

        public void EvaluateProposals()
        {
            if (proposals.Count != 0)

            {
                //figure out how this works with bids which are the same, maybe change it 
                var highest = proposals.OrderByDescending(x => x.Value).FirstOrDefault();

                Send(highest.Key, $"bidaccepted {highest.Value}");

                amountToSell--;
                money += highest.Value;

                Console.WriteLine("seller accepted " + highest.Key + " " + Environment.Memory["Turn"]);

                Send("organiser", "reset");

                Reset();
            }
            else
            {
                Console.WriteLine("no buyers left");
                Broadcast("auctionend");
                BuyAndSellFromProvider();
            }
        }
        
        //Propose in response to organiser CFP
        public void SellerPropose()
        {
            Send("organiser", $"propose {amountToSell}");
        }

        //CFP for buyers
        public void CallForProposals()
        {
            if (buyerList.Count == 0)
            {
                //sends a message to organiser and gets a list of buyers 
                Send("organiser", "buyerlistrequest");
                Console.WriteLine("buyerlistrequest");
            }
            else
            {
                Console.WriteLine("sellercfp " + Environment.Memory["Turn"]);
                foreach (string buyer in buyerList)
                {
                    Send(buyer, "sellercfp");
                }
            }
        }

        public void Reset()
        {
            j = 0;
            proposals.Clear();
        }

        public void BuyAndSellFromProvider()
        {
            if (amountToBuy > 0)
            {
                money -= (amountToBuy * utilityBuyPrice);
                amountToBuy = 0;
            }

            if (amountToSell > 0)
            {
                money += (amountToSell * utilitySellPrice);
                amountToSell = 0;
            }
            
            Console.WriteLine($"{this.Name}: £{money}");
        }
    }
}