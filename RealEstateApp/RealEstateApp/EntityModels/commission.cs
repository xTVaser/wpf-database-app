namespace RealEstateApp.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("commission")]
    public partial class commission
    {
        public int id { get; set; }

        public int payable_to { get; set; }

        [Column(TypeName = "money")]
        public decimal amount { get; set; }

        [Required]
        [StringLength(50)]
        public string reason { get; set; }

        public DateTime date_payed_out { get; set; }
    }
}
