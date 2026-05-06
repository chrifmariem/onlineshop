using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace onlineShop.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "client",
                columns: table => new
                {
                    Idclient = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    prenom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    motpasse = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    telephone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    adresse = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    dateinscrip = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "getdate()"),
                    role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "client")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_client", x => x.Idclient);
                });

            migrationBuilder.CreateTable(
                name: "produit",
                columns: table => new
                {
                    Idproduit = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    aref = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    design = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    venteHT = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    tva = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    ttc = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    qtestock = table.Column<int>(type: "int", nullable: false),
                    promoweb = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    cmdLast = table.Column<DateTime>(type: "date", nullable: true),
                    qtelast = table.Column<int>(type: "int", nullable: true),
                    prixpub = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    coli = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    labo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    dernmiseajour = table.Column<DateTime>(type: "date", nullable: true),
                    image = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    tauxtvav = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    dlc = table.Column<DateTime>(type: "date", nullable: true),
                    codebar = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_produit", x => x.Idproduit);
                });

            migrationBuilder.CreateTable(
                name: "commande",
                columns: table => new
                {
                    Idcmd = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    datecmd = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "getdate()"),
                    statut = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "en cours"),
                    totalHT = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    totalTVA = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    totalTTC = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    Idclient = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_commande", x => x.Idcmd);
                    table.ForeignKey(
                        name: "FK_commande_ToClient",
                        column: x => x.Idclient,
                        principalTable: "client",
                        principalColumn: "Idclient",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "panier",
                columns: table => new
                {
                    Idpanier = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Idclient = table.Column<int>(type: "int", nullable: false),
                    Idproduit = table.Column<int>(type: "int", nullable: false),
                    quantite = table.Column<int>(type: "int", nullable: false),
                    dateajout = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()"),
                    datemodification = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_panier", x => x.Idpanier);
                    table.ForeignKey(
                        name: "FK_Panier_Client",
                        column: x => x.Idclient,
                        principalTable: "client",
                        principalColumn: "Idclient",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Panier_Produit",
                        column: x => x.Idproduit,
                        principalTable: "produit",
                        principalColumn: "Idproduit",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Facture",
                columns: table => new
                {
                    Idfacture = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    numfac = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    dateFac = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "getdate()"),
                    totalHT = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    totalTVA = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    totalTTC = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    statut = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "payée"),
                    Idcmd = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facture", x => x.Idfacture);
                    table.ForeignKey(
                        name: "FK_Facture_Tocommande",
                        column: x => x.Idcmd,
                        principalTable: "commande",
                        principalColumn: "Idcmd",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Lignecmd",
                columns: table => new
                {
                    Idligne = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    qte = table.Column<int>(type: "int", nullable: false),
                    prixuni = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    tva = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    prixtotal = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Idcmd = table.Column<int>(type: "int", nullable: false),
                    Idproduit = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lignecmd", x => x.Idligne);
                    table.ForeignKey(
                        name: "FK_Lignecmd_ToProduit",
                        column: x => x.Idproduit,
                        principalTable: "produit",
                        principalColumn: "Idproduit",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Lignecmd_Tocommande",
                        column: x => x.Idcmd,
                        principalTable: "commande",
                        principalColumn: "Idcmd",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reclamation",
                columns: table => new
                {
                    Idrec = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sujet = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    descrip = table.Column<string>(type: "text", nullable: false),
                    daterec = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "getdate()"),
                    statut = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "en cours"),
                    reponseAdmin = table.Column<string>(type: "text", nullable: false),
                    dateresp = table.Column<DateTime>(type: "datetime", nullable: true),
                    Idclient = table.Column<int>(type: "int", nullable: false),
                    Idcmd = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reclamation", x => x.Idrec);
                    table.ForeignKey(
                        name: "FK_Reclamation_client_Idclient",
                        column: x => x.Idclient,
                        principalTable: "client",
                        principalColumn: "Idclient",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reclamation_commande_Idcmd",
                        column: x => x.Idcmd,
                        principalTable: "commande",
                        principalColumn: "Idcmd",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_client_email",
                table: "client",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_commande_Idclient",
                table: "commande",
                column: "Idclient");

            migrationBuilder.CreateIndex(
                name: "IX_Facture_Idcmd",
                table: "Facture",
                column: "Idcmd",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Facture_numfac",
                table: "Facture",
                column: "numfac",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lignecmd_Idcmd",
                table: "Lignecmd",
                column: "Idcmd");

            migrationBuilder.CreateIndex(
                name: "IX_Lignecmd_Idproduit",
                table: "Lignecmd",
                column: "Idproduit");

            migrationBuilder.CreateIndex(
                name: "IX_panier_Idproduit",
                table: "panier",
                column: "Idproduit");

            migrationBuilder.CreateIndex(
                name: "IX_Panier_UniqueClientProduct",
                table: "panier",
                columns: new[] { "Idclient", "Idproduit" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_produit_codebar",
                table: "produit",
                column: "codebar",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reclamation_Idclient",
                table: "Reclamation",
                column: "Idclient");

            migrationBuilder.CreateIndex(
                name: "IX_Reclamation_Idcmd",
                table: "Reclamation",
                column: "Idcmd");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Facture");

            migrationBuilder.DropTable(
                name: "Lignecmd");

            migrationBuilder.DropTable(
                name: "panier");

            migrationBuilder.DropTable(
                name: "Reclamation");

            migrationBuilder.DropTable(
                name: "produit");

            migrationBuilder.DropTable(
                name: "commande");

            migrationBuilder.DropTable(
                name: "client");
        }
    }
}
