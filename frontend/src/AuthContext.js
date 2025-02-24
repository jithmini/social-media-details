import React, { createContext, useState, useContext, useEffect } from 'react';

const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const [username, setUsername] = useState('');
    const [roles, setRoles] = useState([]);
    const [loading, setLoading] = useState(true); // Add loading state

    useEffect(() => {
        const storedUser = localStorage.getItem('username');
        const storedRoles = localStorage.getItem('roles');
        if (storedUser && storedRoles) {
            setIsAuthenticated(true);
            setUsername(storedUser);
            setRoles(JSON.parse(storedRoles));
        }
        setLoading(false); // Set loading to false after restoring state
    }, []);

    const login = (user, userRoles) => {
        setIsAuthenticated(true);
        setUsername(user);
        setRoles(userRoles);

        localStorage.setItem('username', user);
        localStorage.setItem('roles', JSON.stringify(userRoles));
    };

    const logout = () => {
        setIsAuthenticated(false);
        setUsername('');
        setRoles([]);

        localStorage.removeItem('username');
        localStorage.removeItem('roles');
    };

    return (
        <AuthContext.Provider value={{ isAuthenticated, username, roles, loading, login, logout }}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => useContext(AuthContext);