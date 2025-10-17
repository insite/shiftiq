﻿using System;

using Common.Timeline.Commands;

using InSite.Domain.Banks;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Application.Banks.Write
{
    public class ChangeQuestionLikertColumn : Command
    {
        public Guid Question { get; set; }
        public Guid Column { get; set; }
        public ContentTitle Content { get; set; }

        [JsonConstructor]
        private ChangeQuestionLikertColumn()
        {

        }

        public ChangeQuestionLikertColumn(Guid bank, Guid question, Guid column, ContentTitle content)
        {
            AggregateIdentifier = bank;
            Question = question;
            Column = column;
            Content = content;
        }

        public ChangeQuestionLikertColumn(Guid bank, Guid question, LikertColumn column)
            : this(bank, question, column.Identifier, column.Content)
        {

        }
    }
}
