using System;

namespace Shift.Common
{
    public interface IToken
    {
        /// <summary>
        /// Gets a globally unique identifier for the object.
        /// </summary>
        Guid Identifier { get; }

        /// <summary>
        /// Gets a unique integer value for the object. Values are not intended for use outside the system.
        /// </summary>
        int Key { get; }

        /// <summary>
        /// Gets a unique integer value for the object. Values can be used outside the system.
        /// </summary>
        int Number { get; }

        /// <summary>
        /// Gets an alphanumeric code for the object. Uniqueness is not guaranteed.
        /// </summary>
        string Code { get; }

        /// <summary>
        /// Gets a name for the object. This is most often used for office/filing purposes.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a display title for the object. This is most often used for publication purposes.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Gets the type of the object.
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Gets the subtype of the object (if applicable).
        /// </summary>
        string Subtype { get; }
    }
}
