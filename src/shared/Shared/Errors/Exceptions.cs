namespace Shared.Errors;

public abstract class BusinessException(string message) : Exception(message);

public class ResourceNotFoundException() : BusinessException("Resource not found");
public class ResourceAlreadyExistsException() : BusinessException("Resources already exists");