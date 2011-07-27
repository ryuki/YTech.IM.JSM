using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using NHibernate.Criterion;
using SharpArch.Data.NHibernate;
using YTech.IM.JSM.Core.Master;
using YTech.IM.JSM.Core.RepositoryInterfaces;
using YTech.IM.JSM.Core.Transaction;
using YTech.IM.JSM.Core.Transaction.Inventory;

namespace YTech.IM.JSM.Data.Repository
{
    public class TTransRepository : NHibernateRepositoryWithTypedId<TTrans, string>, ITTransRepository
    {
        public IList<TTrans> GetByWarehouseStatusTransBy(MWarehouse warehouse, string transStatus, string transBy)
        {
            ICriteria criteria = Session.CreateCriteria(typeof(TTrans));
            criteria.Add(Expression.Eq("TransStatus", transStatus));
            criteria.Add(Expression.Eq("WarehouseId", warehouse));
            criteria.Add(Expression.Eq("TransBy", transBy));
            criteria.SetCacheable(true);
            IList<TTrans> list = criteria.List<TTrans>();
            return list;
        }

        public IEnumerable<TTrans> GetPagedTransList(string orderCol, string orderBy, int pageIndex, int maxRows, ref int totalRows, string searchBy, string searchText, string transStatus)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(@"  from TTrans as t
                                where 1=1 ");
            if (!string.IsNullOrEmpty(transStatus))
            {
                sql.AppendLine(@"   and t.TransStatus = :transStatus ");
            }
            if (!string.IsNullOrEmpty(searchBy) && !string.IsNullOrEmpty(searchText))
            {
                sql.AppendFormat(@"   and t.{0} like :searchText", searchBy).AppendLine();
            }

            string queryCount = string.Format(" select count(t.Id) {0}", sql);
            IQuery q = Session.CreateQuery(queryCount);
            if (!string.IsNullOrEmpty(transStatus))
            {
                q.SetString("transStatus", transStatus);
            }
            if (!string.IsNullOrEmpty(searchBy) && !string.IsNullOrEmpty(searchText))
            {
                q.SetString("searchText", searchText);
            }
            totalRows = Convert.ToInt32(q.UniqueResult());

            sql.AppendLine(@"   order by t.TransDate desc  ");
            string query = string.Format(" select t {0}", sql);
            q = Session.CreateQuery(query);
            if (!string.IsNullOrEmpty(transStatus))
            {
                q.SetString("transStatus", transStatus);
            }
            if (!string.IsNullOrEmpty(searchBy) && !string.IsNullOrEmpty(searchText))
            {
                q.SetString("searchText", searchText);
            }
            q.SetMaxResults(maxRows);
            q.SetFirstResult((pageIndex - 1) * maxRows);
            IEnumerable<TTrans> list = q.List<TTrans>();
            return list; 
        }
    }
}
