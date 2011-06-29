using NHibernate.Validator.Constraints;
using SharpArch.Core.DomainModel;
using System;
using SharpArch.Core;

namespace YTech.IM.JSM.Core.Master
{
    public class MWarehouse : EntityWithTypedId<string>, IHasAssignedId<string>
    {
        [DomainSignature]
        [NotNull, NotEmpty]

        public virtual string WarehouseName { get; set; }
        public virtual RefAddress AddressId { get; set; }
        public virtual MCostCenter CostCenterId { get; set; }
        public virtual string WarehouseType { get; set; }
        public virtual MEmployee EmployeeId { get; set; }
        public virtual string WarehouseStatus { get; set; }
        public virtual string WarehouseIsDefault { get; set; }
        public virtual byte[] WarehousePhoto { get; set; }
        public virtual string WarehouseDesc { get; set; }
        
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
