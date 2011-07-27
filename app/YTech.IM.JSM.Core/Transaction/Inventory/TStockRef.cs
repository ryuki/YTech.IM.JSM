using System.Collections.Generic;
using NHibernate.Validator.Constraints;
using SharpArch.Core.DomainModel;
using System;
using SharpArch.Core;
using YTech.IM.JSM.Core.Master;

namespace YTech.IM.JSM.Core.Transaction.Inventory
{
    public class TStockRef : EntityWithTypedId<string>, IHasAssignedId<string>
    {
        public TStockRef()
        {
        }

        public TStockRef(TStock stock)
        {
            Check.Require(stock != null, "stock may not be null");
            StockId = stock;
        }

        [DomainSignature]
        [NotNull, NotEmpty]
        public virtual TStock StockId { get; set; }
        public virtual TTransDet TransDetId { get; set; }
        public virtual decimal? StockRefQty { get; set; }
        public virtual DateTime? StockRefDate { get; set; }
        public virtual decimal? StockRefPrice { get; set; }
        public virtual string StockRefStatus { get; set; }
        public virtual string StockRefDesc { get; set; }

        public virtual IList<TStockRef> StockRefs { get; protected set; }

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
