using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Claims;

using Shift.Common;

namespace Shift.Common
{
    public class ClaimConverter : IClaimConverter
    {
        public const string DefaultLanguage = "en";

        public const string DefaultTimeZone = "UTC";

        private readonly SecuritySettings _settings;

        public ClaimConverter(SecuritySettings securitySettings)
        {
            _settings = securitySettings;
        }

        public Claim ToClaim(ClaimName name, string value)
            => new Claim(name.ToJwtName(), value);

        public IEnumerable<Claim> ToClaims(IPrincipal principal)
        {
            var claims = new List<Claim>
            {
                ToClaim(ClaimName.UserId, principal.User.Identifier.ToString()),
                ToClaim(ClaimName.UserEmail, principal.User.Email),
                ToClaim(ClaimName.UserName, principal.User.Name),
                ToClaim(ClaimName.Authority, principal.Authority.ToString())
            };

            if (principal.User.Phone.IsNotEmpty())
                claims.Add(ToClaim(ClaimName.UserPhone, principal.User.Phone));

            if (principal.User.Language != null)
                claims.Add(ToClaim(ClaimName.UserLanguage, principal.User.Language));
            else
                claims.Add(ToClaim(ClaimName.UserLanguage, DefaultLanguage));

            if (principal.User.TimeZone != null)
                claims.Add(ToClaim(ClaimName.UserTimeZone, principal.User.TimeZone));
            else
                claims.Add(ToClaim(ClaimName.UserTimeZone, DefaultTimeZone));

            var lifetime = principal.Claims.Lifetime;
            if (lifetime != null)
                claims.Add(ToClaim(ClaimName.Lifetime, lifetime.Value.ToString()));

            if (principal.IPAddress.IsNotEmpty())
                claims.Add(ToClaim(ClaimName.UserIp, principal.IPAddress));

            if (principal.Organization != null)
            {
                var org = principal.Organization;

                claims.Add(ToClaim(ClaimName.OrganizationId, org.Identifier.ToString()));

                if (org.Slug.IsNotEmpty())
                    claims.Add(ToClaim(ClaimName.OrganizationCode, org.Slug));
            }

            if (principal.Partition != null)
            {
                var partition = principal.Partition;

                claims.Add(ToClaim(ClaimName.PartitionId, partition.Identifier.ToString()));
                claims.Add(ToClaim(ClaimName.PartitionSlug, partition.Slug));
            }

            if (principal.Person != null)
            {
                var per = principal.Person;

                claims.Add(ToClaim(ClaimName.PersonId, per.Identifier.ToString()));
            }

            if (principal.Proxy?.Agent != null && principal.Proxy?.Subject != null)
            {
                claims.Add(ToClaim(ClaimName.ProxyAgent, principal.Proxy.Agent.Identifier.ToString()));
                claims.Add(ToClaim(ClaimName.ProxySubject, principal.Proxy.Subject.Identifier.ToString()));
            }

            if (principal.Roles != null)
            {
                foreach (var role in principal.Roles)
                {
                    claims.Add(ToClaim(ClaimName.Role, role.Name));
                }
            }

            return claims.ToArray();
        }

        public ClaimsIdentity ToClaimsIdentity(IJwt claims, string authenticationType)
        {
            var list = new List<Claim>();

            var dictionary = claims.ToDictionary();

            foreach (var key in dictionary.Keys)
            {
                var values = dictionary[key];

                foreach (var value in values)
                {
                    list.Add(new Claim(key.ToJwtName(), value));
                }
            }

            return new ClaimsIdentity(list, authenticationType);
        }

        public Dictionary<ClaimName, List<string>> ToDictionary(IEnumerable<Claim> claims)
        {
            var nameMap = typeof(ClaimName)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Select(field => new
                {
                    Enum = (ClaimName)field.GetValue(null),
                    Value = field.GetCustomAttribute<EnumMemberAttribute>()?.Value ?? field.Name
                })
                .ToDictionary(x => x.Value, x => x.Enum, StringComparer.OrdinalIgnoreCase);

            var dictionary = claims
                .Where(c => nameMap.ContainsKey(c.Type))
                .GroupBy(c => nameMap[c.Type])
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(c => c.Value).ToList()
                );

            return dictionary;
        }

        public IPrincipal ToPrincipal(IJwt jwt)
        {
            var list = new List<Claim>();

            var dictionary = jwt.ToDictionary();

            foreach (var key in dictionary.Keys)
            {
                var values = dictionary[key];
                foreach (var value in values)
                    list.Add(new Claim(key.ToJwtName(), value));
            }

            return ToPrincipal(list);
        }

        public IPrincipal ToPrincipal(Dictionary<ClaimName, string> claims)
        {
            return ToPrincipal(claims.Select(x => new Claim(x.Key.ToJwtName(), x.Value)));
        }

