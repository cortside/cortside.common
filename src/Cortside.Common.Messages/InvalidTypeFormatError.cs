namespace Cortside.Common.Messages {
    public class InvalidTypeFormatError : Message {
        readonly string property = null;
        readonly string value = null;

        public InvalidTypeFormatError(string property, string value) : base(string.Format("{1} is not a valid value for {0}.", property, value)) {
            this.property = property;
            this.value = value;
        }

        public string Property {
            get {
                return property;
            }
        }

        public string Value {
            get {
                return value;
            }
        }
    }
}
