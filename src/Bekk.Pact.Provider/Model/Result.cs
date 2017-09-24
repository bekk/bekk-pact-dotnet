using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Bekk.Pact.Common.Contracts;

namespace Bekk.Pact.Provider.Model
{
    class Result : ITestResult
    {
        private readonly List<string> errors;
        private readonly Response expected;

        public Result(string title, Response expected) : this(title, ValidationTypes.None)
        {
            this.expected = expected;
        }
        public Result(string title, ValidationTypes types, params string[] errors)
        {
            this.errors = errors.ToList(); ;
            ErrorTypes = types;
            Title = title;
        }

        public bool Success => ErrorTypes == ValidationTypes.None;

        public void Add(ValidationTypes type, string error)
        {
            if (!string.IsNullOrWhiteSpace(error))
            {
                ErrorTypes |= type;
                errors.Add(error);
            }
        }

        public void Add(ValidationTypes type, IEnumerable<string> errors)
        {
            if (errors != null)
            {
                foreach (var error in errors)
                {
                    Add(type, error);
                }
            }
        }
        public void Add(HttpResponseMessage response)
        {
            ActualResponse = response;
        }

        public ValidationTypes ErrorTypes { get; private set; }
        public string Title { get; }

        public string ExpectedResponseBody => expected.Body.ToString();

        public HttpResponseMessage ActualResponse { get; private set; }

        public override string ToString() => Success ? $"Ok ({Title})" : 
            string.Concat(
                Title,
                Environment.NewLine,
                new string('-', 40),
                Environment.NewLine,
                string.Join(Environment.NewLine, errors));

    }
}