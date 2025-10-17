﻿using System;

using Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class DeleteAttachment : Command
    {
        public Guid Attachment { get; set; }

        public DeleteAttachment(Guid bank, Guid attachment)
        {
            AggregateIdentifier = bank;
            Attachment = attachment;
        }
    }
}
