using System;
using System.Collections.Generic;
using System.Text;

namespace Spring2.Common.DomainEvent.Tests {
    public class TestEvent {
	public static TestEvent Instance { set; get; }

	public int TheInt { set; get; }
	public string TheString { set; get; }
    }
}
