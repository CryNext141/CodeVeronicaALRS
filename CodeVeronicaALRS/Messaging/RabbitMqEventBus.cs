using System;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace CodeVeronicaALRS.Messaging
{
    public class RabbitMqEventBus : IEventBus, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;

        public RabbitMqEventBus(IConfiguration config)
        {
            var factory = new ConnectionFactory
            {
                HostName = config["RabbitMq:Host"],
                UserName = config["RabbitMq:User"],
                Password = config["RabbitMq:Pass"]
            };

            _connection = factory
                .CreateConnectionAsync()
                .GetAwaiter()
                .GetResult();
            _channel = _connection
                .CreateChannelAsync()
                .GetAwaiter()
                .GetResult();

            _channel.ExchangeDeclareAsync(
                    exchange: "alerts.exchange",
                    type: ExchangeType.Topic,
                    durable: true)
                .GetAwaiter()
                .GetResult();
        }

        public void Publish<T>(T @event, string routingKey)
        {
            var json = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(json);

            var props = new BasicProperties();

            _channel.BasicPublishAsync(
                    exchange: "alerts.exchange",
                    routingKey: routingKey,
                    mandatory: false,
                    basicProperties: props,
                    body: body)
                .GetAwaiter()
                .GetResult();
        }

        public void Dispose()
        {
            _channel
                .CloseAsync()
                .GetAwaiter()
                .GetResult();
            _connection
                .CloseAsync()
                .GetAwaiter()
                .GetResult();
        }
    }
}
