using DemoTienda.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace DemoTienda.Api.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly ProblemDetailsFactory _problemDetailsFactory;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, ProblemDetailsFactory problemDetailsFactory)
        {
            _next = next;
            _logger = logger;
            _problemDetailsFactory = problemDetailsFactory;
        }

        public async Task InvokeAsync(HttpContext context) 
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {

                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var (statusCode, title) = MapExceptionToStatus(exception);

            _logger.LogError(exception, "Error no controlado: {Message}", exception.Message);

            var problemDetails = _problemDetailsFactory.CreateProblemDetails(
                context,
                statusCode: statusCode,
                title: title,
                detail: exception is DomainException ? exception.Message : "Se produjo un error inesperado en el servidor."
            );

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = statusCode;

            await context.Response.WriteAsJsonAsync(problemDetails);
        }

        private static (int StatusCode, string Title) MapExceptionToStatus(Exception exception)
        {
            return exception switch
            {
                CategoriaNoEncontradaException => (StatusCodes.Status404NotFound, "Categoría no encontrada"),
                DomainException => (StatusCodes.Status400BadRequest, "Error de negocio"),
                _ => (StatusCodes.Status500InternalServerError, "Error interno del servidor")
            };
        }

    }
}
