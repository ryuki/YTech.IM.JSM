using NHibernate.Validator.Constraints;
using SharpArch.Core.DomainModel;
using System;
using SharpArch.Core;

namespace YTech.IM.JSM.Core.Master
{
    public class MSupplier : EntityWithTypedId<string>, IHasAssignedId<string>
    {
        [DomainSignature]
        [NotNull, NotEmpty]

        public virtual string SupplierName { get; set; }
        public virtual RefAddress AddressId { get; set; }
        public virtual string SupplierStatus { get; set; }
        public virtual decimal? SupplierMaxDebt { get; set; }
        public virtual string SupplierDesc { get; set; }
        
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
