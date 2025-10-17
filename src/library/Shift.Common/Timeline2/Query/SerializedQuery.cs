using System;

namespace Shift.Common
{
    /// <summary>
    /// Provides a database-friendly serialization wrapper for a query.
    /// </summary>
    public class SerializedQuery
    {
        /// <summary>
        /// Uniquely identifies a specific instance of a query.
        /// </summary>
        public Guid Identifier { get; set; }

        /// <summary>
        /// JSON value describing the query origin.
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// JSON value defining the query filter.
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        /// JSON value containing the display text for each GUID-value property in the criteria.
        /// </summary>
        public string Texts { get; set; }

        /// <summary>
        /// JSON value containing the criteria properties.
        /// </summary>
        public string Criteria { get; set; }

        /// <summary>
        /// Query type name (must be unique across namespaces).
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Unique, fully-qualified type name (including namespace).
        /// </summary>
        public string Class { get; set; }
    }
}