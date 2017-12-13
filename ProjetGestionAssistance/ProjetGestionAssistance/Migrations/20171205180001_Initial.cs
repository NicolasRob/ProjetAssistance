using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProjetGestionAssistance.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Departement",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    Nom = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departement", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Equipe",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    DepartementId = table.Column<int>(nullable: false),
                    Nom = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipe", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Equipe_Departement_DepartementId",
                        column: x => x.DepartementId,
                        principalTable: "Departement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Compte",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    Actif = table.Column<bool>(nullable: false),
                    Courriel = table.Column<string>(nullable: false),
                    EquipeId = table.Column<int>(nullable: false),
                    MotPasse = table.Column<string>(nullable: false),
                    Nom = table.Column<string>(nullable: false),
                    Prenom = table.Column<string>(nullable: false),
                    Telephone = table.Column<string>(nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Compte", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Compte_Equipe_EquipeId",
                        column: x => x.EquipeId,
                        principalTable: "Equipe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Billet",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    AuteurId = table.Column<int>(nullable: false),
                    Commentaires = table.Column<string>(nullable: true),
                    CompteId = table.Column<int>(nullable: true),
                    DepartementId = table.Column<int>(nullable: true),
                    Description = table.Column<string>(nullable: false),
                    EquipeId = table.Column<int>(nullable: true),
                    Etat = table.Column<string>(nullable: true),
                    Image = table.Column<string>(nullable: true),
                    Titre = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Billet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Billet_Compte_AuteurId",
                        column: x => x.AuteurId,
                        principalTable: "Compte",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Billet_Compte_CompteId",
                        column: x => x.CompteId,
                        principalTable: "Compte",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Billet_Departement_DepartementId",
                        column: x => x.DepartementId,
                        principalTable: "Departement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Billet_Equipe_EquipeId",
                        column: x => x.EquipeId,
                        principalTable: "Equipe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Commentaire",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    AuteurId = table.Column<int>(nullable: true),
                    BilletId = table.Column<int>(nullable: true),
                    DateCreation = table.Column<DateTime>(nullable: false),
                    Texte = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commentaire", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Commentaire_Compte_AuteurId",
                        column: x => x.AuteurId,
                        principalTable: "Compte",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Commentaire_Billet_BilletId",
                        column: x => x.BilletId,
                        principalTable: "Billet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Billet_AuteurId",
                table: "Billet",
                column: "AuteurId");

            migrationBuilder.CreateIndex(
                name: "IX_Billet_CompteId",
                table: "Billet",
                column: "CompteId");

            migrationBuilder.CreateIndex(
                name: "IX_Billet_DepartementId",
                table: "Billet",
                column: "DepartementId");

            migrationBuilder.CreateIndex(
                name: "IX_Billet_EquipeId",
                table: "Billet",
                column: "EquipeId");

            migrationBuilder.CreateIndex(
                name: "IX_Commentaire_AuteurId",
                table: "Commentaire",
                column: "AuteurId");

            migrationBuilder.CreateIndex(
                name: "IX_Commentaire_BilletId",
                table: "Commentaire",
                column: "BilletId");

            migrationBuilder.CreateIndex(
                name: "Compte_UQ_Courriel",
                table: "Compte",
                column: "Courriel",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Compte_EquipeId",
                table: "Compte",
                column: "EquipeId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipe_DepartementId",
                table: "Equipe",
                column: "DepartementId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Commentaire");

            migrationBuilder.DropTable(
                name: "Billet");

            migrationBuilder.DropTable(
                name: "Compte");

            migrationBuilder.DropTable(
                name: "Equipe");

            migrationBuilder.DropTable(
                name: "Departement");
        }
    }
}
