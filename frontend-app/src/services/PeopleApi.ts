import ApiHelpers, { type ErrorResponse } from "./ApiHelpers";
import {get} from "svelte/store";
import {auth0Client} from "../store";
import axios from "axios";

export type PersonResponseModel = {
    id: number;
    name: string;
    email: string;
}

async function getPeople(): Promise<PersonResponseModel[] | ErrorResponse> {
    try {
        const token = await get(auth0Client).getTokenSilently();
        const url = ApiHelpers.getUrl('/people');
        const config = ApiHelpers.getAxiosConfig(token);
        const response = await axios.get<PersonResponseModel[]>(url, config);
        return response.data;
    } catch (e) {
        return {
            message: (e as any).toString()
        };
    }
}

const peopleApi = {
    getPeople
}

export default peopleApi;