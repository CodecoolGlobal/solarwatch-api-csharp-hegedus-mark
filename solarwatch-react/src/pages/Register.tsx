import React, {useState} from 'react';
import {useAuth} from '../contexts/AuthContext.tsx';
import {IRegisterDetails} from '../types.ts';
import {useNavigate} from "react-router-dom";


interface IRegisterFields {
    email: string;
    username: string;
    password: string;
    confirmPassword: string;
}

const initialState: IRegisterFields = {
    email: '',
    username: '',
    password: '',
    confirmPassword: ''
}


export const Register: React.FC = () => {
    const {register, error} = useAuth();
    const [registerFields, setRegisterFields] = useState<IRegisterFields>(initialState);
    const navigate = useNavigate();

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setRegisterFields({
            ...registerFields,
            [e.target.name]: e.target.value,
        });
    };

    const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        if (registerFields.password !== registerFields.confirmPassword) {
            alert("Passwords do not match!");
            return;
        }

        const registerDetails: IRegisterDetails = {
            email: registerFields.email,
            username: registerFields.username,
            password: registerFields.password,
        }

        await register(registerDetails);

        if(!error){
            navigate('/main');
        }
    };

    return (
        <div className="register-container">
            <h2>Register</h2>
            <form onSubmit={handleSubmit}>
                <div>
                    <label htmlFor="email">Email:</label>
                    <input
                        type="email"
                        name="email"
                        id="email"
                        value={registerFields.email}
                        onChange={handleChange}
                        required
                    />
                </div>
                <div>
                    <label htmlFor="username">Username:</label>
                    <input
                        type="text"
                        name="username"
                        id="username"
                        value={registerFields.username}
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
                        value={registerFields.password}
                        onChange={handleChange}
                        required
                    />
                </div>
                <div>
                    <label htmlFor="confirmPassword">Confirm Password:</label>
                    <input
                        type="password"
                        name="confirmPassword"
                        id="confirmPassword"
                        value={registerFields.confirmPassword}
                        onChange={handleChange}
                        required
                    />
                </div>
                <button type="submit">Register</button>
                {error && <p className="error">{error}</p>}
            </form>
        </div>
    );
};