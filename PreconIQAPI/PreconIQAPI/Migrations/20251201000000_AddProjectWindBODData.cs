using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PreconIQAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectWindBODData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Precon");

            migrationBuilder.CreateTable(
                name: "ProjectWindBODData",
                schema: "Precon",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    OpportunityId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    SourceLocation = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    RunDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProjectName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Owner = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MwacMw = table.Column<decimal>(type: "decimal(10, 2)", nullable: true),
                    InterconnectVoltageKv = table.Column<int>(type: "int", nullable: true),
                    NumberOfTurbines = table.Column<int>(type: "int", nullable: true),
                    TurbineOem = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TurbineModel = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TurbineMw = table.Column<decimal>(type: "decimal(10, 2)", nullable: true),
                    TurbineHubHeightFt = table.Column<int>(type: "int", nullable: true),
                    TurbineRotorDiameterM = table.Column<int>(type: "int", nullable: true),
                    OwnerSuppliedEquipment = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TurbineLocations = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SubstationLocation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TransmissionRoute = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AltaSurvey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LandControl = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EnvironmentalConstraints = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Geotech = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TopoSurveySurfaceLidar = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InterconnectAgreement = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Ppa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Hydrology = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    WetlandDelineation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DesignVehicleComponentDelivery = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AccessRoadDesign = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PublicRoadNetworkHaulRoute = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TempRoadXsection = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PermRoadXsection = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    HorizCurveMinRadiusFt = table.Column<int>(type: "int", nullable: true),
                    AccessRoadsWidthFt = table.Column<int>(type: "int", nullable: true),
                    VerticalCurveRadiusFt = table.Column<int>(type: "int", nullable: true),
                    MaxLongitudinalGradientPct = table.Column<decimal>(type: "decimal(5, 2)", nullable: true),
                    CrossSlopeAccessRoadPct = table.Column<decimal>(type: "decimal(5, 2)", nullable: true),
                    CrossSlopeCranePathPct = table.Column<decimal>(type: "decimal(5, 2)", nullable: true),
                    TurbineCenterpinOffsetFt = table.Column<int>(type: "int", nullable: true),
                    TemporaryIntersections = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StandardTempRadiusFt = table.Column<int>(type: "int", nullable: true),
                    BumpOutDimensions = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BumpOutIntervalFt = table.Column<int>(type: "int", nullable: true),
                    CraneWalkWidthFt = table.Column<int>(type: "int", nullable: true),
                    CrossCountryCranePath = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AccessRoadAggregateGradation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TurbineTempDisturbanceAc = table.Column<decimal>(type: "decimal(10, 2)", nullable: true),
                    TurbineTopOfPadSizeFt = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TurbineTopOfPadShape = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CranePadSizeFt = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GravelRingDimensionFt = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EgSurface = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GradingType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AccessRoadEarthworkCy = table.Column<int>(type: "int", nullable: true),
                    WtgPadsEarthworkCy = table.Column<int>(type: "int", nullable: true),
                    TopsoilOrganicDepthIn = table.Column<int>(type: "int", nullable: true),
                    LowWaterCrossings = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Culverts = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ApproachCulverts = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MinPipeDiameterIn = table.Column<int>(type: "int", nullable: true),
                    DesignStormTurbineFlooding = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DisturbanceRoadsWidthFt = table.Column<int>(type: "int", nullable: true),
                    DisturbanceCraneWalkWidthFt = table.Column<int>(type: "int", nullable: true),
                    DisturbanceSiteAc = table.Column<decimal>(type: "decimal(10, 2)", nullable: true),
                    DisturbanceGradingLimits = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DisturbanceUgCollectionWidthFt = table.Column<int>(type: "int", nullable: true),
                    CoordinateSystem = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CollTopsoilStrippedDepthIn = table.Column<int>(type: "int", nullable: true),
                    CollTopsoilStrippedWidthPerCctFt = table.Column<int>(type: "int", nullable: true),
                    CollClearingAcres = table.Column<decimal>(type: "decimal(10, 2)", nullable: true),
                    CollCulturalAvoidances = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CollEnvironmentalConstraints = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CollPipelineCrossings = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CollGprs = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CollSwpppRequirements = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TrenchInstallationMethod = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CableInstallationMethod = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MvCableOrientation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TrenchDepthFt = table.Column<decimal>(type: "decimal(10, 2)", nullable: true),
                    TrenchDepthToTopOfCablesFt = table.Column<decimal>(type: "decimal(10, 2)", nullable: true),
                    TrenchBedding = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TrenchWidthFt = table.Column<decimal>(type: "decimal(10, 2)", nullable: true),
                    TrenchBackfillLayer1 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Layer1ScreenSizeIn = table.Column<decimal>(type: "decimal(10, 2)", nullable: true),
                    Layer1ScreeningMethod = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Layer1CompactionMethod = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TrenchBackfillLayer2 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Layer2ScreenSizeIn = table.Column<decimal>(type: "decimal(10, 2)", nullable: true),
                    Layer2ScreeningMethod = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Layer2CompactionMethod = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TrenchCompactionPctStdProctor = table.Column<decimal>(type: "decimal(5, 2)", nullable: true),
                    MaxLiftIn = table.Column<int>(type: "int", nullable: true),
                    CompactionTestingFrequency = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AssumedRockPct = table.Column<decimal>(type: "decimal(5, 2)", nullable: true),
                    TerrainIssues = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CarsoniteMarkers = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MarkerBalls = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NativeRho = table.Column<decimal>(type: "decimal(10, 2)", nullable: true),
                    DryRho = table.Column<decimal>(type: "decimal(10, 2)", nullable: true),
                    MaxMvCableSizeMcm = table.Column<int>(type: "int", nullable: true),
                    MaxLoadingAt90cAmp = table.Column<int>(type: "int", nullable: true),
                    MaxSegmentLengthWithTailsFt = table.Column<int>(type: "int", nullable: true),
                    TailLengthOneSideFt = table.Column<int>(type: "int", nullable: true),
                    MaxTurbinesPerFeeder = table.Column<int>(type: "int", nullable: true),
                    FeederCableSizeSpec = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Grounding = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Crossbonding = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MidSpanGrounding = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MvConductorScWithstandKa = table.Column<decimal>(type: "decimal(10, 2)", nullable: true),
                    ConcentricNeutralScWithstandKa = table.Column<decimal>(type: "decimal(10, 2)", nullable: true),
                    TrenchGroundScWithstandKa = table.Column<decimal>(type: "decimal(10, 2)", nullable: true),
                    MvCableVoltageKv = table.Column<decimal>(type: "decimal(10, 2)", nullable: true),
                    MvCableConductor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MvCableConductorShield = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MvCableInsulation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MvCableInsulationShield = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MvCableMetallicShield = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MvCableJacket = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MvCableMaxContTempC = table.Column<int>(type: "int", nullable: true),
                    MvCableEmergencyTempC = table.Column<int>(type: "int", nullable: true),
                    MvCableApprovedVendors = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SingleModeFiber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Innerduct = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InnerductPreloadedWithFiber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FiberApprovedVendors = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BilKv = table.Column<decimal>(type: "decimal(10, 2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectWindBODData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectWindBODData_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Precon",
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectWindBODData_ProjectId",
                schema: "Precon",
                table: "ProjectWindBODData",
                column: "ProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectWindBODData",
                schema: "Precon");
        }
    }
}
