using System.Text;

using Microsoft.Data.SqlClient;

namespace Shift.Hub.Partitions
{
    public class PartitionStore
    {
        // SQL Server caps a command at 2100 parameters and each organization contributes twelve of
        // them, so a batch of a hundred leaves plenty of headroom. Batching matters because every
        // round trip is made while the registration lock is held.
        private const int OrganizationBatchSize = 100;

        // Five seconds is generous now that the lock is scoped to a single partition: the only
        // contention left is two instances of the same app registering at the same moment, and
        // each holder finishes in a handful of round trips.
        private const int LockTimeoutMilliseconds = 5000;

        private readonly ISqlDatabase _db;

        public PartitionStore(ISqlDatabase db)
        {
            _db = db;
        }

        /// <remarks>
        /// This is intentionally NOT done in a SQL Upgrade script, and the snake_case is also
        /// intentional because we want to move the Engine database from SQL Server to PostgreSQL
        /// in the next release. After the back end is migrated then we can remove the SQL
        /// hardcoding in this class.
        /// </remarks>
        public async Task EnsureSchemaAsync()
        {
            var query = @"
IF OBJECT_ID('partition', 'U') IS NULL
    CREATE TABLE [partition] (
        partition_number    INT              NOT NULL CONSTRAINT pk_partition PRIMARY KEY,
        partition_name      VARCHAR(100)     NOT NULL,
        partition_brand     VARCHAR(50)      NOT NULL,
        partition_theme     VARCHAR(50)      NOT NULL,
        partition_domain    VARCHAR(254)     NOT NULL,
        partition_email     VARCHAR(254)     NOT NULL,
        partition_slug      VARCHAR(100)     NOT NULL,
        partition_id        UNIQUEIDENTIFIER NOT NULL,
        partition_whitelist VARCHAR(MAX),
        partition_help_url  VARCHAR(500),
        partition_logo_url  VARCHAR(500)
    );

-- Add the logo column to partition table if it's still missing

IF COL_LENGTH('partition', 'partition_logo_url') IS NULL
    ALTER TABLE [partition] ADD partition_logo_url VARCHAR(500);

-- Every URL column is the same size so no one of them is the first to overflow

IF COL_LENGTH('partition', 'partition_help_url') < 500
    ALTER TABLE [partition] ALTER COLUMN partition_help_url VARCHAR(500) NULL;

IF COL_LENGTH('partition', 'partition_logo_url') < 500
    ALTER TABLE [partition] ALTER COLUMN partition_logo_url VARCHAR(500) NULL;

IF OBJECT_ID('organization', 'U') IS NULL
    CREATE TABLE organization (
        organization_id      UNIQUEIDENTIFIER NOT NULL CONSTRAINT pk_organization PRIMARY KEY,
        partition_number     INT              NOT NULL CONSTRAINT fk_organization_partition REFERENCES [partition] (partition_number),
        organization_slug    VARCHAR(100)     NOT NULL,
        organization_name    VARCHAR(200)     NOT NULL,
        account_name         VARCHAR(200),
        account_status       VARCHAR(50),
        account_code         VARCHAR(50),
        account_number       VARCHAR(50),
        account_opened_at    DATETIMEOFFSET,
        account_closed_at    DATETIMEOFFSET,
        organization_website VARCHAR(500),
        organization_logo    VARCHAR(500)
    );

-- Widen the URL columns on organization tables. Source columns are VARCHAR(500) and SQL Server
-- throws an exception rather than truncating, so one long URL aborts the sync.

IF COL_LENGTH('organization', 'organization_website') < 500
    ALTER TABLE organization ALTER COLUMN organization_website VARCHAR(500) NULL;

IF COL_LENGTH('organization', 'organization_logo') < 500
    ALTER TABLE organization ALTER COLUMN organization_logo VARCHAR(500) NULL;
";

            await _db.ExecuteQueryAsync(query, null);
        }

