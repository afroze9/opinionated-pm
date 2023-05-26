import { ProjectSummaryResponseModel } from "./Project";
import { TagResponseModel } from "./Tag";

export type CompanyResponse = {
  id: number;
  name: string;
  projects: ProjectSummaryResponseModel[];
  tags: TagResponseModel[];
}

export type CompanyRequest = {
  name: string;
  tags: string[];
}

export type UpdateCompanyRequest = {
  id: number;
  name: string;
  // tags: string[];
}

export type CompanySummaryResponseModel = {
  id: number;
  name: string;
  projectCount: number;
  tags: string[];
}