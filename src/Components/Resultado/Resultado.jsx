import React from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import './Resultado.css';

const Resultado = () => {
    const navigate = useNavigate();
    const location = useLocation();
    const { score, incorrectAnswers } = location.state || { score: 0, incorrectAnswers: 0 };

    const handleBackToHome = () => {
        navigate('/');
    };

    return (
        <div className="resultado-container">
            <h1>Resultado</h1>
            <p>Parabéns! Você completou o jogo.</p>
            <p>Pontuação Final: R$ {score}</p>
            <p>Erros Cometidos: {incorrectAnswers}</p> {/* Mostra número de derrotas */}
            <button onClick={handleBackToHome}>Voltar para a Tela Inicial</button>
            <button onClick={() => navigate('/hallDaFama')}>Ver Hall da Fama</button>
        </div>
    );
};

export default Resultado;
