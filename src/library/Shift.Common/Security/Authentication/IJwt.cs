using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Shift.Common
{
    /// <summary>
    /// Defines the interface for a decoded JSON web token.
    /// </summary>
    public interface IJwt
    {
        string Audience { get; set; }
        string Issuer { get; set; }
        string Subject { get; set; }
        List<string> Roles { get; set; }
        DateTimeOffset? Expiry { get; set; }
        int? Lifetime { get; set; }

        int CountClaims();

        bool ContainsClaim(ClaimName claim);
        string GetClaimValue(ClaimName claim);
        List<string> GetClaimValues(ClaimName claim);
        bool HasExpectedClaimValue(ClaimName claim, string value);

        bool IsExpired();
        double GetMinutesSinceExpiry();
        double GetMinutesUntilExpiry();

        Dictionary<ClaimName, List<string>> ToDictionary();
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ClaimName
    {
        [EnumMember(Value = "none")]
        None,

        [EnumMember(Value = "alg")]
        Algorithm,

        [EnumMember(Value = "aud")]
        Audience,

        [EnumMember(Value = "exp")]
        Expiry,

        [EnumMember(Value = "iat")]
        Issued,

        [EnumMember(Value = "iss")]
        Issuer,

        [EnumMember(Value = "ttl")]
        Lifetime,

        [EnumMember(Value = "nbf")]
        NotBefore,

        [EnumMember(Value = "sub")]
        Subject,

        [EnumMember(Value = "typ")]
        Type,

        [EnumMember(Value = "authority")]
        Authority,

        [EnumMember(Value = "org_code")]
        OrganizationCode,

        [EnumMember(Value = "org_id")]
        OrganizationId,

        [EnumMember(Value = "partition_id")]
        PartitionId,

        [EnumMember(Value = "partition_slug")]
        PartitionSlug,

        [EnumMember(Value = "person_id")]
        PersonId,

        [EnumMember(Value = "role")]
        Role,

        [EnumMember(Value = "role_debug")]
        RoleDebug,

        [EnumMember(Value = "user_debug")]
        UserDebug,

        [EnumMember(Value = "user_email")]
        UserEmail,

        [EnumMember(Value = "user_id")]
        UserId,

        [EnumMember(Value = "user_ip")]
        UserIp,

        [EnumMember(Value = "user_language")]
        UserLanguage,

        [EnumMember(Value = "user_name")]
        UserName,

        [EnumMember(Value = "user_phone")]
        UserPhone,

        [EnumMember(Value = "user_timezone")]
        UserTimeZone,

        [EnumMember(Value = "proxy_agent")]
        ProxyAgent,

        [EnumMember(Value = "proxy_subject")]
        ProxySubject
    }

    public static class ClaimNameExtensions
    {
        public static string ToJwtName(this ClaimName name)
        {
            return typeof(ClaimName)
                .GetMember(name.ToString())
                .FirstOrDefault()?
                .GetCustomAttribute<EnumMemberAttribute>()?
                .Value ?? name.ToString();
        }
    }
}