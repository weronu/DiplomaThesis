using System;
using System.Collections.Generic;

namespace Thesis.Services.ResponseTypes
{
    public class ServiceResponse
    {
        public bool Succeeded { get; internal set; }
        public string Error { get; internal set; }
        public string InfoMessage { get; internal set; }
        public string ValidationError { get; internal set; }
        public string SuccessMessage { get; internal set; }
        public Guid? ExceptionId { get; internal set; }
    }
}
