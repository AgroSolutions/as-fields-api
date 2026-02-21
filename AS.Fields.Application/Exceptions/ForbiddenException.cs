using Microsoft.AspNetCore.Http;

namespace AS.Fields.Application.Exceptions
{
    public class ForbiddenException(string message = "Recurso não permitido.") : BaseCustomException(StatusCodes.Status403Forbidden, message)
    {
    }
}
