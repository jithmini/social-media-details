import React from 'react';
import { useAuth } from '../AuthContext';
import { useNavigate } from 'react-router-dom';

const Header = () => {
    const { username, logout } = useAuth();
    const navigate = useNavigate();

    const handleLogout = () => {
        logout();
        navigate('/login');
    };

    return (
        <header className="navbar navbar-expand-lg navbar-light bg-light">
            <div className="container">
                <span className="navbar-brand">SMD</span>
                <div className="collapse navbar-collapse justify-content-end">
                    {username && (
                        <ul className="navbar-nav">
                            <li className="nav-item">
                                <span className="nav-link">{username}</span>
                            </li>
                            <li className="nav-item">
                                <button className="btn btn-link nav-link" onClick={handleLogout}>Logout</button>
                            </li>
                        </ul>
                    )}
                </div>
            </div>
        </header>
    );
};

export default Header;