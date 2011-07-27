using FluentNHibernate.Automapping;
using YTech.IM.JSM.Core.Master;
using FluentNHibernate.Automapping.Alterations;
using YTech.IM.JSM.Core.Transaction;
using YTech.IM.JSM.Core.Transaction.Inventory;
namespace YTech.IM.JSM.Data.NHibernateMaps.Transaction
{
    public class TStockRefMap : IAutoMappingOverride<TStockRef>
    {
        #region Implementation of IAutoMappingOverride<TStockRef>

        public void Override(AutoMapping<TStockRef> mapping)
        {
            mapping.DynamicUpdate();
            mapping.DynamicInsert();
            mapping.SelectBeforeUpdate();

            mapping.Table("dbo.T_STOCK_REF");
            mapping.Id(x => x.Id, "STOCK_REF_ID")
                 .GeneratedBy.Assigned();

            mapping.References(x => x.StockId, "STOCK_ID");
            mapping.References(x => x.TransDetId, "TRANS_DET_ID");
            mapping.Map(x => x.StockRefQty, "STOCK_REF_QTY");
            mapping.Map(x => x.StockRefDate, "STOCK_REF_DATE");
            mapping.Map(x => x.StockRefPrice, "STOCK_REF_PRICE");
            mapping.Map(x => x.StockRefStatus, "STOCK_REF_STATUS");
            mapping.Map(x => x.StockRefDesc, "STOCK_REF_DESC");

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
