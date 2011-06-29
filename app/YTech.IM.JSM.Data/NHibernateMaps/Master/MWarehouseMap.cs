using FluentNHibernate.Automapping;
using YTech.IM.JSM.Core.Master;
using FluentNHibernate.Automapping.Alterations;

namespace YTech.IM.JSM.Data.NHibernateMaps.Master
{
    public class MWarehouseMap : IAutoMappingOverride<MWarehouse>
    {
        #region Implementation of IAutoMappingOverride< MWarehouse>

        public void Override(AutoMapping<MWarehouse> mapping)
        {
            mapping.DynamicUpdate();
            mapping.DynamicInsert();
            mapping.SelectBeforeUpdate();

            mapping.Table("dbo.M_WAREHOUSE");
            mapping.Id(x => x.Id, "WAREHOUSE_ID")
                 .GeneratedBy.Assigned();

            mapping.Map(x => x.WarehouseName, "WAREHOUSE_NAME");
            mapping.References(x => x.AddressId, "ADDRESS_ID").Fetch.Join();
            mapping.References(x => x.CostCenterId, "COST_CENTER_ID").Fetch.Join();
            mapping.Map(x => x.WarehouseType, "WAREHOUSE_TYPE");
            mapping.References(x => x.EmployeeId, "EMPLOYEE_ID").Fetch.Join();
            mapping.Map(x => x.WarehouseStatus, "WAREHOUSE_STATUS");
            mapping.Map(x => x.WarehouseIsDefault, "WAREHOUSE_IS_DEFAULT");
            mapping.Map(x => x.WarehousePhoto, "WAREHOUSE_PHOTO");
            mapping.Map(x => x.WarehouseDesc, "WAREHOUSE_DESC");

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
