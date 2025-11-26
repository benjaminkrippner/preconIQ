export default interface ProjectMetaData {
  id: number;
  address?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  county?: string;
  bidNumber?: string;
  contractType?: string;
  duration?: number;
  marketSegment?: string;
  opportunityDescription?: string;
  bidAsCompany?: string;
  isParentOpportunity: boolean;
  parentProjectId?: number;
  estimatedFinalContractRevenue?: number;
  marginPercent?: number;
  marginDollars?: number;
  megawatts?: number;
  revenueType?: string;
  revenue?: number;
  projectStartDate?: string;
  projectCompletionDate?: string;
  closeDate?: string;
  substantialCompletionDate?: string;
  latitude?: number;
  longitude?: number;
}
