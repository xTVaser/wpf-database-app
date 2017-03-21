namespace RealEstateApp.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("employee")]
    public partial class employee
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(25)]
        public string username { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int office_id { get; set; }

        [Required]
        [StringLength(36)]
        public string password { get; set; }

        [Required]
        [StringLength(100)]
        public string email { get; set; }

        [Required]
        [StringLength(50)]
        public string first_name { get; set; }

        [Required]
        [StringLength(50)]
        public string last_name { get; set; }

        [Required]
        [StringLength(10)]
        public string employee_type { get; set; }

        [Column(TypeName = "text")]
        [Required]
        public string security_question { get; set; }

        [Column(TypeName = "text")]
        [Required]
        public string security_answer { get; set; }

        public bool first_login { get; set; }

        public virtual administrator administrator { get; set; }

        public virtual agent agent { get; set; }

        public virtual office office { get; set; }
    }
}
