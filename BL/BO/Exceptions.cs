﻿namespace BO;

//Lack of permission to access a resource, such as a file or directory
[Serializable]
public class BlUnauthorizedAccessException : Exception
{
    public BlUnauthorizedAccessException(string? message) : base(message) { }
}

[Serializable]
public class BlDoesNotExistException : Exception
{
    public BlDoesNotExistException(string? message) : base(message) { }
    public BlDoesNotExistException(string message, Exception innerException)
                : base(message, innerException) { }
}

//when a required object property is missing its value
[Serializable]
public class BlNullPropertyException : Exception
{
    public BlNullPropertyException(string? message) : base(message) { }
}

[Serializable]
public class BlException : Exception
{
    public BlException(string? message) : base(message) { }
    public BlException(string message, Exception innerException)
                : base(message, innerException) { }
}
[Serializable]
public class BlInvalidOperationException : Exception
{
    public BlInvalidOperationException(string? message) : base(message) { }
}
[Serializable]
public class BlTemporaryNotAvailableException : Exception
{
    public BlTemporaryNotAvailableException(string message) : base(message) { }
}
[Serializable]
public class BlAlreadyExistsException : Exception
{
    public BlAlreadyExistsException(string? message) : base(message) { }
    public BlAlreadyExistsException(string message, Exception innerException)
                : base(message, innerException) { }
}
[Serializable]
public class BlErrorException : Exception
{
    public BlErrorException(string? message) : base(message) { }
    public BlErrorException(string message, Exception innerException)
                : base(message, innerException) { }
}

