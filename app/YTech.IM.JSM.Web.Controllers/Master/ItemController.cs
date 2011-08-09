using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using SharpArch.Core;
using SharpArch.Web.NHibernate;
using YTech.IM.JSM.Core.Master;
using YTech.IM.JSM.Core.RepositoryInterfaces;
using YTech.IM.JSM.Data.Repository;
using YTech.IM.JSM.Enums;

namespace YTech.IM.JSM.Web.Controllers.Master
{
    [HandleError]
    public class ItemController : Controller
    {
        private readonly IMItemRepository _mItemRepository;
        private readonly IMItemCatRepository _mItemCatRepository;
        private readonly IMBrandRepository _mBrandRepository;
        private readonly IMCustomerPriceRepository _mCustomerPriceRepository;
        public ItemController(IMItemRepository mItemRepository, IMItemCatRepository mItemCatRepository, IMBrandRepository mBrandRepository, IMCustomerPriceRepository mCustomerPriceRepository)
        {
            Check.Require(mItemRepository != null, "mItemRepository may not be null");
            Check.Require(mItemCatRepository != null, "mItemCatRepository may not be null");
            Check.Require(mBrandRepository != null, "mBrandRepository may not be null");
            Check.Require(mCustomerPriceRepository != null, "mCustomerPriceRepository may not be null");

            this._mItemRepository = mItemRepository;
            this._mItemCatRepository = mItemCatRepository;
            this._mBrandRepository = mBrandRepository;
            this._mCustomerPriceRepository = mCustomerPriceRepository;
        }

        public ActionResult Search()
        {
            return View();
        }

