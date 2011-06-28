using FluentNHibernate.Automapping;
using YTech.IM.JSM.Core.Master;
using FluentNHibernate.Automapping.Alterations;

namespace YTech.IM.JSM.Data.NHibernateMaps.Master
{
    public class MBrandMap : IAutoMappingOverride<MBrand>
    {
        #region Implementation of IAutoMappingOverride<MBrand>

        public void Override(AutoMapping<MBrand> mapping)
        {
            mapping.DynamicUpdate();
            mapping.DynamicInsert();
            mapping.SelectBeforeUpdate();
            mapping.Cache.ReadWrite();

            mapping.Table("dbo.M_BRAND");
            mapping.Id(x => x.Id, "BRAND_ID")
                .GeneratedBy.Assigned();

            mapping.Map(x => x.BrandName, "BRAND_NAME");
            mapping.Map(x => x.BrandDesc, "BRAND_DESC");

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
