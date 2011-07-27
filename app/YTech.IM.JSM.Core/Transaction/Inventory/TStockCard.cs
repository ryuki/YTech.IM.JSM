using System.Collections.Generic;
using NHibernate.Validator.Constraints;
using SharpArch.Core.DomainModel;
using System;
using SharpArch.Core;
using YTech.IM.JSM.Core.Master;

namespace YTech.IM.JSM.Core.Transaction.Inventory
{
    public class TStockCard : EntityWithTypedId<long>
    {

        [DomainSignature]
        [NotNull, NotEmpty]
        public virtual MWarehouse WarehouseId { get; set; }
        public virtual MItem ItemId { get; set; }
        public virtual TTransDet TransDetId { get; set; }
        public virtual DateTime? StockCardDate { get; set; }
        public virtual bool StockCardStatus { get; set; }
        public virtual decimal? StockCardQty { get; set; }
        public virtual decimal? StockCardSaldo { get; set; }
        public virtual string StockCardDesc { get; set; }

        public virtual string DataStatus { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual string ModifiedBy { get; set; }
        public virtual DateTime? ModifiedDate { get; set; }
        public virtual byte[] RowVersion { get; set; }

        //#region Implementation of IHasAssignedId<string>

        //public virtual void SetAssignedIdTo(string assignedId)
        //{
        //    Check.Require(!string.IsNullOrEmpty(assignedId), "Assigned Id may not be null or empty");
        //    Id = assignedId.Trim();
        //}

        //#endregion
    }
}
