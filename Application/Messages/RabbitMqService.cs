using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Application.Messages
{
    /**
    * @Project ASP.NET Core 7.0
    * @Author: Nguyen Xuan Nhan
    * @Team: 4FT
    * @Copyright (C) 2023 4FT. All rights reserved
    * @License MIT
    * @Create date Mon 23 Jan 2023 00:00:00 AM +07
    */
    
    public class RabbitMqService : IRabbitMqService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _queueName;

        public RabbitMqService(string queueName)
        {
            _queueName = queueName;
            var factory = new ConnectionFactory() { HostName = "127.0.0.1" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: _queueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }

        public void SendClassMessage<T>(T message)
        {
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            _channel.BasicPublish(exchange: "",
                routingKey: _queueName,
                basicProperties: null,
                body: body);
        }

        public string ReceiveClassMessage()
        {
            var result = _channel.BasicGet(_queueName, true);
            return result == null ? "" : Encoding.UTF8.GetString(result.Body.ToArray());
        }

        public void Close() => _connection.Close();
    }
}
