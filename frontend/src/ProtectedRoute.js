import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from './AuthContext';
import LoadingScreen from './LoadingScreen/LoadingScreen'; // Import the LoadingScreen component

const ProtectedRoute = ({ children, requiredRoles }) => {
    const { isAuthenticated, roles, loading } = useAuth();

    if (loading) {
        return <LoadingScreen />; // Show a loading indicator while restoring state
    }

    if (!isAuthenticated) {
        return <Navigate to="/login" />;
    }

    if (requiredRoles && !requiredRoles.some(role => roles.includes(role))) {
        return <Navigate to="/not-authorized" />;
    }

    return children;
};

export default ProtectedRoute;