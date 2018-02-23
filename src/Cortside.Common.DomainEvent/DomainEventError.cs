using System;
using System.Collections.Generic;
using System.Text;

namespace Cortside.Common.DomainEvent
{
    public class DomainEventError
    {
        public string Condition { get; set; }
        public string Description { get; set; }
    }
}
