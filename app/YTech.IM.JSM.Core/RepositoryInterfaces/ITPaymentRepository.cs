using System.Collections.Generic;
using SharpArch.Core.PersistenceSupport.NHibernate;
using YTech.IM.JSM.Core.Master;
using YTech.IM.JSM.Core.Transaction.Payment;


namespace YTech.IM.JSM.Core.RepositoryInterfaces
{
    public interface ITPaymentRepository : INHibernateRepositoryWithTypedId<TPayment, string>
    {
    }
}
