/* eslint-disable react/prop-types */
import { createContext, useState } from 'react';

// Cria o contexto
export const AuthContext = createContext();

// Cria o Provider para envolver os componentes que precisarão acessar o contexto
export const AuthProvider = ({ children }) => {
    const [userId, setUserId] = useState(null);
    const [partidaId, setPartidaId] = useState(null);
    const [pontuacao, setPontuacao] = useState(0);

    return (
        <AuthContext.Provider value={{ userId, setUserId, partidaId, setPartidaId, pontuacao, setPontuacao }}>
            {children}
        </AuthContext.Provider>
    );
};
