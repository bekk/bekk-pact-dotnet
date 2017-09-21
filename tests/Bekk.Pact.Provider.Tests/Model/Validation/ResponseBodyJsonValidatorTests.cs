using Bekk.Pact.Provider.Model.Validation;
using Newtonsoft.Json.Linq;
using System;
using Xunit;
using Bekk.Pact.Provider.Config;

namespace Bekk.Pact.Provider.Tests.Model.Validation
{
    public class ResponseBodyJsonValidatorTests
    {
        private Configuration configuration;

        public ResponseBodyJsonValidatorTests()
        {
            configuration = Bekk.Pact.Provider.Config.Configuration.With.Comparison(StringComparison.InvariantCultureIgnoreCase);  
        }

        [Fact]
        public void Validate_ArrayBodyAndExtraProperties_ReturnsNull()
        {
            var target = new ResponseBodyJsonValidator(configuration);
            var expected = JArray.Parse("[\r\n  {\r\n    \"id\": 1007,\r\n    \"projectId\": 10007,\r\n    \"name\": \"Seven\",\r\n    \"code\": \"107\",\r\n    \"billable\": true\r\n  },\r\n  {\r\n    \"id\": 1034,\r\n    \"projectId\": 10034,\r\n    \"name\": \"Thirtyfour\",\r\n    \"code\": \"134\",\r\n    \"billable\": true\r\n  }\r\n]");
            var actual = "[{\"id\":1007,\"counter\":0,\"code\":\"107\",\"name\":\"Seven\",\"category\":{\"id\":0},\"standardAccount\":false,\"lunchDeduction\":false,\"active\":false,\"paidWork\":false,\"overtimeIncluded\":false,\"workPerformed\":false,\"billable\":true,\"projectId\":10007},{\"id\":1034,\"counter\":0,\"code\":\"134\",\"name\":\"Thirtyfour\",\"category\":{\"id\":0},\"standardAccount\":false,\"lunchDeduction\":false,\"active\":false,\"paidWork\":false,\"overtimeIncluded\":false,\"workPerformed\":false,\"billable\":true,\"projectId\":10034}]";

            var result = target.Validate(expected, actual);

            Assert.Null(result);
        }

        [Fact]
        public void Validate_ActualIsNull_ReturnsErrorText()
        {
            var target = new ResponseBodyJsonValidator(configuration);
            var expected = JObject.Parse("{ id: 0 }");
            string actual = null;

            var result = target.Validate(expected, actual);

            Assert.Equal(result, "Body is not supposed to be empty.");
        }

        [Fact]
        public void Validate_ActualIsEmptyCollection_ReturnsErrorText()
        {
            var target = new ResponseBodyJsonValidator(configuration);
            var expected = JObject.Parse("{}");
            var actual = "[]";

            var result = target.Validate(expected, actual);

            Assert.Equal(result, "Body is not parsable to object");
        }
        
        [Fact]
        public void Validate_ActualIsNotEmptyCollection_ReturnsErrorText()
        {
            var target = new ResponseBodyJsonValidator(configuration);
            var expected = JArray.Parse("[]");
            var actual = "{}";

            var result = target.Validate(expected, actual);

            Assert.Equal(result, "Body is not parsable to array");
        }

        [Fact]
        public void Validate_ArrayPropertyIsEmpty_ReturnsErrorText()
        {
            var target = new ResponseBodyJsonValidator(configuration);
            var expected = JObject.Parse("{ _a:0, _b:[{  _c: 'd' }]}");
            var actual = "{ _a:0, _b:[]}";

            var result = target.Validate(expected, actual);

            Assert.Equal("Array is not supposed to be empty at _b in body.", result);
        }

        [Fact]
        public void Validate_DeepArrayPropertyIsNotEmpty_ReturnsErrorText()
        {
            var target = new ResponseBodyJsonValidator(configuration);
            var expected = JObject.Parse("{ _e:0, _f:[{  _g: 'h', _i: [] }]}");
            var actual = "{ _e:0, _f:[{  _g: 'h', _i: [ { _j: 4 } ] }]}";

            var result = target.Validate(expected, actual);

            Assert.Equal("Array is supposed to be empty at _f[0]._i in body.", result);
        }

        [Fact]
        public void Validate_ReceivesErrorMessage_ReturnsErrorText()
        {
            var expected = JObject.Parse("{\r\n          \"timeEntries\": [\r\n            {\r\n              \"timecode\": \"SomeCode\",\r\n              \"timecodeId\": 17,\r\n              \"comment\": \"Text\",\r\n              \"hours\": 7.5,\r\n              \"date\": \"2000-06-06T00:00:00\"\r\n            }\r\n          ],\r\n          \"timesheetLockDate\": \"2001-01-01T00:00:00\"\r\n        }\r\n");
            var actual = "{\r\n  \"developerMessage\": \"The service threw an exception of type UriFormatException. Check out the log for more information. You`re awesome, thanks :)\",\r\n  \"traceUrl\": \"?q=0HL80UCROIL6L\",\r\n  \"requestTraceId\": \"0HL80UCROIL6L\"\r\n}";
            var target = new ResponseBodyJsonValidator(configuration);

            var result = target.Validate(expected, actual);

            Assert.Equal("Cannot find timeEntries in body.", result);
        }
    }
}
