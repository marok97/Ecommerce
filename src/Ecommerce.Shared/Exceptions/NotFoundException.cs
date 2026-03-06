using System.Net;

namespace Ecommerce.Shared.Exceptions;

public class NotFoundException(string message, HttpStatusCode statusCode = HttpStatusCode.NotFound)
    : BaseException(message, statusCode);
