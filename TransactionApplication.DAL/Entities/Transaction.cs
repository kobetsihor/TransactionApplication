using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransactionApplication.DAL.Entities
{
    public class Transaction
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [MaxLength(50)]
        public string PublicId { get; set; }
        public decimal Amount { get; set; }
        [MinLength(3)]
        [MaxLength(3)]
        public string Code { get; set; }
        public DateTimeOffset Date { get; set; }
        public string Status { get; set; }
    }
}