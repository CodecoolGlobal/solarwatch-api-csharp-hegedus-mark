// src/pages/Login.tsx
import React, {useState} from 'react';
import {useAuth} from '../contexts/AuthContext.tsx';
import {ILoginDetails} from '../types.ts';
import {useNavigate} from "react-router-dom";

export const Login = () => {
    const {login, error, authInfo} = useAuth();
    const [loginDetails, setLoginDetails] = useState<ILoginDetails>({email: '', password: ''});
    const navigate = useNavigate();

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setLoginDetails({
            ...loginDetails,
            [e.target.name]: e.target.value,
        });
    };

    const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        await login(loginDetails);

        if (authInfo.authenticated) {
            navigate('/main');
        }
    };

    return (
        <div className="login-container">
            <h2>Login</h2>
            <form onSubmit={handleSubmit}>
                <div>
                    <label htmlFor="email">Username:</label>
                    <input
                        type="email"
                        name="email"
                        id="email"
                        value={loginDetails.email}
                        onChange={handleChange}
                        required
                    />
                </div>
                <div>
                    <label htmlFor="password">Password:</label>
                    <input
                        type="password"
                        name="password"
                        id="password"
                        value={loginDetails.password}
                        onChange={handleChange}
                        required
                    />
                </div>
                <button type="submit">Login</button>
                {error && <p className="error">{error}</p>}
            </form>
        </div>
    );
};
