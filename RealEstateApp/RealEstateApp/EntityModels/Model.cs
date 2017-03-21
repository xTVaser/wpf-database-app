namespace RealEstateApp.EntityModels
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Model : DbContext
    {
        public Model()
            : base("name=Database")
        {
        }

        public virtual DbSet<administrator> administrators { get; set; }
        public virtual DbSet<agent> agents { get; set; }
        public virtual DbSet<client> clients { get; set; }
        public virtual DbSet<commission> commissions { get; set; }
        public virtual DbSet<employee> employees { get; set; }
        public virtual DbSet<feature> features { get; set; }
        public virtual DbSet<listing> listings { get; set; }
        public virtual DbSet<offer> offers { get; set; }
        public virtual DbSet<office> offices { get; set; }
        public virtual DbSet<street_address> street_address { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<administrator>()
                .Property(e => e.salary)
                .HasPrecision(19, 4);

            modelBuilder.Entity<agent>()
                .Property(e => e.phone_number)
                .IsFixedLength();

            modelBuilder.Entity<agent>()
                .Property(e => e.commission_balance)
                .HasPrecision(19, 4);

            modelBuilder.Entity<client>()
                .Property(e => e.client_type)
                .IsFixedLength();

            modelBuilder.Entity<client>()
                .Property(e => e.phone_number)
                .IsFixedLength();

            modelBuilder.Entity<client>()
                .HasMany(e => e.listings)
                .WithRequired(e => e.client)
                .HasForeignKey(e => e.seller_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<client>()
                .HasMany(e => e.offers)
                .WithRequired(e => e.client)
                .HasForeignKey(e => e.client_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<commission>()
                .Property(e => e.amount)
                .HasPrecision(19, 4);

            modelBuilder.Entity<employee>()
                .Property(e => e.employee_type)
                .IsFixedLength();

            modelBuilder.Entity<employee>()
                .Property(e => e.security_question)
                .IsUnicode(false);

            modelBuilder.Entity<employee>()
                .Property(e => e.security_answer)
                .IsUnicode(false);

            modelBuilder.Entity<employee>()
                .HasOptional(e => e.administrator)
                .WithRequired(e => e.employee);

            modelBuilder.Entity<employee>()
                .HasOptional(e => e.agent)
                .WithRequired(e => e.employee);

            modelBuilder.Entity<feature>()
                .Property(e => e.heading)
                .IsUnicode(false);

            modelBuilder.Entity<feature>()
                .Property(e => e.body)
                .IsUnicode(false);

            modelBuilder.Entity<listing>()
                .HasMany(e => e.features)
                .WithRequired(e => e.listing)
                .HasForeignKey(e => e.listing_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<listing>()
                .HasMany(e => e.offers)
                .WithRequired(e => e.listing)
                .HasForeignKey(e => e.listing_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<offer>()
                .Property(e => e.amount)
                .HasPrecision(19, 4);

            modelBuilder.Entity<office>()
                .Property(e => e.phone_number)
                .IsFixedLength();

            modelBuilder.Entity<office>()
                .Property(e => e.fax_number)
                .IsFixedLength();

            modelBuilder.Entity<office>()
                .HasMany(e => e.employees)
                .WithRequired(e => e.office)
                .HasForeignKey(e => e.office_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<street_address>()
                .Property(e => e.province_short)
                .IsFixedLength();

            modelBuilder.Entity<street_address>()
                .Property(e => e.postal_code)
                .IsFixedLength();

            modelBuilder.Entity<street_address>()
                .HasMany(e => e.listings)
                .WithRequired(e => e.street_address)
                .HasForeignKey(e => e.address_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<street_address>()
                .HasMany(e => e.offices)
                .WithRequired(e => e.street_address)
                .HasForeignKey(e => e.address_id)
                .WillCascadeOnDelete(false);
        }
    }
}
