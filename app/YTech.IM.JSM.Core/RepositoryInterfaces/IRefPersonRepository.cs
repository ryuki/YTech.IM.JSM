using SharpArch.Core.PersistenceSupport.NHibernate;
using YTech.IM.JSM.Core.Master;

namespace YTech.IM.JSM.Core.RepositoryInterfaces
{
    public interface IRefPersonRepository : INHibernateRepositoryWithTypedId<RefPerson, string>
    {
    }
}
