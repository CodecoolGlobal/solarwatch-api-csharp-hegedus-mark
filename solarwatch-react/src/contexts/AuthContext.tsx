import {createContext, ReactNode, useContext, useState} from "react";
import {ILoginDetails, IRegisterDetails} from "../types.ts";
import {clearAccessToken, setAccessToken} from "../utils/accessTokenUtils.ts";
import {getUserRole} from "../utils/jwtHelpers.ts";
import {fetchLogin, fetchRegister} from "../utils/endpointHelpers.ts";

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
    register: (registerDetails: IRegisterDetails) => Promise<void>;
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
    register: async () => {
    },
    error: null,
}

const AuthContext = createContext<IAuthContext>(initialValue);

export const AuthProvider = ({children}: Props) => {

    const [authInfo, setAuthInfo] = useState<AuthInfo>(initialValue.authInfo);
    const [error, setError] = useState<string | null>(null);

    const login = async (loginDetails: ILoginDetails) => {
        setError(null)
        try {
            const result = await fetchLogin(loginDetails);

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

    const register = async (registerDetails: IRegisterDetails) => {
        setError(null)
        try {
            const result = await fetchRegister(registerDetails);
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

    return (
        <AuthContext.Provider value={{authInfo, login, logout, error, register}}>
            {children}
        </AuthContext.Provider>
    );
}

export const useAuth = () => useContext(AuthContext);