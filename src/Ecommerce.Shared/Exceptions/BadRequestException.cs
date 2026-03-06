using System.Net;

namespace Ecommerce.Shared.Exceptions;

public class BadRequestException(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    : BaseException(message, statusCode);
