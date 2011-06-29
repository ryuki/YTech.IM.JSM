using NHibernate.Validator.Constraints;
using SharpArch.Core.DomainModel;
using System;
using SharpArch.Core;

namespace YTech.IM.JSM.Core.Master
{
    public class MCostCenter : EntityWithTypedId<string>, IHasAssignedId<string>
    {
        [DomainSignature]
        [NotNull, NotEmpty]
        
        public virtual MEmployee EmployeeId { get; set; }
        public virtual string CostCenterName { get; set; }
        public virtual decimal? CostCenterTotalBudget { get; set; }
        public virtual string CostCenterStatus { get; set; }
        public virtual DateTime? CostCenterStartDate { get; set; }
        public virtual DateTime? CostCenterEndDate { get; set; }
        public virtual string CostCenterDesc { get; set; }

        public virtual string DataStatus { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual string ModifiedBy { get; set; }
        public virtual DateTime? ModifiedDate { get; set; }
        public virtual byte[] RowVersion { get; set; }

        #region Implementation of IHasAssignedId<string>

        public virtual void SetAssignedIdTo(string assignedId)
        {
            Check.Require(!string.IsNullOrEmpty(assignedId), "Assigned Id may not be null or empty");
            Id = assignedId.Trim();
        }

        #endregion
    }
}
