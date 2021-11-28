using System.Collections.Generic;

namespace Cortside.Common.Security {

    public interface ISubjectPrincipal {
        string SubjectId { get; }

        string Name { get; }

        string GivenName { get; }

        string FamilyName { get; }

        string UserPrincipalName { get; }

        IEnumerable<SubjectClaim> Claims { get; }
    }
}
