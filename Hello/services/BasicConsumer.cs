using Confluent.Kafka;
using Newtonsoft.Json;

namespace Hello
{
    class BasicConsumer : IHelloConsumer
    {
        private IConsumer<Ignore, string> _consumer;

        public BasicConsumer(string groupId, string servers, string topics)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = servers,
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _consumer = new ConsumerBuilder<Ignore, string>(config).Build();

            _consumer.Subscribe(topics);
        }

        public string ReadMessage()
        {
            return _consumer.Consume().Value;
        }
    }
}