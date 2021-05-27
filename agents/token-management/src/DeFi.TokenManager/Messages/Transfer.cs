using System;

namespace DeFi.TokenManager.Messages
{
    public class Transfer
    {
        /*
         * V - transfer values, hold validation data, etc.
         *  -> Yes
         */
        public int amountToTransfer;
        public string recipient;
        public string sender; //==from
    }
}
