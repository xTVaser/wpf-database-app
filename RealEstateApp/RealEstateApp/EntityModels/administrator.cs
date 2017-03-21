namespace RealEstateApp.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("administrator")]
    public partial class administrator
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(25)]
        public string employee_username { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int employee_office_id { get; set; }

        [Column(TypeName = "money")]
        public decimal salary { get; set; }

        public virtual employee employee { get; set; }
    }
}
