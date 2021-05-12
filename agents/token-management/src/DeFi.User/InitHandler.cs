using System;
using DeFi.User.Messages;

namespace DeFi.User
{
    public static class InitHandler
    {
        public static Transfer Run(Init message, string from, State state)
        {
            Console.WriteLine($"[DeFi.User] User received Init({message.To}) from {from}.");
            return new Transfer { To = message.TokenManagerRef };
        }
    } 
}
