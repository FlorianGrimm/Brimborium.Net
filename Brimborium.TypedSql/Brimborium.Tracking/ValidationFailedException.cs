﻿namespace Brimborium.Tracking;

[Serializable]
public class ValidationFailedException : InvalidModificationException {
    public ValidationFailedException(string? message) : base(message) {
    }

    public ValidationFailedException(string message, string property, string primaryKey) : base(message, property, primaryKey) {
    }

    public ValidationFailedException(string? message, Exception? innerException) : base(message, innerException) {
    }

    protected ValidationFailedException(
        SerializationInfo serializationInfo,
        StreamingContext streamingContext
        ) : base(serializationInfo, streamingContext) {
    }

    public override void GetObjectData(
        SerializationInfo info,
        StreamingContext context) {
        base.GetObjectData(info, context);
    }
}


