using System;
using System.Collections;
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
    public class TStockRefRepository : NHibernateRepositoryWithTypedId<TStockRef,string >, ITStockRefRepository
    {
        public void DeleteByTransDetId(object[] detailIdList)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(@"   delete
                                from TStockRef det
                                        where det.TransDetId.Id in (:detailIdList) ");

            IQuery q = Session.CreateQuery(sql.ToString());
            q.SetParameterList("detailIdList", detailIdList);
            q.ExecuteUpdate();
        }
    }
}
