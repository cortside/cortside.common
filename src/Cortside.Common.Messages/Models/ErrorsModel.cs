using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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

        public ErrorsModel(ModelStateDictionary modelState) {
            Errors = modelState.Keys
                        .SelectMany(key => modelState[key].Errors.Select(x => new ErrorModel(x.Exception?.GetType()?.Name ?? "ModelStateValidation", key, x.ErrorMessage)))
                        .ToList();
        }

        public ErrorsModel(Exception ex) {
            Errors = new List<ErrorModel>() { new ErrorModel(ex) };
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
        public void AddError(MessageException message) {
            var error = new ErrorModel() {
                Type = message.GetType().Name,
                Property = message.Property,
                Message = message.Message
            };

            Errors.Add(error);
        }
    }
}
