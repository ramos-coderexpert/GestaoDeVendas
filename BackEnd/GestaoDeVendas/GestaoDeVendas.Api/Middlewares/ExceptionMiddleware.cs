using GestaoDeVendas.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace GestaoDeVendas.Api.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";

                var errorResponse = new
                {
                    Message = error.Message,
                    Success = false
                };

                switch (error)
                {
                    case VendaException:
                    case ClienteException:
                    case ProdutoException:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    case KeyNotFoundException e:
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    default:
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        errorResponse = new
                        {
                            Message = "Ocorreu um erro interno no servidor.",
                            Success = false
                        };
                        break;
                }

                var result = JsonSerializer.Serialize(errorResponse);
                await response.WriteAsync(result);
            }
        }
    }
}