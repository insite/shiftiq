using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Sites.Pages
{
    public class SequenceChanged : Change
    {
        public int Sequence { get; set; }
        public SequenceChanged(int sequence)
        {
            Sequence = sequence;
        }
    }
}
