import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from './AuthContext';

const ProtectedRoute = ({ children, requiredRoles }) => {
    const { isAuthenticated, roles } = useAuth();

    if (!isAuthenticated) {
        return <Navigate to="/login" />;
    }

    if (requiredRoles && !requiredRoles.some(role => roles.includes(role))) {
        return <Navigate to="/not-authorized" />; // Redirect to a not authorized page
    }

    return children;
};

export default ProtectedRoute;