using FluentNHibernate.Automapping;
using YTech.IM.JSM.Core.Master;
using FluentNHibernate.Automapping.Alterations;
using YTech.IM.JSM.Core.Transaction;
using YTech.IM.JSM.Core.Transaction.Inventory;

namespace YTech.IM.JSM.Data.NHibernateMaps.Transaction
{
    public class TStockItemMap : IAutoMappingOverride<TStockItem>
    {
        #region Implementation of IAutoMappingOverride<TStockItem>

        public void Override(AutoMapping<TStockItem> mapping)
        {
            mapping.DynamicUpdate();
            mapping.DynamicInsert();
            mapping.SelectBeforeUpdate();

            mapping.Table("dbo.T_STOCK_ITEM");
            mapping.Id(x => x.Id, "STOCK_ITEM_ID")
                 .GeneratedBy.Assigned();

            mapping.References(x => x.ItemId, "ITEM_ID");
            mapping.References(x => x.WarehouseId, "WAREHOUSE_ID");
            mapping.Map(x => x.ItemStockMax, "ITEM_STOCK_MAX");
            mapping.Map(x => x.ItemStockMin, "ITEM_STOCK_MIN");
            mapping.Map(x => x.ItemStock, "ITEM_STOCK");
            mapping.Map(x => x.ItemStockRack, "ITEM_STOCK_RACK");

            mapping.Map(x => x.DataStatus, "DATA_STATUS");
            mapping.Map(x => x.CreatedBy, "CREATED_BY");
            mapping.Map(x => x.CreatedDate, "CREATED_DATE");
            mapping.Map(x => x.ModifiedBy, "MODIFIED_BY");
            mapping.Map(x => x.ModifiedDate, "MODIFIED_DATE");
            mapping.Map(x => x.RowVersion, "ROW_VERSION").ReadOnly();
        }

        #endregion
    }
}
