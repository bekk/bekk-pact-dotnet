using Bekk.Pact.Provider.Model.Validation;
using Newtonsoft.Json.Linq;
using System;
using Xunit;

namespace Bekk.Pact.Provider.Tests.Model.Validation
{
    public class ResponseBodyJsonValidatorTests
    {
        [Fact]
        public void Validate_ArrayBodyAndExtraProperties_ReturnsNull()
        {
            var target = new ResponseBodyJsonValidator(null);
            var expected = JArray.Parse("[\r\n  {\r\n    \"id\": 1007,\r\n    \"projectId\": 10007,\r\n    \"name\": \"Seven\",\r\n    \"code\": \"107\",\r\n    \"billable\": true\r\n  },\r\n  {\r\n    \"id\": 1034,\r\n    \"projectId\": 10034,\r\n    \"name\": \"Thirtyfour\",\r\n    \"code\": \"134\",\r\n    \"billable\": true\r\n  }\r\n]");
            var actual = "[{\"id\":1007,\"counter\":0,\"code\":\"107\",\"name\":\"Seven\",\"category\":{\"id\":0},\"standardAccount\":false,\"lunchDeduction\":false,\"active\":false,\"paidWork\":false,\"overtimeIncluded\":false,\"workPerformed\":false,\"billable\":true,\"projectId\":10007},{\"id\":1034,\"counter\":0,\"code\":\"134\",\"name\":\"Thirtyfour\",\"category\":{\"id\":0},\"standardAccount\":false,\"lunchDeduction\":false,\"active\":false,\"paidWork\":false,\"overtimeIncluded\":false,\"workPerformed\":false,\"billable\":true,\"projectId\":10034}]";

            var result = target.Validate(expected, actual);

            Assert.Null(result);
        }
    }
}
