import ApiHelpers, { type ErrorResponse } from "./ApiHelpers";
import { get } from "svelte/store";
import { auth0Client } from "../store";
import axios, { Axios, AxiosError } from "axios";

export type PersonResponseModel = {
    id: number;
    identityId: number;
    name: string;
    email: string;
}

export type PersonRequest = {
    name: string;
    email: string;
    password: string;
}

export type UpdatePersonrequest = {
    id: number;
    name?: string;
    email?: string;
}


async function getPeople(): Promise<PersonResponseModel[] | ErrorResponse> {
    try {
        const token = await get(auth0Client).getTokenSilently();
        const url = ApiHelpers.getUrl('/people');
        const config = ApiHelpers.getAxiosConfig(token);
        const response = await axios.get<PersonResponseModel[]>(url, config);
        return response.data;
    } catch (e) {
        let error = e as AxiosError;
        if (error.response?.status == 404) {
            return [];
        }
        return {
            message: (e as any).toString()
        };
    }
}

async function search(name: string): Promise<PersonResponseModel[] | ErrorResponse> {
    try {
        const token = await get(auth0Client).getTokenSilently();
        const url = ApiHelpers.getUrl(`/people/search?name=${name}`);
        const config = ApiHelpers.getAxiosConfig(token);
        const response = await axios.get<PersonResponseModel[]>(url, config);
        return response.data;
    } catch (e) {
        let error = e as AxiosError;
        if (error.response?.status == 404) {
            return [];
        }
        return {
            message: (e as any).toString()
        };
    }
}

async function getPersonById(id: number): Promise<PersonResponseModel | ErrorResponse> {
    try {
        const token = await get(auth0Client).getTokenSilently();
        const url = ApiHelpers.getUrl(`/people/${id}`);
        const config = ApiHelpers.getAxiosConfig(token);
        const response = await axios.get<PersonResponseModel>(url, config);
        return response.data;
    } catch (e) {
        return {
            message: (e as any).toString()
        };
    }
}

async function createPerson(person: PersonRequest) {
    try {
        const token = await get(auth0Client).getTokenSilently();
        const url = ApiHelpers.getUrl(`/people`);
        const config = ApiHelpers.getAxiosConfig(token);
        const response = await axios.post<PersonRequest>(url, person, config);
        return response.data;
    } catch (e) {
        return {
            message: (e as any).toString()
        };
    }
}

async function updatePerson(id: number, company: UpdatePersonrequest) {
    try {
        const token = await get(auth0Client).getTokenSilently();
        const url = ApiHelpers.getUrl(`/people/${id}`);
        const config = ApiHelpers.getAxiosConfig(token);
        const response = await axios.put<PersonResponseModel>(url, company, config);
        return response.data;
    } catch (e) {
        return {
            message: (e as any).toString()
        };
    }
}

async function deletePerson(id: number) {
    try {
        const token = await get(auth0Client).getTokenSilently();
        const url = ApiHelpers.getUrl(`/people/${id}`);
        const config = ApiHelpers.getAxiosConfig(token);
        const response = await axios.delete(url, config);
        return response.data;
    } catch (e) {
        return {
            message: (e as any).toString()
        };
    }
}

const peopleApi = {
    getPeople,
    search,
    getPersonById,
    createPerson,
    updatePerson,
    deletePerson
}

export default peopleApi;