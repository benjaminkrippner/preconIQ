// types/Project.ts (example)


export default interface Project {
  id: string;
  opportunityId: string;
  opportunityName?: string;
  opportunityLegalContractProjectName?: string;
  opportunityStageName?: string;
  projectNumber?: number;
  accountName?: string;
  isWon: boolean;
  isClosed: boolean;
  isActive: boolean;
  isDeleted: boolean;
}