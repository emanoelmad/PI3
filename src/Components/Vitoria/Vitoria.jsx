import { useContext } from 'react';
import './Vitoria.css';
import { AuthContext } from "../../Context/AuthContext";
import { useNavigate } from 'react-router-dom';


const Vitoria = () => {
    const { pontuacao, setPontuacao } = useContext(AuthContext);
    const navigate = useNavigate();

    const handleHome = () => {
        setPontuacao(0);
        navigate('/welcome');
    };

    return (
        <div className="vitoria-container">
            <h1 className="title">🎉 Parabéns! Você Venceu! 🎉</h1>
            <p className="message">Você conseguiu vencer o jogo. Ótimo trabalho!</p>
            <p className="score">Pontuação Final: R$ {pontuacao}</p>
            <a className="back-link" onClick={handleHome}>Voltar para a Tela Inicial</a>
        </div>
    );
};

export default Vitoria;