        [Transaction]
        public virtual ActionResult ListSearch(string sidx, string sord, int page, int rows, string itemId, string itemName, string itemCatId, string customerId)
        {
            int totalRecords = 0;
            MItemCat itemCat = null;
            if (!string.IsNullOrEmpty(itemCatId))
            {
                itemCat = _mItemCatRepository.Get(itemCatId);
            }
            var items = _mItemRepository.GetPagedItemList(sidx, sord, page, rows, ref totalRecords, itemId, itemName, itemCat);
            int pageSize = rows;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            var jsonData = new
            {

                total = totalPages,
                page = page,
                records = totalRecords,
                rows = (
                    from item in items
                    select new
                    {
                        i = item.Id.ToString(),
                        cell = new string[] {
                            item.Id, 
                            item.ItemName, 
                           item.ItemCatId != null ? item.ItemCatId.ItemCatName : null,
                           item.BrandId != null ? item.BrandId.BrandName : null,
                           item.ItemUoms.Count > 0 ? item.ItemUoms[0].ItemUomName : null,
                       item.ItemUoms.Count > 0 ?  item.ItemUoms[0].ItemUomPurchasePrice.HasValue ? item.ItemUoms[0].ItemUomPurchasePrice.Value.ToString(Helper.CommonHelper.NumberFormat) : "0" : "0",
                    GetCustomerPrice(item,customerId),   
                            item.ItemDesc
                        }
                    }).ToArray()
            };


            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        private string GetCustomerPrice(MItem item, string customerId)
        {
            if (!string.IsNullOrEmpty(customerId))
            {
                MCustomerPrice cp = _mCustomerPriceRepository.GetByItemCustomer(item.Id, customerId);
                if (cp != null)
                {
                    return cp.Price.HasValue ? cp.Price.Value.ToString(Helper.CommonHelper.NumberFormat) : "0";
                }
            }
            return item.ItemUoms.Count > 0 ? item.ItemUoms[0].ItemUomSalePrice.HasValue ? item.ItemUoms[0].ItemUomSalePrice.Value.ToString(Helper.CommonHelper.NumberFormat) : "0" : "0";
        }


        public ActionResult Index()
        {
            return View();
        }

        [Transaction]
        public virtual ActionResult List(string sidx, string sord, int page, int rows)
        {
            int totalRecords = 0;
            var itemCats = _mItemRepository.GetPagedItemList(sidx, sord, page, rows, ref totalRecords, null, null, null);
            int pageSize = rows;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = (
                    from itemCat in itemCats
                    select new
                    {
                        i = itemCat.Id.ToString(),
                        cell = new string[] {
                            itemCat.Id, 
                            itemCat.ItemName, 
                           itemCat.ItemCatId != null ? itemCat.ItemCatId.Id : null,
                           itemCat.ItemCatId != null ? itemCat.ItemCatId.ItemCatName : null,
                           itemCat.BrandId != null ? itemCat.BrandId.Id : null,
                           itemCat.BrandId != null ? itemCat.BrandId.BrandName : null,
                         itemCat.ItemUoms.Count > 0 ?   itemCat.ItemUoms[0].Id : null,
                           itemCat.ItemUoms.Count > 0 ? itemCat.ItemUoms[0].ItemUomName : null,
                            itemCat.ItemUoms.Count > 0 ?  itemCat.ItemUoms[0].ItemUomPurchasePrice.HasValue ? itemCat.ItemUoms[0].ItemUomPurchasePrice.Value.ToString(Helper.CommonHelper.NumberFormat) : null : null,
                       itemCat.ItemUoms.Count > 0 ?  itemCat.ItemUoms[0].ItemUomSalePrice.HasValue ? itemCat.ItemUoms[0].ItemUomSalePrice.Value.ToString(Helper.CommonHelper.NumberFormat) : null : null,  
                            itemCat.ItemDesc
                        }
                    }).ToArray()
            };


            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [Transaction]
        public ActionResult Insert(MItem viewModel, FormCollection formCollection)
        {
            MItem mItemToInsert = new MItem();
            TransferFormValuesTo(mItemToInsert, viewModel);
            mItemToInsert.ItemCatId = _mItemCatRepository.Get(formCollection["ItemCatId"]);
            mItemToInsert.BrandId = _mBrandRepository.Get(formCollection["BrandId"]);

            mItemToInsert.SetAssignedIdTo(viewModel.Id);
            mItemToInsert.CreatedDate = DateTime.Now;
            mItemToInsert.CreatedBy = User.Identity.Name;
            mItemToInsert.DataStatus = EnumDataStatus.New.ToString();

            //IList<MItemUom> listItemUom = new List<MItemUom>();

            mItemToInsert.ItemUoms.Clear();

            MItemUom itemUom = new MItemUom(mItemToInsert);
            itemUom.ItemUomName = formCollection["ItemUomName"];
            UpdateNumericData(itemUom, formCollection);
            itemUom.SetAssignedIdTo(Guid.NewGuid().ToString());

            mItemToInsert.ItemUoms.Add(itemUom);

            //mItemToInsert.ItemUoms = listItemUom;

            _mItemRepository.Save(mItemToInsert);

            try
            {
                _mItemRepository.DbContext.CommitChanges();
            }
            catch (Exception e)
            {

                _mItemRepository.DbContext.RollbackTransaction();

                //throw e.GetBaseException();
                return Content(e.GetBaseException().Message);
            }

            return Content("success");
        }

        [Transaction]
        public ActionResult Delete(MItem viewModel, FormCollection formCollection)
        {
            MItem mItemToDelete = _mItemRepository.Get(viewModel.Id);

            if (mItemToDelete != null)
            {
                _mItemRepository.Delete(mItemToDelete);
            }

            try
            {
                _mItemRepository.DbContext.CommitChanges();
            }
            catch (Exception e)
            {

                _mItemRepository.DbContext.RollbackTransaction();

                return Content(e.GetBaseException().Message);
            }

            return Content("success");
        }

        [Transaction]
        public ActionResult Update(MItem viewModel, FormCollection formCollection)
        {
            MItem mItemToUpdate = _mItemRepository.Get(viewModel.Id);
            mItemToUpdate.ItemCatId = _mItemCatRepository.Get(formCollection["ItemCatId"]);
            mItemToUpdate.BrandId = _mBrandRepository.Get(formCollection["BrandId"]);
            TransferFormValuesTo(mItemToUpdate, viewModel);
            mItemToUpdate.ModifiedDate = DateTime.Now;
            mItemToUpdate.ModifiedBy = User.Identity.Name;
            mItemToUpdate.DataStatus = EnumDataStatus.Updated.ToString();

            //IList<MItemUom> listItemUom = new List<MItemUom>();

            //MItemUom itemUom = new MItemUom(mItemToUpdate);
            //itemUom.ItemUomName = formCollection["ItemUomName"];
            //itemUom.ItemUomPurchasePrice = Convert.ToDecimal(formCollection["ItemUomPurchasePrice"]);
            //itemUom.SetAssignedIdTo(Guid.NewGuid().ToString());

            //listItemUom.Add(itemUom);

            //mItemToUpdate.ItemUoms = listItemUom;


            MItemUom itemUom;
            if (mItemToUpdate.ItemUoms.Count == 0)
            {
                itemUom = new MItemUom(mItemToUpdate);
                itemUom.SetAssignedIdTo(Guid.NewGuid().ToString());
            }
            else
            {
                itemUom = mItemToUpdate.ItemUoms[0];
            }
            itemUom.ItemUomName = formCollection["ItemUomName"];
            UpdateNumericData(itemUom, formCollection);


            mItemToUpdate.ItemUoms.Clear();
            mItemToUpdate.ItemUoms.Add(itemUom);

            _mItemRepository.Update(mItemToUpdate);

            try
            {
                _mItemRepository.DbContext.CommitChanges();
            }
            catch (Exception e)
            {

                _mItemRepository.DbContext.RollbackTransaction();

                return Content(e.GetBaseException().Message);
            }

            return Content("success");
        }

        private static void UpdateNumericData(MItemUom itemUom, FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["ItemUomPurchasePrice"]))
            {
                string ItemUomPurchasePrice = formCollection["ItemUomPurchasePrice"].Replace(",", "");
                itemUom.ItemUomPurchasePrice = Convert.ToDecimal(ItemUomPurchasePrice);
            }
            else
            {
                itemUom.ItemUomPurchasePrice = null;
            }
            if (!string.IsNullOrEmpty(formCollection["ItemUomSalePrice"]))
            {
                string ItemUomSalePrice = formCollection["ItemUomSalePrice"].Replace(",", "");
                itemUom.ItemUomSalePrice = Convert.ToDecimal(ItemUomSalePrice);
            }
            else
            {
                itemUom.ItemUomSalePrice = null;
            }
        }

