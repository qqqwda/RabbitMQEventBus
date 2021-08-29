using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus
{
    public class EventBus : IEventBus
    {
        private readonly IConnection _connection;
        private IModel _channel;

        /**Singleton for channel
        */
        public IModel Channel
        {
            get
            {
                if (_channel == null)
                    _channel = _connection.CreateModel();

                return _channel;
            }
        }
        public EventBus(IConfiguration config)
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = config["EventBus:HostName"],
                Port = int.Parse(config["EventBus:Port"]),
                UserName = config["EventBus:UserName"],
                Password = config["EventBus:Password"],
                DispatchConsumersAsync = true

            };

            _connection = connectionFactory.CreateConnection();

        }

        public void Publish(IIntegrationEvent @event, string exchangeName)
        {
            CreateExchangeIfNotExists(exchangeName);

            var jsonEvent = JsonConvert


        }

        public void Subscribe<TH, TE>(string exchangeName, string subscriberName)
            where TH : IIntegrationEventHandler<TE>
            where TE : IIntegrationEvent
        {
            throw new NotImplementedException();
        }

        private void CreateExchangeIfNotExists(string exchangeName)
        {
            Channel.ExchangeDeclare(exchangeName,ExchangeType.Fanout, true);
        }
    }
}
