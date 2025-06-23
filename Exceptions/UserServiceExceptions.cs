namespace GymTracker.UserService.Exceptions;

public class UserAlreadyExistsException : Exception
{
    public UserAlreadyExistsException(string email)
        : base($"User with email '{email}' already exists") { }
}

public class UserNotFoundException : Exception
{
    public UserNotFoundException(Guid id)
        : base($"User with ID '{id}' not found") { }
}

public class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException()
        : base("Invalid email or password") { }
}