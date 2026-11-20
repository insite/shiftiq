using System;

namespace Shift.Common
{
    /// <summary>
    /// Catalogue settings that differ by environment, so they live in configuration rather than in code.
    /// </summary>
    public class Catalogues
    {
        /// <summary>
        /// The catalogue promoted by its own button on the partition home page: the "Academy" catalogue of paid
        /// programs and hosted third-party courses. Null hides the button.
        /// </summary>
        public Guid? Featured { get; set; }

        /// <summary>
        /// Whether the portal course catalogue shows program cards alongside course cards. Off by default, so a
        /// partition shows course cards only until it opts in.
        /// </summary>
        public bool DisplayPrograms { get; set; }
    }
}
