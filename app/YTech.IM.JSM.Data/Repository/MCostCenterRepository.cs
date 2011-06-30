using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using SharpArch.Data.NHibernate;
using YTech.IM.JSM.Core.Master;
using YTech.IM.JSM.Core.RepositoryInterfaces;

namespace YTech.IM.JSM.Data.Repository
{
    public class MCostCenterRepository : NHibernateRepositoryWithTypedId<MCostCenter, string>, IMCostCenterRepository
    {
        public IEnumerable<MCostCenter> GetPagedCostCenterList(string orderCol, string orderBy, int pageIndex, int maxRows, ref int totalRows)
        {
            ICriteria criteria = Session.CreateCriteria(typeof(MCostCenter));

            //calculate total rows
            totalRows = Session.CreateCriteria(typeof(MCostCenter))
                .SetProjection(Projections.RowCount())
                .FutureValue<int>().Value;

            //get list results
            criteria.SetMaxResults(maxRows)
              .SetFirstResult((pageIndex - 1) * maxRows)
              .AddOrder(new Order(orderCol, orderBy.Equals("asc") ? true : false))
              ;

            IEnumerable<MCostCenter> list = criteria.List<MCostCenter>();
            return list;
        }
    }
}
