using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using SharpArch.Data.NHibernate;
using YTech.IM.JSM.Core.Master;
using YTech.IM.JSM.Core.RepositoryInterfaces;

namespace YTech.IM.JSM.Data.Repository
{
    public class MItemRepository : NHibernateRepositoryWithTypedId<MItem, string>, IMItemRepository
    {
        public IEnumerable<MItem> GetPagedItemList(string orderCol, string orderBy, int pageIndex, int maxRows, ref int totalRows, string itemId, string itemName, MItemCat itemCat)
        {
            ICriteria criteria = CreateNewCriteria(itemId, itemName, itemCat);

            //calculate total rows
            totalRows = criteria
                .SetProjection(Projections.RowCount())
                .FutureValue<int>().Value;

            //get list results
            criteria = CreateNewCriteria(itemId, itemName, itemCat);
            criteria.SetMaxResults(maxRows)
              .SetFirstResult((pageIndex - 1) * maxRows)
              .AddOrder(new Order(orderCol, orderBy.Equals("asc") ? true : false))
              ;

            IEnumerable<MItem> list = criteria.List<MItem>();
            return list;
        }

        private ICriteria CreateNewCriteria(string itemId, string itemName, MItemCat itemCat)
        {
            ICriteria criteria = Session.CreateCriteria(typeof(MItem));
            if (!string.IsNullOrEmpty(itemId))
            {
                criteria.Add(Expression.Like("Id", itemId, MatchMode.Anywhere));
            }
            if (!string.IsNullOrEmpty(itemName))
            {
                criteria.Add(Expression.Like("ItemName", itemName, MatchMode.Anywhere));
            }
            if (itemCat != null)
            {
                criteria.Add(Expression.Eq("ItemCatId", itemCat));
            }
            return criteria;
        }
    }
}
