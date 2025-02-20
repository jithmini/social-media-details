import React, { createContext, useState, useContext, useEffect } from "react";

const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
    const [isAuthenticated, setIsAuthenticated] = useState(() =>
        localStorage.getItem("isAuthenticated") === "true"
    );
    const [username, setUsername] = useState(() =>
        localStorage.getItem("username") || ""
    );

    useEffect(() => {
        localStorage.setItem("isAuthenticated", isAuthenticated);
        localStorage.setItem("username", username);
    }, [isAuthenticated, username]);

    useEffect(() => {
        const handleStorageChange = () => {
            setIsAuthenticated(localStorage.getItem("isAuthenticated") === "true");
            setUsername(localStorage.getItem("username") || "");
        };

        window.addEventListener("storage", handleStorageChange);
        return () => window.removeEventListener("storage", handleStorageChange);
    }, []);

    const login = (username) => {
        setIsAuthenticated(true);
        setUsername(username);
    };

    const logout = () => {
        setIsAuthenticated(false);
        setUsername("");
        localStorage.removeItem("isAuthenticated");
        localStorage.removeItem("username");
    };

    return (
        <AuthContext.Provider value={{ isAuthenticated, username, login, logout }}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => {
    return useContext(AuthContext);
};
