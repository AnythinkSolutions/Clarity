using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Clarity.Models.Finances
{
    public class Bill
    {
        public Bill()
        {
        }

        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public string Name { get; set; }

        public decimal Amount { get; set; }
        public bool IsFixed { get; set; }
        public TimeSpan Frequency { get; set; }

        [NotMapped]
        public DateTime LastPayment { get; set; }

        [Range(1, 31)]
        public int PaymentDay { get; set; }

        public virtual List<Payment> Payments { get; set; }

        /// <summary>
        /// Projects out the payments for this bill into the future
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public IEnumerable<Payment> ProjectPayments(TimeSpan duration)
        {
            return new Collection<Payment>();
        }
    }

    public class TestDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [Required]
        public string Name { get; set; }

    }

    public class BillDto
    {
        public BillDto() { }
        public BillDto(Bill bill)
        {
            this.Id = bill.Id;
            this.UserId = bill.UserId;
            this.Name = bill.Name;
            this.Amount = bill.Amount;
            this.IsFixed = bill.IsFixed;
            this.PaymentDay = bill.PaymentDay;

            //this.FuturePayments = new List<PaymentDto>();
            //this.PastPayments = new List<PaymentDto>();

            //this.PastPayments = bill.Payments.Take(6).Select(p => new PaymentDto(p)).ToList();

            ////Project future payments forward 6 months
            //this.FuturePayments = bill.ProjectPayments(TimeSpan.FromDays(183)).Select(p => new PaymentDto(p, true)).ToList();
        }

        public int Id { get; set; }

        public int UserId { get; set; }
        [Required]
        public string Name { get; set; }

        public decimal Amount { get; set; }
        public bool IsFixed { get; set; }

        [Range(1, 31)]
        public int PaymentDay { get; set; }

        //public List<PaymentDto> FuturePayments { get; set; }
        //public List<PaymentDto> PastPayments { get; set; }

        public Bill ToEntity()
        {
            Bill bill = new Bill()
            {
                Id = this.Id,
                UserId = this.UserId,
                Name = this.Name,
                Amount = this.Amount,
                IsFixed = this.IsFixed,
                PaymentDay = this.PaymentDay
            };

            return bill;
        }
    }

    public class Payment
    {
        public Payment()
        {
        }

        public int Id { get; set; }
        [ForeignKey("Bill")]
        public int BillId { get; set; }
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; }
        public DateTime PaymentDate { get; set; }
        public virtual Bill Bill { get; set; }
    }

    public class PaymentDto
    {
        public PaymentDto() { }
        public PaymentDto(Payment payment, bool isProjected = false)
        {
            this.Id = payment.Id;
            this.BillId = payment.BillId;
            this.Amount = payment.Amount;
            this.PaymentDate = payment.PaymentDate;
            this.IsPaid = payment.IsPaid;
            this.IsProjected = isProjected;
        }

        public int Id { get; set; }
        public int BillId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public bool IsPaid { get; set; }
        public bool IsProjected { get; set; }
    }
}