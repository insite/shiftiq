using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

using Newtonsoft.Json;

namespace InSite.Application.Courses.Write
{
    public class ConnectCourseMessage : Command, IHasRun
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public CourseMessageType MessageType { get; set; }

        public Guid? MessageId { get; set; }
        public int? AfterDays { get; set; }
        public int? MaxCount { get; set; }

        public ConnectCourseMessage(Guid courseId, CourseMessageType messageType, Guid? messageId, int? afterDays, int? maxCount)
        {
            AggregateIdentifier = courseId;
            MessageType = messageType;
            MessageId = messageId;
            AfterDays = afterDays;
            MaxCount = maxCount;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var data = course.Data;

            switch (MessageType)
            {
                case CourseMessageType.StalledToLearner:
                    if (data.GetGuidValue(CourseField.StalledToLearnerMessageIdentifier) == MessageId
                        && data.GetIntValue(CourseField.SendMessageStalledAfterDays) == AfterDays
                        && data.GetIntValue(CourseField.SendMessageStalledMaxCount) == MaxCount
                        )
                    {
                        return false;
                    }
                    break;
                case CourseMessageType.StalledToAdministrator:
                    if (data.GetGuidValue(CourseField.StalledToAdministratorMessageIdentifier) == MessageId)
                        return false;
                    break;
                case CourseMessageType.CompletedToLearner:
                    if (data.GetGuidValue(CourseField.CompletedToLearnerMessageIdentifier) == MessageId)
                        return false;
                    break;
                case CourseMessageType.CompletedToAdministrator:
                    if (data.GetGuidValue(CourseField.CompletedToAdministratorMessageIdentifier) == MessageId)
                        return false;
                    break;
                default:
                    throw new ArgumentException($"Unsupported message: {MessageType}");
            }

            course.Apply(new CourseMessageConnected(MessageType, MessageId, AfterDays, MaxCount));
            return true;
        }
    }
}
