using System;
using Inquiries.Api.Infrastructure.Ef;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Inquiries.Api.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Inquiries.Api.Infrastructure.Ef.DepartmentEf", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("Departments");
                });

            modelBuilder.Entity("Inquiries.Api.Infrastructure.Ef.InquiryDepartmentEf", b =>
                {
                    b.Property<int>("InquiryId")
                        .HasColumnType("int");

                    b.Property<int>("DepartmentId")
                        .HasColumnType("int");

                    b.HasKey("InquiryId", "DepartmentId");

                    b.HasIndex("DepartmentId");

                    b.ToTable("InquiryDepartments");
                });

            modelBuilder.Entity("Inquiries.Api.Infrastructure.Ef.InquiryEf", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(160)
                        .HasColumnType("nvarchar(160)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(120)
                        .HasColumnType("nvarchar(120)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)");

                    b.HasKey("Id");

                    b.ToTable("Inquiries");
                });

            modelBuilder.Entity("Inquiries.Api.Infrastructure.Ef.InquiryDepartmentEf", b =>
                {
                    b.HasOne("Inquiries.Api.Infrastructure.Ef.DepartmentEf", "Department")
                        .WithMany("InquiryDepartments")
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Inquiries.Api.Infrastructure.Ef.InquiryEf", "Inquiry")
                        .WithMany("InquiryDepartments")
                        .HasForeignKey("InquiryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Department");

                    b.Navigation("Inquiry");
                });

            modelBuilder.Entity("Inquiries.Api.Infrastructure.Ef.DepartmentEf", b =>
                {
                    b.Navigation("InquiryDepartments");
                });

            modelBuilder.Entity("Inquiries.Api.Infrastructure.Ef.InquiryEf", b =>
                {
                    b.Navigation("InquiryDepartments");
                });
#pragma warning restore 612, 618
        }
    }
}
