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
    public class CustomerController : Controller
    {
        public CustomerController()
            : this(new MCustomerRepository(), new RefAddressRepository(), new RefPersonRepository())
        { }

        private readonly IMCustomerRepository _mCustomerRepository;
        private readonly IRefAddressRepository _refAddressRepository;
        private readonly IRefPersonRepository _refPersonRepository;
        public CustomerController(IMCustomerRepository mCustomerRepository, IRefAddressRepository refAddressRepository, IRefPersonRepository refPersonRepository)
        {
            Check.Require(mCustomerRepository != null, "mCustomerRepository may not be null");
            Check.Require(refAddressRepository != null, "refAddressRepository may not be null");
            Check.Require(refPersonRepository != null, "refPersonRepository may not be null");

            this._mCustomerRepository = mCustomerRepository;
            this._refAddressRepository = refAddressRepository;
            this._refPersonRepository = refPersonRepository;
        }

        public ActionResult Search()
        {
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }

        [Transaction]
        public virtual ActionResult ListSearch(string sidx, string sord, int page, int rows, string searchBy, string searchText)
        {
            int totalRecords = 0;
            var sups = _mCustomerRepository.GetPagedCustomerList(sidx, sord, page, rows, ref totalRecords, searchBy, searchText);
            int pageSize = rows;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);
            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = (
                    from sup in sups
                    select new
                    {
                        i = sup.Id.ToString(),
                        cell = new string[] {
                            sup.Id,  
                          sup.PersonId != null?  sup.PersonId.PersonName : null,  
                          sup.PersonId != null?  sup.PersonId.PersonGender : null, 
                            //sup.CustomerHealthProblem, 
                            //sup.CustomerProductDisc.HasValue ? sup.CustomerProductDisc.Value.ToString(Helper.CommonHelper.NumberFormat) : null,
                            //sup.CustomerServiceDisc.HasValue ? sup.CustomerServiceDisc.Value.ToString(Helper.CommonHelper.NumberFormat) : null, 
                            //sup.CustomerMassageStrength,
                            sup.CustomerDesc,
                        }
                    }).ToArray()
            };


            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        [Transaction]
        public virtual ActionResult List(string sidx, string sord, int page, int rows)
        {
            int totalRecords = 0;
            var sups = _mCustomerRepository.GetPagedCustomerList(sidx, sord, page, rows, ref totalRecords, null, null);
            int pageSize = rows;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);
            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = (
                    from sup in sups
                    select new
                    {
                        i = sup.Id.ToString(),
                        cell = new string[] {
                            sup.Id, 
                          sup.PersonId != null?  sup.PersonId.PersonFirstName : null, 
                          sup.PersonId != null?  sup.PersonId.PersonLastName : null, 
                          sup.PersonId != null?  sup.PersonId.PersonName : null, 
                          sup.AddressId != null?  sup.AddressId.AddressLine1 : null,
                          sup.AddressId != null?  sup.AddressId.AddressLine2 : null,
                          sup.AddressId != null?  sup.AddressId.Address : null,
                          sup.AddressId != null?  sup.AddressId.AddressPhone : null,
                          sup.PersonId != null?  sup.PersonId.PersonGender : null,
                          sup.PersonId != null && sup.PersonId.PersonDob.HasValue ?  sup.PersonId.PersonDob.Value.ToString(Helper.CommonHelper.DateFormat) : null, 
                          sup.PersonId != null?  sup.PersonId.PersonReligion : null, 
                          sup.PersonId != null?  sup.PersonId.PersonRace : null, 
                            //sup.CustomerHealthProblem,
                            sup.CustomerJoinDate.HasValue ? sup.CustomerJoinDate.Value.ToString(Helper.CommonHelper.DateFormat) : null,
                            sup.CustomerLastBuy.HasValue ? sup.CustomerLastBuy.Value.ToString(Helper.CommonHelper.DateFormat) : null,
                            //sup.CustomerProductDisc.HasValue ? sup.CustomerProductDisc.Value.ToString(Helper.CommonHelper.NumberFormat) : null,
                            //sup.CustomerServiceDisc.HasValue ? sup.CustomerServiceDisc.Value.ToString(Helper.CommonHelper.NumberFormat) : null,
                            sup.CustomerStatus,
                            //sup.CustomerMassageStrength,
                            sup.CustomerDesc,
                        }
                    }).ToArray()
            };


            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [Transaction]
        public ActionResult Insert(MCustomer viewModel, FormCollection formCollection)
        {
            RefAddress address = new RefAddress();
            TransferFormValuesTo(address, formCollection);
            address.SetAssignedIdTo(Guid.NewGuid().ToString());
            address.CreatedDate = DateTime.Now;
            address.CreatedBy = User.Identity.Name;
            address.DataStatus = EnumDataStatus.New.ToString();
            _refAddressRepository.Save(address);

            RefPerson person = new RefPerson();
            TransferFormValuesTo(person, formCollection);
            person.SetAssignedIdTo(Guid.NewGuid().ToString());
            person.CreatedDate = DateTime.Now;
            person.CreatedBy = User.Identity.Name;
            person.DataStatus = EnumDataStatus.New.ToString();
            _refPersonRepository.Save(person);

            //UpdateNumericData(viewModel, formCollection);
            MCustomer mCustomerToInsert = new MCustomer();
            TransferFormValuesTo(mCustomerToInsert, viewModel);
            mCustomerToInsert.SetAssignedIdTo(viewModel.Id);
            mCustomerToInsert.CreatedDate = DateTime.Now;
            mCustomerToInsert.CreatedBy = User.Identity.Name;
            mCustomerToInsert.DataStatus = EnumDataStatus.New.ToString();

            mCustomerToInsert.AddressId = address;
            mCustomerToInsert.PersonId = person;

            _mCustomerRepository.Save(mCustomerToInsert);

            try
            {
                _mCustomerRepository.DbContext.CommitChanges();
            }
            catch (Exception e)
            {

                _mCustomerRepository.DbContext.RollbackTransaction();

                //throw e.GetBaseException();
                return Content(e.GetBaseException().Message);
            }

            return Content("success");
        }

        [Transaction]
        public ActionResult Delete(MCustomer viewModel, FormCollection formCollection)
        {
            MCustomer mCustomerToDelete = _mCustomerRepository.Get(viewModel.Id);

            if (mCustomerToDelete != null)
            {
                _mCustomerRepository.Delete(mCustomerToDelete);
                _refAddressRepository.Delete(mCustomerToDelete.AddressId);
                _refPersonRepository.Delete(mCustomerToDelete.PersonId);
            }

            try
            {
                _mCustomerRepository.DbContext.CommitChanges();
            }
            catch (Exception e)
            {

                _mCustomerRepository.DbContext.RollbackTransaction();

                return Content(e.GetBaseException().Message);
            }

            return Content("success");
        }



        //since upgrade to jqgrid 4, url property not run well
        //so use formcollection "oper" to filter the action
        //this just for insert and update, delete filter not change
        [Transaction]
        public ActionResult InsertOrUpdate(MCustomer viewModel, FormCollection formCollection)
        {
            if (formCollection["oper"].Equals("add"))
            {
                return Insert(viewModel, formCollection);
            }
            else if (formCollection["oper"].Equals("edit"))
            {
                return Update(viewModel, formCollection);
            }
            else if (formCollection["oper"].Equals("delete"))
            {
                return Delete(viewModel, formCollection);
            }
            return View();
        }



        [Transaction]
        public ActionResult Update(MCustomer viewModel, FormCollection formCollection)
        {
            //UpdateNumericData(viewModel, formCollection);
            MCustomer mCustomerToUpdate = _mCustomerRepository.Get(viewModel.Id);
            TransferFormValuesTo(mCustomerToUpdate, viewModel);
            mCustomerToUpdate.ModifiedDate = DateTime.Now;
            mCustomerToUpdate.ModifiedBy = User.Identity.Name;
            mCustomerToUpdate.DataStatus = EnumDataStatus.Updated.ToString();

            RefAddress address = mCustomerToUpdate.AddressId;
            TransferFormValuesTo(address, formCollection);
            address.ModifiedDate = DateTime.Now;
            address.ModifiedBy = User.Identity.Name;
            address.DataStatus = EnumDataStatus.Updated.ToString();

            RefPerson person = mCustomerToUpdate.PersonId;
            TransferFormValuesTo(person, formCollection);
            person.ModifiedDate = DateTime.Now;
            person.ModifiedBy = User.Identity.Name;
            person.DataStatus = EnumDataStatus.Updated.ToString();

            _mCustomerRepository.Update(mCustomerToUpdate);

            try
            {
                _mCustomerRepository.DbContext.CommitChanges();
            }
            catch (Exception e)
            {

                _mCustomerRepository.DbContext.RollbackTransaction();

                return Content(e.GetBaseException().Message);
            }

            return Content("success");
        }


        //private static void UpdateNumericData(MCustomer viewModel, FormCollection formCollection)
        //{
        //    if (!string.IsNullOrEmpty(formCollection["CustomerProductDisc"]))
        //    {
        //        string CustomerProductDisc = formCollection["CustomerProductDisc"].Replace(",", "");
        //        viewModel.CustomerProductDisc = Convert.ToDecimal(CustomerProductDisc);
        //    }
        //    else
        //    {
        //        viewModel.CustomerProductDisc = null;
        //    }
        //    if (!string.IsNullOrEmpty(formCollection["CustomerServiceDisc"]))
        //    {
        //        string CustomerServiceDisc = formCollection["CustomerServiceDisc"].Replace(",", "");
        //        viewModel.CustomerServiceDisc = Convert.ToDecimal(CustomerServiceDisc);
        //    }
        //    else
        //    {
        //        viewModel.CustomerServiceDisc = null;
        //    }
        //}

        private void TransferFormValuesTo(RefPerson person, FormCollection formCollection)
        {
            person.PersonFirstName = formCollection["PersonFirstName"];
            person.PersonLastName = formCollection["PersonLastName"];
            person.PersonGender = formCollection["PersonGender"];
            person.PersonDob = string.IsNullOrEmpty(formCollection["PersonDob"]) ? (DateTime?)null : Convert.ToDateTime(formCollection["PersonDob"]);
            person.PersonReligion = formCollection["PersonReligion"];
            person.PersonRace = formCollection["PersonRace"];
            person.PersonFirstName = formCollection["PersonFirstName"];
            person.PersonFirstName = formCollection["PersonFirstName"];
        }

        private void TransferFormValuesTo(RefAddress address, FormCollection formCollection)
        {
            address.AddressLine1 = formCollection["AddressLine1"];
            address.AddressLine2 = formCollection["AddressLine2"];
            address.AddressLine3 = formCollection["AddressLine3"];
            address.AddressPhone = formCollection["AddressPhone"];

        }

        private static void TransferFormValuesTo(MCustomer mCustomerToUpdate, MCustomer mCustomerFromForm)
        {
            //mCustomerToUpdate.CustomerHealthProblem = mCustomerFromForm.CustomerHealthProblem;
            mCustomerToUpdate.CustomerJoinDate = mCustomerFromForm.CustomerJoinDate;
            mCustomerToUpdate.CustomerLastBuy = mCustomerFromForm.CustomerLastBuy;
            //mCustomerToUpdate.CustomerProductDisc = mCustomerFromForm.CustomerProductDisc;
            //mCustomerToUpdate.CustomerServiceDisc = mCustomerFromForm.CustomerServiceDisc;
            mCustomerToUpdate.CustomerStatus = mCustomerFromForm.CustomerStatus;
            //mCustomerToUpdate.CustomerMassageStrength = mCustomerFromForm.CustomerMassageStrength;
            mCustomerToUpdate.CustomerDesc = mCustomerFromForm.CustomerDesc;
        }

        public virtual ActionResult GetGenderList()
        {
            return Content(Helper.CommonHelper.GetEnumListForGrid<EnumPersonGender>("-Pilih Jenis Kelamin-"));
        }

        public virtual ActionResult GetReligionList()
        {
            return Content(Helper.CommonHelper.GetEnumListForGrid<EnumPersonReligion>("-Pilih Agama-"));
        }



        //public virtual ActionResult GetMassageStrengthList()
        //{
        //    return Content(Helper.CommonHelper.GetEnumListForGrid<EnumMassageStrength>("-Pilih Kekuatan Pijitan-"));
        //}


    }
}
