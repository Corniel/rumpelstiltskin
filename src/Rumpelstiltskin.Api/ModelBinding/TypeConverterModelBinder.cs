using Microsoft.AspNetCore.Mvc.ModelBinding;
using Qowaiv;
using Qowaiv.Reflection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;

namespace Rumpelstiltskin.Api.ModelBinding
{
    /// <summary>A model binder for types with a custom type converter.</summary>
    /// <remarks>
    /// This binder supports models that have there own TypeConverter.
    ///
    /// The message of the exception thrown by there type converter in case of
    /// failure is the error message added to the model state.
    ///
    /// This binder is needed because the default model binder
    /// does not show the message of the exception thrown by the type converter.
    /// </remarks>
    public class TypeConverterModelBinder : IModelBinderProvider, IModelBinder
    {
        /// <summary>A Dictionary that keeps track of the registered type converters.</summary>
        private readonly ConcurrentDictionary<Type, TypeConverter> typeConverters = new ConcurrentDictionary<Type, TypeConverter>();

        /// <summary>Static constructor.</summary>
        /// <remarks>
        /// Add all types of Qowaiv that are supported by the model binder.
        /// </remarks>
        public TypeConverterModelBinder()
        {
            // Adds all Qowaiv types.
            AddAssembly(typeof(SingleValueObjectAttribute).Assembly);
        }

        /// <summary>Adds the types of the assembly that are marked with teh SingleValueObjectAttribute.</summary>
        /// <param name="assembly">
        /// Assembly to add.
        /// </param>
        public TypeConverterModelBinder AddAssembly(Assembly assembly)
        {
            Guard.NotNull(assembly, nameof(assembly));
            var tps = assembly.GetTypes();
            AddTypes(tps);
            return this;
        }

        /// <summary>Adds the specified types.</summary>
        /// <param name="tps">
        /// Types to add.
        /// </param>
        /// <remarks>
        /// Only adds the types that are supported by the model binder.
        /// </remarks>
        public TypeConverterModelBinder AddTypes(params Type[] tps)
        {
            Guard.NotNull(tps, nameof(tps));
            foreach (var tp in tps)
            {
                AddType(tp);
            }
            return this;
        }

        /// <summary>Adds the specified type.</summary>
        /// <param name="tp">
        /// Type to add.
        /// </param>
        /// <remarks>
        /// Only adds the types that are supported by the model binder.
        /// </remarks>
        public TypeConverterModelBinder AddType(Type tp)
        {
            // Don't add types twice.
            if (!typeConverters.ContainsKey(tp))
            {
                var converter = TypeDescriptor.GetConverter(tp);
                // Not the default type converter or the enumerator converter.
                if (converter.GetType() != typeof(TypeConverter) &&
                    converter.GetType() != typeof(EnumConverter) &&
                    converter.CanConvertFrom(typeof(string)))
                {
                    typeConverters[tp] = converter;
                }
            }
            return this;
        }

        /// <summary>Gets the types that where added to the model binder.</summary>
        public IEnumerable<Type> Types => typeConverters.Keys;

        /// <inheritdoc />
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            Guard.NotNull(context, nameof(context));

            var type = QowaivType.GetNotNullableType(context.Metadata?.ModelType);

            return typeConverters.ContainsKey(type)
                ? this
                : null;
        }

        /// <inheritdoc />
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            Guard.NotNull(bindingContext, nameof(bindingContext));

            var modelName = bindingContext.ModelName;

            // Try to fetch the value of the argument by name
            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

            var value = valueProviderResult.FirstValue;

            var type = bindingContext.ModelType;
            var nullable = !type.IsValueType || QowaivType.IsNullable(type);

            if (nullable && string.IsNullOrEmpty(value))
            {
                bindingContext.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }

            // We want the converter for the not nullable type.
            var converter = typeConverters[QowaivType.GetNotNullableType(type)];

            try
            {
                var result = converter.ConvertFrom(
                    context: null,
                    culture: valueProviderResult.Culture,
                    value: value);

                bindingContext.Result = ModelBindingResult.Success(result);
            }
#pragma warning disable S2221 // "Exception" should not be caught when not required by called methods
            catch (Exception x)
#pragma warning restore S2221 // "Exception" should not be caught when not required by called methods
            {
                // add the error.
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, x.Message);
            }

            return Task.CompletedTask;
        }
    }
}
