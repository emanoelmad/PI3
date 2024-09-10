import React from 'react';
import { useNavigate } from 'react-router-dom';
import './RemoveAccount.css';

const RemoveAccount = () => {
    const navigate = useNavigate();

    const handleRemoveAccount = () => {
        // Lógica para remover a conta
        alert("Conta removida com sucesso.");
        navigate('/');
    };

    return (
        <div className="remove-account-container">
            <h1>Remover Conta</h1>
            <p>Tem certeza de que deseja remover sua conta? Esta ação não pode ser desfeita.</p>
            <button onClick={handleRemoveAccount}>Remover Conta</button>
            <button onClick={() => navigate(-1)}>Voltar</button>
        </div>
    );
};

export default RemoveAccount;
