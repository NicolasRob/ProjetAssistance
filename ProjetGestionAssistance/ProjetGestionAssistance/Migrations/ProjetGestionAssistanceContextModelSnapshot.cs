﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using ProjetGestionAssistance.Models;

namespace ProjetGestionAssistance.Migrations
{
    [DbContext(typeof(ProjetGestionAssistanceContext))]
    partial class ProjetGestionAssistanceContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ProjetGestionAssistance.Models.Billet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AuteurId");

                    b.Property<string>("Commentaires");

                    b.Property<int?>("CompteId");

                    b.Property<int?>("DepartementId");

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<int?>("EquipeId");

                    b.Property<string>("Etat");

                    b.Property<string>("Image");

                    b.Property<string>("Titre")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("AuteurId");

                    b.HasIndex("CompteId");

                    b.HasIndex("DepartementId");

                    b.HasIndex("EquipeId");

                    b.ToTable("Billet");
                });

            modelBuilder.Entity("ProjetGestionAssistance.Models.Commentaire", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("AuteurId");

                    b.Property<int?>("BilletId");

                    b.Property<DateTime>("DateCreation");

                    b.Property<string>("Texte")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("AuteurId");

                    b.HasIndex("BilletId");

                    b.ToTable("Commentaire");
                });

            modelBuilder.Entity("ProjetGestionAssistance.Models.Compte", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Actif");

                    b.Property<string>("Courriel")
                        .IsRequired();

                    b.Property<int>("EquipeId");

                    b.Property<string>("MotPasse")
                        .IsRequired();

                    b.Property<string>("Nom")
                        .IsRequired();

                    b.Property<string>("Prenom")
                        .IsRequired();

                    b.Property<string>("Telephone")
                        .IsRequired();

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.HasIndex("Courriel")
                        .IsUnique()
                        .HasName("Compte_UQ_Courriel");

                    b.HasIndex("EquipeId");

                    b.ToTable("Compte");
                });

            modelBuilder.Entity("ProjetGestionAssistance.Models.Departement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Nom");

                    b.HasKey("Id");

                    b.ToTable("Departement");
                });

            modelBuilder.Entity("ProjetGestionAssistance.Models.Equipe", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("DepartementId");

                    b.Property<string>("Nom");

                    b.HasKey("Id");

                    b.HasIndex("DepartementId");

                    b.ToTable("Equipe");
                });

            modelBuilder.Entity("ProjetGestionAssistance.Models.Billet", b =>
                {
                    b.HasOne("ProjetGestionAssistance.Models.Compte", "Auteur")
                        .WithMany()
                        .HasForeignKey("AuteurId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ProjetGestionAssistance.Models.Compte", "Compte")
                        .WithMany()
                        .HasForeignKey("CompteId");

                    b.HasOne("ProjetGestionAssistance.Models.Departement", "Departement")
                        .WithMany()
                        .HasForeignKey("DepartementId");

                    b.HasOne("ProjetGestionAssistance.Models.Equipe", "Equipe")
                        .WithMany()
                        .HasForeignKey("EquipeId");
                });

            modelBuilder.Entity("ProjetGestionAssistance.Models.Commentaire", b =>
                {
                    b.HasOne("ProjetGestionAssistance.Models.Compte", "Auteur")
                        .WithMany()
                        .HasForeignKey("AuteurId");

                    b.HasOne("ProjetGestionAssistance.Models.Billet", "Billet")
                        .WithMany()
                        .HasForeignKey("BilletId");
                });

            modelBuilder.Entity("ProjetGestionAssistance.Models.Compte", b =>
                {
                    b.HasOne("ProjetGestionAssistance.Models.Equipe", "Equipe")
                        .WithMany()
                        .HasForeignKey("EquipeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ProjetGestionAssistance.Models.Equipe", b =>
                {
                    b.HasOne("ProjetGestionAssistance.Models.Departement", "Departement")
                        .WithMany()
                        .HasForeignKey("DepartementId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
