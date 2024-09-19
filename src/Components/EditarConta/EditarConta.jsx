import { useState, useContext } from 'react';
import { useNavigate } from 'react-router-dom';
import './EditarConta.css';
import { AuthContext } from "../../Context/AuthContext";


const EditarConta = () => {
    const [nome, setNome] = useState('');
    const [nickname, setNickname] = useState('');
    const [email, setEmail] = useState('');
    const [successMessage, setSuccessMessage] = useState('');
    const [errorMessage, setErrorMessage] = useState('');
    const navigate = useNavigate(); // Hook para navegação
    const { userId } = useContext(AuthContext);

    const handleSave = async () => {
        // Construir o corpo da requisição com todos os campos
        const body = {
            Nome: nome.length > 0 ? nome : null,
            Nickname: nickname.length > 0 ? nickname : null,
            Email: email.length > 0 ? email : null,
            Id: userId
        };

        // Verificar se pelo menos um campo possui valor
        if (Object.values(body).every(value => value === null)) {
            setErrorMessage('Pelo menos um campo deve ser preenchido.');
            setSuccessMessage('');
            return;
        }

        try {
            const response = await fetch('https://localhost:7183/api/usuario/update-user', {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(body),
                credentials: 'include'
            });

            // Verifica se o status da resposta está dentro do intervalo de sucesso (2xx)
            if (!response.ok) {
                const errorText = await response.text(); // Lê a resposta como texto
                throw new Error('Erro ao atualizar conta: ' + errorText);
            }

            // Verifica se a resposta tem um corpo antes de tentar fazer o parse como JSON
            const responseBodyText = await response.text();
            if (responseBodyText) {
                const responseBody = JSON.parse(responseBodyText);
                console.log(responseBody); // Verifica o conteúdo da resposta
            }

            setSuccessMessage('Alterações salvas com sucesso!');
            setErrorMessage('');
            setEmail("");
            setNome("");
            setNickname("");
        } catch (error) {
            console.error('Erro ao salvar alterações:', error);
            setErrorMessage(error.message || 'Erro ao salvar alterações');
            setSuccessMessage('');
        }
    };

    const handleBack = () => {
        navigate('/welcome'); // Navega para a tela de boas-vindas
    };

    return (
        <div className="editar-conta-container">
            <h2>Editar Conta</h2>
            <input
                type="text"
                placeholder="Nome"
                value={nome}
                onChange={(e) => setNome(e.target.value)}
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
            <button onClick={handleSave}>Salvar</button>
            <button onClick={handleBack}>Voltar</button>
            {successMessage && <p className="success-message">{successMessage}</p>}
            {errorMessage && <p className="error-message">{errorMessage}</p>}
        </div>
    );
};

export default EditarConta;
