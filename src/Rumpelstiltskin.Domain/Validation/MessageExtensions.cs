using FluentValidation;
using System.Globalization;

namespace Rumpelstiltskin.Domain.Validation
{
    internal static class MessageExtensions
    {
        public static IRuleBuilderOptions<T, TProperty> WithFormattedMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, string errorMessage, params object[] args)
            => rule.WithMessage(string.Format(CultureInfo.CurrentCulture, errorMessage, args));
    }
}
