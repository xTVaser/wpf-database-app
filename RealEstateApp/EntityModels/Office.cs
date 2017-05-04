namespace RealEstateApp.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Office")]
    public partial class Office
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Office()
        {
            Employees = new HashSet<Employee>();
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
        public virtual ICollection<Employee> Employees { get; set; }

        public virtual StreetAddress StreetAddress { get; set; }
    }
}
