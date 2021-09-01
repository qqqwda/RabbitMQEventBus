using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace EventBus
{
    public class EventBus : IEventBus
    {
        private readonly IServiceProvider _serviceProvider;
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


        public EventBus(IConfiguration config, IServiceProvider serviceProvider)
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
            _serviceProvider = serviceProvider;

        }

        public void Publish(IIntegrationEvent @event, string exchangeName)
        {
            CreateExchangeIfNotExists(exchangeName);

            var jsonEvent = JsonConvert.SerializeObject(@event);
            var bytesEvent = Encoding.UTF8.GetBytes(jsonEvent);

            Channel.BasicPublish(exchangeName,"", body:bytesEvent);

        }

        public void Subscribe<TH, TE>(string exchangeName, string subscriberName)
            where TH : IIntegrationEventHandler<TE>
            where TE : IIntegrationEvent
        {
            BindQueue(exchangeName, subscriberName);

            var consumer = new AsyncEventingBasicConsumer(Channel);

            consumer.Received += async (obj, args) =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var handler = scope.ServiceProvider.GetRequiredService<IIntegrationEventHandler<TE>>();
                    var jsonMessage = Encoding.UTF8.GetString(args.Body.ToArray());

                    var message = JsonConvert.DeserializeObject<TE>(jsonMessage);

                    await handler.HandleAsync(message);

                    /**Acknowledge that message is recieved*/
                    Channel.BasicAck(args.DeliveryTag, false);
                }
            };

            Channel.BasicConsume(subscriberName, false, consumer);
        }


        private void CreateExchangeIfNotExists(string exchangeName)
        {
            Channel.ExchangeDeclare(exchangeName,ExchangeType.Fanout, true);
        }

        private void BindQueue(string exchangeName, string subscriberName)
        {
            CreateExchangeIfNotExists(exchangeName);

            /**durable: true - exclusive: false - autoDelete: false*/
            Channel.QueueDeclare(subscriberName, true, false, false);

            Channel.QueueBind(subscriberName, exchangeName, "");


        }
    }
}
