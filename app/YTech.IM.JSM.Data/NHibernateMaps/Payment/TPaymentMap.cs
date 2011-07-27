using FluentNHibernate.Automapping;
using YTech.IM.JSM.Core.Master;
using FluentNHibernate.Automapping.Alterations;
using YTech.IM.JSM.Core.Transaction.Payment;

namespace YTech.IM.JSM.Data.NHibernateMaps.Payment
{
    public class TPaymentMap : IAutoMappingOverride<TPayment>
    {
        #region Implementation of IAutoMappingOverride<TPayment>

        public void Override(AutoMapping<TPayment> mapping)
        {
            mapping.DynamicUpdate();
            mapping.DynamicInsert();
            mapping.SelectBeforeUpdate();
            mapping.Cache.ReadOnly();

            mapping.Table("dbo.T_PAYMENT");
            mapping.Id(x => x.Id, "PAYMENT_ID")
                 .GeneratedBy.Assigned();

            mapping.Map(x => x.PaymentType, "PAYMENT_TYPE");
            mapping.Map(x => x.PaymentDate, "PAYMENT_DATE");
            mapping.Map(x => x.PaymentTotal, "PAYMENT_TOTAL");
            mapping.Map(x => x.PaymentStatus, "PAYMENT_STATUS");
            mapping.Map(x => x.PaymentDesc, "PAYMENT_DESC");

            mapping.Map(x => x.DataStatus, "DATA_STATUS");
            mapping.Map(x => x.CreatedBy, "CREATED_BY");
            mapping.Map(x => x.CreatedDate, "CREATED_DATE");
            mapping.Map(x => x.ModifiedBy, "MODIFIED_BY");
            mapping.Map(x => x.ModifiedDate, "MODIFIED_DATE");
            mapping.Map(x => x.RowVersion, "ROW_VERSION").ReadOnly();

            mapping.HasMany(x => x.PaymentDets)
                .AsBag()
                .Inverse()
                .KeyColumn("PAYMENT_ID")
                .Cascade.All();
        }

        #endregion
    }
}

