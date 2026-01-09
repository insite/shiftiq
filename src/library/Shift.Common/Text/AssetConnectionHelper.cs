namespace Shift.Common
{
    public static class AssetConnectionHelper
    {
        public static string GetAssetRelationshipName(string connection)
        {
            if (string.IsNullOrEmpty(connection))
                return null;

            switch (connection.ToLower())
            {
                case "contains":
                case "is in":
                    return "containment";
                case "satisfies":
                    return "fulfillment";
                case "requires":
                    return "requirement";
                case "triggers":
                    return "trigger";
                case "evaluates":
                    return "evaluation";
                case "resembles":
                    return "similarity";
                case "uses":
                    return "usage";
                default:
                    return null;
            }
        }
    }
}
