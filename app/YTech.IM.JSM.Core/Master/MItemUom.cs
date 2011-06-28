using NHibernate.Validator.Constraints;
using SharpArch.Core.DomainModel;
using System;
using SharpArch.Core;

namespace YTech.IM.JSM.Core.Master
{
    public class MItemUom : EntityWithTypedId<string>, IHasAssignedId<string>
    {
        protected MItemUom() { }

        public MItemUom(MItem item)
        {
            Check.Require(item != null, "item may not be null");

            ItemId = item;
        }

        [DomainSignature]
        [NotNull, NotEmpty]
        
        public virtual MItem ItemId { get; protected set; }
        public virtual string ItemUomName { get; set; }

        public virtual MItemUom ItemUomRefId { get; set; }
        public virtual string ItemUomConverterValue { get; set; }
        public virtual decimal? ItemUomSalePrice { get; set; }
        public virtual decimal? ItemUomPurchasePrice { get; set; }
        public virtual decimal? ItemUomHppPrice { get; set; }
        public virtual string ItemUomDesc { get; set; }

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
