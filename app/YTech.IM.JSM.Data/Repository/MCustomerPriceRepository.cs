using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using NHibernate.Criterion;
using SharpArch.Data.NHibernate;
using YTech.IM.JSM.Core.Master;
using YTech.IM.JSM.Core.RepositoryInterfaces;

namespace YTech.IM.JSM.Data.Repository
{
    public class MCustomerPriceRepository : NHibernateRepositoryWithTypedId<MCustomerPrice, string>, IMCustomerPriceRepository
    {
        #region Implementation of IMItemUomRepository

        public MCustomerPrice GetByItem(MItem item)
        {
            ICriteria criteria = Session.CreateCriteria(typeof(MCustomerPrice));
            criteria.Add(Expression.Eq("ItemId", item));
            return criteria.UniqueResult() as MCustomerPrice;
        }

        public IList<MCustomerPrice> GetListByItemId(string itemId)
        {
            var sql = new StringBuilder();

            sql.AppendLine(@"   select cp
                                from MCustomerPrice as cp");

            if (!string.IsNullOrEmpty(itemId))
            {
                sql.AppendLine(@" where cp.ItemId.Id = :itemId");
            }

            IQuery q = Session.CreateQuery(sql.ToString());

            if (!string.IsNullOrEmpty(itemId))
            {
                q.SetString("itemId", itemId);
            }

            return q.List<MCustomerPrice>();
        }

        public MCustomerPrice GetByItemCustomer(string itemId, string customerId)
        {
            StringBuilder sql = new StringBuilder();

            sql.AppendLine(@"   select cp
                                from MCustomerPrice as cp where 1=1 ");

            if (!string.IsNullOrEmpty(itemId))
                sql.AppendLine(@" and cp.ItemId.Id = :itemId");

            if (!string.IsNullOrEmpty(customerId))
                sql.AppendLine(@" and cp.CustomerId.Id = :customerId");

            IQuery q = Session.CreateQuery(sql.ToString());

            if (!string.IsNullOrEmpty(itemId))
                q.SetString("itemId", itemId);

            if (!string.IsNullOrEmpty(customerId))
                q.SetString("customerId", customerId);

            q.SetMaxResults(1);
            return q.UniqueResult<MCustomerPrice>();
        }

        #endregion
    }
}
