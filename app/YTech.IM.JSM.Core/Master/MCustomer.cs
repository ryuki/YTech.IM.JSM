using NHibernate.Validator.Constraints;
using SharpArch.Core.DomainModel;
using System;
using SharpArch.Core;

namespace YTech.IM.JSM.Core.Master
{
    public class MCustomer : EntityWithTypedId<string>, IHasAssignedId<string>
    {
        [DomainSignature]
        [NotNull, NotEmpty]

        public virtual RefPerson PersonId { get; set; }
        public virtual RefAddress AddressId { get; set; }
        public virtual decimal? CustomerMaxCredit { get; set; }
        public virtual string CustomerStatus { get; set; }
        public virtual string CustomerDesc { get; set; }

        public virtual string DataStatus { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual string ModifiedBy { get; set; }
        public virtual DateTime? ModifiedDate { get; set; }
        public virtual byte[] RowVersion { get; set; }

        public virtual decimal? CustomerServiceDisc { get; set; }
        public virtual decimal? CustomerProductDisc { get; set; }
        public virtual DateTime? CustomerJoinDate { get; set; }
        public virtual DateTime? CustomerLastBuy { get; set; }
        public virtual string CustomerMassageStrength { get; set; }
        public virtual string CustomerHealthProblem { get; set; }
       
        
        #region Implementation of IHasAssignedId<string>

        public virtual void SetAssignedIdTo(string assignedId)
        {
            Check.Require(!string.IsNullOrEmpty(assignedId), "Assigned Id may not be null or empty");
            Id = assignedId.Trim();
        }

        #endregion
    }
}
