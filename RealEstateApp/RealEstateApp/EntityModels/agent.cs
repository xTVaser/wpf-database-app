namespace RealEstateApp.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Agent")]
    public partial class Agent
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Key]
        [Column(Order = 0)]
        [StringLength(25)]
        public string employee_username { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int employee_office_id { get; set; }

        [Required]
        [StringLength(10)]
        public string phone_number { get; set; }

        [Column(TypeName = "money")]
        public decimal commission_balance { get; set; }

        public float commission_percentage { get; set; }

        public float broker_share { get; set; }

        public virtual Employee Employee { get; set; }
    }
}
