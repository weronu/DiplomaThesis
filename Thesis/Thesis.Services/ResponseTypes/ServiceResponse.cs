using System;
using System.Collections.Generic;

namespace Thesis.Services.ResponseTypes
{
    public class ServiceResponse
    {
        public bool Succeeded { get; internal set; }
        public List<string> Errors { get; internal set; }
        public List<string> InfoMessages { get; internal set; }
        public List<string> ValidationErrors { get; internal set; }
        public List<string> SuccessMessages { get; internal set; }
        public Guid? ExceptionId { get; internal set; }

        public ServiceResponse()
        {
            Errors = new List<string>();
            InfoMessages = new List<string>();
            ValidationErrors = new List<string>();
        }

        public void AddError(string errorMessage)
        {
            Errors.Add(errorMessage);
        }

        public void AddInfo(string infoMessage)
        {
            InfoMessages.Add(infoMessage);
        }

        public void AddSuccessMessage(string infoMessage)
        {
            InfoMessages.Add(infoMessage);
        }
    }
}
