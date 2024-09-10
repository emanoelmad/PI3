// src/Components/Cadastro/Cadastro.jsx

import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import './Cadastro.css';

const Cadastro = () => {
    const [nickname, setNickname] = useState('');
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [errorMessage, setErrorMessage] = useState('');
    const navigate = useNavigate();

    const handleRegisterClick = () => {
        if (nickname && email && password && confirmPassword) {
            if (password !== confirmPassword) {
                setErrorMessage('As senhas não coincidem.');
                return;
            }
            if (password.length < 8) {
                setErrorMessage('A senha deve ter pelo menos 8 dígitos.');
                return;
            }

            // Adicione aqui a lógica para o registro do usuário

            navigate('/welcome'); // Redireciona para a tela de boas-vindas
        } else {
            setErrorMessage('Por favor, preencha todos os campos.');
        }
    };

    const handleBackClick = () => {
        navigate('/'); // Redireciona para a tela inicial
    };

    return (
        <div className="cadastro-container">
            <h2>Cadastro</h2>
            <input 
                type="text" 
                placeholder="Nickname" 
                value={nickname}
                onChange={(e) => setNickname(e.target.value)}
            />
            <input 
                type="email" 
                placeholder="Email" 
                value={email}
                onChange={(e) => setEmail(e.target.value)}
            />
            <input 
                type="password" 
                placeholder="Senha" 
                value={password}
                onChange={(e) => setPassword(e.target.value)}
            />
            <input 
                type="password" 
                placeholder="Confirmar Senha" 
                value={confirmPassword}
                onChange={(e) => setConfirmPassword(e.target.value)}
            />
            <button
                style={{ width: '250px', padding: '8px', fontSize: '0.9em' }}
                onClick={handleRegisterClick}
            >
                Cadastrar
            </button>
            <button
                style={{ width: '250px', padding: '8px', fontSize: '0.9em', backgroundColor: '#2cc158' }}
                className="back-button"
                onClick={handleBackClick}
            >
                Voltar
            </button>
            {errorMessage && <p className="error-message">{errorMessage}</p>}
        </div>
    );
};

export default Cadastro;
