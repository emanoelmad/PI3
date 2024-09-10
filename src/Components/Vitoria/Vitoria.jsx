import React from 'react';
import { useLocation } from 'react-router-dom';
import './Vitoria.css';

const Vitoria = () => {
    const location = useLocation();
    const { state } = location;
    const score = state?.score || 0;
    const incorrectAnswers = state?.incorrectAnswers || 0;

    return (
        <div className="vitoria-container">
            <h1 className="title">🎉 Parabéns! Você Venceu! 🎉</h1>
            <p className="message">Você conseguiu vencer o jogo. Ótimo trabalho!</p>
            <p className="score">Pontuação Final: R$ {score}</p>
            <p className="incorrect-answers">Número de Derrotas por Erro: {incorrectAnswers}</p>
            <a className="back-link" href="/">Voltar para a Tela Inicial</a>
        </div>
    );
};

export default Vitoria;
