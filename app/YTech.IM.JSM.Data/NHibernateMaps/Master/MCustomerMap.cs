using FluentNHibernate.Automapping;
using YTech.IM.JSM.Core.Master;
using FluentNHibernate.Automapping.Alterations;

namespace YTech.IM.JSM.Data.NHibernateMaps.Master
{
    public class MCustomerPriceMap : IAutoMappingOverride<MCustomer>
    {
        #region Implementation of IAutoMappingOverride<MCustomer>

        public void Override(AutoMapping<MCustomer> mapping)
        {
            mapping.DynamicUpdate();
            mapping.DynamicInsert();
            mapping.SelectBeforeUpdate();

            mapping.Table("dbo.M_CUSTOMER");
            mapping.Id(x => x.Id, "CUSTOMER_ID")
                 .GeneratedBy.Assigned();

            mapping.References(x => x.PersonId, "PERSON_ID").Fetch.Join();
            mapping.References(x => x.AddressId, "ADDRESS_ID").Fetch.Join();
            mapping.Map(x => x.CustomerMaxCredit, "CUSTOMER_MAX_CREDIT");
            mapping.Map(x => x.CustomerStatus, "CUSTOMER_STATUS");
            mapping.Map(x => x.CustomerDesc, "CUSTOMER_DESC");

            mapping.Map(x => x.DataStatus, "DATA_STATUS");
            mapping.Map(x => x.CreatedBy, "CREATED_BY");
            mapping.Map(x => x.CreatedDate, "CREATED_DATE");
            mapping.Map(x => x.ModifiedBy, "MODIFIED_BY");
            mapping.Map(x => x.ModifiedDate, "MODIFIED_DATE");
            mapping.Map(x => x.RowVersion, "ROW_VERSION").ReadOnly();

            mapping.Map(x => x.CustomerServiceDisc, "CUSTOMER_SERVICE_DISC");
            mapping.Map(x => x.CustomerProductDisc, "CUSTOMER_PRODUCT_DISC");
            mapping.Map(x => x.CustomerJoinDate, "CUSTOMER_JOIN_DATE");
            mapping.Map(x => x.CustomerLastBuy, "CUSTOMER_LAST_BUY");
            mapping.Map(x => x.CustomerMassageStrength, "CUSTOMER_MASSAGE_STRENGTH");
            mapping.Map(x => x.CustomerHealthProblem, "CUSTOMER_HEALTH_PROBLEM");
        }

        #endregion
    }
}