        private void TransferFormValuesTo(MItem mItemToUpdate, MItem mItemFromForm)
        {
            mItemToUpdate.ItemName = mItemFromForm.ItemName;
            mItemToUpdate.ItemDesc = mItemFromForm.ItemDesc;
            mItemToUpdate.ItemStatus = mItemFromForm.ItemStatus;
            mItemToUpdate.ItemPhoto = mItemFromForm.ItemPhoto;
        }

        [Transaction]
        public virtual ActionResult GetList()
        {
            var items = _mItemRepository.GetAll();
            StringBuilder sb = new StringBuilder();
            MItem mItem = new MItem();
            sb.AppendFormat("{0}:{1};", string.Empty, "-Pilih Produk-");
            for (int i = 0; i < items.Count; i++)
            {
                mItem = items[i];
                sb.AppendFormat("{0}:{1}", mItem.Id, mItem.ItemName);
                if (i < items.Count - 1)
                    sb.Append(";");
            }
            return Content(sb.ToString());
        }

        [Transaction]
        public virtual ActionResult Get(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                MItem mItem = _mItemRepository.Get(id);
                if (mItem.ItemUoms.Count > 0)
                {
                    if (mItem.ItemUoms[0].ItemUomPurchasePrice.HasValue)
                        return Content(mItem.ItemUoms[0].ItemUomPurchasePrice.Value.ToString(Helper.CommonHelper.NumberFormat).Replace(",", ""));
                }
            }
            return Content("0");
        }

        //since upgrade to jqgrid 4, url property not run well
        //so use formcollection "oper" to filter the action
        //this just for insert and update, delete filter not change
        [Transaction]
        public ActionResult InsertOrUpdate(MItem viewModel, FormCollection formCollection)
        {
            if (formCollection["oper"].Equals("add"))
            {
                return Insert(viewModel, formCollection);
            }
            else if (formCollection["oper"].Equals("edit"))
            {
                return Update(viewModel, formCollection);
            }
            return View();
        }
    }
}
