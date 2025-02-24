import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../AuthContext';
import LoadingScreen from '../LoadingScreen/LoadingScreen'; // Import the LoadingOverlay component


const Login = () => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [message, setMessage] = useState('');
    const [loading, setLoading] = useState(false); 
    const navigate = useNavigate();
    const { login } = useAuth();

    const handleLogin = async () => {
        setLoading(true);
        const response = await fetch(`${process.env.REACT_APP_API_BASE_URL}/api/Login`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ username, password })
        });

        const data = await response.json();
        setLoading(false);
        if (response.ok) {
            const userRoles = data.roles; // Assume roles are returned from the server

            login(username, userRoles);  
            navigate('/home'); 
        } else {
            setMessage(data.message);
        }
    };

    const handleKeyDown = (event) => {
        if (event.key === 'Enter') {
            handleLogin();
        }
    };

    useEffect(() => {
        window.addEventListener('keydown', handleKeyDown);
        return () => {
            window.removeEventListener('keydown', handleKeyDown);
        };
    }, [username, password]);

    return (
        <div className="container d-flex justify-content-center align-items-center vh-100">
            {loading && <LoadingScreen />}
            <div className="card p-4" style={{ width: '400px' }}>
                <h2 className="text-center mb-4">Login</h2>
                <div className="form-group">
                    <input
                        type="text"
                        placeholder="Username"
                        value={username}
                        onChange={(e) => setUsername(e.target.value)}
                        className="form-control mb-3"
                        onKeyDown={handleKeyDown}
                    />
                </div>
                <div className="form-group">
                    <input
                        type="password"
                        placeholder="Password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        className="form-control mb-3"
                        onKeyDown={handleKeyDown}
                    />
                </div>
                <button onClick={handleLogin} className="btn btn-primary btn-block" disabled={loading}>
                    Login
                </button>
                <p className="text-center mt-3 text-danger">{message}</p>
            </div>
        </div>
    );
};

export default Login;