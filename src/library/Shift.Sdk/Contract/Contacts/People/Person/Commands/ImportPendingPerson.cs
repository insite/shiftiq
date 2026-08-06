using System;
using System.ComponentModel.DataAnnotations;

namespace Shift.Contract
{
    public class ImportPendingPerson
    {
        public enum ActionEnum { Ignore, Create, Match }

        [Required]
        public Guid PendingPersonId { get; set; }

        public Guid? MatchUserId { get; set; }

        [Required]
        public ActionEnum Action { get; set; }
    }
}
