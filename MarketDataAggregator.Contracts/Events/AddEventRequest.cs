using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketDataAggregator.Contracts.Events;

public class AddEventRequest
{
    public required DateTime DateTime { get; set; }

    public required string EventType { get; set; }

    public required string EntityType { get; set; }

    public required string EntityId { get; set; }

    public required string EventData { get; set; }
}
