import React from 'react';
import { useLocation } from 'react-router-dom';
import './Derrota.css';

const Derrota = () => {
    const location = useLocation();
    const { state } = location;
    const score = state?.score || 0;
    const incorrectAnswers = state?.incorrectAnswers || 0;

    return (
        <div className="derrota-container">
            <h1 className="title">💔 Você Perdeu! 💔</h1>
            <p className="message">Infelizmente, você não conseguiu vencer o jogo desta vez.</p>
            <p className="score">Pontuação Final: R$ {score}</p>
            <p className="incorrect-answers">Número de Derrotas por Erro: {incorrectAnswers}</p>
            <a className="back-link" href="/">Voltar para a Tela Inicial</a>
        </div>
    );
};

export default Derrota;
