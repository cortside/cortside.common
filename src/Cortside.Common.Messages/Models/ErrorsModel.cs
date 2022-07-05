using System.Collections.Generic;

namespace Cortside.Common.Messages.Models {
    /// <summary>
    /// Errors Model
    /// </summary>
    public class ErrorsModel {
        /// <summary>
        /// Errors model constructor
        /// </summary>
        public ErrorsModel() {
            Errors = new List<ErrorModel>();
        }

        /// <summary>
        /// Errors List
        /// </summary>
        public List<ErrorModel> Errors { get; set; }

        /// <summary>
        /// Adds error model
        /// </summary>
        /// <param name="type"></param>
        /// <param name="message"></param>
        public void AddError(string type, string field, string message, string description, List<MessageException> fields) {
            var error = new ErrorModel() {
                Type = type,
                Field = field,
                Message = message,
                Description = description
            };

            foreach (var f in fields ?? new List<MessageException>()) {
                error.Fields.Add(new ErrorModel() {
                    Type = f.GetType().Name,
                    Field = f.Field,
                    Message = f.Message,
                    Description = f.Description
                });
            }

            Errors.Add(error);
        }
    }
}
