namespace Cortside.Common.Messages.MessageExceptions {
    public class MissingRequiredFieldError : MessageException {
        readonly string fieldName;

        public MissingRequiredFieldError(string fieldName) : base(string.Format("{0} is required.", fieldName)) {
            this.fieldName = fieldName;
        }

        public MissingRequiredFieldError(string fieldName, string isrequired) : base(string.Format("{0}", fieldName)) {
            this.fieldName = fieldName;
        }

        public string FieldName {
            get {
                return fieldName;
            }
        }
    }
}
