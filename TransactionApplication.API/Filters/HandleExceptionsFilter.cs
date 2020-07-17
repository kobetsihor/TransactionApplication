using System;
using System.Linq;
using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using NLog;
using TransactionApplication.BL.Exceptions;

namespace TransactionApplication.API.Filters
{
    public class HandleExceptionsFilter : IExceptionFilter
    {
        protected static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public void OnException(ExceptionContext filterContext)
        {
            if (!TryCreateResponse(filterContext.Exception, out var response))
            {
                return;
            }

            filterContext.Result = response.Item2;
            filterContext.HttpContext.Response.StatusCode = (int)response.Item1;
            filterContext.ExceptionHandled = true;
            _logger.Error(response.Item2);
        }

        private bool TryCreateResponse(Exception exception, out Tuple<HttpStatusCode, ContentResult> response)
        {
            response = null;

            switch (exception.GetType().Name)
            {
                case nameof(InvalidFileException):
                    var invalidModelException = (InvalidFileException)exception;
                    var invalidModel = new InvalidFileException(invalidModelException.Errors);
                    var invalidModelJson = JsonConvert.SerializeObject(invalidModel.Errors, Formatting.Indented);
                    response = CreateErrorResponse(invalidModelJson, contentType: "application/json");

                    break;
                case nameof(ValidationException):
                    var validationException = (ValidationException)exception;
                    var validationJson = JsonConvert.SerializeObject(
                        validationException.Errors.Select(x => new
                        {
                            Item = x.PropertyName.Split('.')[0],
                            PropertyName = x.PropertyName.Split('.')[1],
                            x.ErrorMessage
                        }), Formatting.Indented);
                    response = CreateErrorResponse(validationJson, contentType: "application/json");

                    break;
            }

            return response != null;
        }

        private Tuple<HttpStatusCode, ContentResult> CreateErrorResponse(string message,
                                                                         HttpStatusCode code = HttpStatusCode.BadRequest,
                                                                         string contentType = "text/plain")
        {
            var result = new ContentResult
            {
                Content = message,
                ContentType = contentType
            };
            return new Tuple<HttpStatusCode, ContentResult>(code, result);
        }
    }
}