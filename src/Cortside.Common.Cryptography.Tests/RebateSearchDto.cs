using System;
using System.Collections.Generic;

namespace Cortside.Common.Cryptography.Tests {
    internal class RebateSearchDto {
        public List<int> ContractorIds { get; set; }
        public RebateRequestStatus RebateStatus { get; set; }
        public Guid LoanId { get; set; }
    }
}
