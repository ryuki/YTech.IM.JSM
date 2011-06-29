using FluentNHibernate.Automapping;
using YTech.IM.JSM.Core.Master;
using FluentNHibernate.Automapping.Alterations;

namespace YTech.IM.JSM.Data.NHibernateMaps.Master
{
    public class MSupplierMap : IAutoMappingOverride<MSupplier>
    {
        #region Implementation of IAutoMappingOverride<MSupplier>

        public void Override(AutoMapping<MSupplier> mapping)
        {
            mapping.DynamicUpdate();
            mapping.DynamicInsert();
            mapping.SelectBeforeUpdate();

            mapping.Table("M_SUPPLIER");
            mapping.Id(x => x.Id, "SUPPLIER_ID")
                 .GeneratedBy.Assigned();

            mapping.Map(x => x.SupplierName, "SUPPLIER_NAME");
            mapping.References(x => x.AddressId, "ADDRESS_ID").Fetch.Join();

            mapping.Map(x => x.SupplierStatus, "SUPPLIER_STATUS");
            mapping.Map(x => x.SupplierMaxDebt, "SUPPLIER_MAX_DEBT");
            mapping.Map(x => x.SupplierDesc, "SUPPLIER_DESC");

            //mapping.HasOne(x => x.AddressId).EntityName(typeof(RefAddress).Name);//.ForeignKey("ADDRESS_ID");
            //mapping.HasOne(x => x.AddressId).ForeignKey("ADDRESS_ID");

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
