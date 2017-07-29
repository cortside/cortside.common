using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spring2.Common.DomainEvent {
    public class ServiceBusSettings {
	public string AppName { set; get; }
	public string Address { set; get; }
	public string Protocol { set; get; }
	public string PolicyName { set; get; }
	public string Key { set; get; }
	public string Namespace { set; get; }
	public int Credits { set; get; } = 10;
    }
}
