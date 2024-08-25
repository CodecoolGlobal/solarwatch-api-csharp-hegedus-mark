import React from "react";
import {useNavigate} from 'react-router-dom';
import {HomeContainer, Title, StyledButton} from './Home.styles';



export const Home: React.FC = () => {
    const navigate = useNavigate();

    const handleLoginClick = () => {
        navigate('/login');
    };
    const handleRegisterClick = () => {
        navigate('/register');
    };

    return (
        <HomeContainer>
            <Title>Home</Title>
            <StyledButton onClick={handleLoginClick}>Login</StyledButton>
            <StyledButton onClick={handleRegisterClick}>Register</StyledButton>
        </HomeContainer>
    );
};