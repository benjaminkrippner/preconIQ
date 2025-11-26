using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PreconIQAPI.Migrations
{
    /// <inheritdoc />
    public partial class initialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Precon");

            migrationBuilder.CreateTable(
                name: "Project",
                schema: "Precon",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OpportunityId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OpportunityName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    OpportunityLegalContractProjectName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    OpportunityStageName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProjectNumber = table.Column<int>(type: "int", nullable: true),
                    AccountName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsWon = table.Column<bool>(type: "bit", nullable: false),
                    IsClosed = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Project", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectMetaData",
                schema: "Precon",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    City = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    State = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ZipCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    County = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BidNumber = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ContractType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Duration = table.Column<int>(type: "int", nullable: true),
                    MarketSegment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OpportunityDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BidAsCompany = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsParentOpportunity = table.Column<bool>(type: "bit", nullable: false),
                    ParentProjectId = table.Column<int>(type: "int", nullable: true),
                    EstimatedFinalContractRevenue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MarginPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MarginDollars = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Megawatts = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RevenueType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Revenue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProjectStartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ProjectCompletionDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CloseDate = table.Column<DateOnly>(type: "date", nullable: true),
                    SubstantialCompletionDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(10,6)", nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(10,6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectMetaData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectMetaData_Project_ParentProjectId",
                        column: x => x.ParentProjectId,
                        principalSchema: "Precon",
                        principalTable: "Project",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProjectMetaData_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Precon",
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectNormals",
                schema: "Precon",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    AvgHighTemp = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    AvgLowTemp = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    RecordAvgHighTemp = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    RecordAvgLowTemp = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    AvgRain = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    RecordHighAvgRain = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    RecordLowAvgRain = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    AvgSnow = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    RecordHighAvgSnow = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    RecordLowAvgSnow = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    AvgHighWind = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    AvgLowWind = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    RecordAvgHighWind = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    RecordAvgLowWind = table.Column<decimal>(type: "decimal(5,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectNormals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectNormals_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Precon",
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectWindData",
                schema: "Precon",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    ValidTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(10,6)", nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(10,6)", nullable: false),
                    U10 = table.Column<double>(type: "float", nullable: false),
                    V10 = table.Column<double>(type: "float", nullable: false),
                    Number = table.Column<int>(type: "int", nullable: false),
                    Expver = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectWindData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectWindData_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Precon",
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMetaData_ParentProjectId",
                schema: "Precon",
                table: "ProjectMetaData",
                column: "ParentProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMetaData_ProjectId",
                schema: "Precon",
                table: "ProjectMetaData",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectNormals_ProjectId",
                schema: "Precon",
                table: "ProjectNormals",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectWindData_ProjectId",
                schema: "Precon",
                table: "ProjectWindData",
                column: "ProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectMetaData",
                schema: "Precon");

            migrationBuilder.DropTable(
                name: "ProjectNormals",
                schema: "Precon");

            migrationBuilder.DropTable(
                name: "ProjectWindData",
                schema: "Precon");

            migrationBuilder.DropTable(
                name: "Project",
                schema: "Precon");
        }
    }
}
