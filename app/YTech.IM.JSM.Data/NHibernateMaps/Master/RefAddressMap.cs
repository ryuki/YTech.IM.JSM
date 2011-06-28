using FluentNHibernate.Automapping;
using YTech.IM.JSM.Core.Master;
using FluentNHibernate.Automapping.Alterations;

namespace YTech.IM.JSM.Data.NHibernateMaps.Master
{
    public class RefAddressMap : IAutoMappingOverride<RefAddress>
    {
        #region Implementation of IAutoMappingOverride<RefAddress>

        public void Override(AutoMapping<RefAddress> mapping)
        {
            mapping.DynamicUpdate();
            mapping.DynamicInsert();
            mapping.SelectBeforeUpdate();

            mapping.Table("REF_ADDRESS");
            mapping.Id(x => x.Id, "ADDRESS_ID")
                 .GeneratedBy.Assigned();

            mapping.Map(x => x.AddressLine1, "ADDRESS_LINE1");
            mapping.Map(x => x.AddressLine2, "ADDRESS_LINE2");
            mapping.Map(x => x.AddressLine3, "ADDRESS_LINE3");
            mapping.Map(x => x.AddressPhone, "ADDRESS_PHONE");
            mapping.Map(x => x.AddressFax, "ADDRESS_FAX");
            mapping.Map(x => x.AddressCity, "ADDRESS_CITY");
            mapping.Map(x => x.AddressContact, "ADDRESS_CONTACT");
            mapping.Map(x => x.AddressContactMobile, "ADDRESS_CONTACT_MOBILE");
            mapping.Map(x => x.AddressEmail, "ADDRESS_EMAIL");

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
