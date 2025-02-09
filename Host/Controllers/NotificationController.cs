using Microsoft.AspNetCore.Mvc;
using SubSonic.Notification;

namespace Host.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificationController
        : ControllerBase
    {
        private readonly ILogger<NotificationController> _logger;
        private readonly INotifier _notifier;

        public NotificationController(ILogger<NotificationController> logger, INotifier notifier)
        {
            _logger = logger;
            _notifier = notifier;
        }

        [HttpPost("BroadCast")]
        [ProducesResponseType(204)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> BroadCast([FromBody]string message)
        {
            using var scope = _logger.BeginScope((nameof(message), message));

            try
            {
                await _notifier.BroadCastAsync(message);

                _logger.LogInformation("Sent out a broadcast message.");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            return Problem();
        }
    }
}
