import { useState } from 'react';
import { Navigate, Route, BrowserRouter as Router, Routes } from 'react-router-dom';
import Cadastro from './Components/Cadastro/Cadastro';
import Derrota from './Components/Derrota/Derrota';
import EditarConta from './Components/EditarConta/EditarConta';
import HallDaFama from './Components/HallDaFama/HallDaFama';
import Login from './Components/Login/Login';
import PasswordReset from './Components/Login/PasswordReset/PasswordReset';
import Perguntas from './Components/Perguntas/Perguntas';
import RemoveAccount from './Components/RemoveAccount/RemoveAccount';
import Resultado from './Components/Resultado/Resultado';
import Vitoria from './Components/Vitoria/Vitoria';
import Welcome from './Components/Welcome/Welcome';

const App = () => {
    const [isLoggedIn, setIsLoggedIn] = useState(false);

    const handleLogin = (id) => {
        setIsLoggedIn(true);
        localStorage.setItem('userId', id); // Armazena o ID no localStorage
    };

    const handleLogout = () => {
        setIsLoggedIn(false);
        localStorage.removeItem('userId'); // Remove o ID do localStorage
    };

    return (
        <Router>
            <Routes>
                <Route
                    path="/"
                    element={!isLoggedIn ? <Login onLogin={handleLogin} /> : <Navigate to="/welcome" />}
                />
                <Route
                    path="/login"
                    element={!isLoggedIn ? <Login onLogin={handleLogin} /> : <Navigate to="/welcome" />}
                />
                <Route
                    path="/welcome"
                    element={isLoggedIn ? <Welcome onLogout={handleLogout} /> : <Navigate to="/login" />}
                />
                <Route path="/cadastro" element={<Cadastro />} />
                <Route path="/passwordreset" element={<PasswordReset />} />
                <Route path="/perguntas" element={isLoggedIn ? <Perguntas /> : <Navigate to="/login" />} />
                <Route path="/resultado" element={isLoggedIn ? <Resultado /> : <Navigate to="/login" />} />
                <Route path="/hallDaFama" element={isLoggedIn ? <HallDaFama /> : <Navigate to="/login" />} />
                <Route path="/removeAccount" element={<RemoveAccount />} />
                <Route path="/vitoria" element={isLoggedIn ? <Vitoria /> : <Navigate to="/login" />} />
                <Route path="/derrota" element={isLoggedIn ? <Derrota /> : <Navigate to="/login" />} />
                <Route path="/editarConta" element={isLoggedIn ? <EditarConta /> : <Navigate to="/login" />} />
            </Routes>
        </Router>
    );
};

export default App;
