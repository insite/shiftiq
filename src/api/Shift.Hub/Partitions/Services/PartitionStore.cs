namespace Shift.Hub.Partitions
{
    public class PartitionStore
    {
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

            const string organizationQuery = @"
UPDATE organization
   SET partition_number = @PartitionNumber, organization_slug = @Slug, organization_name = @Name,
       account_name = @AccountName, account_status = @AccountStatus, account_code = @AccountCode,
       account_number = @AccountNumber, account_opened_at = @OpenedAt, account_closed_at = @ClosedAt,
       organization_website = @Website, organization_logo = @Logo
 WHERE organization_id = @Identifier;
IF @@ROWCOUNT = 0
    INSERT INTO organization (organization_id, partition_number, organization_slug, organization_name, account_name,
        account_status, account_code, account_number, account_opened_at, account_closed_at, organization_website, organization_logo)
    VALUES (@Identifier, @PartitionNumber, @Slug, @Name, @AccountName,
        @AccountStatus, @AccountCode, @AccountNumber, @OpenedAt, @ClosedAt, @Website, @Logo);
";

            const string acquireLockQuery = @"
DECLARE @result INT;
EXEC @result = sp_getapplock
    @Resource    = 'partition-registration',
    @LockMode    = 'Exclusive',
    @LockOwner   = 'Transaction',
    @LockTimeout = 15000;
IF @result < 0
    THROW 51000, 'Could not acquire the partition-registration lock.', 1;";

            // Upsert the partition and all of its organizations atomically.
            var statements = new List<(string Query, object? Parameters)>
            {
                (acquireLockQuery, null),
                (partitionQuery, partitionParameters)
            };

            foreach (var organization in partition.Organizations)
            {
                var account = organization.Account ?? new AccountRegistration();

                var organizationParameters = new Dictionary<string, object?>
                {
                    { "@PartitionNumber", partition.Number },
                    { "@Identifier", organization.Identifier },
                    { "@Slug", organization.Slug },
                    { "@Name", organization.Name },
                    { "@AccountName", account.Name },
                    { "@AccountStatus", account.Status },
                    { "@AccountCode", account.Code },
                    { "@AccountNumber", account.Number },
                    { "@OpenedAt", account.OpenedAt },
                    { "@ClosedAt", account.ClosedAt },
                    { "@Website", organization.WebsiteUrl },
                    { "@Logo", organization.LogoUrl }
                };

                statements.Add((organizationQuery, organizationParameters));
            }

            await _db.ExecuteInTransactionAsync(statements);
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
