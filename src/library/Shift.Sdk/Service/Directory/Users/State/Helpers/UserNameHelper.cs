namespace InSite.Domain.Contacts
{
    public static class UserNameHelper
    {
        public static string GetFullName(string policy, string firstName, string middleName, string lastName, string suffix)
        {
            var full = string.Empty;

            switch (policy)
            {
                case "{First} {Last}":
                    full = $"{firstName} {lastName}";
                    break;

                case "{First} {Middle} {Last}":
                    full = !string.IsNullOrWhiteSpace(middleName)
                        ? $"{firstName} {middleName} {lastName}"
                        : $"{firstName} {lastName}";
                    break;

                case "{Last}, {First}":
                    full = $"{lastName}, {firstName}";
                    break;

                case "{Last}, {First} {Middle}":
                    full = !string.IsNullOrWhiteSpace(middleName)
                        ? $"{lastName}, {firstName} {middleName}"
                        : $"{lastName}, {firstName}";
                    break;

                default:
                    full = $"{firstName} {lastName}";
                    break;
            }

            if (!string.IsNullOrEmpty(suffix))
                full += $" [{suffix}]";

            return full;
        }
    }
}
