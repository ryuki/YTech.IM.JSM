using FluentNHibernate.Automapping;
using YTech.IM.JSM.Core.Master;
using FluentNHibernate.Automapping.Alterations;
using YTech.IM.JSM.Core.Transaction;
using YTech.IM.JSM.Core.Transaction.Inventory;

namespace YTech.IM.JSM.Data.NHibernateMaps.Transaction
{
    public class TTransDetMap : IAutoMappingOverride<TTransDet>
    {
        #region Implementation of IAutoMappingOverride<TTransDet>

        public void Override(AutoMapping<TTransDet> mapping)
        {
            mapping.DynamicUpdate();
            mapping.DynamicInsert();
            mapping.SelectBeforeUpdate();

            mapping.Table("dbo.T_TRANS_DET");
            mapping.Id(x => x.Id, "TRANS_DET_ID")
                 .GeneratedBy.Assigned();

            mapping.References(x => x.TransId, "TRANS_ID").Not.Nullable();
            mapping.References(x => x.ItemId, "ITEM_ID").Fetch.Join();
            mapping.References(x => x.ItemUomId, "ITEM_UOM_ID").Fetch.Join();
            mapping.Map(x => x.TransDetNo, "TRANS_DET_NO");
            mapping.Map(x => x.TransDetQty  , "TRANS_DET_QTY");
            mapping.Map(x => x.TransDetPrice, "TRANS_DET_PRICE");
            mapping.Map(x => x.TransDetDisc, "TRANS_DET_DISC");
            mapping.Map(x => x.TransDetTotal, "TRANS_DET_TOTAL");
            mapping.Map(x => x.TransDetDesc, "TRANS_DET_DESC");
            //mapping.References(x => x.EmployeeId, "EMPLOYEE_ID");
            //mapping.Map(x => x.TransDetCommissionProduct, "TRANS_DET_COMMISSION_PRODUCT");
            //mapping.Map(x => x.TransDetCommissionService, "TRANS_DET_COMMISSION_SERVICE");

            mapping.Map(x => x.DataStatus, "DATA_STATUS");
            mapping.Map(x => x.CreatedBy, "CREATED_BY");
            mapping.Map(x => x.CreatedDate, "CREATED_DATE");
            mapping.Map(x => x.ModifiedBy, "MODIFIED_BY");
            mapping.Map(x => x.ModifiedDate, "MODIFIED_DATE");
            mapping.Map(x => x.RowVersion, "ROW_VERSION").ReadOnly();

            //mapping.HasMany(x => x.TTransDetItems)
            //    //.Access.Property()
            //    .AsBag()
            //    .Inverse()
            //    .KeyColumn("TRANS_DET_ID")
            //    .Cascade.All();
        }

        #endregion
    }
}
