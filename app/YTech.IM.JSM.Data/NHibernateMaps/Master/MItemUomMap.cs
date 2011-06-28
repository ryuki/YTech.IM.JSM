using FluentNHibernate.Automapping;
using YTech.IM.JSM.Core.Master;
using FluentNHibernate.Automapping.Alterations;

namespace YTech.IM.JSM.Data.NHibernateMaps.Master
{
    public class MItemUomMap : IAutoMappingOverride<MItemUom>
    {
        #region Implementation of IAutoMappingOverride< MItemUom>

        public void Override(AutoMapping<MItemUom> mapping)
        {
            mapping.DynamicUpdate();
            mapping.DynamicInsert();
            mapping.SelectBeforeUpdate();

            mapping.Table("dbo.M_ITEM_UOM");
            mapping.Id(x => x.Id, "ITEM_UOM_ID")
                 .GeneratedBy.Assigned();

            //mapping.References(x => x.ItemId, "ITEM_ID").Fetch.Join();
            mapping.References(x => x.ItemId, "ITEM_ID").Not.Nullable();
            mapping.Map(x => x.ItemUomName, "ITEM_UOM_NAME");
            mapping.References(x => x.ItemUomRefId, "ITEM_UOM_REF_ID").Fetch.Join();
            mapping.Map(x => x.ItemUomConverterValue, "ITEM_UOM_CONVERTER_VALUE");
            mapping.Map(x => x.ItemUomSalePrice, "ITEM_UOM_SALE_PRICE");
            mapping.Map(x => x.ItemUomPurchasePrice, "ITEM_UOM_PURCHASE_PRICE");
            mapping.Map(x => x.ItemUomHppPrice, "ITEM_UOM_HPP_PRICE");
            mapping.Map(x => x.ItemUomDesc, "ITEM_UOM_DESC");

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
