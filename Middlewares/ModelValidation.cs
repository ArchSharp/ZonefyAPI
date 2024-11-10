using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Application.Helpers;
using ZonefyDotnet.Helpers;

namespace ZonefyDotnet.Middlewares
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class ValidationError
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Field { get; }

        public string Message { get; }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public ValidationError(string field, string message)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            Field = field != string.Empty ? field : null;
            Message = message;
        }
    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class ValidationResultModel : ErrorResponse<List<ValidationError>>
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public ValidationResultModel(ModelStateDictionary modelState)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            Message = "Validation Failed";
            Error = modelState.Keys
                .SelectMany(key => modelState[key].Errors.Select(x => new ValidationError(key, x.ErrorMessage)))
                .ToList();
        }
    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class ValidationFailedResult : ObjectResult
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public ValidationFailedResult(ModelStateDictionary modelState)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
            : base(new ValidationResultModel(modelState))
        {
            StatusCode = StatusCodes.Status400BadRequest;
        }
    }
}