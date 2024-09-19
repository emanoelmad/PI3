import React from 'react';
import ReactDOM from 'react-dom';
import App from './App';
import './index.css';
import { AuthProvider } from './Context/AuthContext';
import { PartidaProvider } from './Context/PartidaContext';


ReactDOM.render(
    <React.StrictMode>
        <AuthProvider>
            <PartidaProvider>
                <App/>
            </PartidaProvider>
        </AuthProvider>
    </React.StrictMode>,
    document.getElementById('root')
);
