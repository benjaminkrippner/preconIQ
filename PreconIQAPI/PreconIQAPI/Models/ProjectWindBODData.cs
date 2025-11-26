using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PreconIQAPI.Models
{
    /// <summary>
    /// Represents detailed wind basis of design data associated with a project.
    /// </summary>
    public class ProjectWindBODData
    {
        public int Id { get; set; }

        /// <summary>
        /// Foreign key to the project entity.
        /// </summary>
        public int ProjectId { get; set; }

        [ForeignKey(nameof(ProjectId))]
        public Project Project { get; set; } = null!;

        [StringLength(64)]
        public string? OpportunityId { get; set; }

        [StringLength(2048)]
        public string? SourceLocation { get; set; }

        public DateTime? RunDate { get; set; }

        [StringLength(255)]
        public string? ProjectName { get; set; }

        [StringLength(255)]
        public string? Owner { get; set; }

        [StringLength(100)]
        public string? Location { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? MwacMw { get; set; }

        public int? InterconnectVoltageKv { get; set; }

        public int? NumberOfTurbines { get; set; }

        [StringLength(100)]
        public string? TurbineOem { get; set; }

        [StringLength(100)]
        public string? TurbineModel { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? TurbineMw { get; set; }

        public int? TurbineHubHeightFt { get; set; }

        public int? TurbineRotorDiameterM { get; set; }

        [StringLength(100)]
        public string? OwnerSuppliedEquipment { get; set; }

        [StringLength(100)]
        public string? TurbineLocations { get; set; }

        [StringLength(100)]
        public string? SubstationLocation { get; set; }

        [StringLength(100)]
        public string? TransmissionRoute { get; set; }

        [StringLength(100)]
        public string? AltaSurvey { get; set; }

        [StringLength(100)]
        public string? LandControl { get; set; }

        [StringLength(100)]
        public string? EnvironmentalConstraints { get; set; }

        [StringLength(100)]
        public string? Geotech { get; set; }

        [StringLength(100)]
        public string? TopoSurveySurfaceLidar { get; set; }

        [StringLength(100)]
        public string? InterconnectAgreement { get; set; }

        [StringLength(100)]
        public string? Ppa { get; set; }

        [StringLength(100)]
        public string? Hydrology { get; set; }

        [StringLength(100)]
        public string? WetlandDelineation { get; set; }

        [StringLength(100)]
        public string? DesignVehicleComponentDelivery { get; set; }

        [StringLength(100)]
        public string? AccessRoadDesign { get; set; }

        [StringLength(100)]
        public string? PublicRoadNetworkHaulRoute { get; set; }

        [StringLength(100)]
        public string? TempRoadXsection { get; set; }

        [StringLength(100)]
        public string? PermRoadXsection { get; set; }

        public int? HorizCurveMinRadiusFt { get; set; }

        public int? AccessRoadsWidthFt { get; set; }

        public int? VerticalCurveRadiusFt { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? MaxLongitudinalGradientPct { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? CrossSlopeAccessRoadPct { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? CrossSlopeCranePathPct { get; set; }

        public int? TurbineCenterpinOffsetFt { get; set; }

        [StringLength(100)]
        public string? TemporaryIntersections { get; set; }

        public int? StandardTempRadiusFt { get; set; }

        [StringLength(100)]
        public string? BumpOutDimensions { get; set; }

        public int? BumpOutIntervalFt { get; set; }

        public int? CraneWalkWidthFt { get; set; }

        [StringLength(100)]
        public string? CrossCountryCranePath { get; set; }

        [StringLength(100)]
        public string? AccessRoadAggregateGradation { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? TurbineTempDisturbanceAc { get; set; }

        [StringLength(100)]
        public string? TurbineTopOfPadSizeFt { get; set; }

        [StringLength(100)]
        public string? TurbineTopOfPadShape { get; set; }

        [StringLength(100)]
        public string? CranePadSizeFt { get; set; }

        [StringLength(100)]
        public string? GravelRingDimensionFt { get; set; }

        [StringLength(100)]
        public string? EgSurface { get; set; }

        [StringLength(100)]
        public string? GradingType { get; set; }

        public int? AccessRoadEarthworkCy { get; set; }

        public int? WtgPadsEarthworkCy { get; set; }

        public int? TopsoilOrganicDepthIn { get; set; }

        [StringLength(100)]
        public string? LowWaterCrossings { get; set; }

        [StringLength(100)]
        public string? Culverts { get; set; }

        [StringLength(100)]
        public string? ApproachCulverts { get; set; }

        public int? MinPipeDiameterIn { get; set; }

        [StringLength(100)]
        public string? DesignStormTurbineFlooding { get; set; }

        public int? DisturbanceRoadsWidthFt { get; set; }

        public int? DisturbanceCraneWalkWidthFt { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? DisturbanceSiteAc { get; set; }

        [StringLength(100)]
        public string? DisturbanceGradingLimits { get; set; }

        public int? DisturbanceUgCollectionWidthFt { get; set; }

        [StringLength(100)]
        public string? CoordinateSystem { get; set; }

        public int? CollTopsoilStrippedDepthIn { get; set; }

        public int? CollTopsoilStrippedWidthPerCctFt { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? CollClearingAcres { get; set; }

        [StringLength(100)]
        public string? CollCulturalAvoidances { get; set; }

        [StringLength(100)]
        public string? CollEnvironmentalConstraints { get; set; }

        [StringLength(100)]
        public string? CollPipelineCrossings { get; set; }

        [StringLength(100)]
        public string? CollGprs { get; set; }

        [StringLength(100)]
        public string? CollSwpppRequirements { get; set; }

        [StringLength(100)]
        public string? TrenchInstallationMethod { get; set; }

        [StringLength(100)]
        public string? CableInstallationMethod { get; set; }

        [StringLength(100)]
        public string? MvCableOrientation { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? TrenchDepthFt { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? TrenchDepthToTopOfCablesFt { get; set; }

        [StringLength(100)]
        public string? TrenchBedding { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? TrenchWidthFt { get; set; }

        [StringLength(100)]
        public string? TrenchBackfillLayer1 { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? Layer1ScreenSizeIn { get; set; }

        [StringLength(100)]
        public string? Layer1ScreeningMethod { get; set; }

        [StringLength(100)]
        public string? Layer1CompactionMethod { get; set; }

        [StringLength(100)]
        public string? TrenchBackfillLayer2 { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? Layer2ScreenSizeIn { get; set; }

        [StringLength(100)]
        public string? Layer2ScreeningMethod { get; set; }

        [StringLength(100)]
        public string? Layer2CompactionMethod { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? TrenchCompactionPctStdProctor { get; set; }

        public int? MaxLiftIn { get; set; }

        [StringLength(100)]
        public string? CompactionTestingFrequency { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? AssumedRockPct { get; set; }

        [StringLength(100)]
        public string? TerrainIssues { get; set; }

        [StringLength(100)]
        public string? CarsoniteMarkers { get; set; }

        [StringLength(100)]
        public string? MarkerBalls { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? NativeRho { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? DryRho { get; set; }

        public int? MaxMvCableSizeMcm { get; set; }

        public int? MaxLoadingAt90cAmp { get; set; }

        public int? MaxSegmentLengthWithTailsFt { get; set; }

        public int? TailLengthOneSideFt { get; set; }

        public int? MaxTurbinesPerFeeder { get; set; }

        [StringLength(100)]
        public string? FeederCableSizeSpec { get; set; }

        [StringLength(100)]
        public string? Grounding { get; set; }

        [StringLength(100)]
        public string? Crossbonding { get; set; }

        [StringLength(100)]
        public string? MidSpanGrounding { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? MvConductorScWithstandKa { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? ConcentricNeutralScWithstandKa { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? TrenchGroundScWithstandKa { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? MvCableVoltageKv { get; set; }

        [StringLength(100)]
        public string? MvCableConductor { get; set; }

        [StringLength(100)]
        public string? MvCableConductorShield { get; set; }

        [StringLength(100)]
        public string? MvCableInsulation { get; set; }

        [StringLength(100)]
        public string? MvCableInsulationShield { get; set; }

        [StringLength(100)]
        public string? MvCableMetallicShield { get; set; }

        [StringLength(100)]
        public string? MvCableJacket { get; set; }

        public int? MvCableMaxContTempC { get; set; }

        public int? MvCableEmergencyTempC { get; set; }

        [StringLength(100)]
        public string? MvCableApprovedVendors { get; set; }

        [StringLength(100)]
        public string? SingleModeFiber { get; set; }

        [StringLength(100)]
        public string? Innerduct { get; set; }

        [StringLength(100)]
        public string? InnerductPreloadedWithFiber { get; set; }

        [StringLength(100)]
        public string? FiberApprovedVendors { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? BilKv { get; set; }
    }
}
