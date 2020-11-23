using Qowaiv.Validation.Abstractions;
using Qowaiv.Validation.TestTools;

namespace Rumpelstiltskin.UnitTests.TestTools
{
    internal static class FluentAssert
    {
        public static TestResult AssertIsValid(this TestResult test, params IValidationMessage[] expectedMessages)
        {
            ValidationMessageAssert.IsValid(test.Result, expectedMessages);
            return test;
        }

        public static Result AssertInvalid(this TestResult test, params IValidationMessage[] expectedMessages)
        {
            ValidationMessageAssert.WithErrors(test.Result, expectedMessages);
            return test.Result;
        }
    }
}
