using System;

namespace DeFi.User.Messages
{
    
    private class BaseMessage
	{
        public string To { get; set; }
	}


    public class Init : BaseMessage
    {
        public string TokenManagerRef { get; set; }
    }
}
