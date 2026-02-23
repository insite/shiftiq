using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class QuestionComposedVoiceChanged : Change
    {
        public Guid Question { get; set; }
        public ComposedVoice ComposedVoice { get; set; }

        public QuestionComposedVoiceChanged(Guid question, ComposedVoice composedVoice)
        {
            Question = question;
            ComposedVoice = composedVoice;
        }
    }
}
