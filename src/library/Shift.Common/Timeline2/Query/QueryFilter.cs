namespace Shift.Common
{
    public class QueryFilter
    {
        /// <summary>
        /// The subset of items requested within a paged data set
        /// </summary>
        /// <remarks>
        /// Indexing starts at one (not zero) because we want page numbers to be readable and user-friendly in URLs.
        /// </remarks>
        public int Page { get; set; } = 1;

        /// <summary>
        /// The number of items requested per page
        /// </summary>
        public int PageSize { get; set; } = 20;

        /// <summary>
        /// A comma-separated list of property names.
        /// </summary>
        /// <remarks>
        /// Here we support sorting direction specifiers "asc" and "desc" for ascending and
        /// descending. If there is no specifier then the default is to sort ascending.
        /// </remarks>
        /// <example>
        /// Sort = "Age+desc,LastName+asc,FirstName"
        /// </example>
        public string Sort { get; set; }

        /// <summary>
        /// A comma-separated list of properties to be specifically excluded from the data set.
        /// </summary>
        /// <remarks>
        /// This property can be used (optionally) to decrease the size of the data set requested by
        /// client code when there is a need decrease bandwidth usage and/or increase performance on
        /// the server by limiting the amount of work it is expected to do in building a response.
        /// </remarks>
        /// <example>
        /// Excludes = "IgnorePropertyA,IgnorePropertyB"
        /// </example>
        public string Excludes { get; set; }

        /// <summary>
        /// A comma-separated list of properties to be specifically included in the data set.
        /// </summary>
        /// <remarks>
        /// This property can be used (optionally) to include additional information in the 
        /// requested data set, when the additional data may not be included by default.
        /// </remarks>
        /// <example>
        /// Excludes = "MassivePropertyX,MassivePropertyY"
        /// </example>
        public string Includes { get; set; }

        /// <summary>
        /// Export file format (always JSON by default)
        /// </summary>
        public string Format { get; set; } = "json";
    }
}