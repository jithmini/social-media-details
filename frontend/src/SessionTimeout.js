import { useEffect } from "react";
import { useAuth } from "./AuthContext";

const SessionTimeout = ({ timeout = 15 * 60 * 1000 }) => { // Default: 15 minutes
    const { logout, isAuthenticated } = useAuth();

    useEffect(() => {
        if (!isAuthenticated) return;

        let timer;

        const resetTimer = () => {
            clearTimeout(timer);
            timer = setTimeout(() => {
                alert("Session expired due to inactivity.");
                logout();
            }, timeout);
        };

        // Events to listen for activity
        const events = ["mousemove", "keydown", "click", "scroll"];

        events.forEach(event => window.addEventListener(event, resetTimer));

        resetTimer(); // Initialize timer

        return () => {
            events.forEach(event => window.removeEventListener(event, resetTimer));
            clearTimeout(timer);
        };
    }, [isAuthenticated, logout, timeout]);

    return null;
};

export default SessionTimeout;
