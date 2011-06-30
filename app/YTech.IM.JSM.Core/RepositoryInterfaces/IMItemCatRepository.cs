using System.Collections.Generic;
using SharpArch.Core.PersistenceSupport.NHibernate;
using YTech.IM.JSM.Core.Master;

namespace YTech.IM.JSM.Core.RepositoryInterfaces
{ /// <summary>
    /// Needs to implement INHibernateRepositoryWithTypedId because it has an assigned Id
    /// and will need to be explicit about called Save or Update appropriately.  Assigned
    /// Ids are EVil with a capital E and V...yes, they're just that evil.
    /// </summary>
    public interface IMItemCatRepository : INHibernateRepositoryWithTypedId<MItemCat, string>
    {
        IEnumerable<MItemCat> GetPagedItemCatList(string orderCol, string orderBy, int pageIndex, int maxRows, ref int totalRows);
    }
}