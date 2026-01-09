using System;
using System.Collections.Generic;

namespace Shift.Common
{
    /// <summary>
    /// Defines a query with a strongly-typed result.
    /// </summary>
    public interface IQuery<TResult>
    {
        /// <summary>
        /// Unique identifier for the query (optional).
        /// </summary>
        /// <remarks>
        /// Similar to the unique identifier for a command, this is used to track a specific 
        /// instance of a query. For example, a query that takes a long time to run could be 
        /// executed in a background service, and the identifier could be used to retrieve the 
        /// result at a future time.
        /// </remarks>
        Guid? Identifier { get; set; }

        /// <summary>
        /// Identifies the origin for the query.
        /// </summary>
        Origin Origin { get; set; }

        /// <summary>
        /// Identifies a subset of items and/or properties in the query result.
        /// </summary>
        /// <remarks>
        /// The property names and values of a query object determine its criteria. When a query is 
        /// executed, the server finds all matching database records and then uses a filter to 
        /// determine the specific subset of data returned to the caller. A filter indicates the 
        /// requested page number and the page size. A filter also specifies sorting options and 
        /// properties to include or exclude from each page. (In other words, a filter specifies the 
        /// rows and columns in paged subset of the data that matches the query criteria.)
        /// </remarks>
        QueryFilter Filter { get; set; }

        /// <summary>
        /// User-friendly display text for each GUID-value query property.
        /// </summary>
        Dictionary<Guid, string> Texts { get; set; }
    }

    /// <summary>
    /// Base class implementation for the IQuery interface.
    /// </summary>
    public class Query<TResult> : IQuery<TResult>
    {
        public Guid? Identifier { get; set; }

        public Origin Origin { get; set; }

        public QueryFilter Filter { get; set; } = new QueryFilter();

        public Dictionary<Guid, string> Texts { get; set; } = new Dictionary<Guid, string>();
    }

    public interface IQueryRunner
    {
        TResult Run<TResult>(IQuery<TResult> query);

        bool CanRun(Type queryType);
    }

    public interface IQueryDispatcher
    {
        TResult Dispatch<TResult>(IQuery<TResult> query);
    }
}