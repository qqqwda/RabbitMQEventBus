using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventBus
{
    

    public interface IIntegrationEventHandler<in TE>
        where TE : IIntegrationEvent
    {
        Task HandleAsync(TE @event); 
    }
}
