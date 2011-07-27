using FluentNHibernate.Automapping;
using YTech.IM.JSM.Core.Master;
using FluentNHibernate.Automapping.Alterations;
using YTech.IM.JSM.Core.Transaction;
using YTech.IM.JSM.Core.Transaction.Inventory;

namespace YTech.IM.JSM.Data.NHibernateMaps.Transaction
{
    public class TStockMap : IAutoMappingOverride<TStock>
    {
        #region Implementation of IAutoMappingOverride<TStock>

        public void Override(AutoMapping<TStock> mapping)
        {
            mapping.DynamicUpdate();
            mapping.DynamicInsert();
            mapping.SelectBeforeUpdate();

            mapping.Table("dbo.T_STOCK");
            mapping.Id(x => x.Id, "STOCK_ID")
                 .GeneratedBy.Assigned();

            mapping.References(x => x.ItemId, "ITEM_ID");
            mapping.References(x => x.WarehouseId, "WAREHOUSE_ID");
            mapping.References(x => x.TransDetId, "TRANS_DET_ID");
            mapping.Map(x => x.StockDate, "STOCK_DATE");
            mapping.Map(x => x.StockQty, "STOCK_QTY");
            mapping.Map(x => x.StockPrice, "STOCK_PRICE");
            mapping.Map(x => x.StockStatus, "STOCK_STATUS");
            mapping.Map(x => x.StockDesc, "STOCK_DESC");

            mapping.Map(x => x.DataStatus, "DATA_STATUS");
            mapping.Map(x => x.CreatedBy, "CREATED_BY");
            mapping.Map(x => x.CreatedDate, "CREATED_DATE");
            mapping.Map(x => x.ModifiedBy, "MODIFIED_BY");
            mapping.Map(x => x.ModifiedDate, "MODIFIED_DATE");
            mapping.Map(x => x.RowVersion, "ROW_VERSION").ReadOnly();

            mapping.HasMany(x => x.StockRefs)
                .AsBag()
                .Inverse()
                .KeyColumn("STOCK_ID")
                .Cascade.All();
        }

        #endregion
    }
}
