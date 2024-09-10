// src/Components/Login/Login.jsx

import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import './Login.css';

const Login = ({ onLogin }) => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [errorMessage, setErrorMessage] = useState('');
    const navigate = useNavigate();

    const handleLoginClick = () => {
        if (email && password) {
            // Verifica se a senha tem pelo menos 8 dígitos
            if (password.length < 8) {
                setErrorMessage('A senha deve ter pelo menos 8 dígitos.');
                return;
            }

            // Aqui você pode adicionar a lógica para verificar as credenciais do usuário
            const nickname = "Usuário"; // Substitua pelo nickname real, se disponível
            onLogin(nickname);
            navigate('/welcome', { state: { nickname } });
        } else {
            setErrorMessage('Por favor, preencha todos os campos.');
        }
    };

    return (
        <div className="login-container">
            <h2>Login</h2>
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
            <button onClick={handleLoginClick}>Entrar</button>
            <a href="/cadastro">Cadastrar</a>
            <a href="/passwordreset">Recuperar Senha</a>
            {errorMessage && <p className="error-message">{errorMessage}</p>}
        </div>
    );
};

export default Login;
