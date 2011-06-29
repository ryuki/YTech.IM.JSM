using FluentNHibernate.Automapping;
using YTech.IM.JSM.Core.Master;
using FluentNHibernate.Automapping.Alterations;

namespace YTech.IM.JSM.Data.NHibernateMaps.Master
{
    public class MCostCenterMap : IAutoMappingOverride<MCostCenter>
    {
        #region Implementation of IAutoMappingOverride<MCostCenter>

        public void Override(AutoMapping<MCostCenter> mapping)
        {
            mapping.DynamicUpdate();
            mapping.DynamicInsert();
            mapping.SelectBeforeUpdate();

            mapping.Table("M_COST_CENTER");
            mapping.Id(x => x.Id, "COST_CENTER_ID")
                 .GeneratedBy.Assigned();

            mapping.References(x => x.EmployeeId, "EMPLOYEE_ID").Fetch.Join();
            mapping.Map(x => x.CostCenterName, "COST_CENTER_NAME");
            mapping.Map(x => x.CostCenterTotalBudget, "COST_CENTER_TOTAL_BUDGET");
            mapping.Map(x => x.CostCenterStatus, "COST_CENTER_STATUS");
            mapping.Map(x => x.CostCenterStartDate, "COST_CENTER_START_DATE");
            mapping.Map(x => x.CostCenterEndDate, "COST_CENTER_END_DATE");
            mapping.Map(x => x.CostCenterDesc, "COST_CENTER_DESC");

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
