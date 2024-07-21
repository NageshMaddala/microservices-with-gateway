
using Mango.Services.EmailAPI.Services;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Mango.Services.EmailAPI.Messaging
{
    public class RabbitMQAuthConsumer : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;

        private readonly string _hostName;
        private readonly string _userName;
        private readonly string _password;

        private IConnection? _connection;
        private IModel _channel;

        public RabbitMQAuthConsumer(IConfiguration configuration, EmailService emailService)
        {
            _configuration = configuration;
            _emailService = emailService;

            _hostName = "localhost";
            _password = "guest";
            _userName = "guest";

            var factory = new ConnectionFactory
            {
                HostName = _hostName,
                UserName = _userName,
                Password = _password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            var queueName = _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue");
            _channel.QueueDeclare(queueName, false, false, false, null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            // consume the message in our queue

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, e) =>
            {
                var content = Encoding.UTF8.GetString(e.Body.ToArray());
                string email = JsonConvert.DeserializeObject<string>(content);

                HandleMessage(email).GetAwaiter().GetResult();

                // Sending ack back to the channel
                _channel.BasicAck(e.DeliveryTag, false);
            };

            // Register consumer on the channel on the queue
            _channel.BasicConsume(_configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue"), false, consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(string email)
        {
            _emailService.RegisterUserEmailAndLog(email).GetAwaiter().GetResult();
        }
    }
}