        public async Task UpsertAsync(PartitionRegistration partition)
        {
            var partitionParameters = new Dictionary<string, object?>
            {
                { "@Identifier", partition.Identifier },
                { "@Number", partition.Number },
                { "@Name", partition.Name },
                { "@Brand", partition.Brand },
                { "@Theme", partition.Theme },
                { "@Domain", partition.Domain },
                { "@Email", partition.Email },
                { "@Slug", partition.Slug },
                { "@Whitelist", JoinWhitelist(partition.Whitelist) },
                { "@HelpUrl", partition.HelpUrl },
                { "@LogoUrl", partition.LogoUrl }
            };

            const string partitionQuery = @"
UPDATE [partition]
   SET partition_number = @Number, partition_name = @Name, partition_brand = @Brand, partition_theme = @Theme,
       partition_domain = @Domain, partition_email = @Email, partition_slug = @Slug,
       partition_whitelist = @Whitelist, partition_help_url = @HelpUrl, partition_logo_url = @LogoUrl
 WHERE partition_number = @Number;
IF @@ROWCOUNT = 0
    INSERT INTO [partition] (partition_number, partition_name, partition_brand, partition_theme, partition_domain,
        partition_email, partition_slug, partition_id, partition_whitelist, partition_help_url, partition_logo_url)
    VALUES (@Number, @Name, @Brand, @Theme, @Domain, @Email, @Slug, @Identifier, @Whitelist, @HelpUrl, @LogoUrl);
";

            // The lock guards the update-then-insert race for one partition_number, so the resource
            // name carries that number. A single global resource would serialize every partition
            // against every other one even though they write disjoint rows, and a deploy restarts
            // all of the app pools at once.

            var lockParameters = new Dictionary<string, object?>
            {
                { "@LockResource", $"partition-registration-{partition.Number}" },
                { "@LockTimeoutMs", LockTimeoutMilliseconds }
            };

            const string acquireLockQuery = @"
DECLARE @result INT;
DECLARE @message NVARCHAR(200);

EXEC @result = sp_getapplock
    @Resource    = @LockResource,
    @LockMode    = 'Exclusive',
    @LockOwner   = 'Transaction',
    @LockTimeout = @LockTimeoutMs;

IF @result < 0
BEGIN
    SET @message = CONCAT('Could not acquire the ', @LockResource, ' lock: sp_getapplock returned ', @result, '.');

    -- 51001 is contention (-1 timeout, -3 deadlock victim) and is worth retrying.
    -- 51002 is a broken call (-2 canceled, -999 parameter error) and is not.

    IF @result IN (-1, -3)
        THROW 51001, @message, 1;

    THROW 51002, @message, 1;
END;";

            // Upsert the partition and all of its organizations atomically.
            var statements = new List<(string Query, object? Parameters)>
            {
                (acquireLockQuery, lockParameters),
                (partitionQuery, partitionParameters)
            };

            var organizations = partition.Organizations ?? new List<OrganizationRegistration>();

            for (var index = 0; index < organizations.Count; index += OrganizationBatchSize)
            {
                var size = Math.Min(OrganizationBatchSize, organizations.Count - index);

                var batch = organizations.GetRange(index, size);

                statements.Add(BuildOrganizationBatch(partition.Number, batch));
            }

            try
            {
                await _db.ExecuteInTransactionAsync(statements);
            }
            catch (SqlException ex) when (ex.Number == PartitionLockException.RetryableErrorNumber)
            {
                throw new PartitionLockException(ex.Message, true, ex);
            }
            catch (SqlException ex) when (ex.Number == PartitionLockException.FatalErrorNumber)
            {
                throw new PartitionLockException(ex.Message, false, ex);
            }
        }

        // One statement per organization meant one network round trip per organization, all of them
        // taken while the registration lock was held. Suffixed parameter names let a whole batch of
        // upserts travel in a single command instead.

