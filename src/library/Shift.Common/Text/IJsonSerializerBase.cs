using System;

namespace Shift.Common
{
    /// <summary>
    /// Provides serialization and deserialization functionality to and from string values. This 
    /// allows your client code to use any JSON serialization library you want (System.Text.Json or
    /// Newtonsoft.Json, for example).
    /// </summary>
    public interface IJsonSerializerBase
    {
        /// <summary>
        /// Returns the serialized string value for an object of a specific type.
        /// </summary>
        string Serialize<T>(T value);

        /// <summary>
        /// Returns a object deserialized from the input string.
        /// </summary>
        T Deserialize<T>(string value);

        /// <summary>
        /// Returns the serialized string value for an object, excluding properties in the 
        /// exclusions array.
        /// </summary>
        /// <param name="disablePropertyConverters">If true, the default JSON property converter is 
        /// removed from every property of the object before it is serialized.</param>
        string Serialize(object value, JsonPurpose mode, bool disablePropertyConverters = false, string[] excludeProperties = null);

        /// <summary>
        /// Returns a object of the desired type, deserialized from the input string.
        /// </summary>
        /// <param name="disablePropertyConverters">If true, the default JSON property converter is 
        /// removed from every property of the object before it is deserialized.</param>
        T Deserialize<T>(Type type, string value, JsonPurpose mode, bool disablePropertyConverters = false, string[] excludeProperties = null);
    }
}