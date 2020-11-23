using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Qowaiv.Json.Newtonsoft;
using Qowaiv.Web;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Rumpelstiltskin.Shared.Serialization
{
    /// <summary>Helper class to centralize <see cref="JsonConvert"/> logic.</summary>
    /// <remarks>
    /// This helper is here to ensure:
    /// * All JSON serialization uses the same settings.
    /// * New line character is environment independent.
    /// </remarks>
    public static class Json
    {
        public static readonly InternetMediaType MediaType = InternetMediaType.Parse("application/json");

        private static JsonSerializerSettings Settings { get; } = NewSettings();

        public static void Initialize(this JsonSerializerSettings settings)
        {
            Guard.NotNull(settings, nameof(settings));
            foreach (var converter in Settings.Converters)
            {
                if (settings.Converters.All(c => c.GetType() != converter.GetType()))
                {
                    settings.Converters.Add(converter);
                }
            }
        }

        public static string Serialize(object model)
        => Serialize(model, Formatting.None);

        public static string Serialize(object model, Formatting formatting)
        {
            var sb = new StringBuilder();
            var writer = new StringWriter(sb, CultureInfo.InvariantCulture);

            Serialize(model, writer, formatting);
            return sb.ToString();
        }
        public static void Serialize(object model, TextWriter writer, Formatting formatting)
        {
            Guard.NotNull(writer, nameof(writer));

            var jsonWriter = new JsonTextWriter(writer);
            jsonWriter.Formatting = formatting;
            JsonSerializer.CreateDefault(NewSettings()).Serialize(jsonWriter, model, model?.GetType());
            jsonWriter.Flush();
        }

        public static T Deserialize<T>(string json) =>
            JsonConvert.DeserializeObject<T>(json, Settings);

        public static HttpContent HttpContent(object model)
            => new StringContent(
                content: Serialize(model),
                encoding: Encoding.UTF8,
                mediaType: MediaType.ToString());

        private static JsonSerializerSettings NewSettings()
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            };
            settings.Converters.Add(new QowaivJsonConverter());
            settings.Converters.Add(new StringEnumConverter());
            return settings;
        }
    }
}
