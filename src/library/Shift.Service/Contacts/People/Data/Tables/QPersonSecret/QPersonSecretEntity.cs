namespace Shift.Service.Directory;

public partial class QPersonSecretEntity
{
    public Guid PersonIdentifier { get; set; }
    public Guid SecretIdentifier { get; set; }

    public string SecretName { get; set; } = null!;
    public string SecretType { get; set; } = null!;
    public string SecretValue { get; set; } = null!;

    public int? SecretLifetimeLimit { get; set; }

    public DateTimeOffset SecretExpiry { get; set; }
}