using System;
using System.Collections.Generic;
using System.Linq;
using Bekk.Pact.Common.Contracts;

namespace Bekk.Pact.Provider.Model
{
    class Result : ITestResult
    {
        private readonly List<string> errors;

        public Result():this(ValidationTypes.None)
        {
            
        }
        public Result(ValidationTypes types, params string[] errors)
        {
            this.errors = errors.ToList();;
            ErrorTypes = types;
        }

        public bool Success => ErrorTypes == ValidationTypes.None;

        public void Add(ValidationTypes type, string error)
        {
            if(!string.IsNullOrWhiteSpace(error)){
                ErrorTypes |= type;
                errors.Add(error);
            }
        }

        public void Add(ValidationTypes type, IEnumerable<string> errors)
        {
            if(errors != null){
                foreach(var error in errors){
                    Add(type, error);
                }
            }
        }

        public ValidationTypes ErrorTypes { get; private set; }

        public override string ToString() => Success ? "Ok" : string.Join(Environment.NewLine, errors);
    }
}