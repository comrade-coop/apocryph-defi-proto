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

        public string RegisterAgent(Type stateType, Type[] whitelist = null)
        {
            var agent = Guid.NewGuid().ToString();
            var assembly = Assembly.GetAssembly(stateType);
            _agents[agent] = (assembly, assembly.CreateInstance(stateType.FullName));
            
            var result = agent;
            if (whitelist != null)
            {
                result = Guid.NewGuid().ToString();
                _references[result] = (agent, whitelist);
            }
            return result;
        }

        public void Start(object message)
        {
            _messages.Enqueue((message, nameof(AgentsEnvironment)));
            RunMessagesLoop();
        }

        private void RunMessagesLoop()
        {
            while (_messages.Count > 0)
            {
                var (sourceMessage, from) = _messages.Dequeue();
                var sourceMessageType = sourceMessage.GetType();

                var toRef = (string)sourceMessageType.GetProperty("To").GetValue(sourceMessage);
                var (agent, whitelist) = _references[toRef];

                var messageType = whitelist.FirstOrDefault(t => t.Name == sourceMessageType.Name);
                if (messageType == null)
                {
                    throw new Exception($"Reference doesn't support message type {sourceMessageType.Name}");
                }

                var message = CloneMessage(messageType, sourceMessage, from);
                Execute(agent, message, from);
            }
        }

        private void Execute(string agent, object message, string from)
        {
            var (assmebly, state) = _agents[agent];
            var handler = assmebly.GetTypes().First(t => t.Name == $"{message.GetType().Name}Handler").GetMethod("Run");
            var output = handler.Invoke(null, new object[] { message, from, state });

            if (output is IEnumerable outputMessages)
            {
                foreach (var outputMessage in outputMessages)
                {
                    _messages.Enqueue((outputMessage, agent));
                }
            }
            else if(output != null)
            {
                _messages.Enqueue((output, agent));
            }
        }

        private object CloneMessage(Type targetType, object sourceMessage, string sourceAgent)
        {
            var result = Activator.CreateInstance(targetType);
            var sourceMessageType = sourceMessage.GetType();
            foreach (var property in sourceMessageType.GetProperties())
            {
                var value = property.GetValue(sourceMessage);
                if (property.Name.EndsWith("Ref") && value is Type[] whitelist)
                {
                    var reference = Guid.NewGuid().ToString();
                    _references[reference] = (sourceAgent, whitelist);
                    value = reference;
                }
                targetType.GetProperty(property.Name).SetValue(result, value);
            }
            return result;
        }
    }
}
