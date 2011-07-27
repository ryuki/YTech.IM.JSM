using System.Collections.Generic;
using NHibernate.Validator.Constraints;
using SharpArch.Core.DomainModel;
using System;
using SharpArch.Core;
using YTech.IM.JSM.Core.Master;

namespace YTech.IM.JSM.Core.Transaction.Inventory
{
    public class TTransDetItem: EntityWithTypedId<string>, IHasAssignedId<string>
    {
        public TTransDetItem() { }

        public TTransDetItem(TTransDet transDet)
        {
            Check.Require(transDet != null, "transDet may not be null");

            TransDetId = transDet;
        }

        [DomainSignature]
        [NotNull, NotEmpty]
        public virtual TTransDet TransDetId { get; protected set; }
        public virtual MItemCat ItemCatId { get; set; }
        public virtual MItem ItemId { get; set; }
        public virtual MItemUom ItemUomId { get; set; }
        public virtual decimal? ItemQty { get; set; }
        public virtual string ItemStatus { get; set; }
        public virtual string ItemDesc { get; set; }  

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
