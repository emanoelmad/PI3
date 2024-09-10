import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import './HallDaFama.css';

// Simulação de dados. No futuro, você irá buscar os dados do backend
const initialPlayers = [
    { name: "Jogador 1", score: "R$ 1.000.000", correctAnswers: 7, prize: "R$ 1.000.000" },
    { name: "Jogador 2", score: "R$ 500.000", correctAnswers: 6, prize: "R$ 500.000" },
    { name: "Jogador 3", score: "R$ 100.000", correctAnswers: 4, prize: "R$ 100.000" },
];

const HallDaFama = () => {
    const [players, setPlayers] = useState(initialPlayers);
    const navigate = useNavigate();

    const handleBackToWelcome = () => {
        navigate('/welcome'); // Redireciona para a tela de boas-vindas
    };

    return (
        <div className="hallDaFama-container">
            <h1>Hall da Fama</h1>
            <ul>
                {players.length > 0 ? (
                    players.map((player, index) => (
                        <li key={index}>
                            {index + 1}. {player.name} - Pontuação: {player.score} - Acertos: {player.correctAnswers} - Prêmio: {player.prize}
                        </li>
                    ))
                ) : (
                    <li>Nenhum jogador registrado ainda.</li>
                )}
            </ul>
            <button onClick={handleBackToWelcome}>Voltar à Tela de Boas-Vindas</button>
        </div>
    );
};

export default HallDaFama;