        public IPrincipal ToPrincipal(IEnumerable<Claim> claims)
        {
            var principal = new Principal
            {
                CookieId = GetClaimAsGuid(ClaimName.CookieId),
                User = new Actor
                {
                    Identifier = GetClaimAsGuid(ClaimName.UserId),
                    Email = GetClaim(ClaimName.UserEmail),
                    Name = GetClaim(ClaimName.UserName),
                    Phone = GetClaim(ClaimName.UserPhone),
                    Language = GetClaim(ClaimName.UserLanguage) ?? DefaultLanguage,
                    TimeZone = GetClaim(ClaimName.UserTimeZone) ?? DefaultTimeZone
                },
                Organization = new Model
                {
                    Identifier = GetClaimAsGuid(ClaimName.OrganizationId),
                    Name = GetClaim(ClaimName.OrganizationCode),
                    Slug = GetClaim(ClaimName.OrganizationCode),
                },
                Partition = new Model { Identifier = GetClaimAsGuid(ClaimName.PartitionId), Slug = GetClaim(ClaimName.PartitionSlug) },
                Person = new Model { Identifier = GetClaimAsGuid(ClaimName.PersonId) },
                Roles = GetRoles(ClaimName.Role),
                IPAddress = GetClaim(ClaimName.UserIp),
                Authority = GetAuthority(ClaimName.Authority),
            };

            principal.IsAuthenticated = principal.User.Identifier != Guid.Empty;

            principal.Proxy = GetProxy();

            principal.Claims.Lifetime = GetClaimAsInt(ClaimName.Lifetime);

            return principal;

            string GetClaim(ClaimName type)
            {
                var claim = claims.FirstOrDefault(x => type.ToJwtName() == x.Type || x.Properties.Any(p => p.Value == type.ToJwtName()));
                if (claim != null)
                    return claim.Value;

                return null;
            }

            Guid GetClaimAsGuid(ClaimName type)
            {
                var claim = claims.FirstOrDefault(x => type.ToJwtName() == x.Type || x.Properties.Any(p => p.Value == type.ToJwtName()));
                if (claim != null && Guid.TryParse(claim.Value, out var result))
                    return result;
                return Guid.Empty;
            }

            int GetClaimAsInt(ClaimName type)
            {
                var claim = claims.FirstOrDefault(x => type.ToJwtName() == x.Type || x.Properties.Any(p => p.Value == type.ToJwtName()));
                if (claim != null && int.TryParse(claim.Value, out var result))
                    return result;
                return 0;
            }

            List<Role> GetRoles(ClaimName type)
            {
                var list = new List<Role>();

                var roleClaims = claims.Where(x => type.ToJwtName() == x.Type);

                foreach (var roleClaim in roleClaims)
                {
                    var value = roleClaim.Value;

                    if (!string.IsNullOrEmpty(value))
                        list.Add(new Role(value));
                }

                return list;
            }

            AuthorityAccess GetAuthority(ClaimName type)
            {
                var claim = GetClaim(type);

                return Enum.TryParse<AuthorityAccess>(claim, out var result)
                    ? result
                    : AuthorityAccess.Unspecified;
            }

            Proxy GetProxy()
            {
                var agent = GetClaimAsGuid(ClaimName.ProxyAgent);
                var subject = GetClaimAsGuid(ClaimName.ProxySubject);
                if (agent != Guid.Empty && subject != Guid.Empty)
                    return new Proxy(agent, subject);

                var agentEmail = GetClaim(ClaimName.ProxyAgentEmail);

                return !string.IsNullOrEmpty(agentEmail)
                    ? new Proxy(new Actor { Email = agentEmail }, null)
                    : null;
            }
        }

        public bool IsSentinel(string sentinelSecret)
        {
            if (_settings.Sentinels == null)
                return false;

            var sentinels = _settings.Sentinels.ToArray();

            return sentinels.Any(s => s?.Secret == sentinelSecret);
        }

        public IPrincipal ToSentinel(JwtRequest request)
        {
            if (!IsSentinel(request.Secret))
                throw new SentinelNotFoundException();

            var sentinels = _settings.Sentinels.ToArray();

            var sentinel = sentinels.Single(s => s?.Secret == request.Secret);

            var principal = new Principal();

            principal.User = new Actor
            {
                Email = sentinel.Email,
                Name = sentinel.Name,
                Language = sentinel.Language ?? DefaultLanguage,
                TimeZone = sentinel.TimeZone ?? DefaultTimeZone
            };

            if (sentinel.Identifier != Guid.Empty)
                principal.User.Identifier = sentinel.Identifier;
            else
                principal.User.Identifier = UuidFactory.CreateV5(sentinel.Email);

            if (request.Organization.HasValue)
                principal.Organization = new Model { Identifier = request.Organization.Value };

            // The meaning of a request that explicitly specifies both a proxy agent and a proxy
            // subject is ambiguous, and therefore disallowed, because the security implications are
            // significant. For example, both these statements cannot be true at the same time in
            // the same context:
            //   * Alice (Proxy Agent)   acts on behalf of  John (Proxy Subject).
            //   * Alice (Proxy Subject) is impersonated by John (Proxy Agent).

            if (request.Agent.HasValue && request.Subject.HasValue)
                throw new ArgumentException($"This is an invalid JWT request because it specifies a Proxy Agent {request.Agent} and a Proxy Subject {request.Subject}. A JWT request is permitted to specify one or the other (or neither), but not both.");

            else if (request.Agent.HasValue)
                principal.Proxy = new Proxy
                {
                    Agent = new Actor { Identifier = request.Agent.Value },
                    Subject = new Actor { Identifier = principal.User.Identifier }
                };

            else if (request.Subject.HasValue)
                principal.Proxy = new Proxy
                {
                    Agent = new Actor { Identifier = principal.User.Identifier },
                    Subject = new Actor { Identifier = request.Subject.Value }
                };

            if (sentinel.Roles != null && sentinel.Roles.Length > 0)
                principal.Roles = sentinel.Roles.Select(x => new Role(x)).ToList();

            principal.Roles.Add(new Role(sentinel.Email));

            return principal;
        }

        public int CalculateLifetime(int? assigned, int? requested, int? @default)
        {
            // If the token has a lifetime already assigned to it, and if a lifetime is explicitly
            // requested, then use the smaller of the two values. Otherwise, use the default.

            if (requested.HasValue && requested.Value > 0)
            {
                if (assigned.HasValue && assigned.Value > 0)
                    return Math.Min(assigned.Value, requested.Value);

                return requested.Value;
            }

            return @default ?? JwtRequest.DefaultLifetime;
        }
    }
}