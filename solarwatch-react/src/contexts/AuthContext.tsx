import {createContext, ReactNode, useContext, useState} from "react";
import {ILoginDetails} from "../types.ts";
import {clearAccessToken, setAccessToken} from "../utils/accessTokenUtils.ts";
import {fetcher} from "../utils/fetcher.ts";
import {getUserRole} from "../utils/jwtHelpers.ts";

type Props = {
    children?: ReactNode;
}

type AuthInfo = {
    authenticated: boolean;
    userRole: string | null;
}

type IAuthContext = {
    authInfo: AuthInfo;
     login: (loginDetails: ILoginDetails) => Promise<void>;
    logout: () => void;
    error: string | null;
}

const initialValue: IAuthContext = {
    authInfo: {
        authenticated: false,
        userRole: null
    },
    login: async () => {
    },
    logout: () => {
    },
    error: null,
}

const AuthContext = createContext<IAuthContext>(initialValue);

export const AuthProvider = ({children}: Props) => {

    const [authInfo, setAuthInfo] = useState<AuthInfo>(initialValue.authInfo);
    const [error, setError] = useState<string | null>(null);

    const login = async (loginDetails: ILoginDetails) => {

        const options: RequestInit = {
            method: "POST",
            body: JSON.stringify(loginDetails),
        }

        try {
            const result = await fetcher('LOGIN', options);

            setAccessToken(result.token);
            setAuthInfo({
                authenticated: true,
                userRole: getUserRole(result.token)
            });

        } catch (error) {
            console.log(error);
            setError("Failed to login");
        }

    }

    const logout = () => {
        setAuthInfo({
            authenticated: false,
            userRole: null
        });
        clearAccessToken();
    }

    return (
        <AuthContext.Provider value={{authInfo, login, logout, error}}>
            {children}
        </AuthContext.Provider>
    );
}

export const useAuth = () => useContext(AuthContext);