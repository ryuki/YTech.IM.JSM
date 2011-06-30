using SharpArch.Core.PersistenceSupport.NHibernate;
using YTech.IM.JSM.Core.Master;

namespace YTech.IM.JSM.Core.RepositoryInterfaces
{
    public interface IMItemUomRepository : INHibernateRepositoryWithTypedId<MItemUom, string>
    {
        MItemUom GetByItem(MItem item);
    }
}
