/* eslint-disable react/prop-types */
import { createContext, useState } from 'react';

// Cria o contexto
export const PartidaContext = createContext();

// Cria o Provider para envolver os componentes que precisarão acessar o contexto
export const PartidaProvider = ({ children }) => {
    const [partidaId, setPartidaId] = useState(null);

    return (
        <PartidaContext.Provider value={{ partidaId, setPartidaId }}>
            {children}
        </PartidaContext.Provider>
    );
};
