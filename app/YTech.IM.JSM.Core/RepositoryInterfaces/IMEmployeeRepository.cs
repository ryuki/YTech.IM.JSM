using System.Collections.Generic;
using SharpArch.Core.PersistenceSupport.NHibernate;
using YTech.IM.JSM.Core.Master;

namespace YTech.IM.JSM.Core.RepositoryInterfaces
{
    public interface IMEmployeeRepository : INHibernateRepositoryWithTypedId<MEmployee, string>
    {
        IEnumerable<MEmployee> GetPagedEmployeeList(string orderCol, string orderBy, int pageIndex, int maxRows, ref int totalRows);
    }
}
