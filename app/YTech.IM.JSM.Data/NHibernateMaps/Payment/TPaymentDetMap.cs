using FluentNHibernate.Automapping;
using YTech.IM.JSM.Core.Master;
using FluentNHibernate.Automapping.Alterations;
using YTech.IM.JSM.Core.Transaction.Payment;

namespace YTech.IM.JSM.Data.NHibernateMaps.Payment
{
    public class TPaymentDetMap : IAutoMappingOverride<TPaymentDet>
    {
        #region Implementation of IAutoMappingOverride<TPaymentDet>

        public void Override(AutoMapping<TPaymentDet> mapping)
        {
            mapping.DynamicUpdate();
            mapping.DynamicInsert();
            mapping.SelectBeforeUpdate();
            mapping.Cache.ReadOnly();

            mapping.Table("dbo.T_PAYMENT_DET");
            mapping.Id(x => x.Id, "PAYMENT_DET_ID")
                 .GeneratedBy.Assigned();

            mapping.References(x => x.PaymentId, "PAYMENT_ID");
            mapping.References(x => x.TransId, "TRANS_ID");

            mapping.Map(x => x.PaymentDetValue, "PAYMENT_DET_VALUE");
            mapping.Map(x => x.PaymentDetStatus, "PAYMENT_DET_STATUS");
            mapping.Map(x => x.PaymentDetDesc, "PAYMENT_DET_DESC"); 

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
