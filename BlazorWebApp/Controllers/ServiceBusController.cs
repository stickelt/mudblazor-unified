using BlazorWebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlazorWebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceBusController : ControllerBase
    {
        private readonly ServiceBusService _serviceBusService;
        private readonly ILogger<ServiceBusController> _logger;

        public ServiceBusController(ServiceBusService serviceBusService, ILogger<ServiceBusController> logger)
        {
            _serviceBusService = serviceBusService;
            _logger = logger;
        }

        [HttpGet("test-connection")]
        public IActionResult TestConnection()
        {
            try
            {
                // Log the test
                _logger.LogInformation("Testing Service Bus connection");

                // Also log to console for immediate visibility
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("*******************************************************************");
                Console.WriteLine("*** TESTING SERVICE BUS CONNECTION ***");
                Console.WriteLine("*******************************************************************");
                Console.ResetColor();

                return Ok(new { success = true, message = "Connection string is configured" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing Service Bus connection");

                // Also log to console for immediate visibility
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("*******************************************************************");
                Console.WriteLine("*** ERROR TESTING SERVICE BUS CONNECTION ***");
                Console.WriteLine($"*** EXCEPTION: {ex.GetType().Name}: {ex.Message} ***");
                Console.WriteLine("*******************************************************************");
                Console.ResetColor();

                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("queue/send")]
        public async Task<IActionResult> SendToQueue([FromQuery] string? queueName, [FromBody] string message)
        {
            try
            {
                // Use default queue name if not provided
                var queue = string.IsNullOrEmpty(queueName) ? ServiceBusService.DefaultQueueName : queueName;

                _logger.LogInformation("API: Sending message to queue {QueueName}", queue);
                await _serviceBusService.SendToQueueAsync(queue, message);

                return Ok(new { success = true, message = $"Message sent to queue '{queue}'" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "API: Error sending message to queue");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("topic/send")]
        public async Task<IActionResult> SendToTopic([FromQuery] string? topicName, [FromBody] string message)
        {
            try
            {
                // Use default topic name if not provided
                var topic = string.IsNullOrEmpty(topicName) ? ServiceBusService.DefaultTopicName : topicName;

                _logger.LogInformation("API: Sending message to topic {TopicName}", topic);
                await _serviceBusService.SendToTopicAsync(topic, message);

                return Ok(new { success = true, message = $"Message sent to topic '{topic}'" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "API: Error sending message to topic");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("queue/receive")]
        public async Task<IActionResult> ReceiveFromQueue([FromQuery] string? queueName)
        {
            try
            {
                // Use default queue name if not provided
                var queue = string.IsNullOrEmpty(queueName) ? ServiceBusService.DefaultQueueName : queueName;

                _logger.LogInformation("API: Receiving message from queue {QueueName}", queue);
                var message = await _serviceBusService.ReceiveFromQueueAsync(queue);

                if (message == null)
                {
                    return Ok(new { success = true, message = "(No message available)" });
                }

                return Ok(new { success = true, message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "API: Error receiving message from queue");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("subscription/receive")]
        public async Task<IActionResult> ReceiveFromSubscription(
            [FromQuery] string? topicName,
            [FromQuery] string? subscriptionName)
        {
            try
            {
                // Use default topic and subscription names if not provided
                var topic = string.IsNullOrEmpty(topicName) ? ServiceBusService.DefaultTopicName : topicName;
                var subscription = string.IsNullOrEmpty(subscriptionName) ?
                    ServiceBusService.DefaultSubscriptionName : subscriptionName;

                _logger.LogInformation("API: Receiving message from topic {TopicName}, subscription {SubscriptionName}",
                    topic, subscription);

                var message = await _serviceBusService.ReceiveFromSubscriptionAsync(topic, subscription);

                if (message == null)
                {
                    return Ok(new { success = true, message = "(No message available)" });
                }

                return Ok(new { success = true, message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "API: Error receiving message from subscription");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
