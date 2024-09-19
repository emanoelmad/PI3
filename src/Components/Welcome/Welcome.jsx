import { useNavigate } from 'react-router-dom';
import { useContext } from 'react';
import './Welcome.css';
import { AuthContext } from "../../Context/AuthContext";

const Welcome = ({ onLogout }) => {
    const navigate = useNavigate();
    const { userId, setPartidaId } = useContext(AuthContext);

    const handleStartGame = async () => {
        try {
            const response = await fetch(`https://localhost:7183/api/partida/iniciar/${userId}`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                },
                credentials: 'include', // Inclui os cookies na requisição
            });

            if (!response.ok) {
                const errorData = await response.json();
                console.error('Erro ao iniciar o jogo:', errorData);
                return;
            }

            const partidaData = await response.json();

            const id = partidaData.partidaId;

            setPartidaId(id);

            navigate('/perguntas');
        } catch (error) {
            console.error('Erro ao conectar ao servidor:', error);
        }
    };

    const handleLogout = async () => {
        try {
            const response = await fetch('https://localhost:7183/Auth/logout', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                credentials: 'include', // Inclui os cookies na requisição
            });

            if (!response.ok) {
                const errorData = await response.json();
                console.error('Erro ao fazer logout:', errorData);
                return;
            }

            // Chamando a função `onLogout`
            onLogout();

            // Redireciona para a página de login
            navigate('/login');
        } catch (error) {
            console.error('Erro ao conectar ao servidor:', error);
        }
    };

    const handleEditAccount = () => {
        navigate('/editarConta'); // Apenas navega para a tela de edição de conta
    };

    return (
        <div className="welcome-container">
            <h1>Bem-vindo ao Jogo!</h1>
            <div className="button-group">
                <button onClick={handleStartGame}>Iniciar Jogo</button>
                <button onClick={handleLogout}>Logout</button>
                <button onClick={handleEditAccount}>Editar Conta</button>
            </div>
        </div>
    );
};

export default Welcome;
