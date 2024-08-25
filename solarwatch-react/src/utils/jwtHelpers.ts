import {jwtDecode} from 'jwt-decode';

type DecodedToken = {
    sub: string;
    role: string;
}

export const getUserRole = (token: string) => {
    if (token) {
        const decodedToken = jwtDecode<DecodedToken>(token);
        return decodedToken.role;
    }
    return null;
}