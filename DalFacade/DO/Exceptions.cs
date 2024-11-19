
namespace DO;

[Serializable]
public class DalDoesNotExistException : Exception
{
    public DalDoesNotExistException(string? message) : base(message) { }
}

[Serializable]
public class DalAlreadyExistsException : Exception
{
    public DalAlreadyExistsException(string? message) : base(message) { }
}
[Serializable]
public class DalDeletionImpossible : Exception
{
    public DalDeletionImpossible(string? message) : base(message) { }
}
[Serializable]
public class DalFormatException : Exception
{
    public DalFormatException(string? message) : base(message) { }
}

[Serializable]
public class DalInvalidOperationException : Exception
{
    public DalInvalidOperationException(string? message) : base(message)
    {
    }
}
[Serializable]
public class ConfigNotFoundException : Exception 
{ 
    public ConfigNotFoundException(string? message) : base(message) {
    }
}
