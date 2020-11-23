using Qowaiv.Validation.Abstractions;

namespace Rumpelstiltskin.Shared.Validation
{
    public static class Must
    {
        public static Result<T> Be<T>(T model, bool condition, string propertyName, string message, params object[] args)
           => condition
               ? Result.For(model)
               : Result.WithMessages<T>(ValidationMessage.Error(string.Format(message, args), propertyName));

        public static Result<T> NotBe<T>(T model, bool condition, string propertyName, string message, params object[] args)
           => Be(model, !condition, propertyName, message, args);
    }
}
