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
            _references[target] = (agent, new[] { message.GetType() });
            Execute(target, message, nameof(AgentsEnvironment));

            RunMessagesLoop();
        }

        private void RunMessagesLoop()
        {
            while (_messages.Count > 0)
            {
                var (sourceMessage, from) = _messages.Dequeue();
                var sourceMessageType = sourceMessage.GetType();

                var toRef = (string)sourceMessageType.GetProperty("To").GetValue(sourceMessage);
                var (target, whitelist) = _references[toRef];
                
                var messageType = whitelist.FirstOrDefault(t => t.Name == sourceMessageType.Name);
                if(messageType == null)
                {
                    throw new Exception($"Reference doesn't support message type {sourceMessageType.Name}");    
                }

                var message = CloneMessage(messageType, sourceMessage, from);
                Execute(target, message, from);
            }
        }

        private void Execute(string target, object message, string from)
        {
            var (agent, types) = _references[target];
            var targetType = types.First(t => t.Name == message.GetType().Name);
            var targetMessage = CloneMessage(targetType, message, from);

            var (assmebly, state) = _agents[agent];
            var handler = assmebly.GetType($"{targetType.Name}Handler").GetMethod("Run");
            var output = handler.Invoke(null, new object[] { targetMessage, from, state });

            if (output is IEnumerable outputMessages)
            {
                foreach (var outputMessage in outputMessages)
                {
                    _messages.Enqueue((outputMessage, target));
                }
            }
            else
            {
                _messages.Enqueue((output, target));
            }
        }

        private object CloneMessage(Type targetType, object sourceMessage, string sourceAgent)
        {
            var sourceMessageType = sourceMessage.GetType();
            foreach(var property in sourceMessageType.GetProperties())
            {
                if(property.Name.EndsWith("Ref"))
                {

                }
            }
        }
    }
}
