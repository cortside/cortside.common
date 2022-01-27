namespace Cortside.Common.Security {
    public class SubjectClaim {
        private readonly string type;
        private readonly string value;

        public SubjectClaim(string type, string value) {
            this.type = type;
            this.value = value;
        }

        public string Type { get { return type; } }
        public string Value { get { return value; } }
    }
}
