using Microsoft.Data.SqlClient;

namespace Shift.Hub.Integration.GoDaddy;

internal class DbHelper
{
    private string _connectionString;

    public DbHelper(string connectionString)
    {
        _connectionString = connectionString;
    }

    public class PartitionInfo
    {
        public string HostName { get; set; }
        public string PartitionSlug { get; set; }

        public PartitionInfo(SqlDataReader reader)
        {
            HostName = reader[nameof(HostName)] as string ?? string.Empty;
            PartitionSlug = reader[nameof(PartitionSlug)] as string ?? string.Empty;
        }
    }

    public PartitionInfo GetPartition()
    {
        const string query = @"
SELECT
    (SELECT TOP 1 SettingValue FROM [security].TPartitionSetting WHERE SettingName = 'Host:Name') AS HostName
   ,(SELECT TOP 1 SettingValue FROM [security].TPartitionSetting WHERE SettingName = 'Partition:Slug') AS PartitionSlug";

        using (var conn = new SqlConnection(_connectionString))
        {
            conn.Open();

            using (var cmd = new SqlCommand(query, conn))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        throw new Exception("Can't get partition info");

                    return new PartitionInfo(reader);
                }
            }
        }
    }

    public class OrganizationInfo
    {
        public string CompanyName { get; set; }
        public string OrganizationCode { get; set; }

        public OrganizationInfo(SqlDataReader reader)
        {
            CompanyName = (string)reader[nameof(CompanyName)];
            OrganizationCode = (string)reader[nameof(OrganizationCode)];
        }
    }

    public List<OrganizationInfo> GetOpenOrganizations()
    {
        const string query = @"
SELECT
    CompanyName
   ,OrganizationCode
FROM
    accounts.QOrganization
WHERE
    AccountStatus = 'Open'";

        var result = new List<OrganizationInfo>();

        using (var conn = new SqlConnection(_connectionString))
        {
            conn.Open();

            using (var cmd = new SqlCommand(query, conn))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        result.Add(new OrganizationInfo(reader));
                }

            }
        }

        return result;
    }
}
