﻿using System;

using Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class DeleteForm : Command
    {
        public Guid Form { get; set; }

        public DeleteForm(Guid bank, Guid form)
        {
            AggregateIdentifier = bank;
            Form = form;
        }
    }
}