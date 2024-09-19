import { useState, useContext } from 'react';
import { useNavigate } from 'react-router-dom';
import './Login.css'; // Estilos CSS
import { AuthContext } from "../../Context/AuthContext";

const Login = ({ onLogin  }) => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [errorMessage, setErrorMessage] = useState('');
    const navigate = useNavigate();
    const { setUserId } = useContext(AuthContext);

    const handleLoginClick = async () => {
        if (email && password) {
            // Verifica se a senha tem pelo menos 6 dígitos
            if (password.length < 6) {
                setErrorMessage('A senha deve ter pelo menos 6 dígitos.');
                return;
            }

            try {
                const response = await fetch('https://localhost:7183/Auth/login', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify({ email, senha: password }), // Certifique-se de que "senha" é o nome correto
                });

                if (!response.ok) {
                    const errorData = await response.json();
                    console.error('Erro na resposta da API:', errorData);
                    setErrorMessage(errorData.message || 'Falha na autenticação.'); // Corrija para 'message'
                    return;
                }

                const data = await response.json();

                console.log(data.id);

                console.log('Dados recebidos da API:', data);

                const id = data.id;

                if (data.success) { // Corrija para 'success'
                    // Login bem-sucedido
                    setErrorMessage(''); // Limpar mensagens de erro, se houver
                    onLogin(); // Atualiza o estado de login no App, se necessário
                    navigate('/welcome'); // Redireciona para a página de boas-vindas
                    setUserId(id);

                } else {
                    // Caso o login falhe
                    setErrorMessage(data.message || 'Falha na autenticação.'); // Corrija para 'message'
                    console.error('Erro: Falha na autenticação.', data);
                }
            } catch (error) {
                console.error('Erro na requisição:', error);
                setErrorMessage('Erro na conexão com o servidor: ' + error.message);
            }
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
                className="login-input"
            />
            <input
                type="password"
                placeholder="Senha"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                className="login-input"
            />
            <button onClick={handleLoginClick} className="login-button">Entrar</button>
            <div className="login-links">
                <a href="/cadastro" className="link">Cadastrar</a>
                <a href="/passwordreset" className="link">Recuperar Senha</a>
            </div>

            {errorMessage && <p className="error-message">{errorMessage}</p>}
        </div>
    );
};

export default Login;
