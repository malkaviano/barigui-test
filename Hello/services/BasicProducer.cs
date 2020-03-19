using System;
using Confluent.Kafka;
using Newtonsoft.Json;
using Serilog;

namespace Hello
{
    class BasicProducer : IHelloProducer
    {
        private IProducer<Null, string> _producer;
        private string _name;
        private string _topics;

        public BasicProducer(string servers, string topics)
        {
            _name = Guid.NewGuid().ToString();
            _topics = topics;

            var config = new ProducerConfig
            {
                BootstrapServers = servers
            };

            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public void SendMessage(string value)
        {
            var message = new HelloMessage
            {
                Id = Guid.NewGuid(),
                Payload = value,
                Timestamp = DateTime.Now,
                Identifier = _name
            };

            var json = JsonConvert.SerializeObject(message);


            _producer.Produce(
                _topics,
                new Message<Null, string>
                {
                    Value = json
                },
                r =>
                {
                    Log.Information(
                        r.Error.IsError ?
                        string.Format("Error: {0}", r.Error.Reason) :
                        string.Format("Wrote to offset: {0}", r.Offset)
                    );
                }
            );
        }
    }
}