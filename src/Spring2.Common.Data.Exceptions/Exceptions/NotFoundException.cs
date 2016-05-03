using System;

namespace Spring2.Common.Data.Exceptions {

    public class NotFoundException : Exception {

	public NotFoundException() {
	}

	public NotFoundException(string message)
	: base(message) {
	}

	public NotFoundException(string message, Exception inner)
	: base(message, inner) {
	}

	protected NotFoundException(System.Runtime.Serialization.SerializationInfo info,
	System.Runtime.Serialization.StreamingContext context) { }
    }
}