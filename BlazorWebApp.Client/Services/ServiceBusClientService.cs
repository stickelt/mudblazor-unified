using System.Net.Http.Json;
using System.Text.Json;

namespace BlazorWebApp.Client.Services
{
    /// <summary>
    /// Client-side service for interacting with Azure Service Bus through the API
    /// </summary>
    public class ServiceBusClientService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        // Expose the HttpClient for direct access
        public HttpClient HttpClient => _httpClient;

        // Default queue and topic names
        public const string DefaultQueueName = "stone-queue";
        public const string DefaultTopicName = "stone-topic";
        public const string DefaultSubscriptionName = "shipper-sub";

        public ServiceBusClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        /// <summary>
        /// Sends a message to a Service Bus queue
        /// </summary>
        public async Task<ServiceBusResponse> SendToQueueAsync(string message, string? queueName = null)
        {
            try
            {
                // Use default queue name if not provided
                var queue = string.IsNullOrEmpty(queueName) ? DefaultQueueName : queueName;

                // Send the message to the queue
                var response = await _httpClient.PostAsJsonAsync($"api/ServiceBus/queue/send?queueName={queue}", message);

                // Ensure the request was successful
                response.EnsureSuccessStatusCode();

                // Parse the response
                var result = await response.Content.ReadFromJsonAsync<ServiceBusResponse>(_jsonOptions);
                return result ?? new ServiceBusResponse { Success = true, Message = "Message sent (no details returned)" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message to queue: {ex.Message}");
                return new ServiceBusResponse { Success = false, Message = ex.Message };
            }
        }

        /// <summary>
        /// Sends a message to a Service Bus topic
        /// </summary>
        public async Task<ServiceBusResponse> SendToTopicAsync(string message, string? topicName = null)
        {
            try
            {
                // Use default topic name if not provided
                var topic = string.IsNullOrEmpty(topicName) ? DefaultTopicName : topicName;

                // Send the message to the topic
                var response = await _httpClient.PostAsJsonAsync($"api/ServiceBus/topic/send?topicName={topic}", message);

                // Ensure the request was successful
                response.EnsureSuccessStatusCode();

                // Parse the response
                var result = await response.Content.ReadFromJsonAsync<ServiceBusResponse>(_jsonOptions);
                return result ?? new ServiceBusResponse { Success = true, Message = "Message sent (no details returned)" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message to topic: {ex.Message}");
                return new ServiceBusResponse { Success = false, Message = ex.Message };
            }
        }

        /// <summary>
        /// Receives a message from a Service Bus queue
        /// </summary>
        public async Task<ServiceBusResponse> ReceiveFromQueueAsync(string? queueName = null)
        {
            try
            {
                // Use default queue name if not provided
                var queue = string.IsNullOrEmpty(queueName) ? DefaultQueueName : queueName;

                // Receive a message from the queue
                var response = await _httpClient.GetAsync($"api/ServiceBus/queue/receive?queueName={queue}");

                // Ensure the request was successful
                response.EnsureSuccessStatusCode();

                // Parse the response
                var result = await response.Content.ReadFromJsonAsync<ServiceBusResponse>(_jsonOptions);
                return result ?? new ServiceBusResponse { Success = true, Message = "No message available" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving message from queue: {ex.Message}");
                return new ServiceBusResponse { Success = false, Message = ex.Message };
            }
        }

        /// <summary>
        /// Receives a message from a Service Bus topic subscription
        /// </summary>
        public async Task<ServiceBusResponse> ReceiveFromSubscriptionAsync(
            string? topicName = null,
            string? subscriptionName = null)
        {
            try
            {
                // Use default topic and subscription names if not provided
                var topic = string.IsNullOrEmpty(topicName) ? DefaultTopicName : topicName;
                var subscription = string.IsNullOrEmpty(subscriptionName) ? DefaultSubscriptionName : subscriptionName;

                // Receive a message from the subscription
                var response = await _httpClient.GetAsync(
                    $"api/ServiceBus/subscription/receive?topicName={topic}&subscriptionName={subscription}");

                // Ensure the request was successful
                response.EnsureSuccessStatusCode();

                // Parse the response
                var result = await response.Content.ReadFromJsonAsync<ServiceBusResponse>(_jsonOptions);
                return result ?? new ServiceBusResponse { Success = true, Message = "No message available" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving message from subscription: {ex.Message}");
                return new ServiceBusResponse { Success = false, Message = ex.Message };
            }
        }
    }

    /// <summary>
    /// Response from the Service Bus API
    /// </summary>
    public class ServiceBusResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
