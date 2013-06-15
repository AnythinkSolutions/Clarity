using Clarity.Filters;
using Clarity.Models.Finances;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebMatrix.WebData;

namespace Clarity.Controllers
{
    [Authorize]
    public class BillsController : ApiController
    {
        private FinancesContext _db = new FinancesContext();

        private int UserId
        {
            get
            {
                InitializeSimpleMembershipAttribute.Initialize();
                return WebSecurity.GetUserId(User.Identity.Name);
            }
        }

        /// <summary>
        /// Gets a list of all bills
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BillDto> GetBills()
        {
            int userId = this.UserId;
            var bills = _db.Bills.Include("Payments").Where(b => b.UserId == userId).OrderBy(b => b.PaymentDay).ToList();
            var billDtos = bills.Select(b => new BillDto(b));
            return billDtos;
        }

        /// <summary>
        /// Gets a specific bill
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BillDto GetBill(int id)
        {
            var bill = _db.Bills.FirstOrDefault(b => b.Id == id && b.UserId == this.UserId);
            if (bill == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            if (bill.UserId != this.UserId)
            {
                // Trying to modify a record that does not belong to the user
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.Unauthorized));
            }

            return new BillDto(bill);
        }

        /// <summary>
        /// Adds a new Bill
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public HttpResponseMessage PostBill(BillDto dto)
        {
            if (ModelState.IsValid)
            {
                dto.UserId = this.UserId;

                var bill = dto.ToEntity();
                bill.LastPayment = DateTime.Today;

                _db.Bills.Add(bill);
                _db.SaveChanges();

                dto.Id = bill.Id;

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, dto);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = bill.Id }));
                return response;
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        /// <summary>
        /// PUT: Updates and existing bill
        /// </summary>
        /// <param name="id">The id of the Bill</param>
        /// <param name="billDto"></param>
        /// <returns></returns>
        public HttpResponseMessage PutBill(int id, BillDto billDto)
        {
            if (ModelState.IsValid && id == billDto.Id)
            {
                Bill bill = billDto.ToEntity();
                if (_db.Entry(bill).Entity.UserId != this.UserId)
                {
                    // Trying to modify a record that does not belong to the user
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }

                _db.Entry(bill).State = EntityState.Modified;

                try
                {
                    _db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }

                return Request.CreateResponse(HttpStatusCode.NoContent);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        /// <summary>
        /// DELETE: Deletes a bill
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public HttpResponseMessage DeleteBill(int id)
        {
            Bill bill = _db.Bills.Find(id);
            if (bill == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            if (_db.Entry(bill).Entity.UserId != this.UserId)
            {
                // Trying to delete a record that does not belong to the user
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }

            BillDto billDto = new BillDto(bill);
            _db.Bills.Remove(bill);

            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return Request.CreateResponse(HttpStatusCode.OK, billDto);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

    }
}
