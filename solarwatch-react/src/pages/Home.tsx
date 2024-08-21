// @flow
import {useNavigate} from 'react-router-dom';


export const Home = () => {
    const navigate = useNavigate();

    const handleLoginClick = () => {
        navigate('/login');
    };

    return (
        <div>
            <h1>Home</h1>
            <button onClick={handleLoginClick}>Login</button>
        </div>
    );
};