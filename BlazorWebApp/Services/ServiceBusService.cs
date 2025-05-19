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
            _connectionString = configuration["ServiceBus:ConnectionString"] ?? 
                throw new ArgumentNullException("ServiceBus:ConnectionString", 
                    "Service Bus connection string is not configured");
            
            _client = new ServiceBusClient(_connectionString);
            
            _logger.LogInformation("ServiceBusService initialized");
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
                var sender = _client.CreateSender(queueName);
                
                // Create a message with the content
                var message = new ServiceBusMessage(messageContent)
                {
                    ContentType = "text/plain",
                    Subject = $"Message sent at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}"
                };
                
                // Send the message
                await sender.SendMessageAsync(message);
                
                _logger.LogInformation("Message sent to queue {QueueName}: {MessageContent}", 
                    queueName, messageContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message to queue {QueueName}", queueName);
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
