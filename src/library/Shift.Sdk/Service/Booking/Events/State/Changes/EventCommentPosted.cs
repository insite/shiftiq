﻿using System;

using Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class EventCommentPosted : Change
    {
        public Guid CommentIdentifier { get; set; }
        public string CommentText { get; set; }
        public Guid AuthorIdentifier { get; set; }

        public EventCommentPosted(Guid comment, Guid author, string text)
        {
            CommentIdentifier = comment;
            AuthorIdentifier = author;
            CommentText = text;
        }
    }
}
