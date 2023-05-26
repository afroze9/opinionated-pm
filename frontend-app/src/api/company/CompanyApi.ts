import { CompanyRequest, CompanyResponse, CompanySummaryResponseModel, UpdateCompanyRequest } from "../../@types";
import axios from 'axios';
import { ErrorResponse } from "../ErrorResponse";
import { getAxiosConfig, getUrl } from "../configs/axiosConfig";

const getCompanies = async (token: string): Promise<CompanySummaryResponseModel[] | ErrorResponse> => {
  const url = getUrl('/company');
  const config = getAxiosConfig(token);

  try {
    const response = await axios.get<CompanySummaryResponseModel[]>(url, config);
    return response.data;
  } catch (e) {
    console.error(e);
    return { message: (e as any).toString() };
  }
}

const getCompanyById = async (id: number, token: string): Promise<CompanyResponse | ErrorResponse> => {
  const url = getUrl(`/company/${id}`);
  const config = getAxiosConfig(token);

  try {
    const response = await axios.get<CompanyResponse>(url, config);
    return response.data;
  } catch (e) {
    console.error(e);
    return { message: (e as any).toString() };
  }
}

const createCompany = async (company: CompanyRequest, token: string): Promise<CompanyResponse | ErrorResponse> => {
  const url = getUrl('/company');
  const config = getAxiosConfig(token);

  try {
    const response = await axios.post<CompanyResponse>(url, company, config);
    return response.data;
  } catch (e) {
    console.error(e);
    return { message: (e as any).toString() };
  }
}

const updateCompany = async (id: number, company: UpdateCompanyRequest, token: string): Promise<CompanyResponse | ErrorResponse> => {
  const url = getUrl(`/company/${id}`);
  const config = getAxiosConfig(token);

  try {
    const response = await axios.put<CompanyResponse>(url, company, config);
    return response.data;
  } catch (e) {
    console.error(e);
    return { message: (e as any).toString() };
  }
}

const deleteCompany = async (companyId: number, token: string): Promise<void> => {
  const url = getUrl(`/company/${companyId}`);
  const config = getAxiosConfig(token);

  try {
    await axios.delete<CompanyResponse>(url, config);
  } catch (e) {
    console.error(e);
  }
}


const deleteCompanyTag = async (companyId: number, tagName: string, token: string): Promise<void> => {
  const url = getUrl(`/company/${companyId}/tag?tagName=${tagName}`);
  const config = getAxiosConfig(token);

  try {
    await axios.delete<CompanyResponse>(url, config);
  } catch (e) {
    console.error(e);
  }
}

export default { getCompanies, getCompanyById, createCompany, updateCompany, deleteCompany, deleteCompanyTag }
