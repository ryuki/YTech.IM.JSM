using NHibernate.Validator.Constraints;
using SharpArch.Core.DomainModel;
using System;
using SharpArch.Core;

namespace YTech.IM.JSM.Core.Master
{
    public class MEmployee : EntityWithTypedId<string>, IHasAssignedId<string>
    {
        [DomainSignature]
        [NotNull, NotEmpty]

        public virtual RefPerson PersonId { get; set; }
        public virtual MDepartment DepartmentId { get; set; }
        public virtual string EmployeeStatus { get; set; }
        public virtual string EmployeeDesc { get; set; }

        public virtual string DataStatus { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual string ModifiedBy { get; set; }
        public virtual DateTime? ModifiedDate { get; set; }
        public virtual byte[] RowVersion { get; set; }

        public virtual decimal? EmployeeCommissionProductVal { get; set; }
        public virtual decimal? EmployeeCommissionServiceVal { get; set; }
        public virtual string EmployeeCommissionProductType { get; set; }
        public virtual string EmployeeCommissionServiceType { get; set; }
        
        #region Implementation of IHasAssignedId<string>

        public virtual void SetAssignedIdTo(string assignedId)
        {
            Check.Require(!string.IsNullOrEmpty(assignedId), "Assigned Id may not be null or empty");
            Id = assignedId.Trim();
        }

        #endregion
    }
}
