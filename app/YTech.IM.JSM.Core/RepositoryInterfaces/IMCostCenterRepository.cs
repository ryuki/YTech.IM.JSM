using System.Collections.Generic;
using SharpArch.Core.PersistenceSupport.NHibernate;
using YTech.IM.JSM.Core.Master;

namespace YTech.IM.JSM.Core.RepositoryInterfaces
{
    public interface IMCostCenterRepository : INHibernateRepositoryWithTypedId<MCostCenter, string>
    {
        IEnumerable<MCostCenter> GetPagedCostCenterList(string orderCol, string orderBy, int pageIndex, int maxRows, ref int totalRows);
    }
}
