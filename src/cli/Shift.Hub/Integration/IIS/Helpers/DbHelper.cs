using Microsoft.Data.SqlClient;

namespace Shift.Hub.Integration.IIS;

internal class DbHelper
{
    private string _connectionString;

    public DbHelper(string connectionString)
    {
        _connectionString = connectionString;
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
