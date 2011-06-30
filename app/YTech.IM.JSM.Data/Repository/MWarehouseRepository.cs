using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using SharpArch.Data.NHibernate;
using YTech.IM.JSM.Core.Master;
using YTech.IM.JSM.Core.RepositoryInterfaces;

namespace YTech.IM.JSM.Data.Repository
{
    public class MWarehouseRepository : NHibernateRepositoryWithTypedId<MWarehouse, string>, IMWarehouseRepository
    {
        public IEnumerable<MWarehouse> GetPagedWarehouseList(string orderCol, string orderBy, int pageIndex, int maxRows, ref int totalRows)
        {
            ICriteria criteria = Session.CreateCriteria(typeof(MWarehouse));

            //calculate total rows
            totalRows = Session.CreateCriteria(typeof(MWarehouse))
                .SetProjection(Projections.RowCount())
                .FutureValue<int>().Value;

            //get list results
            criteria.SetMaxResults(maxRows)
              .SetFirstResult((pageIndex - 1) * maxRows)
              .AddOrder(new Order(orderCol, orderBy.Equals("asc") ? true : false))
              ;

            IEnumerable<MWarehouse> list = criteria.List<MWarehouse>();
            return list;
        }
    }
}
