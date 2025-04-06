using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Lojinha.Models
{
    public class CartItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProductId { get; set; }

        public string UserId { get; set; }

        public string ProductName { get; set; }

        public int Quantity { get; set; }

        //[Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        //[Column(TypeName = "decimal(18, 2)")]
        public decimal Total { get; set; }

        [ForeignKey("AspNetUser")]
        public string AspNetUsers_Id { get; set; }

        public virtual ApplicationUser AspNetUser { get; set; }

        [NotMapped]
        public string ProductId2 { get; set; }
    }
}