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

        private int boughtFromSeller = 0;
        private int boughtFromSellerMoney = 0;
        private int boughtFromUtility = 0;

        private int soldToBuyerMoney = 0;
        private int soldToBuyer = 0;
        private int soldToUtility = 0;

        private Dictionary<string, int> proposals;
        private List<string> buyerList;

        private int buyerMessagesReceived = 0;

        public override void Setup()
        {
            proposals = new Dictionary<string, int>();
            buyerList = new List<string>();
            Send("environment", "start");
            Globals.messageCount++;
        }

        public override void Act(Message message)
        {
            message.Parse(out string action, out List<String> parameters);
            switch (action)
            {
                //All HouseholdAgents receive this and send their status to the organiser
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
                    Console.WriteLine($"{this.Name} - Needs {dailyNeed} kWh, generates {dailyGenerate} kWh, buy from utility for {utilityBuyPrice} pence, sell to utility for {utilitySellPrice} pence, type: {status}");
                    Send("organiser", content);
                    Globals.messageCount++;
                    break;

                //Sellers receive this from organiser and propose or reject the request
                case "CFP_Sellers":
                    if (amountToSell > 0)
                    {
                        SellerPropose();
                    }
                    else
                    {
                        Send("organiser", "Seller_Reject");
                        Globals.messageCount++;
                    }
                    break;

                //Sellers receive this when their bid has been accepted by the organiser,
                //meaning they are the next seller
                case "Seller_BidAccepted":
                    CallForProposals();
                    break;

                //Received by a seller when the organiser sends the list of all buyers
                case "BuyerList":
                    foreach (string buyer in parameters)
                    {
                        buyerList.Add(buyer);
                    }
                    CallForProposals();
                    break;

                //Buyers receive this from seller and propose or reject the request
                case "CFP_Buyers":
                    if (amountToBuy > 0)
                    {
                        Send(message.Sender, $"Buyer_Propose {utilityBuyPrice - 1}");
                        Globals.messageCount++;
                        Console.WriteLine($"    {this.Name} bids {utilityBuyPrice - 1} pence");
                    }
                    else
                    {
                        Send(message.Sender, "Buyer_Reject");
                        Globals.messageCount++;
                    }
                    break;

                //Sellers receive this when a buyer submits a proposal
                case "Buyer_Propose":
                    buyerMessagesReceived++;
                    proposals.Add(message.Sender, Int32.Parse(parameters[0]));
                    if (buyerMessagesReceived == buyerList.Count)
                    {
                        EvaluateProposals();
                    }                    
                    break;

                //Sellers receive this when a buyer submits a rejection
                case "Buyer_Reject":
                    buyerMessagesReceived++;
                    if (buyerMessagesReceived == buyerList.Count)
                    {
                        EvaluateProposals();
                    }
                    break;

                //Buyers receive this when their bid has been accepted by the seller,
                //meaning they have won the kWh of energy
                case "Buyer_BidAccepted":
                    money -= Int32.Parse(parameters[0]);
                    boughtFromSeller++;
                    boughtFromSellerMoney += Int32.Parse(parameters[0]);
                    amountToBuy--;
                    break;

                //Received when the auction is finished because there are no more remaining buyers or sellers
                case "AuctionEnd":
                    BuyAndSellFromProvider();
                    break;

                case "final":
                    FinalTotal();
                    break;
            }
        }

        public void EvaluateProposals()
        {
            if (proposals.Count != 0)

            {
                //Vickrey auction
                var highest = proposals.OrderByDescending(x => x.Value).FirstOrDefault();

                var secondHighest = proposals.OrderByDescending(x => x.Value).ElementAtOrDefault(1);

                var paid = secondHighest;

                if (secondHighest.Value == 0)
                {
                     paid = highest;
                } 

                Send(highest.Key, $"Buyer_BidAccepted {paid.Value}");
                Globals.messageCount++;

                amountToSell--;
                soldToBuyer++;
                soldToBuyerMoney += paid.Value;
                money += paid.Value;

                Console.WriteLine($"\n    {highest.Key} wins and pays {paid.Value} pence");                

                Send("organiser", "Reset");
                Globals.messageCount++;

                Reset();
            }
            else
            {
                Console.WriteLine("    There are no more remaining buyers.");
                Broadcast("AuctionEnd");
                Console.WriteLine();
                Globals.messageCount += Globals.broadcastNo;
                BuyAndSellFromProvider();
            }
        }
        
        //Propose in response to organiser CFP
        public void SellerPropose()
        {
            Send("organiser", $"Seller_Propose {amountToSell}");
            Globals.messageCount++;
        }

        //CFP for buyers
        public void CallForProposals()
        {
            if (buyerList.Count == 0)
            {
                //sends a message to organiser and gets a list of buyers 
                Send("organiser", "Request_BuyerList");
                Globals.messageCount++;
            }
            else
            {
                Console.WriteLine($"\nAuction Started - Seller: {this.Name} \n");
                //Console.WriteLine("sellercfp " + Environment.Memory["Turn"]);
                foreach (string buyer in buyerList)
                {
                    Send(buyer, "CFP_Buyers");
                    Globals.messageCount++;
                }
            }
        }

        public void Reset()
        {
            buyerMessagesReceived = 0;
            proposals.Clear();
        }

        public void BuyAndSellFromProvider()
        {
            //if agent is a buyer still to buy energy
            if (amountToBuy > 0)
            {
                Console.WriteLine($"{this.Name} bought {amountToBuy}kWh from their utility company for {utilityBuyPrice} pence each");
                money -= (amountToBuy * utilityBuyPrice);
                boughtFromUtility = amountToBuy;
                amountToBuy = 0;
            }

            //if agent is a seller still to sell energy
            if (amountToSell > 0)
            {
                Console.WriteLine($"{this.Name} sold {amountToSell}kWh to their utility company for {utilitySellPrice} pence each");
                money += (amountToSell * utilitySellPrice);
                soldToUtility = amountToSell;
                amountToSell = 0;
            }
            Send("organiser", "Done");           
        }

        public void FinalTotal()
        {
            if (status == "buyer")
            {
                int totalBought = boughtFromSeller + boughtFromUtility;
                Console.WriteLine($"{this.Name} bought {totalBought} kWh for a total cost of {(Math.Abs(money))} pence");
                Console.WriteLine($"To buy this only from the utility company would've cost {totalBought * utilityBuyPrice} pence\n");

                /* string first = boughtFromSeller.ToString();
                string second = boughtFromSellerMoney.ToString();
                string third = boughtFromUtility.ToString();
                string fourth = (boughtFromUtility * utilityBuyPrice).ToString();
                string fifth = money.ToString();

                //amount bought from sellers, money from sellers, amount bought from utility, money from utility, total

                var newLine = string.Format("{0},{1},{2},{3},{4}", first, second, third, fourth, fifth);
                Globals.buyercsv.AppendLine(newLine); */
            }
            else
            {
                int totalSold = soldToBuyer + soldToUtility;
                Console.WriteLine($"{this.Name} sold {soldToBuyer + soldToUtility} kWh for a total of {(money)} pence");
                Console.WriteLine($"To sell this only to the utility company would've only made {totalSold * utilitySellPrice} pence\n");

                /*string first = soldToBuyer.ToString();
                string second = soldToBuyerMoney.ToString();
                string third = soldToUtility.ToString();
                string fourth = (soldToUtility * utilitySellPrice).ToString();
                string fifth = money.ToString();

                //amount sold to buyers, money from buyers, amount sold to utility, money from utility, total

                var newLine = string.Format("{0},{1},{2},{3},{4}", first, second, third, fourth, fifth);
                Globals.sellercsv.AppendLine(newLine); */
            }
        }

    }
}