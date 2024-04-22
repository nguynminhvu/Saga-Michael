
namespace SagaPatternMichael.Order.RabbitMQ
{
    public class MessageConsume : BackgroundService, IDisposable
    {
        private MessageConnection _messageConnection;

        public MessageConsume(MessageConnection messageConnection)
        {
            _messageConnection= messageConnection;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
           //_channel= _messageConnection.GetConnection()
           await Task.CompletedTask; 
        }
    }
}
