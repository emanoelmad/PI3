import React from 'react';
import { useNavigate } from 'react-router-dom';
import './Welcome.css';

const Welcome = ({ onLogout }) => {
    const navigate = useNavigate();

    const handleStartGame = () => {
        navigate('/perguntas');
    };

    const handleLogout = () => {
        onLogout();
        navigate('/');
    };

    const handleEditAccount = () => {
        navigate('/editarConta');
    };

    return (
        <div className="welcome-container">
            <h1>Bem-vindo ao Jogo!</h1>
            <div className="button-group">
                <button onClick={handleStartGame}>Iniciar Jogo</button>
                <button onClick={handleLogout}>Logout</button>
                <button onClick={handleEditAccount}>Editar Conta</button>
            </div>
        </div>
    );
};

export default Welcome;
