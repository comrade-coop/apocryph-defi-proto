using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Agents.Test
{
    public class AgentsEnvironment
    {
        private readonly Dictionary<string, (Assembly, object)> _agents;
        private readonly Dictionary<string, (string, Type[])> _references;
        private readonly Queue<(object, string)> _messages;

        public AgentsEnvironment()
        {
            _agents = new Dictionary<string, (Assembly, object)>();
            _references = new Dictionary<string, (string, Type[])>();
            _messages = new Queue<(object, string)>();
        }

        public string RegisterAgent(Type stateType)
        {
            var result = Guid.NewGuid().ToString();
            var assembly = Assembly.GetAssembly(stateType);
            _agents[result] = (assembly, assembly.CreateInstance(stateType.FullName));
            return result;
        }

        public void Start(string agent, object message = null)
        {
            var target = Guid.NewGuid().ToString();
            _references[target] = (agent, new[]{message.GetType()});
            Execute(target, message, nameof(AgentsEnvironment));

            RunMessagesLoop();
        }

        private void RunMessagesLoop()
        {
            while(_messages.Count > 0)
            {
                     
            }            
        }

        private void Execute(string target, object sourceMessage, string sourceAgent)
        {
            var (agent, types) = _references[target];
            var targetType = types.First(t => t.Name == sourceMessage.GetType().Name);
            var targetMessage = CloneMessage(targetType, sourceMessage, sourceAgent);            
            
            var (assmebly, state) = _agents[agent];
            var handler = assmebly.GetType($"{targetType.Name}Handler").GetMethod("Run");
            var output = handler.Invoke(null, new object[]{targetMessage, sourceAgent, state});

            if(output is IEnumerable outputMessages)
            {
                foreach (var outputMessage in outputMessages)
                {
                    _messages.Enqueue(outputMessage);
                } 
            }
            else
            {
                _messages.Enqueue(output);
            }
        }

        private object CloneMessage(Type targetType, object sourceMessage, string sourceAgent)
        {
            throw new NotImplementedException();
        }
    }
}
