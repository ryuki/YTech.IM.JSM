using FluentNHibernate.Automapping;
using YTech.IM.JSM.Core.Master;
using FluentNHibernate.Automapping.Alterations;

namespace YTech.IM.JSM.Data.NHibernateMaps.Master
{
    public class MCustomerPriceMap : IAutoMappingOverride<MCustomerPrice>
    {
        #region Implementation of IAutoMappingOverride<MCustomerPrice>

        public void Override(AutoMapping<MCustomerPrice> mapping)
        {
            mapping.DynamicUpdate();
            mapping.DynamicInsert();
            mapping.SelectBeforeUpdate();

            mapping.Table("dbo.M_CUSTOMER_PRICE");
            mapping.Id(x => x.Id, "CUSTOMER_PRICE_ID")
                 .GeneratedBy.Assigned();

            mapping.References(x => x.CustomerId, "CUSTOMER_ID").Fetch.Join();
            mapping.References(x => x.ItemId, "ITEM_ID").Fetch.Join();
            mapping.Map(x => x.Price, "PRICE");
         
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
