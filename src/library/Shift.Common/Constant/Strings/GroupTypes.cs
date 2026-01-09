namespace Shift.Constant
{
    public class GroupTypes
    {
        public const string Department = "Department";
        public const string District = "District";
        public const string Employer = "Employer";
        public const string List = "List";
        public const string Role = "Role";
        public const string Team = "Team";
        public const string Venue = "Venue";

        public static string[] Values => new string[] { Department, District, Employer, List, Role, Team, Venue };

        public static string GetIcon(string type)
        {
            switch (type)
            {
                case Department: return "far fa-line-columns";
                case District: return "far fa-line-columns";
                case Employer: return "far fa-user-tie";
                case List: return "far fa-list-alt";
                case Role: return "far fa-key";
                case Team: return "far fa-sitemap";
                case Venue: return "far fa-location";
            }
            return "fas fa-users";
        }
    }
}
