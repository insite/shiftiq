using System;
using System.Collections.Generic;

namespace InSite.Domain.Foundations
{
    [Serializable]
    public struct AccessControl
    {
        public bool Execute { get; set; }

        public bool Read { get; set; }
        public bool Write { get; set; }

        public bool Create { get; set; }
        public bool Delete { get; set; }

        public bool Administrate { get; set; }
        public bool Configure { get; set; }

        public bool Trial { get; set; }

        public bool FullControl => Configure;

        public void Cascade()
        {
            if (Configure)
                Administrate = true;

            if (Administrate)
                Create = Delete = true;

            if (Delete || Create)
                Write = true;

            if (Write)
                Read = true;
        }

        public AccessControl SetRead(bool allow)
        {
            Read = allow;
            return this;
        }

        public AccessControl SetWrite(bool allow, bool cascade = true)
        {
            Write = allow;
            if (cascade)
                Cascade();
            return this;
        }

        public AccessControl SetCreate(bool allow, bool cascade = true)
        {
            Create = allow;
            if (cascade)
                Cascade();
            return this;
        }

        public AccessControl SetDelete(bool allow, bool cascade = true)
        {
            Delete = allow;
            if (cascade)
                Cascade();
            return this;
        }

        public AccessControl SetAdministrate(bool allow, bool cascade = true)
        {
            Administrate = allow;
            if (cascade)
                Cascade();
            return this;
        }

        public AccessControl SetConfigure(bool allow, bool cascade = true)
        {
            Configure = allow;
            if (cascade)
                Cascade();
            return this;
        }

        public AccessControl SetAll(bool allow)
        {
            Read = Write = Create = Delete = Administrate = Configure = allow;
            Cascade();
            return this;
        }

        public static AccessControl CreateDefaultForCmds()
        {
            return new AccessControl
            {
                Read = true,
                Write = true,
                Create = true,
                Delete = true
            };
        }

        public override string ToString()
        {
            var list = new List<string>();

            if (Read)
                list.Add("Read");

            if (Write)
                list.Add("Write");

            if (Create)
                list.Add("Create");

            if (Delete)
                list.Add("Delete");

            if (Administrate)
                list.Add("Administrate");

            if (Configure)
                list.Add("Configure");

            if (Trial)
                list.Add("Trial");

            return Shift.Common.CsvConverter.ConvertListToCsvText(list);
        }
    }
}