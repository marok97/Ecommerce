using System.Net;

namespace Ecommerce.Shared.Exceptions;

public class ConflictException(string message, HttpStatusCode statusCode = HttpStatusCode.Conflict)
    : BaseException(message, statusCode);
