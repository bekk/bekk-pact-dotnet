using Bekk.Pact.Common.Contracts;
using Bekk.Pact.Common.Extensions;
using Newtonsoft.Json.Linq;

namespace Bekk.Pact.Provider.Model.Validation
{
    class ResponseBodyJsonValidator
    {
        private readonly IProviderConfiguration configuration;
        public ResponseBodyJsonValidator(IProviderConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public string Validate(JContainer expected, string actual)
        {
            switch (expected)
            {
                case JObject o: return ValidateBodyAsObject(actual, o);
                case JArray a: return ValidateBodyAsArray(actual, a);
            }
            return null;
        }
        private string ValidateBodyAsObject(string actual, JObject expected)
        {
            var actualJson = JObject.Parse(actual);
            foreach (var token in expected.AsJEnumerable())
            {
                var actualToken = actualJson.GetValue(token.Path, configuration.BodyKeyStringComparison);
                var expectedValue = expected.GetValue(token.Path);
                if (actualToken.IsNull() && !expectedValue.IsNull()) return $"Cannot find {token.Path} in body.";
                if (!(actualToken.IsNull() && expectedValue.IsNull()) && !JToken.DeepEquals(actualToken, expectedValue))
                {
                    return $"Not match at {token.Path} in body. Expected: {token} but received {actualToken}.";
                }
            }
            return null;
        }

        private string ValidateBodyAsArray(string actual, JArray expected)
        {
            var actualJson = JArray.Parse(actual);
            if (!expected.HasValues)
            {
                if (actualJson.HasValues)
                {
                    return $"Array is supposed to be empty at {expected.Path} in body.";
                }
                return null;
            }
            if (!actualJson.HasValues)
            {
                return $"Array is not supposed to be empty at {expected.Path} in body.";
            }
            var e = expected.GetEnumerator();
            var a = actualJson.GetEnumerator();
            while (e.MoveNext())
            {
                if (!a.MoveNext()) return $"Element not found in array. Expected {e.Current} at {expected.Path}.";
                var error = Validate(a.Current, e.Current);
                if (error != null) return error;
            }
            if (a.MoveNext()) return $"Unexpected element found in array {a.Current} at {expected.Path}.";
            return null;
        }
        private string Validate(JToken actual, JToken expected)
        {
            if (actual.IsNull() && !expected.IsNull()) return $"Cannot find {actual.Path} in body.";
            if (!(actual.IsNull() && expected.IsNull()) && !JToken.DeepEquals(actual, expected))
            {
                return $"Not match at {expected.Path} in body. Expected: {expected} but received {actual}.";
            }
            return null;
        }
    }
}