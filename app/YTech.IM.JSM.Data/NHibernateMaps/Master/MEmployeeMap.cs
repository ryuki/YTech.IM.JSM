using FluentNHibernate.Automapping;
using YTech.IM.JSM.Core.Master;
using FluentNHibernate.Automapping.Alterations;

namespace YTech.IM.JSM.Data.NHibernateMaps.Master
{
    public class MEmployeeMap : IAutoMappingOverride<MEmployee>
    {
        #region Implementation of IAutoMappingOverride<MEmployee>

        public void Override(AutoMapping<MEmployee> mapping)
        {
            mapping.DynamicUpdate();
            mapping.DynamicInsert();
            mapping.SelectBeforeUpdate();

            mapping.Table("M_EMPLOYEE");
            mapping.Id(x => x.Id, "EMPLOYEE_ID")
                 .GeneratedBy.Assigned();

            mapping.References(x => x.PersonId, "PERSON_ID").Fetch.Join();
            mapping.References(x => x.DepartmentId, "DEPARTMENT_ID").Fetch.Join();
            mapping.Map(x => x.EmployeeStatus, "EMPLOYEE_STATUS");
            mapping.Map(x => x.EmployeeDesc, "EMPLOYEE_DESC");

            mapping.Map(x => x.DataStatus, "DATA_STATUS");
            mapping.Map(x => x.CreatedBy, "CREATED_BY");
            mapping.Map(x => x.CreatedDate, "CREATED_DATE");
            mapping.Map(x => x.ModifiedBy, "MODIFIED_BY");
            mapping.Map(x => x.ModifiedDate, "MODIFIED_DATE");
            mapping.Map(x => x.RowVersion, "ROW_VERSION").ReadOnly();
            
            mapping.Map(x => x.EmployeeCommissionProductVal, "EMPLOYEE_COMMISSION_PRODUCT_VAL");
            mapping.Map(x => x.EmployeeCommissionServiceVal, "EMPLOYEE_COMMISSION_SERVICE_VAL");
            mapping.Map(x => x.EmployeeCommissionProductType, "EMPLOYEE_COMMISSION_PRODUCT_TYPE");
            mapping.Map(x => x.EmployeeCommissionServiceType, "EMPLOYEE_COMMISSION_SERVICE_TYPE");
        }

        #endregion
    }
}
