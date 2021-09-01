using Common;
using EventBus;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ChildService.IntegrationEvents
{
    public class MainIntegrationEventHandler : IIntegrationEventHandler<MainIntegrationEvent>
    {
        private readonly ILogger<MainIntegrationEventHandler> _logger;
        public MainIntegrationEventHandler(ILogger<MainIntegrationEventHandler> logger)
        {
            _logger = logger;
        }
        public Task HandleAsync(MainIntegrationEvent @event)
        {
            _logger.LogInformation($"Accepted event: {@event.Value}");
            return Task.CompletedTask;
        }
    }
}
