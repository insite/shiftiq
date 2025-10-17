using System.Collections.Generic;
using System.Text;

namespace Shift.Common
{
    public class BasicAccessHelper
    {
        public BasicAccessHelper(BasicAccess basic)
        {
            Basic = basic;
        }

        public BasicAccess Basic { get; set; }

        public bool Deny => Basic == BasicAccess.Deny;
        public bool Allow => Basic.HasFlag(BasicAccess.Allow);

        public string Abbreviate(BasicAccess access)
        {
            switch (access)
            {
                case BasicAccess.Allow:
                    return "+";
            }

            return "-";
        }

        public string Abbreviate()
        {
            if (!AnyGranted())
                return Abbreviate(BasicAccess.Deny);

            var list = new StringBuilder();

            if (Allow)
                list.Append(Abbreviate(BasicAccess.Allow));

            return list.ToString();
        }

        public string Describe()
        {
            if (!AnyGranted())
                return nameof(Deny);

            var list = new List<string>();

            if (Allow)
                list.Add(nameof(Allow));

            return string.Join(", ", list);
        }

        public bool AllGranted()
            => Basic == BasicAccess.Allow;

        public bool AnyGranted()
            => Basic > 0;

        public bool IsGranted(string access)
        {
            switch (access)
            {
                case nameof(Allow):
                    return Allow;

                default:
                    return false;
            }
        }

        public bool IsGranted(BasicAccess access)
        {
            if (access == BasicAccess.Allow)
                return Allow;

            return false;
        }
    }

    public class DataAccessHelper
    {
        public DataAccessHelper(DataAccess data)
        {
            Data = data;
        }

        public DataAccess Data { get; set; }

        public bool None => Data == DataAccess.None;
        public bool Read => Data.HasFlag(DataAccess.Read);
        public bool Write => Data.HasFlag(DataAccess.Write);
        public bool Create => Data.HasFlag(DataAccess.Create);
        public bool Delete => Data.HasFlag(DataAccess.Delete);
        public bool Administrate => Data.HasFlag(DataAccess.Administrate);
        public bool Configure => Data.HasFlag(DataAccess.Configure);

        public string Abbreviate(DataAccess access)
        {
            switch (access)
            {
                case DataAccess.Read:
                    return "r";
                case DataAccess.Write:
                    return "w";
                case DataAccess.Create:
                    return "c";
                case DataAccess.Delete:
                    return "d";
                case DataAccess.Administrate:
                    return "a";
                case DataAccess.Configure:
                    return "f";
            }

            return "-";
        }

        public string Abbreviate()
        {
            if (!AnyGranted())
                return Abbreviate(DataAccess.None);

            var list = new StringBuilder();

            if (Read)
                list.Append(Abbreviate(DataAccess.Read));

            if (Write)
                list.Append(Abbreviate(DataAccess.Write));

            if (Create)
                list.Append(Abbreviate(DataAccess.Create));

            if (Delete)
                list.Append(Abbreviate(DataAccess.Delete));

            if (Administrate)
                list.Append(Abbreviate(DataAccess.Administrate));

            if (Configure)
                list.Append(Abbreviate(DataAccess.Configure));

            return list.ToString();
        }

        public string Describe()
        {
            if (!AnyGranted())
                return "-";

            var list = new List<string>();

            if (Read)
                list.Add(nameof(Read));

            if (Write)
                list.Add(nameof(Write));

            if (Create)
                list.Add(nameof(Create));

            if (Delete)
                list.Add(nameof(Delete));

            if (Administrate)
                list.Add(nameof(Administrate));

            if (Configure)
                list.Add(nameof(Configure));

            return string.Join(", ", list);
        }

        public bool AllGranted()
            => Data == DataAccess.All;

        public bool AnyGranted()
            => Data > 0;

        public bool IsGranted(string access)
        {
            switch (access)
            {
                case nameof(Read):
                    return Read;

                case nameof(Write):
                    return Write;

                case nameof(Create):
                    return Create;

                case nameof(Delete):
                    return Delete;

                case nameof(Administrate):
                    return Administrate;

                case nameof(Configure):
                    return Configure;

                default:
                    return false;
            }
        }

        public bool IsGranted(DataAccess access)
        {
            switch (access)
            {
                case DataAccess.Read:
                    return Read;

                case DataAccess.Write:
                    return Write;

                case DataAccess.Create:
                    return Create;

                case DataAccess.Delete:
                    return Delete;

                case DataAccess.Administrate:
                    return Administrate;

                case DataAccess.Configure:
                    return Configure;

                default:
                    return false;
            }
        }
    }

    public class HttpAccessHelper
    {
        public HttpAccessHelper(HttpAccess http)
        {
            Http = http;
        }

        public HttpAccess Http { get; set; }

        public bool None => Http == HttpAccess.None;
        public bool Head => Http.HasFlag(HttpAccess.Head);
        public bool Get => Http.HasFlag(HttpAccess.Get);
        public bool Put => Http.HasFlag(HttpAccess.Put);
        public bool Post => Http.HasFlag(HttpAccess.Post);
        public bool Delete => Http.HasFlag(HttpAccess.Delete);

        public string Abbreviate(HttpAccess access)
        {
            switch (access)
            {
                case HttpAccess.Head:
                    return "h";
                case HttpAccess.Get:
                    return "g";
                case HttpAccess.Put:
                    return "u";
                case HttpAccess.Post:
                    return "p";
                case HttpAccess.Delete:
                    return "d";
            }

            return "-";
        }

        public string Abbreviate()
        {
            if (!AnyGranted())
                return Abbreviate(HttpAccess.None);

            var list = new StringBuilder();

            if (Head)
                list.Append(Abbreviate(HttpAccess.Head));

            if (Get)
                list.Append(Abbreviate(HttpAccess.Get));

            if (Put)
                list.Append(Abbreviate(HttpAccess.Put));

            if (Post)
                list.Append(Abbreviate(HttpAccess.Post));

            if (Delete)
                list.Append(Abbreviate(HttpAccess.Delete));

            return list.ToString();
        }

        public string Describe()
        {
            if (!AnyGranted())
                return "-";

            var list = new List<string>();

            if (Head)
                list.Add(nameof(Head));

            if (Get)
                list.Add(nameof(Get));

            if (Put)
                list.Add(nameof(Put));

            if (Post)
                list.Add(nameof(Post));

            if (Delete)
                list.Add(nameof(Delete));

            return string.Join(", ", list);
        }

        public bool AllGranted()
            => Http == HttpAccess.All;

        public bool AnyGranted()
            => Http > 0;

        public bool IsGranted(string access)
        {
            switch (access)
            {
                case nameof(Head):
                    return Head;

                case nameof(Get):
                    return Get;

                case nameof(Put):
                    return Put;

                case nameof(Post):
                    return Post;

                case nameof(Delete):
                    return Delete;

                default:
                    return false;
            }
        }

        public bool IsGranted(HttpAccess access)
        {
            switch (access)
            {
                case HttpAccess.Head:
                    return Head;

                case HttpAccess.Get:
                    return Get;

                case HttpAccess.Put:
                    return Put;

                case HttpAccess.Post:
                    return Post;

                case HttpAccess.Delete:
                    return Delete;

                default:
                    return false;
            }
        }
    }
}