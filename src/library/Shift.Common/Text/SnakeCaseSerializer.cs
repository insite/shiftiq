using System.Text;

namespace Shift.Common
{
    public static class SnakeCaseSerializer
    {
        public static string Serialize<T>(object o)
        {
            var sb = new StringBuilder();

            sb.Append("{\n");

            var props = typeof(T).GetProperties();

            for (int i = 0; i < props.Length; i++)
            {
                var prop = props[i];

                var key = ToSnakeCase(prop.Name);

                var value = prop.GetValue(o)?.ToString()?.Replace("\\", "\\\\").Replace("\"", "\\\"") ?? "";

                sb.Append($"  \"{key}\": \"{value}\"");

                if (i < props.Length - 1)
                    sb.Append(",\n");
            }

            sb.Append("\n}");

            return sb.ToString();
        }

        private static string ToSnakeCase(string name)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < name.Length; i++)
            {
                if (char.IsUpper(name[i]) && i > 0)
                    sb.Append('_');

                sb.Append(char.ToLowerInvariant(name[i]));
            }

            return sb.ToString();
        }
    }
}