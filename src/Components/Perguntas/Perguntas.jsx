import { useState, useEffect, useContext } from 'react';
import { useNavigate } from 'react-router-dom';
import './Perguntas.css';
import { AuthContext } from "../../Context/AuthContext";

const Perguntas = () => {
    const [pergunta, setPergunta] = useState(null);
    //const [selectedOption, setSelectedOption] = useState('');
    const [message, setMessage] = useState('');
    const [score, setScore] = useState(0);
    const [isGameOver, setIsGameOver] = useState(false);
    const navigate = useNavigate();
    const { partidaId, setPontuacao } = useContext(AuthContext);
    const [vitoria, setVitoria] = useState(0);

    //const { setUserId } = useContext(AuthContext);

    const fetchPerguntaAtual = async () => {
        try {
            const response = await fetch(`https://localhost:7183/api/partida/perguntaAtual/${partidaId}`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                },
                credentials: 'include',
            });

            if (!response.ok) {
                const errorData = await response.json();
                console.error('Erro ao obter a pergunta atual:', errorData);
                return;
            }

            const perguntaData = await response.json();

            setPergunta(perguntaData.data); // Define a pergunta no estado
        } catch (error) {
            console.error('Erro ao conectar ao servidor:', error);
        }
    };

    useEffect(() => {
        fetchPerguntaAtual(); // Chama a função para obter a pergunta ao carregar a tela
    }, [partidaId]);

    const handleAnswer = (option) => {
        if (pergunta && pergunta.respostaCorreta === option) {
            setMessage('Você acertou!');
            setScore(prevScore => prevScore + 1000); // Pontuação fictícia por acerto
            setVitoria(prevVitoria => prevVitoria + 1);
            console.log(vitoria);
            if (vitoria == 7) {
                setPontuacao(score);
                navigate("/vitoria");
                return;
            }
            fetchPerguntaAtual(); // Chama a função para obter a pergunta ao carregar a tela
        } else {
            setMessage('Você errou!');
            setIsGameOver(true); // Finaliza o jogo em caso de erro
            setPontuacao(score);
            navigate("/derrota");
        }
    };

    const handleLogout = () => {
        navigate('/'); // Redireciona para a tela de login
    };

    return (
        <div className="perguntas-container">
            <h2>Pontuação: R$ {score}</h2> {/* Exibe a pontuação atual */}
            {pergunta ? (
                <>
                    <h2>Pergunta</h2>
                    <p>{pergunta.enunciado}</p>
                    <div className="options">
                        <button onClick={() => handleAnswer(pergunta.alternativaA)}>{pergunta.alternativaA}</button>
                        <button onClick={() => handleAnswer(pergunta.alternativaB)}>{pergunta.alternativaB}</button>
                        <button onClick={() => handleAnswer(pergunta.alternativaC)}>{pergunta.alternativaC}</button>
                        <button onClick={() => handleAnswer(pergunta.alternativaD)}>{pergunta.alternativaD}</button>
                    </div>
                </>
            ) : (
                <p>Carregando pergunta...</p>
            )}
            <div className="button-group">
                <button onClick={handleLogout}>Logout</button>
            </div>
            {message && <div className="message">{message}</div>}
        </div>
    );
};

export default Perguntas;
