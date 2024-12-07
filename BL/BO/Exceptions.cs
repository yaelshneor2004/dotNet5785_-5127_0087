namespace BO;
[Serializable]
public class BlDoesNotExistException : Exception
{

}

//Lack of permission to access a resource, such as a file or directory
[Serializable]
public class BlUnauthorizedAccessException : Exception
{
    public BlUnauthorizedAccessException(string? message) : base(message) { }
    public BlUnauthorizedAccessException(string message, Exception innerException)
                : base(message, innerException) { }
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



