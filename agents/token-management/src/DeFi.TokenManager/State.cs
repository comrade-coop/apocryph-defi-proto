using System.Collections.Generic;

namespace DeFi.TokenManager
{
    public class State
    {
        /*
         * V - Current values for agents (tokens, balance, maps etc)
         *  -> yes
         */
        
        // store balances/data for each user
        public Dictionary<string, int> balances = new Dictionary<string, int>();
    }
}