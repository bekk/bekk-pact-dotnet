using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Bekk.Pact.Common.Contracts;
using Bekk.Pact.Common.Extensions;
using Bekk.Pact.Provider.Model.Validation;
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
            Configuration.LogSafe(LogLevel.Info, ToString());
            Configuration.LogSafe(LogLevel.Verbose, $"Pact: {ProviderState}");
            var response = await client.SendAsync(interaction.Request.BuildMessage());
            var expected = interaction.Response;
            var errors = new Result(ToString(), expected);
            if (response.StatusCode != expected.Status)
            {
                errors.Add(ValidationTypes.StatusCode,$"Status code was {response.StatusCode}. Expected {expected.Status}.");
            }
            errors.Add(ValidationTypes.Headers, new ResponseHeadersValidator().Validate(expected, response.Content.Headers));
            errors.Add(ValidationTypes.Body, await ValidateBody(response.Content, expected));
            errors.Add(response);
            return errors;
        }

        public IProviderConfiguration Configuration { get; }

        private async Task<string> ValidateBody(HttpContent actual, Response expected)
        {
            var contentType = GetHeader("content-type", expected.Headers)?.Split(';').Select(p => p.Trim()).ToArray();
            if(contentType != null && contentType.Any() && contentType[0] == "application/json"){
                try
                {
                    var actualAsString = await actual.ReadAsStringAsync();
                    Configuration.LogSafe(LogLevel.Verbose, $"Response body: \n{actualAsString}");
                    return new ResponseBodyJsonValidator(Configuration).Validate(expected.Body, actualAsString);
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
        }

        private string GetHeader(string header, IDictionary<string,string> headers)
        {
            var values =  headers.Where(h => h.Key.Equals(header, StringComparison.OrdinalIgnoreCase))
                                 .Select(h => h.Value).ToList();
            if(values.Any()) return string.Join(",", values);
            return null;
        }

        public override string ToString() => $"{interaction.Consumer}: {interaction.Request.Method} {interaction.Request.Path} [{interaction.Description}]";
    }
}