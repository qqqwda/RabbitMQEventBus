using EventBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class MainIntegrationEvent : IIntegrationEvent
    {
        public string Value { get; set; }
    }
}