        private static (string Query, object? Parameters) BuildOrganizationBatch(int partitionNumber, List<OrganizationRegistration> organizations)
        {
            var builder = new StringBuilder();

            var parameters = new Dictionary<string, object?>
            {
                { "@PartitionNumber", partitionNumber }
            };

            for (var i = 0; i < organizations.Count; i++)
            {
                var organization = organizations[i];

                var account = organization.Account ?? new AccountRegistration();

                parameters.Add($"@Identifier{i}", organization.Identifier);
                parameters.Add($"@Slug{i}", organization.Slug);
                parameters.Add($"@Name{i}", organization.Name);
                parameters.Add($"@AccountName{i}", account.Name);
                parameters.Add($"@AccountStatus{i}", account.Status);
                parameters.Add($"@AccountCode{i}", account.Code);
                parameters.Add($"@AccountNumber{i}", account.Number);
                parameters.Add($"@OpenedAt{i}", account.OpenedAt);
                parameters.Add($"@ClosedAt{i}", account.ClosedAt);
                parameters.Add($"@Website{i}", organization.WebsiteUrl);
                parameters.Add($"@Logo{i}", organization.LogoUrl);

                // @@ROWCOUNT reads the UPDATE immediately above it, so the pairs stay independent
                // no matter how many of them are concatenated into one batch.

                builder.Append($@"
UPDATE organization
   SET partition_number = @PartitionNumber, organization_slug = @Slug{i}, organization_name = @Name{i},
       account_name = @AccountName{i}, account_status = @AccountStatus{i}, account_code = @AccountCode{i},
       account_number = @AccountNumber{i}, account_opened_at = @OpenedAt{i}, account_closed_at = @ClosedAt{i},
       organization_website = @Website{i}, organization_logo = @Logo{i}
 WHERE organization_id = @Identifier{i};
IF @@ROWCOUNT = 0
    INSERT INTO organization (organization_id, partition_number, organization_slug, organization_name, account_name,
        account_status, account_code, account_number, account_opened_at, account_closed_at, organization_website, organization_logo)
    VALUES (@Identifier{i}, @PartitionNumber, @Slug{i}, @Name{i}, @AccountName{i},
        @AccountStatus{i}, @AccountCode{i}, @AccountNumber{i}, @OpenedAt{i}, @ClosedAt{i}, @Website{i}, @Logo{i});
");
            }

            return (builder.ToString(), parameters);
        }

        public async Task<List<PartitionRegistration>> GetAllAsync()
        {
            var partitionRows = await _db.SelectAsync<PartitionRow>(@"
SELECT partition_number AS Number, partition_name AS Name, partition_brand AS Brand, partition_theme AS Theme,
       partition_domain AS Domain, partition_email AS Email, partition_slug AS Slug, partition_id AS Identifier,
       partition_whitelist AS Whitelist, partition_help_url AS HelpUrl, partition_logo_url AS LogoUrl
  FROM [partition]
 ORDER BY partition_number");

            var partitions = partitionRows
                .Select(r => new PartitionRegistration
                {
                    Number = r.Number,
                    Name = r.Name,
                    Brand = r.Brand,
                    Theme = r.Theme,
                    Domain = r.Domain,
                    Email = r.Email,
                    Slug = r.Slug,
                    Identifier = r.Identifier,
                    Whitelist = SplitWhitelist(r.Whitelist),
                    HelpUrl = r.HelpUrl,
                    LogoUrl = r.LogoUrl
                })
                .ToList();

            var organizations = await _db.SelectAsync<OrganizationRow>(@"
SELECT organization_id AS Identifier, partition_number AS PartitionNumber, organization_slug AS Slug,
       organization_name AS Name, account_name AS AccountName, account_status AS AccountStatus,
       account_code AS AccountCode, account_number AS AccountNumber, account_opened_at AS OpenedAt,
       account_closed_at AS ClosedAt, organization_website AS WebsiteUrl, organization_logo AS LogoUrl
  FROM organization
 ORDER BY organization_name");

            foreach (var partition in partitions)
            {
                partition.Organizations = organizations
                    .Where(o => o.PartitionNumber == partition.Number)
                    .Select(o => new OrganizationRegistration
                    {
                        Slug = o.Slug,
                        Name = o.Name,
                        Identifier = o.Identifier,
                        WebsiteUrl = o.WebsiteUrl,
                        LogoUrl = o.LogoUrl,
                        Account = new AccountRegistration
                        {
                            Name = o.AccountName,
                            Status = o.AccountStatus,
                            Code = o.AccountCode,
                            Number = o.AccountNumber,
                            OpenedAt = o.OpenedAt,
                            ClosedAt = o.ClosedAt
                        }
                    })
                    .ToList();
            }

            return partitions;
        }

        // SQL Server has no array type, so the whitelist is stored as a
        // comma-delimited string and converted at the store boundary.
        private static string? JoinWhitelist(List<string>? domains)
        {
            if (domains == null || domains.Count == 0)
                return null;

            return string.Join(",", domains);
        }

        private static List<string> SplitWhitelist(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return new List<string>();

            return value
                .Split(',')
                .Select(x => x.Trim())
                .Where(x => x.Length > 0)
                .ToList();
        }

        private class PartitionRow
        {
            public int Number { get; set; }
            public string Name { get; set; } = null!;
            public string Brand { get; set; } = null!;
            public string Theme { get; set; } = null!;
            public string Domain { get; set; } = null!;
            public string Email { get; set; } = null!;
            public string Slug { get; set; } = null!;
            public Guid Identifier { get; set; }
            public string? Whitelist { get; set; }
            public string? HelpUrl { get; set; }
            public string? LogoUrl { get; set; }
        }

        private class OrganizationRow
        {
            public Guid Identifier { get; set; }
            public int PartitionNumber { get; set; }
            public string Slug { get; set; } = null!;
            public string Name { get; set; } = null!;
            public string? AccountName { get; set; }
            public string? AccountStatus { get; set; }
            public string? AccountCode { get; set; }
            public string? AccountNumber { get; set; }
            public DateTimeOffset? OpenedAt { get; set; }
            public DateTimeOffset? ClosedAt { get; set; }
            public string? WebsiteUrl { get; set; }
            public string? LogoUrl { get; set; }
        }
    }
}
