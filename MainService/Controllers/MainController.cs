using Common;
using EventBus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MainService.Controllers
{
    [ApiController]
    [Route("main")]
    public class MainController : ControllerBase
    {
        
        private readonly ILogger<MainController> _logger;
        private readonly IEventBus _eventBus;

        public MainController(ILogger<MainController> logger, IEventBus eventBus)
        {
            _logger = logger;
            _eventBus = eventBus;
        }

        [HttpPost]
        public void SendMessageAsync([FromBody] MainIntegrationEvent message)
        {
            _eventBus.Publish(message, nameof(MainIntegrationEvent));
        }
    }
}
