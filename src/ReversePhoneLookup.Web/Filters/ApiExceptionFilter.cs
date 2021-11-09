using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ReversePhoneLookup.Models;
using ReversePhoneLookup.Models.Exceptions;
using ReversePhoneLookup.Models.Responses;
using System.Collections.Generic;
using System.Net;

namespace ReversePhoneLookup.Web.Filters
{
    public class ApiExceptionFilter : IExceptionFilter
    {
        private static Dictionary<StatusCode, string> CodeMessages = new Dictionary<StatusCode, string>()
        {
            { StatusCode.ServerError, "Internal error" },
            { StatusCode.InvalidPhoneNumber, "Invalid phone number" },
            { StatusCode.NoDataFound, "No data found" },
            { StatusCode.Conflict, "Resource already exists" },
        };

        public void OnException(ExceptionContext context)
        {
            if (context.Exception is ApiException apiException)
            {
                ErrorResponse response = new ErrorResponse()
                {
                    Code = apiException.Code,
                    Message = CodeMessages[apiException.Code]
                };

                context.Result = new ObjectResult(response)
                {
                    StatusCode = (int?)GetHttpStatusCode(response.Code)
                };
            }
        }

        private HttpStatusCode GetHttpStatusCode(StatusCode statusCode)
        {
            switch (statusCode)
            {
                case StatusCode.ServerError: return HttpStatusCode.InternalServerError;
                case StatusCode.NoDataFound: return HttpStatusCode.NotFound;
                case StatusCode.Conflict: return HttpStatusCode.Conflict;
                default: return HttpStatusCode.BadRequest;
            }
        }
    }
}
