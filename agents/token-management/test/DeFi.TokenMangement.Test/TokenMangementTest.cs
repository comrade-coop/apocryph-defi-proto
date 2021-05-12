using Agents.Test;
using Xunit;

namespace DeFi.TokenMangement.Test
{
    public class TokenMangementTest
    {
        private readonly AgentsEnvironment _env;

        public TokenMangementTest()
        {
            _env = new AgentsEnvironment();
        }

        [Fact]
        public void Test()
        {
            var tokenManagerRef = _env.RegisterAgent(typeof(DeFi.TokenManager.State), new[] { typeof(TokenManager.Messages.Transfer) });
            var userRef = _env.RegisterAgent(typeof(DeFi.User.State), new[] { typeof(DeFi.User.Messages.Init) });

            _env.Start(new DeFi.User.Messages.Init { To = userRef, TokenManagerRef = tokenManagerRef});
        }
    }
}
