using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

        private int j = 0;

        public override void Setup()
        {
            proposals = new Dictionary<string, int>();
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
                    //Console.WriteLine(message.Format() + "\n" + this.Name + " - Needs:" + dailyNeed.ToString() + "kWh, Generates: " + dailyGenerate.ToString() + "kWh, BuyPrice: £" + utilityBuyPrice.ToString() + ", SellPrice: £" + utilitySellPrice.ToString() + " " + status);
                    Send("organiser", content);
                    Console.WriteLine("inform " + Environment.Memory["Turn"]);
                    break;
                /*case "query":
                    string content1 = $"{status} {amountToSell} {amountToBuy}";
                    Send(message.Sender, content1);
                    break; */
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
                    if (status == "buyer" && amountToBuy > 0)
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
                    if (j == 9)
                    {
                        EvaluateProposals();
                    }
                    
                    break;
                case "nopropose":
                    j++;
                    if (j == 9)
                    {
                        EvaluateProposals();
                    }
                    break;
                case "bidaccepted":
                    money -= Int32.Parse(parameters[0]);
                    amountToBuy--;
                    break;
            }
        }

        public void EvaluateProposals()
        {
            if (proposals.Count != 0)
            {
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
            Console.WriteLine("sellercfp " + Environment.Memory["Turn"]);
            Broadcast("sellercfp");
            //If cfp sent to all agents, only the buyers will respond with an offer
        }

        public void Reset()
        {
            j = 0;
            proposals.Clear();
        }
    }
}