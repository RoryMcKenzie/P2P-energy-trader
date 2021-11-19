using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        private List <string> buyerList;
        private Dictionary<string, int> proposals;


        public override void Setup()
        {
            proposals = new Dictionary<string, int>();
            buyerList = new List<string>();
            Send("environment", "start");
        }

        public override void ActDefault()
        {
           if (Environment.Memory["Turn"] == 9 && proposals.Count != 0)
            {
                EvaluateProposals();
            } 
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
                    Console.WriteLine("inform " + Environment.Memory["Turn"]);
                    string content = $"{status} {amountToSell} {amountToBuy}";
                    //Console.WriteLine(message.Format() + "\n" + this.Name + " - Needs:" + dailyNeed.ToString() + "kWh, Generates: " + dailyGenerate.ToString() + "kWh, BuyPrice: £" + utilityBuyPrice.ToString() + ", SellPrice: £" + utilitySellPrice.ToString() + " " + status);
                    Send("organiser", content);
                    break;
                /*case "query":
                    string content1 = $"{status} {amountToSell} {amountToBuy}";
                    Send(message.Sender, content1);
                    break; */
                case "organisercfp":
                    Console.WriteLine(this.Name + " cfp received " + Environment.Memory["Turn"]);
                    SellerPropose();
                    break;
                case "accepted":
                    CallForProposals();
                    break;
                //buyer receives seller cfp
                case "sellercfp":
                    if (status == "buyer")
                    {
                        Send(message.Sender, $"propose {utilityBuyPrice - 1}");
                        Console.WriteLine("buyerpropose " + Environment.Memory["Turn"]);
                    }
                    break;
                //seller receives proposal from buyer
                case "propose":
                    Console.WriteLine(message.Sender + " " + parameters[0]);
                    proposals.Add(message.Sender, Int32.Parse(parameters[0]));
                    Console.WriteLine("receiveproposal " + Environment.Memory["Turn"]);
                    break;
                case "bidaccepted":
                    money -= Int32.Parse(parameters[0]);
                    amountToBuy--;
                    break;
            }
        }

        public void EvaluateProposals()
        {
            var highest = proposals.OrderByDescending(x => x.Value).FirstOrDefault();

            Send(highest.Key, $"bidaccepted {highest.Value}");

            amountToSell--;
            money += highest.Value;

            Console.WriteLine("accepted " + highest.Key + " " + Environment.Memory["Turn"]);
            
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
    }
}
