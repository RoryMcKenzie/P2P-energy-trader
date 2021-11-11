using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActressMas;

namespace Coursework1
{
    class Program
    {
        static void Main(string[] args)
        {

            var env = new EnvironmentMas(noTurns: 100);
            var e = new EnvironmentAgent(); env.Add(e, "environment");
            var brokerAgent = new BrokerAgent(); env.Add(brokerAgent, "broker");

            ProviderAgent pa1 = new ProviderAgent(ServiceType.Auction); env.Add(pa1, "provider1");
            ProviderAgent pa2 = new ProviderAgent(ServiceType.BuyFromUtility); env.Add(pa2, "provider2");
            ProviderAgent pa3 = new ProviderAgent(ServiceType.SellToUtility); env.Add(pa3, "provider3");

            var h1 = new HouseholdAgent(); env.Add(h1, "agent1");
            var h2 = new HouseholdAgent(); env.Add(h2, "agent2");
            var h3 = new HouseholdAgent(); env.Add(h3, "agent3");
            var h4 = new HouseholdAgent(); env.Add(h4, "agent4");
            var h5 = new HouseholdAgent(); env.Add(h5, "agent5");
            var h6 = new HouseholdAgent(); env.Add(h6, "agent6");


            env.Start();
            Console.ReadLine();
        }
    }
}
