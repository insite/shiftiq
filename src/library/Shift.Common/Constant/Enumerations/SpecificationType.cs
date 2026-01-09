using System.ComponentModel;

namespace Shift.Constant
{
    public enum SpecificationType
    {
        /// <summary>
        /// A form with this specification does not have a fixed set of questions. If two learners start an attempt on 
        /// this type of form, then each learner may be presented with a different set of questions and/or questions in 
        /// a different sequence.
        /// </summary>
        [Description("Generated randomly per attempt")]
        Dynamic,

        /// <summary>
        /// A form with this specification has a fixed set of questions in a fixed sequence, which is determined by the
        /// author of the form. Every learner who starts an attempt on form with a Static specification sees exactly the 
        /// same set of questions in exactly the same sequence.
        /// </summary>
        [Description("Fixed identically for all attempts")]
        Static
    }
}