using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace Cortside.Common.Security {
    /// <summary>
    /// Subject principal class
    /// </summary>
    public class SubjectPrincipal : ISubjectPrincipal {
        private readonly List<ClaimsIdentity> identities = new List<ClaimsIdentity>();
        private SubjectPrincipal actor;

        /// <summary>
        /// Returns the correct SubjectPrincipal instance
        /// </summary>
        /// <param name="principal">IPrincipal</param>
        /// <returns>SubjectPrincipal</returns>
        public static SubjectPrincipal From(IPrincipal principal) {
            if (principal != null) {
                return new SubjectPrincipal(principal);
            }
            return new SubjectPrincipal();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        private SubjectPrincipal() {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public SubjectPrincipal(IPrincipal principal) {
            identities.Add((ClaimsIdentity)principal.Identity);
        }

        public SubjectPrincipal(List<Claim> claims) {
            identities.Add(new ClaimsIdentity(claims));
        }

        public string SubjectId { get => Actor != null ? Actor.SubjectId : Claims.FirstOrDefault(c => c.Type == "sub")?.Value; }
        public string Name { get => Actor != null ? Actor.Name : FindFirst(c => c.Type == "name")?.Value; }
        public string GivenName { get => Actor != null ? Actor.GivenName : FindFirst(c => c.Type == "given_name")?.Value; }
        public string FamilyName { get => Actor != null ? Actor.FamilyName : FindFirst(c => c.Type == "family_name")?.Value; }
        public string UserPrincipalName { get => Actor != null ? Actor.UserPrincipalName : FindFirst(c => c.Type == "upn")?.Value ?? FindFirst(c => c.Type == "client_id")?.Value; }

        public SubjectPrincipal Actor {
            get {
                if (actor != null) {
                    return actor;
                }

                var act = Claims.FirstOrDefault(x => x.Type == "act");
                if (act != null) {
                    var claims = Claims.Where(x => x.Type.StartsWith("act_")).Select(x => new Claim(x.Type.Replace("act_", ""), x.Value)).ToList();
                    claims.Add(new Claim("sub", act.Value));

                    actor = new SubjectPrincipal(claims);
                }

                return actor;
            }
        }

        /// <summary>
        /// Retrieves a <see cref="IEnumerable{Claim}"/> where each claim is matched by <param name="match"/>.
        /// </summary>
        /// <param name="match">The predicate that performs the matching logic.</param>
        /// <returns>A <see cref="IEnumerable{Claim}"/> of matched claims.</returns>
        /// <remarks>Returns claims from all Identities</remarks>
        /// SafeCritical since it access m_identities
        public IEnumerable<SubjectClaim> Claims {
            get {
                List<Claim> claims = new List<Claim>();

                foreach (ClaimsIdentity identity in identities) {
                    if (identity != null) {
                        claims.AddRange(identity.Claims);
                    }
                }

                var subjectClaims = claims.Select(x => new SubjectClaim(x.Type, x.Value)).ToList();
                return subjectClaims.AsReadOnly().OrderBy(x => x.Type);
            }
        }

        /// <summary>
        /// Retrieves the first <see cref="Claim"/> that is matched by <param name="match"/>.
        /// </summary>
        /// <param name="match">The predicate that performs the matching logic.</param>
        /// <returns>A <see cref="Claim"/>, null if nothing matches.</returns>
        /// <remarks>All identities are queried.</remarks>
        private Claim FindFirst(Predicate<Claim> match) {
            if (match == null) {
                throw new ArgumentNullException("match");
            }

            Claim claim = null;

            foreach (ClaimsIdentity identity in identities) {
                if (identity != null) {
                    claim = identity.FindFirst(match);
                    if (claim != null) {
                        return claim;
                    }
                }
            }

            return claim;
        }

        /// <summary>
        /// Retrieves the first <see cref="Claim"/> where the Claim.Type equals <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type of the claim to match.</param>
        /// <returns>A <see cref="Claim"/>, null if nothing matches.</returns>
        /// <remarks>Comparison is made using Ordinal case in-sensitive, all identities are queried.</remarks>
        public Claim FindFirst(string type) {
            if (type == null) {
                throw new ArgumentNullException("type");
            }

            Claim claim = null;

            foreach (ClaimsIdentity identity in identities) {
                if (identity != null) {
                    claim = identity.FindFirst(type);
                    if (claim != null) {
                        return claim;
                    }
                }
            }

            return claim;
        }
    }
}
