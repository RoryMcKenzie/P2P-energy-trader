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

        public override void Setup()
        {
            Send("environment", "start");
        }

        public override void ActDefault()
        {
            
        }

        public override void Act(Message message)
        {
            string messageC = message.Content;

            string[] words = messageC.Split(' ');

            switch (words[0])
            {
                case "inform":
                    dailyNeed = Int32.Parse(words[1]);
                    dailyGenerate = Int32.Parse(words[2]);
                    utilityBuyPrice = Int32.Parse(words[3]);
                    utilitySellPrice = Int32.Parse(words[4]);
                    break;
                default:
                    break;
            }
            Console.WriteLine(message.Format() + "\n" + this.Name + " - Needs:" + dailyNeed.ToString() + "kWh, Generates: " + dailyGenerate.ToString() + "kWh, BuyPrice: £" + utilityBuyPrice.ToString() + ", SellPrice: £" + utilitySellPrice.ToString());
        }
    }
}
