using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Bekk.Pact.Common.Contracts;
using Bekk.Pact.Common.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bekk.Pact.Provider.Model
{
    class InteractionPact : IPact
    {
        private readonly Interaction interaction;

        public InteractionPact(Interaction interaction, IProviderConfiguration config)
        {
            this.interaction = interaction;
            Configuration = config;
        }
        public string ProviderState => interaction.ProviderState;

        public async Task<ITestResult> Assert(HttpClient client)
        {
            Configuration.LogSafe(LogLevel.Verbose, $"Pact: {ProviderState}");
            Configuration.LogSafe(LogLevel.Verbose, $"Requesting service at {interaction.Request.Path}.");
            var response = await client.SendAsync(interaction.Request.BuildMessage());
            var errors = new Result();
            var expected = interaction.Response;
            if (response.StatusCode != expected.Status)
            {
                errors.Add(ValidationTypes.StatusCode,$"Status code was {response.StatusCode}. Expected {expected.Status}.");
            }
            errors.Add(ValidationTypes.Headers, ValidateHeaders(expected.Headers, response.Content.Headers));
            errors.Add(ValidationTypes.Body, await ValidateBody(response.Content, expected));
            return errors;
        }

        public IProviderConfiguration Configuration { get; }

        private async Task<string> ValidateBody(HttpContent actual, Response expected)
        {
            var contentType = GetHeader("content-type", expected.Headers)?.Split(';').Select(p => p.Trim()).ToArray();
            if(contentType != null && contentType.Any() && contentType[0] == "application/json"){
                try
                {
                    var actualJson = JObject.Parse(await actual.ReadAsStringAsync());
                    foreach (var token in expected.Body.AsJEnumerable())
                    {
                        var actualToken = actualJson.GetValue(token.Path, Configuration.BodyKeyStringComparison);
                        var expectedValue = expected.Body.GetValue(token.Path);
                        if (actualToken == null && expectedValue != null) return $"Cannot find {token.Path} in body (with expected value {expectedValue} {expectedValue.GetType()})";
                        if (!JToken.DeepEquals(actualToken, expectedValue))
                        {
                            return $"Not match at {token.Path} in body. Expected: {token} but received {actualToken}.";
                        }
                    }
                }
                catch (JsonReaderException exception)
                {
                    return $"Error reading body ({exception.Message})";
                }
            }
            else
            {
                throw new NotImplementedException($"Only content type json is implemented. This seems to be {string.Join(", ", contentType)}");
            }
            return null;
        }
        
        private IEnumerable<string> ValidateHeaders(IDictionary<string,string> expected, HttpContentHeaders actual)
        {
            foreach(var expectedHeader in expected){
                var actualHeader = actual
                    .Where(a => a.Key.Equals(expectedHeader.Key, StringComparison.OrdinalIgnoreCase));
                if (!actualHeader.Any())
                {
                    yield return $"Header {expectedHeader.Key} is missing";
                }
                else
                {
                    var value = string.Join(",", actualHeader.SelectMany(h => h.Value));
                    if (!expectedHeader.Value.Equals(value))
                    {
                        yield return $"Header {expectedHeader.Key} had value {value}. Expected {expectedHeader.Value}.";
                    }
                }
            }
        }

        private string GetHeader(string header, IDictionary<string,string> headers)
        {
            var values =  headers.Where(h => h.Key.Equals(header, StringComparison.OrdinalIgnoreCase))
                                 .Select(h => h.Value).ToList();
            if(values.Any()) return string.Join(",", values);
            return null;
        }

        public override string ToString() => interaction.Description;
    }
}