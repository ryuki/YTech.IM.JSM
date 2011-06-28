using FluentNHibernate.Automapping;
using YTech.IM.JSM.Core.Master;
using FluentNHibernate.Automapping.Alterations;

namespace YTech.IM.JSM.Data.NHibernateMaps.Master
{
    public class MItemMap : IAutoMappingOverride<MItem>
    {
        #region Implementation of IAutoMappingOverride< MItem>

        public void Override(AutoMapping<MItem> mapping)
        {
            mapping.DynamicUpdate();
            mapping.DynamicInsert();
            mapping.SelectBeforeUpdate();

            mapping.Table("dbo.M_ITEM");
            mapping.Id(x => x.Id, "ITEM_ID")
                 .GeneratedBy.Assigned();

            mapping.References(x => x.ItemCatId, "ITEM_CAT_ID").Fetch.Join();
            mapping.References(x => x.BrandId, "BRAND_ID").Fetch.Join();
            mapping.Map(x => x.ItemName, "ITEM_NAME");
            mapping.Map(x => x.ItemStatus, "ITEM_STATUS");
            mapping.Map(x => x.ItemPhoto, "ITEM_PHOTO");
            mapping.Map(x => x.ItemDesc, "ITEM_DESC");

            mapping.Map(x => x.DataStatus, "DATA_STATUS");
            mapping.Map(x => x.CreatedBy, "CREATED_BY");
            mapping.Map(x => x.CreatedDate, "CREATED_DATE");
            mapping.Map(x => x.ModifiedBy, "MODIFIED_BY");
            mapping.Map(x => x.ModifiedDate, "MODIFIED_DATE");
            mapping.Map(x => x.RowVersion, "ROW_VERSION").ReadOnly();

            mapping.HasMany(x => x.ItemUoms)
                //.Access.Property()
                .AsBag()
                .Inverse()
                .KeyColumn("ITEM_ID")
                .Cascade.All();
        }

        #endregion
    }
}
