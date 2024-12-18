/******************************************************************************
 * File Name:    JsonUtils.cs
 * Project:      Arma Reforger Dedicated Server Tool for Windows
 * Description:  Static class containing utility methods for 
 *               performing various JSON (de)serialisation tasks,
 *               including the housing of specific converters
 * 
 * Author:       Bradley Newman
 ******************************************************************************/

using System.Text.Json.Serialization;
using System.Text.Json;
using System.Diagnostics;

namespace ReforgerServerApp.Utils
{
    /// <summary>
    /// Static class containing utility methods for performing various JSON(de)serialisation tasks, 
    /// including the housing of specific converters
    /// </summary>
    internal class JsonUtils
    {
        /// <summary>
        /// Custom JSON Converter for conditional fields
        /// </summary>
        public class ConditionalFieldConverter : JsonConverter<string>
        {
            public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.GetString()!;
            }

            public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
            {
                // This method is intentionally left empty because we handle the writing logic in the Write method of the ModConverter.
            }
        }

        /// <summary>
        /// JSON Converter for Enums. This will convert an enum to a lowercase string when
        /// serialising to JSON
        /// </summary>
        /// <typeparam name="T">Enum</typeparam>
        public class LowercaseEnumConverter<T> : JsonConverter<T> where T : Enum
        {
            public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                try
                {
                    var enumString = reader.GetString();
                    return enumString == null ? throw new JsonException() : (T) Enum.Parse(typeof(T), enumString, true);
                }
                catch (Exception ex)
                {
                    Utilities.DisplayErrorMessage("Unable to load value from configuration file.", ex.Message);
                    return default;
                }
            }

