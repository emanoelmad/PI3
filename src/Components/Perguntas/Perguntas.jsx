import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import './Perguntas.css';

const questions = [
    { question: "O que é um sistema operacional?", options: ["Um programa que gerencia hardware e software no computador.", "Um software para editar textos.", "Um aplicativo para acessar a internet.", "Um software para criar apresentações."], answer: "Um programa que gerencia hardware e software no computador." },
    { question: "O que significa HTTP?", options: ["HyperText Transfer Protocol", "Hyperlink Text Transfer Protocol", "HighText Transfer Protocol", "HyperText Transfer Process"], answer: "HyperText Transfer Protocol" },
    { question: "Qual das seguintes opções representa um banco de dados relacional?", options: ["MySQL", "JavaScript", "HTML", "Windows"], answer: "MySQL" },
    { question: "Qual das opções abaixo é uma linguagem de programação?", options: ["HTTP", "Python", "DNS", "HTML"], answer: "Python" },
    { question: "O que é RAM?", options: ["Tipo de processador", "Sistema operacional", "Placa de vídeo", "Memória de acesso rápido"], answer: "Memória de acesso rápido" },
    { question: "O que é um loop?", options: ["Um erro de código", "Um laço de repetição", "Um tipo de memória", "Um dispositivo de entrada"], answer: "Um laço de repetição" },
    { question: "O que é API?", options: ["Interface de programação de aplicações", "Programa de edição de textos", "Sistema operacional", "Protocolo de segurança"], answer: "Interface de programação de aplicações" }
];

const prizeTable = {
    1: { correct: 1000, stop: 0, wrong: 0 },
    2: { correct: 5000, stop: 1000, wrong: 500 },
    3: { correct: 50000, stop: 5000, wrong: 2500 },
    4: { correct: 100000, stop: 50000, wrong: 25000 },
    5: { correct: 300000, stop: 100000, wrong: 50000 },
    6: { correct: 500000, stop: 300000, wrong: 150000 },
    7: { correct: 1000000, stop: 500000, wrong: 0 }
};

const Perguntas = () => {
    const [currentQuestion, setCurrentQuestion] = useState(0);
    const [selectedOption, setSelectedOption] = useState('');
    const [message, setMessage] = useState('');
    const [score, setScore] = useState(0);
    const [isGameOver, setIsGameOver] = useState(false);
    const [eliminatedOptions, setEliminatedOptions] = useState([]);
    const [incorrectAnswers, setIncorrectAnswers] = useState(0);
    const navigate = useNavigate();

    const handleAnswer = (option) => {
        if (questions[currentQuestion].answer === option) {
            setMessage('Você acertou!');
            setScore(prevScore => prevScore + prizeTable[currentQuestion + 1].correct);
        } else {
            setMessage('Você errou!');
            setScore(prevScore => prevScore + prizeTable[currentQuestion + 1].wrong);
            setIncorrectAnswers(prevIncorrectAnswers => prevIncorrectAnswers + 1);
        }

        if (currentQuestion < questions.length - 1) {
            setCurrentQuestion(currentQuestion + 1);
            setEliminatedOptions([]);
        } else {
            setIsGameOver(true);
        }
    };

    const handleStop = () => {
        const prize = prizeTable[currentQuestion + 1]?.stop || 0;
        setScore(prevScore => prevScore + prize);
        setMessage(`Você parou o jogo e ganhou R$ ${prize}`);
        setIsGameOver(true);
    };

    const handleEliminateOptions = () => {
        if (eliminatedOptions.length < 2) {
            const incorrectOptions = questions[currentQuestion].options.filter(option => option !== questions[currentQuestion].answer);
            const randomIndexes = [];

            while (randomIndexes.length < 2) {
                const randomIndex = Math.floor(Math.random() * incorrectOptions.length);
                if (!randomIndexes.includes(randomIndex)) {
                    randomIndexes.push(randomIndex);
                }
            }

            const newEliminatedOptions = randomIndexes.map(index => incorrectOptions[index]);
            setEliminatedOptions(prevEliminatedOptions => [...prevEliminatedOptions, ...newEliminatedOptions]);
        }
    };

    const handleLogout = () => {
        navigate('/'); // Redireciona para a tela de login
    };

    useEffect(() => {
        if (isGameOver) {
            if (score >= 1000000) {
                navigate('/vitoria', { state: { score, incorrectAnswers } });
            } else {
                navigate('/derrota', { state: { score, incorrectAnswers } });
            }
        }
    }, [isGameOver, score, navigate, incorrectAnswers]);

    return (
        <div className="perguntas-container">
            <h2>Pontuação: R$ {score}</h2> {/* Exibe a pontuação atual */}
            <h2>Pergunta {currentQuestion + 1}</h2>
            <p>{questions[currentQuestion].question}</p>
            <div className="options">
                {questions[currentQuestion].options
                    .filter(option => !eliminatedOptions.includes(option))
                    .map((option, index) => (
                        <button key={index} onClick={() => handleAnswer(option)}>
                            {option}
                        </button>
                    ))}
            </div>
            <div className="button-group">
                <button onClick={handleStop}>Parar</button>
                <button onClick={handleLogout}>Logout</button>
                <button onClick={handleEliminateOptions} disabled={eliminatedOptions.length >= 2}>Eliminar Duas Respostas Erradas</button>
            </div>
            {message && <div className="message">{message}</div>}
        </div>
    );
};

export default Perguntas;
