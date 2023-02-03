namespace Application.Messages
{
    public interface IRabbitMqService
    {
        public void SendClassMessage<T>(T message);
        public string ReceiveClassMessage();
        public void Close();
    }
}
