import {API_ENDPOINTS} from "../../config.ts";
import {fetcher} from "./fetcher.ts";
import {ILoginDetails} from "../types.ts";

export interface ILoginResponse {
    email: string,
    userName: string,
    token: string
}

export interface ICityWithSunriseSunsetResponse {
    cityName: string,
    country: string,
    state: string,
    latitude: string,
    longitude: string,
    sunrise: string,
    sunset: string
}


export const fetchLogin = async (loginDetails: ILoginDetails): Promise<ILoginResponse> => {

    const options: RequestInit = {
        method: 'POST',
        body: JSON.stringify(loginDetails)
    }

    const url = API_ENDPOINTS.LOGIN;
    return await fetcher(url, options)
}

export const fetchSunriseSunset = async (cityName: string): Promise<ICityWithSunriseSunsetResponse[]> => {

    const url = API_ENDPOINTS.SUNRISE_SUNSET(cityName)
    return await fetcher(url)
}