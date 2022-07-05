namespace Cortside.Common.Messages.MessageExceptions {
    public class MissingRequiredFieldError : MessageException {
        public MissingRequiredFieldError(string fieldName) : base(string.Format("{0} is required.", fieldName), fieldName, null) {
            this.FieldName = fieldName;
        }

        public MissingRequiredFieldError(string fieldName, string isrequired) : base(string.Format("{0}", fieldName), fieldName, null) {
            this.FieldName = fieldName;
        }

        public string FieldName { get; }
    }
}
