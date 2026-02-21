using Microsoft.AspNetCore.Http;

namespace AS.Fields.Application.Exceptions
{
    public class NotFoundException(string message = "Recurso não encontrado.") : BaseCustomException(StatusCodes.Status404NotFound, message)
    {
    }
}
