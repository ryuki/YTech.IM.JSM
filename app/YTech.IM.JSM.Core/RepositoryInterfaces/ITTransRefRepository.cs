using System.Collections.Generic;
using SharpArch.Core.PersistenceSupport.NHibernate;
using YTech.IM.JSM.Core.Master;
using YTech.IM.JSM.Core.Transaction;
using YTech.IM.JSM.Core.Transaction.Inventory;

namespace YTech.IM.JSM.Core.RepositoryInterfaces
{
    public interface ITTransRefRepository : INHibernateRepository<TTransRef>
    {
    }
}
