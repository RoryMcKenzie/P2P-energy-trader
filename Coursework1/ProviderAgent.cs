using System;
using System.Collections.Generic;
using ActressMas;

namespace Coursework1
{
    
    public enum ServiceType
    {
        Auction,
        BuyFromUtility,
        SellToUtility
    };
    public class ProviderAgent : Agent
    {

        private ServiceType _type;

        public ProviderAgent(ServiceType serviceType)
        {
            _type = serviceType;
        }
        
        public override void Setup()
        {
            Send("broker", $"register {_type}");
        }
        
        public override void Act(Message message)
        {
            try
            {
                Console.WriteLine($"\t{message.Format()}");
                message.Parse(out string action, out List<string> parameters);

                switch (action)
                {
                    case "force-unregister":
                        HandleForceUnregister();
                        break;

                    case "request":
                        HandleRequest(message, parameters);
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        
        private void HandleForceUnregister()
        {
            Send("broker", $"unregister {_type}");
        }

        private void HandleRequest(Message message, List<string> parameters)
        {

        }
    }
    
}