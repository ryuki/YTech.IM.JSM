using FluentNHibernate.Automapping;
using YTech.IM.JSM.Core.Master;
using FluentNHibernate.Automapping.Alterations;
using YTech.IM.JSM.Core.Transaction;
using YTech.IM.JSM.Core.Transaction.Inventory;

namespace YTech.IM.JSM.Data.NHibernateMaps.Transaction
{
    public class TStockCardMap : IAutoMappingOverride<TStockCard>
    {
        #region Implementation of IAutoMappingOverride<TStockCard>

        public void Override(AutoMapping<TStockCard> mapping)
        {
            mapping.DynamicUpdate();
            mapping.DynamicInsert();
            mapping.SelectBeforeUpdate();

            mapping.Table("dbo.T_STOCK_CARD");
            mapping.Id(x => x.Id, "STOCK_CARD_ID")
                 .GeneratedBy.Identity();

            mapping.References(x => x.ItemId, "ITEM_ID");
            mapping.References(x => x.WarehouseId, "WAREHOUSE_ID");
            mapping.References(x => x.TransDetId, "TRANS_DET_ID");
            mapping.Map(x => x.StockCardDate, "STOCK_CARD_DATE");
            mapping.Map(x => x.StockCardStatus, "STOCK_CARD_STATUS");
            mapping.Map(x => x.StockCardQty, "STOCK_CARD_QTY");
            mapping.Map(x => x.StockCardSaldo, "STOCK_CARD_SALDO");
            mapping.Map(x => x.StockCardDesc, "STOCK_CARD_DESC");

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
