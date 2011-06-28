using FluentNHibernate.Automapping;
using YTech.IM.JSM.Core.Master;
using FluentNHibernate.Automapping.Alterations;

namespace YTech.IM.JSM.Data.NHibernateMaps.Master
{
    public class MItemCatMap : IAutoMappingOverride<MItemCat>
    {
        #region Implementation of IAutoMappingOverride<MItemCat>

        public void Override(AutoMapping<MItemCat> mapping)
        {
            mapping.DynamicUpdate();
            mapping.DynamicInsert();
            mapping.SelectBeforeUpdate();

            mapping.Table("M_ITEM_CAT");
            mapping.Id(x => x.Id, "ITEM_CAT_ID")
                 .GeneratedBy.Assigned();

            mapping.Map(x => x.ItemCatName, "ITEM_CAT_NAME");
            mapping.Map(x => x.ItemCatDesc, "ITEM_CAT_DESC");

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
