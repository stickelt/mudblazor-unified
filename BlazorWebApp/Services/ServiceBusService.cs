using Azure.Messaging.ServiceBus;
using System.Text.Json;

namespace BlazorWebApp.Services
{
    /// <summary>
    /// Service for interacting with Azure Service Bus queues and topics
    /// </summary>
    public class ServiceBusService
    {
        private readonly ILogger<ServiceBusService> _logger;
        private readonly ServiceBusClient _client;
        private readonly string _connectionString;

        // Default queue and topic names
        public const string DefaultQueueName = "stone-queue";
        public const string DefaultTopicName = "stone-topic";
        public const string DefaultSubscriptionName = "shipper-sub";

        public ServiceBusService(IConfiguration configuration, ILogger<ServiceBusService> logger)
        {
            _logger = logger;

            try
            {
                _connectionString = configuration["ServiceBus:ConnectionString"];

                if (string.IsNullOrEmpty(_connectionString))
                {
                    throw new ArgumentNullException("ServiceBus:ConnectionString",
                        "Service Bus connection string is not configured in appsettings.json");
                }

                _logger.LogInformation("Connection string retrieved from configuration");

                // Create the client
                _client = new ServiceBusClient(_connectionString);
                _logger.LogInformation("ServiceBusClient created successfully");

                _logger.LogInformation("ServiceBusService initialized successfully");

                // Log to console for immediate visibility
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("*******************************************************************");
                Console.WriteLine("*** SERVICE BUS SERVICE INITIALIZED SUCCESSFULLY ***");
                Console.WriteLine("*******************************************************************");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing ServiceBusService");

                // Log to console for immediate visibility
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("*******************************************************************");
                Console.WriteLine("*** ERROR INITIALIZING SERVICE BUS SERVICE ***");
                Console.WriteLine($"*** EXCEPTION: {ex.GetType().Name}: {ex.Message} ***");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"*** INNER EXCEPTION: {ex.InnerException.GetType().Name}: {ex.InnerException.Message} ***");
                }
                Console.WriteLine("*******************************************************************");
                Console.ResetColor();

                throw; // Rethrow to fail fast
            }
        }

        /// <summary>
        /// Sends a message to a Service Bus queue
        /// </summary>
        public async Task SendToQueueAsync(string queueName, string messageContent)
        {
            try
            {
                _logger.LogInformation("Sending message to queue {QueueName}", queueName);

                // Create a sender for the queue
                _logger.LogInformation("Creating sender for queue {QueueName}", queueName);
                var sender = _client.CreateSender(queueName);
                _logger.LogInformation("Sender created successfully");

                // Create a message with the content
                _logger.LogInformation("Creating message with content: {MessageContent}", messageContent);
                var message = new ServiceBusMessage(messageContent)
                {
                    ContentType = "text/plain",
                    Subject = $"Message sent at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}"
                };
                _logger.LogInformation("Message created successfully");

                // Send the message
                _logger.LogInformation("Sending message to queue {QueueName}", queueName);

                // Also log to console for immediate visibility
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("*******************************************************************");
                Console.WriteLine($"*** SENDING MESSAGE TO QUEUE: {queueName} ***");
                Console.WriteLine($"*** MESSAGE: {messageContent} ***");
                Console.WriteLine("*******************************************************************");
                Console.ResetColor();

                await sender.SendMessageAsync(message);

                _logger.LogInformation("Message sent to queue {QueueName}: {MessageContent}",
                    queueName, messageContent);

                // Also log to console for immediate visibility
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("*******************************************************************");
                Console.WriteLine($"*** MESSAGE SENT SUCCESSFULLY TO QUEUE: {queueName} ***");
                Console.WriteLine("*******************************************************************");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message to queue {QueueName}. Exception: {ExceptionType}, {ExceptionMessage}",
                    queueName, ex.GetType().Name, ex.Message);

                if (ex.InnerException != null)
                {
                    _logger.LogError("Inner exception: {InnerExceptionType}, {InnerExceptionMessage}",
                        ex.InnerException.GetType().Name, ex.InnerException.Message);
                }

                // Also log to console for immediate visibility
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("*******************************************************************");
                Console.WriteLine($"*** ERROR IN ServiceBusService.SendToQueueAsync ***");
                Console.WriteLine($"*** QUEUE: {queueName} ***");
                Console.WriteLine($"*** EXCEPTION: {ex.GetType().Name}: {ex.Message} ***");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"*** INNER EXCEPTION: {ex.InnerException.GetType().Name}: {ex.InnerException.Message} ***");
                }
                Console.WriteLine($"*** STACK TRACE: {ex.StackTrace} ***");
                Console.WriteLine("*******************************************************************");
                Console.ResetColor();

                throw;
            }
        }

        /// <summary>
        /// Sends a message to a Service Bus topic
        /// </summary>
        public async Task SendToTopicAsync(string topicName, string messageContent)
        {
            try
            {
                _logger.LogInformation("Sending message to topic {TopicName}", topicName);

                // Create a sender for the topic
                var sender = _client.CreateSender(topicName);

                // Create a message with the content
                var message = new ServiceBusMessage(messageContent)
                {
                    ContentType = "text/plain",
                    Subject = $"Message sent at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}"
                };

                // Send the message
                await sender.SendMessageAsync(message);

                _logger.LogInformation("Message sent to topic {TopicName}: {MessageContent}",
                    topicName, messageContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message to topic {TopicName}", topicName);
                throw;
            }
        }

        /// <summary>
        /// Receives a message from a Service Bus queue
        /// </summary>
        public async Task<string?> ReceiveFromQueueAsync(string queueName)
        {
            try
            {
                _logger.LogInformation("Receiving message from queue {QueueName}", queueName);

                // Create a receiver for the queue
                var receiver = _client.CreateReceiver(queueName);

                // Receive a message with a timeout
                var message = await receiver.ReceiveMessageAsync(TimeSpan.FromSeconds(5));

                if (message == null)
                {
                    _logger.LogInformation("No message available in queue {QueueName}", queueName);
                    return null;
                }

                // Get the message content
                var messageContent = message.Body.ToString();

                // Complete the message (remove it from the queue)
                await receiver.CompleteMessageAsync(message);

                _logger.LogInformation("Message received from queue {QueueName}: {MessageContent}",
                    queueName, messageContent);

                return messageContent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error receiving message from queue {QueueName}", queueName);
                throw;
            }
        }

        /// <summary>
        /// Receives a message from a Service Bus topic subscription
        /// </summary>
        public async Task<string?> ReceiveFromSubscriptionAsync(string topicName, string subscriptionName)
        {
            try
            {
                _logger.LogInformation("Receiving message from topic {TopicName}, subscription {SubscriptionName}",
                    topicName, subscriptionName);

                // Create a receiver for the subscription
                var receiver = _client.CreateReceiver(topicName, subscriptionName);

                // Receive a message with a timeout
                var message = await receiver.ReceiveMessageAsync(TimeSpan.FromSeconds(5));

                if (message == null)
                {
                    _logger.LogInformation("No message available in topic {TopicName}, subscription {SubscriptionName}",
                        topicName, subscriptionName);
                    return null;
                }

                // Get the message content
                var messageContent = message.Body.ToString();

                // Complete the message (remove it from the subscription)
                await receiver.CompleteMessageAsync(message);

                _logger.LogInformation("Message received from topic {TopicName}, subscription {SubscriptionName}: {MessageContent}",
                    topicName, subscriptionName, messageContent);

                return messageContent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error receiving message from topic {TopicName}, subscription {SubscriptionName}",
                    topicName, subscriptionName);
                throw;
            }
        }
    }
}