            public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString().ToLower());
            }
        }

        /// <summary>
        /// JSON Converter for the Mod model, this will exclude the 'version' field if version == latest (not a valid value for the server config)
        /// </summary>
        public class ModConverter : JsonConverter<Mod>
        {
            public override Mod Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                Mod mod = new Mod();

                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        break;
                    }

                    if (reader.TokenType == JsonTokenType.PropertyName)
                    {
                        string propertyName = reader.GetString();
                        reader.Read();

                        switch (propertyName)
                        {
                            case nameof(mod.modId):
                            mod.modId = reader.GetString();
                            break;
                            case nameof(mod.name):
                            mod.name = reader.GetString();
                            break;
                            case nameof(mod.version):
                            mod.version = reader.GetString();
                            break;
                            case nameof(mod.required):
                            mod.required = reader.GetBoolean();
                            break;
                        }
                    }
                }

                // Initialize version with 'latest' if its not present
                mod.version ??= "latest";

                return mod;
            }

            public override void Write(Utf8JsonWriter writer, Mod value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                writer.WriteString(nameof(value.modId), value.modId);
                writer.WriteString(nameof(value.name), value.name);

                // Only write version if it is not 'latest'
                if (value.version != "latest")
                {
                    writer.WriteString(nameof(value.version), value.version);
                }

                writer.WriteBoolean(nameof(value.required), value.required);
                writer.WriteEndObject();
            }
        }

        /// <summary>
        /// JSON converter for the Rcon model. This allows Rcon to be excluded when it's 'disabled' which is not actually part of the 
        /// server configuration model.
        /// </summary>
        public class RconConditionalConverter : JsonConverter<Rcon>
        {
            public override Rcon Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.StartObject)
                {
                    throw new JsonException("Expected StartObject token.");
                }

                string address     = null;
                int port           = 0;
                string password    = null;
                string permission  = null;
                string[] blacklist = null;
                string[] whitelist = null;
                int maxClients     = 0;

                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        break;
                    }

                    if (reader.TokenType != JsonTokenType.PropertyName)
                    {
                        throw new JsonException($"Unexpected token type: {reader.TokenType}");
                    }

                    string propertyName = reader.GetString();
                    reader.Read();

                    switch (propertyName)
                    {
                        case nameof(Rcon.address):
                        address = reader.GetString();
                        break;
                        case nameof(Rcon.port):
                        port = reader.GetInt16();
                        break;
                        case nameof(Rcon.password):
                        password = reader.GetString();
                        break;
                        case nameof(Rcon.permission):
                        permission = reader.GetString();
                        break;
                        case nameof(Rcon.blacklist):
                        blacklist = JsonSerializer.Deserialize<string[]>(ref reader, options);
                        break;
                        case nameof(Rcon.whitelist):
                        whitelist = JsonSerializer.Deserialize<string[]>(ref reader, options);
                        break;
                        case nameof(Rcon.maxClients):
                        maxClients = reader.GetInt16();
                        break;
                        default:
                        reader.Skip();
                        break;
                    }
                }
                return new Rcon(address, port, password, Utilities.StringToEnum<RconPermission>(permission),
                    blacklist, whitelist, maxClients);
            }

            public override void Write(Utf8JsonWriter writer, Rcon value, JsonSerializerOptions options)
            {
                if (ConfigurationManager.GetInstance().GetServerConfiguration().rconEnabled)
                {
                    writer.WriteStartObject();
                    writer.WriteString(nameof(Rcon.address), value.address);
                    writer.WriteNumber(nameof(Rcon.port), value.port);
                    writer.WriteString(nameof(Rcon.password), value.password);
                    writer.WriteString(nameof(Rcon.permission), Utilities.RconPermissionToString(value.permission));
                    writer.WritePropertyName(nameof(Rcon.blacklist));
                    writer.WriteStartArray();
                    foreach (string item in value.blacklist)
                    { writer.WriteStringValue(item); }
                    writer.WriteEndArray();
                    writer.WritePropertyName(nameof(Rcon.whitelist));
                    writer.WriteStartArray();
                    foreach (string item in value.whitelist)
                    { writer.WriteStringValue(item); }
                    writer.WriteEndArray();
                    writer.WriteNumber(nameof(Rcon.maxClients), value.maxClients);
                    writer.WriteEndObject();
                }
            }
        }

        /// <summary>
        /// JSON converter for the Operating model. This allows disableNavmeshStreaming to be excluded when all Navmeshes are to be disabled
        /// which is not actually part of the server configuration model.
        /// </summary>
        public class OperatingConditionalConverter : JsonConverter<Operating>
        {
            public override Operating Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.StartObject)
                {
                    throw new JsonException("Expected StartObject token.");
                }

                Operating oper = Operating.Default;

                // Make this null so if we don't find this tag in the JSON, we can check if it's null later
                oper.disableNavmeshStreaming = null;

                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        break;
                    }

                    if (reader.TokenType != JsonTokenType.PropertyName)
                    {
                        throw new JsonException($"Unexpected token type: {reader.TokenType}");
                    }

                    string propertyName = reader.GetString();
                    reader.Read();

                    switch (propertyName)
                    {
                        case nameof(Operating.lobbyPlayerSynchronise):
                        oper.lobbyPlayerSynchronise = reader.GetBoolean();
                        break;
                        case nameof(Operating.playerSaveTime):
                        oper.playerSaveTime = reader.GetInt32();
                        break;
                        case nameof(Operating.aiLimit):
                        oper.aiLimit = reader.GetInt32();
                        break;
                        case nameof(Operating.slotReservationTimeout):
                        oper.slotReservationTimeout = reader.GetInt32();
                        break;
                        case nameof(Operating.disableNavmeshStreaming):
                        oper.disableNavmeshStreaming = JsonSerializer.Deserialize<string[]>(ref reader, options);
                        break;
                        case nameof(Operating.disableServerShutdown):
                        oper.disableServerShutdown = reader.GetBoolean();
                        break;
                        case nameof(Operating.disableCrashReporter):
                        oper.disableCrashReporter = reader.GetBoolean();
                        break;
                        case nameof(Operating.disableAI):
                        oper.disableAI = reader.GetBoolean();
                        break;
                        case nameof(Operating.joinQueue):
                        oper.joinQueue = JsonSerializer.Deserialize<JoinQueue>(ref reader, options);
                        break;
                        default:
                        reader.Skip();
                        break;
                    }
                }
                return oper;
            }

            public override void Write(Utf8JsonWriter writer, Operating value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                writer.WriteBoolean(nameof(Operating.lobbyPlayerSynchronise), value.lobbyPlayerSynchronise);
                writer.WriteNumber(nameof(Operating.playerSaveTime), value.playerSaveTime);
                writer.WriteNumber(nameof(Operating.aiLimit), value.aiLimit);
                writer.WriteNumber(nameof(Operating.slotReservationTimeout), value.slotReservationTimeout);

                // Only include the disableNavmeshStreaming option if relevant,
                // handle this special case as if it is included with an empty list, it means
                // disable streaming ALL navmeshes
                if (ConfigurationManager.GetInstance().GetServerConfiguration().toggleDisableNavmeshStreaming)
                {
                    writer.WritePropertyName(nameof(Operating.disableNavmeshStreaming));
                    writer.WriteStartArray();
                    foreach (string item in value.disableNavmeshStreaming)
                    { writer.WriteStringValue(item); }
                    writer.WriteEndArray();
                }

                writer.WriteBoolean(nameof(Operating.disableServerShutdown), value.disableServerShutdown);
                writer.WriteBoolean(nameof(Operating.disableCrashReporter), value.disableCrashReporter);
                writer.WriteBoolean(nameof(Operating.disableAI), value.disableAI);

                if (value.joinQueue != null)
                {
                    writer.WritePropertyName(nameof(Operating.joinQueue));
                    JsonSerializer.Serialize(writer, value.joinQueue, options);
                }

                writer.WriteEndObject();
            }
        }
    }
}
