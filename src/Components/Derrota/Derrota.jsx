import { useContext } from 'react';
import './Derrota.css';
import { AuthContext } from "../../Context/AuthContext";
import { useNavigate } from 'react-router-dom';

const Derrota = () => {
    const { pontuacao, setPontuacao } = useContext(AuthContext);
    const navigate = useNavigate();

    const handleHome = () => {
        setPontuacao(0);
        navigate('/welcome');
    };

    return (
        <div className="derrota-container">
            <h1 className="title">💔 Você Perdeu! 💔</h1>
            <p className="message">Infelizmente, você não conseguiu vencer o jogo desta vez.</p>
            <p className="score">Pontuação Final: R$ {pontuacao}</p>
            <a className="back-link" onClick={handleHome}>Voltar para a Tela Inicial</a>
        </div>
    );
};

export default Derrota;
