using FluentNHibernate.Automapping;
using YTech.IM.JSM.Core.Master;
using FluentNHibernate.Automapping.Alterations;
using YTech.IM.JSM.Core.Transaction;
using YTech.IM.JSM.Core.Transaction.Inventory;

namespace YTech.IM.JSM.Data.NHibernateMaps.Transaction
{
    public class TTransDetItemMap : IAutoMappingOverride<TTransDetItem>
    {
        #region Implementation of IAutoMappingOverride<TTransDetItem>

        public void Override(AutoMapping<TTransDetItem> mapping)
        {
            mapping.DynamicUpdate();
            mapping.DynamicInsert();
            mapping.SelectBeforeUpdate();

            mapping.Table("dbo.T_TRANS_DET_ITEM");
            mapping.Id(x => x.Id, "TRANS_DET_ITEM_ID")
                 .GeneratedBy.Assigned();

            mapping.References(x => x.TransDetId, "TRANS_DET_ID").Not.Nullable();
            mapping.References(x => x.ItemCatId, "ITEM_CAT_ID") ;
            mapping.References(x => x.ItemId, "ITEM_ID") ;
            mapping.References(x => x.ItemUomId, "ITEM_UOM_ID") ;
            mapping.Map(x => x.ItemQty, "ITEM_QTY");
            mapping.Map(x => x.ItemStatus, "ITEM_STATUS");
            mapping.Map(x => x.ItemDesc, "ITEM_DESC"); 

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
