using System.Collections.Generic;
using NHibernate.Validator.Constraints;
using SharpArch.Core.DomainModel;
using System;
using SharpArch.Core;
using YTech.IM.JSM.Core.Master;

namespace YTech.IM.JSM.Core.Transaction.Payment
{
    public class TPayment : EntityWithTypedId<string>, IHasAssignedId<string>
    {
        public TPayment()
        {
            InitMembers();
        }

        /// <summary>
        /// Since we want to leverage automatic properties, init appropriate members here.
        /// </summary>
        private void InitMembers()
        {
            PaymentDets = new List<TPaymentDet>();
        }


        [DomainSignature]
        [NotNull, NotEmpty]
        public virtual string PaymentType { get; set; }
        public virtual DateTime? PaymentDate { get; set; }
        public virtual decimal? PaymentTotal { get; set; }
        public virtual string PaymentStatus { get; set; }
        public virtual string PaymentDesc { get; set; }

        public virtual IList<TPaymentDet> PaymentDets { get; protected set; }
        
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
