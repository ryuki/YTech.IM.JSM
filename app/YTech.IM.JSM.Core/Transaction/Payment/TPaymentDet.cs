using System.Collections.Generic;
using NHibernate.Validator.Constraints;
using SharpArch.Core.DomainModel;
using System;
using SharpArch.Core;
using YTech.IM.JSM.Core.Master;
using YTech.IM.JSM.Core.Transaction.Inventory;

namespace YTech.IM.JSM.Core.Transaction.Payment
{
    public class TPaymentDet: EntityWithTypedId<string>, IHasAssignedId<string>
    {
        public TPaymentDet() { }

        public TPaymentDet(TPayment payment)
        {
            Check.Require(payment != null, "payment may not be null");

            PaymentId = payment; 
        }

        [DomainSignature]
        [NotNull, NotEmpty]
        public virtual TTrans TransId { get; set; }
        public virtual TPayment PaymentId { get; protected set; }
        public virtual decimal? PaymentDetValue { get; set; } 
        public virtual string PaymentDetStatus { get; set; }
        public virtual string PaymentDetDesc { get; set; } 

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
