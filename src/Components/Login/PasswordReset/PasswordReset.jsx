import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import './PasswordReset.css';

const PasswordReset = () => {
    const [successMessage, setSuccessMessage] = useState('');
    const navigate = useNavigate();

    const handleReset = () => {
        // Lógica de recuperação
        setSuccessMessage('Sucesso!');
    };

    const handleBackToLogin = () => {
        navigate('/');
    };

    return (
        <div className="password-reset-container">
            <h1>Recuperar Senha</h1>
            <div className="reset-form">
                <input type="email" placeholder="Email" />
                <div className="button-container">
                    <button onClick={handleReset}>Recuperar Senha</button>
                    <button className="back-to-home" onClick={handleBackToLogin}>Voltar</button>
                </div>
                {successMessage && <p className="success-message">{successMessage}</p>}
            </div>
        </div>
    );
};

export default PasswordReset;
