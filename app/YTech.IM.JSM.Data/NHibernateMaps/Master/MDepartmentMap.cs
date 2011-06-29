using FluentNHibernate.Automapping;
using YTech.IM.JSM.Core.Master;
using FluentNHibernate.Automapping.Alterations;

namespace YTech.IM.JSM.Data.NHibernateMaps.Master
{
    public class MDepartmentMap : IAutoMappingOverride<MDepartment>
    {
        #region Implementation of IAutoMappingOverride<MDepartment>

        public void Override(AutoMapping<MDepartment> mapping)
        {
            mapping.DynamicUpdate();
            mapping.DynamicInsert();
            mapping.SelectBeforeUpdate();

            mapping.Table("M_DEPARTMENT");
            mapping.Id(x => x.Id, "DEPARTMENT_ID")
                 .GeneratedBy.Assigned();

            mapping.Map(x => x.DepartmentName, "DEPARTMENT_NAME");
            mapping.Map(x => x.DepartmentStatus, "DEPARTMENT_STATUS");
            mapping.Map(x => x.DepartmentDesc, "DEPARTMENT_DESC");
            
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
