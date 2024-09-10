import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import './EditarConta.css';

const EditarConta = () => {
    const [nickname, setNickname] = useState('');
    const [email, setEmail] = useState('');
    const [successMessage, setSuccessMessage] = useState('');
    const navigate = useNavigate();

    const handleSave = () => {
        //  adicionar a lógica para salvar as alterações (por exemplo, enviar para um backend)
        setSuccessMessage('Sucesso!');
    };

    const handleBack = () => {
        navigate('/welcome');
    };

    return (
        <div className="editar-conta-container">
            <h1>Editar Conta</h1>
            <input
                type="text"
                placeholder="Novo Nickname"
                value={nickname}
                onChange={(e) => setNickname(e.target.value)}
            />
            <input
                type="email"
                placeholder="Novo Email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                pattern=".*@gmail\.com$"
                title="O email deve ser um endereço Gmail."
            />
            <button onClick={handleSave}>Salvar</button>
            <button onClick={handleBack}>Voltar</button>
            {successMessage && <p className="success-message">{successMessage}</p>}
        </div>
    );
};

export default EditarConta;
