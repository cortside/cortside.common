using System;

namespace Spring2.Common.Data.Exceptions {

    public class EntityExistsException : Exception {

	public EntityExistsException() {
	}

	public EntityExistsException(string message)
	: base(message) {
	}

	public EntityExistsException(string message, Exception inner)
	: base(message, inner) {
	}

	protected EntityExistsException(System.Runtime.Serialization.SerializationInfo info,
	System.Runtime.Serialization.StreamingContext context) { }
    }
}