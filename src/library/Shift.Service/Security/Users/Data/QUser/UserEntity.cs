using Shift.Service.Booking;
using Shift.Service.Content;
using Shift.Service.Directory;

namespace Shift.Service.Security;

public partial class UserEntity
{
    public ICollection<EventUserEntity> Events { get; set; } = new List<EventUserEntity>();
    public ICollection<MembershipEntity> Memberships { get; set; } = new List<MembershipEntity>();
    public ICollection<PersonEntity> People { get; set; } = new List<PersonEntity>();
    public ICollection<FileEntity> Files { get; set; } = new List<FileEntity>();

    public Guid UserIdentifier { get; set; }

    public bool AccessGrantedToCmds { get; set; }
    public bool MultiFactorAuthentication { get; set; }

    public string? DefaultPassword { get; set; }
    public string Email { get; set; } = null!;
    public string? EmailAlternate { get; set; }
    public string? EmailVerified { get; set; }
    public string FirstName { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string? Honorific { get; set; }
    public string? ImageUrl { get; set; }
    public string? Initials { get; set; }
    public string LastName { get; set; } = null!;
    public string? LoginOrganizationCode { get; set; }
    public string? MiddleName { get; set; }
    public string? MultiFactorAuthenticationCode { get; set; }
    public string? OAuthProviderUserId { get; set; }
    public string? OldUserPasswordHash { get; set; }
    public string? PhoneMobile { get; set; }
    public string? SoundexFirstName { get; set; }
    public string? SoundexLastName { get; set; }
    public string TimeZone { get; set; } = null!;
    public string UserPasswordHash { get; set; } = null!;

    public int? PrimaryLoginMethod { get; set; }
    public int? SecondaryLoginMethod { get; set; }
    public int? UserPasswordChangeRequested { get; set; }

    public DateTimeOffset? AccountCloaked { get; set; }
    public DateTimeOffset? DefaultPasswordExpired { get; set; }
    public DateTimeOffset? UserLicenseAccepted { get; set; }
    public DateTimeOffset? UserPasswordChanged { get; set; }
    public DateTimeOffset UserPasswordExpired { get; set; }
    public DateTimeOffset? UtcArchived { get; set; }
    public DateTimeOffset? UtcUnarchived { get; set; }
}