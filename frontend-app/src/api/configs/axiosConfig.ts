import { AxiosRequestConfig } from "axios";
import { baseUrl } from "../../constants";

const getAxiosConfig = (token: string): AxiosRequestConfig<any> => {
  return {
    headers: {
      "content-type": "application/json",
      'Authorization': `bearer ${token}`
    },
  };
}

const getUrl = (path: string): string => {
  return `${baseUrl}${path}`;
}

export { getAxiosConfig, getUrl };
