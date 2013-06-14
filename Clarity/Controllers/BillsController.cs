using Clarity.Filters;
using Clarity.Models.Finances;
using System;
using System.Collections.Generic;
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
        private static bool _initialized;

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
                _db.Bills.Add(bill);
                //_db.SaveChanges();

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
    }
}
