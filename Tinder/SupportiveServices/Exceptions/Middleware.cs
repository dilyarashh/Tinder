using System.Net;
using System.Text.Json;
using Tinder.DBContext.Models;

namespace Tinder.SupportiveServices.Exceptions;

public class Middleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext db)
        {
            try
            {
                await next(db);
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(db, exception);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var statusCode = (int)HttpStatusCode.InternalServerError;
            var message = "Internal Server Error";

            switch (exception)
            {
                case BadRequestException:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    message = !string.IsNullOrEmpty(exception.Message) ? exception.Message : "Плохой запрос";
                    break;
                case UnauthorizedException:
                    statusCode = (int)HttpStatusCode.Unauthorized;
                    message = !string.IsNullOrEmpty(exception.Message) ? exception.Message : "Данный пользователь не авторизован";
                    break;
                case NotFoundException:
                    statusCode = (int)HttpStatusCode.NotFound;
                    message = !string.IsNullOrEmpty(exception.Message) ? exception.Message : "Не найдено";
                    break;
            }
            response.StatusCode = statusCode;

            var error = new Error
            {
                StatusCode = statusCode.ToString(),
                Message = !string.IsNullOrEmpty(message) ? message : "Внутренняя ошибка сервера"
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(error, options);

            await response.WriteAsync(json);
        }
    }