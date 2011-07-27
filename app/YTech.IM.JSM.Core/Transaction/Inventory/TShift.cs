using System.Collections.Generic;
using NHibernate.Validator.Constraints;
using SharpArch.Core.DomainModel;
using System;
using SharpArch.Core;
using YTech.IM.JSM.Core.Master;

namespace YTech.IM.JSM.Core.Transaction.Inventory
{
    public class TShift : EntityWithTypedId<string>, IHasAssignedId<string>
    {

        [DomainSignature]
        [NotNull, NotEmpty]
        public virtual MEmployee EmployeeId { get; set; }
        public virtual DateTime? ShiftDate { get; set; }
        public virtual int? ShiftNo { get; set; }
        public virtual DateTime? ShiftDateFrom { get; set; }
        public virtual DateTime? ShiftDateTo { get; set; }
        public virtual string ShiftStatus { get; set; }
        public virtual string ShiftDesc { get; set; }

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
