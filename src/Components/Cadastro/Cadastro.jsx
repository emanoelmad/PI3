// src/Components/Cadastro/Cadastro.jsx

import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import './Cadastro.css';

const Cadastro = () => {
    const [name, setName] = useState(''); // Adicionar campo para o nome
    const [nickname, setNickname] = useState('');
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [errorMessage, setErrorMessage] = useState('');
    const navigate = useNavigate();

    // Função para realizar o cadastro
    const handleRegisterClick = async () => {
        if (name && nickname && email && password) {
            if (password.length < 6) {
                setErrorMessage('A senha deve ter pelo menos 8 dígitos.');
                return;
            }

            try {
                const response = await fetch('https://localhost:7183/api/Usuario', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify({
                        nome: name,
                        nickname: nickname,
                        email: email,
                        senha: password
                    }),
                });

                if (!response.ok) {
                    const errorData = await response.json();
                    console.error('Erro ao criar conta:', errorData);
                    setErrorMessage(errorData.message || 'Erro ao criar a conta.');
                    return;
                }

                // Sucesso no cadastro, redireciona para a tela de boas-vindas
                navigate('/welcome');
            } catch (error) {
                console.error('Erro na conexão com o servidor:', error);
                setErrorMessage('Erro na conexão com o servidor.');
            }
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
                placeholder="Nome"
                value={name}
                onChange={(e) => setName(e.target.value)}
            />
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
