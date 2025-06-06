namespace ChatApp.Domain.Exceptions;

public class UserValidationException : DomainException
{
    public UserValidationException(string message) : base(message) { }
    
    public UserValidationException(string message, Exception innerException) : base(message, innerException) { }
} 