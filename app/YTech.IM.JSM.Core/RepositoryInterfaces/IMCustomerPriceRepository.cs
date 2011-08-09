using System.Collections.Generic;
using SharpArch.Core.PersistenceSupport.NHibernate;
using YTech.IM.JSM.Core.Master;

namespace YTech.IM.JSM.Core.RepositoryInterfaces
{
    public interface IMCustomerPriceRepository : INHibernateRepositoryWithTypedId<MCustomerPrice, string>
    {
        //MCustomerPrice GetByItem(MItem item);

        IList<MCustomerPrice> GetListByItemId(string itemId);

        MCustomerPrice GetByItemCustomer(string itemId, string customerId);
    }
}
