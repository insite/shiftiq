using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveFile : Query<FileModel>
    {
        public Guid FileIdentifier { get; set; }
    }
}