import mockBoD from "./MockDataWindBoD.json";

/**
 * Flat representation of the Wind Basis of Design.
 *
 * NOTE:
 * - All fields are currently strings because the mock JSON uses strings.
 * - You can later migrate specific fields to `number`/`boolean` plus a parsing layer.
 */
export interface WindBoDFlat {
  OpportunityId: string;
  ProjectId: string;
  RunDate: string;
  SourceLocation: string;
  ProjectName: string;
  Owner: string;
  Location: string;
  MwacMw: string;
  InterconnectVoltageKv: string;
  NumberOfTurbines: string;
  TurbineOem: string;
  TurbineModel: string;
  TurbineMw: string;
  TurbineHubHeightFt: string;
  TurbineRotorDiameterM: string;
  OwnerSuppliedEquipment: string;
  TurbineLocations: string;
  SubstationLocation: string;
  TransmissionRoute: string;
  AltaSurvey: string;
  LandControl: string;
  EnvironmentalConstraints: string;
  Geotech: string;
  TopoSurveySurfaceLidar: string;
  InterconnectAgreement: string;
  Ppa: string;
  Hydrology: string;
  WetlandDelineation: string;
  DesignVehicleComponentDelivery: string;
  AccessRoadDesign: string;
  PublicRoadNetworkHaulRoute: string;
  TempRoadXsection: string;
  PermRoadXsection: string;
  HorizCurveMinRadiusFt: string;
  AccessRoadsWidthFt: string;
  VerticalCurveRadiusFt: string;
  MaxLongitudinalGradientPct: string;
  CrossSlopeAccessRoadPct: string;
  CrossSlopeCranePathPct: string;
  TurbineCenterpinOffsetFt: string;
  TemporaryIntersections: string;
  StandardTempRadiusFt: string;
  BumpOutDimensions: string;
  BumpOutIntervalFt: string;
  CraneWalkWidthFt: string;
  CrossCountryCranePath: string;
  AccessRoadAggregateGradation: string;
  TurbineTempDisturbanceAc: string;
  TurbineTopOfPadSizeFt: string;
  TurbineTopOfPadShape: string;
  CranePadSizeFt: string;
  GravelRingDimensionFt: string;
  EgSurface: string;
  GradingType: string;
  AccessRoadEarthworkCy: string;
  WtgPadsEarthworkCy: string;
  TopsoilOrganicDepthIn: string;
  LowWaterCrossings: string;
  Culverts: string;
  ApproachCulverts: string;
  MinPipeDiameterIn: string;
  DesignStormTurbineFlooding: string;
  DisturbanceRoadsWidthFt: string;
  DisturbanceCraneWalkWidthFt: string;
  DisturbanceSiteAc: string;
  DisturbanceGradingLimits: string;
  DisturbanceUgCollectionWidthFt: string;
  CoordinateSystem: string;
  CollTopsoilStrippedDepthIn: string;
  CollTopsoilStrippedWidthPerCctFt: string;
  CollClearingAcres: string;
  CollCulturalAvoidances: string;
  CollEnvironmentalConstraints: string;
  CollPipelineCrossings: string;
  CollGprs: string;
  CollSwpppRequirements: string;
  TrenchInstallationMethod: string;
  CableInstallationMethod: string;
  MvCableOrientation: string;
  TrenchDepthFt: string;
  TrenchDepthToTopOfCablesFt: string;
  TrenchBedding: string;
  TrenchWidthFt: string;
  TrenchBackfillLayer1: string;
  Layer1ScreenSizeIn: string;
  Layer1ScreeningMethod: string;
  Layer1CompactionMethod: string;
  TrenchBackfillLayer2: string;
  Layer2ScreenSizeIn: string;
  Layer2ScreeningMethod: string;
  Layer2CompactionMethod: string;
  TrenchCompactionPctStdProctor: string;
  MaxLiftIn: string;
  CompactionTestingFrequency: string;
  AssumedRockPct: string;
  TerrainIssues: string;
  CarsoniteMarkers: string;
  MarkerBalls: string;
  NativeRho: string;
  DryRho: string;
  MaxMvCableSizeMcm: string;
  MaxLoadingAt90cAmp: string;
  MaxSegmentLengthWithTailsFt: string;
  TailLengthOneSideFt: string;
  MaxTurbinesPerFeeder: string;
  FeederCableSizeSpec: string;
  Grounding: string;
  Crossbonding: string;
  MidSpanGrounding: string;
  MvConductorScWithstandKa: string;
  ConcentricNeutralScWithstandKa: string;
  TrenchGroundScWithstandKa: string;
  MvCableVoltageKv: string;
  MvCableConductor: string;
  MvCableConductorShield: string;
  MvCableInsulation: string;
  MvCableInsulationShield: string;
  MvCableMetallicShield: string;
  MvCableJacket: string;
  MvCableMaxContTempC: string;
  MvCableEmergencyTempC: string;
  MvCableApprovedVendors: string;
  SingleModeFiber: string;
  Innerduct: string;
  InnerductPreloadedWithFiber: string;
  FiberApprovedVendors: string;
  BilKv: string;
}

const MOCK_NETWORK_DELAY_MS = 150;

/**
 * Simple async delay helper to simulate network latency.
 */
function delay(ms: number): Promise<void> {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

/**
 * Fetch Basis of Design data for a project.
 *
 * Currently:
 *   - returns local MockDataWindBoD.json
 *   - simulates a small async delay
 *
 * Later:
 *   - replace with real API call and validation
 */
export async function fetchBasisOfDesign(
  projectId?: string
): Promise<WindBoDFlat> {
  // simulate latency
  await delay(MOCK_NETWORK_DELAY_MS);

  // projectId unused for now in mock mode
  return mockBoD as WindBoDFlat;
}
