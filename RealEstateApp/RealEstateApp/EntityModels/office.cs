namespace RealEstateApp.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("office")]
    public partial class office
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public office()
        {
            employees = new HashSet<employee>();
        }

        public int id { get; set; }

        public int address_id { get; set; }

        [Required]
        [StringLength(25)]
        public string broker_username { get; set; }

        [Required]
        [StringLength(10)]
        public string phone_number { get; set; }

        [Required]
        [StringLength(10)]
        public string fax_number { get; set; }

        [Required]
        [StringLength(100)]
        public string email { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<employee> employees { get; set; }

        public virtual street_address street_address { get; set; }
    }
}
