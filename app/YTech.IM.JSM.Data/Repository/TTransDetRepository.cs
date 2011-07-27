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
using YTech.IM.JSM.Enums;

namespace YTech.IM.JSM.Data.Repository
{
    public class TTransDetRepository : NHibernateRepositoryWithTypedId<TTransDet, string>, ITTransDetRepository
    {
        public IList<TTransDet> GetByItemWarehouse(MItem item, MWarehouse warehouse)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(@"   select det
                                from TTransDet as det
                                    left outer join det.TransId trans
                                    where trans.TransStatus = :TransStatus ");
            if (item != null)
            {
                sql.AppendLine(@"   and det.ItemId = :item");
            }
            if (warehouse != null)
            {
                sql.AppendLine(@"   and trans.WarehouseId = :warehouse");
            }
            IQuery q = Session.CreateQuery(sql.ToString());
            q.SetString("TransStatus", Enums.EnumTransactionStatus.Budgeting.ToString());
            if (item != null)
            {
                q.SetEntity("item", item);
            }
            if (warehouse != null)
            {
                q.SetEntity("warehouse", warehouse);
            }
            return q.List<TTransDet>();
        }

        public decimal? GetTotalUsed(MItem item, MWarehouse warehouse)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(@"   select sum(det.TransDetQty)
                                from TTransDet as det
                                    left outer join det.TransId trans
                                    where trans.TransStatus = :TransStatus ");
            if (item != null)
            {
                sql.AppendLine(@"   and det.ItemId = :item");
            }
            if (warehouse != null)
            {
                sql.AppendLine(@"   and trans.WarehouseId = :warehouse");
            }

            IQuery q = Session.CreateQuery(sql.ToString());
            q.SetString("TransStatus", Enums.EnumTransactionStatus.Using.ToString());
            if (item != null)
            {
                q.SetEntity("item", item);
            }
            if (warehouse != null)
            {
                q.SetEntity("warehouse", warehouse);
            }
            if (q.UniqueResult() != null)
            {
                 return (decimal)q.UniqueResult();
            }
            return null;
        }

        public IList<TTransDet> GetListByTransId(string transId, Enums.EnumTransactionStatus enumTransactionStatus)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(@"   select det
                                from TTransDet as det
                                    left outer join det.TransId trans 
                                        where trans.TransStatus = :TransStatus ");
            if (!string.IsNullOrEmpty(transId))
            {
                sql.AppendLine(@"   and trans.Id = :transId");
            }
            IQuery q = Session.CreateQuery(sql.ToString());
            q.SetString("TransStatus", enumTransactionStatus.ToString());
            if (!string.IsNullOrEmpty(transId))
            {
                q.SetString("transId", transId);
            }
            return q.List<TTransDet>();
        }

        public IList<TTransDet> GetListByTrans(TTrans trans)
        {
            ICriteria criteria = Session.CreateCriteria(typeof(TTransDet));
            if (trans != null)
            {
                criteria.Add(Expression.Eq("TransId", trans));
            }

            IList<TTransDet> list = criteria.List<TTransDet>();
            return list;
        }

        public IList<TTransDet> GetListByDate(EnumTransactionStatus TransStatus, DateTime? dateFrom, DateTime? dateTo)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(@"   select det
                                from TTransDet as det
                                    left outer join det.TransId trans 
                                        where trans.TransStatus = :TransStatus ");
            sql.AppendLine(@"   and trans.TransDate between :dateFrom and :dateTo ");

            IQuery q = Session.CreateQuery(sql.ToString());
            q.SetString("TransStatus", TransStatus.ToString());
            q.SetDateTime("dateFrom", dateFrom.Value);
            q.SetDateTime("dateTo", dateTo.Value);
            return q.List<TTransDet>();
        }

        public IList<TTransDet> GetListById(object[] detailIdList)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(@"   select det
                                from TTransDet det
                                        where det.Id in (:detailIdList) ");

            IQuery q = Session.CreateQuery(sql.ToString());
            q.SetParameterList("detailIdList", detailIdList);
            return q.List<TTransDet>();
        }

        public void DeleteById(object[] detailIdList)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(@"   delete
                                from TTransDet det
                                        where det.Id in (:detailIdList) ");

            IQuery q = Session.CreateQuery(sql.ToString());
            q.SetParameterList("detailIdList", detailIdList);
            q.ExecuteUpdate();
        }

        public IList<TTransDet> GetByDateWarehouse(DateTime? dateFrom, DateTime? dateTo, MWarehouse warehouse, string transStatus)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(@"   select det
                                from TTransDet as det
                                    left outer join det.TransId trans
                                    where trans.TransStatus = :TransStatus
                                        and trans.TransDate between :dateFrom and :dateTo ");
            if (warehouse != null)
                sql.AppendLine(@"   and trans.WarehouseId = :warehouse");

            IQuery q = Session.CreateQuery(sql.ToString());
            q.SetString("TransStatus", transStatus);
            q.SetDateTime("dateFrom", dateFrom.Value);
            q.SetDateTime("dateTo", dateTo.Value);
            if (warehouse != null)
                q.SetEntity("warehouse", warehouse);
            return q.List<TTransDet>();
        }

        public IList<TTransDet> GetByDateWarehouseTransBy(DateTime? dateFrom, DateTime? dateTo, string transBy, string warehouseId, string transStatus)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(@"   select det
                                from TTransDet as det
                                    left outer join det.TransId trans
                                    where trans.TransStatus = :TransStatus
                                        and trans.TransDate between :dateFrom and :dateTo ");
            if (!string.IsNullOrEmpty(transBy))
                sql.AppendLine(@"   and trans.TransBy = :transBy");
            if (!string.IsNullOrEmpty(warehouseId))
                sql.AppendLine(@"   and trans.WarehouseId.Id = :warehouseId");

            IQuery q = Session.CreateQuery(sql.ToString());
            q.SetString("TransStatus", transStatus);
            q.SetDateTime("dateFrom", dateFrom.Value);
            q.SetDateTime("dateTo", dateTo.Value); 
            if (!string.IsNullOrEmpty(transBy))
                q.SetString("transBy", transBy);
            if (!string.IsNullOrEmpty(warehouseId))
                q.SetString("warehouseId", warehouseId);
            return q.List<TTransDet>();
        }
    }
}
