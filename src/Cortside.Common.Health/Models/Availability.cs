namespace Cortside.Health.Models {
    public class Availability {
        public int Count { get; set; }
        public int Success { get; set; }
        public int Failure { get; set; }
        public double Uptime { get; set; }
        public long TotalDuration { get; set; }
        public double AverageDuration { get; set; }
    }
}
