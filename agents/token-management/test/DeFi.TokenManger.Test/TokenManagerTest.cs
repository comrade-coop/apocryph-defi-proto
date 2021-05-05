using Agents.Test;
using Xunit;

namespace DeFi.TokenManger.Test
{
    public class TokenManagerTest
    {
        private readonly AgentsEnvironment _env;

        public TokenManagerTest()
        {
            _env = new AgentsEnvironment();
        }

        [Fact]
        public void Test()
        {
            var tokenManager = _env.RegisterAgent(typeof(DeFi.TokenManager.State));
            
            _env.Start(tokenManager, new DeFi.TokenManager.Messages.Transfer());
        }
    }
}
