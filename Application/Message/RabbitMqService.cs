using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Application.Message
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
        private readonly string _queueName;
        private readonly IModel _channel;

        public RabbitMqService(string queueName)
        {
            _queueName = queueName;
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "admin",
                Password = "Abc@1234",
                VirtualHost = "/"
            };
            var conn = factory.CreateConnection();
            _channel = conn.CreateModel();
            _channel.QueueDeclare(_queueName, true);

        }

        public void SendMessage<T>(T message)
        {
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            _channel.BasicPublish("", _queueName, null, body);
        }
    }
}
