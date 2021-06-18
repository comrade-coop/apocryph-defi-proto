using System;
using DeFi.TokenManager.Messages;

namespace DeFi.TokenManager
{
    public static class TransferHandler
    {
        /*
         * V - Invoked by what?
         *  -> By reflection at the test ENV.
         *  
         * How do I setup unit test for the things below?
         */

        public static void Run(Transfer message, string from, State state)
        {
            /*
             * V - So message holds token recipient, and the token sender is passed in as "from" parameter?
             *  -> yes
             * V - State holds current client balance?
             *  -> yes
             * V - How do you indentify the agent that is the recipient?
             *  -> Agent only does the transfer, is not a participating end-point (therefore no need to hold identity data)
             * V - Does current client recieve benefits from the transaction?
             *  -> No, benefits are distributed by Agent0
             *  -> open to comissions too
             */

            //
            // ----------------
            // Agent meat goes here:
              
            state.balances[message.sender] = state.balances[message.sender] - message.amountToTransfer;
            state.balances[message.recipient] = state.balances[message.recipient] + message.amountToTransfer;
            
            Console.WriteLine($"[DeFi.TokenManager] TokenManager received Transfer({message.sender} -> {message.amountToTransfer} -> {message.recipient}) from {from}.");
        }
    } 

}