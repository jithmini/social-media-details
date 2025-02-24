import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import Login from './Login/Login';
import Summary from './Summary/Summary';
import Detail from './Detail/Detail';
import { AuthProvider } from './AuthContext';
import ProtectedRoute from './ProtectedRoute';
import Layout from './Layouts/Layout';
import NotFound from './Layouts/NotFound';
import SessionTimeout from './SessionTimeout';
import NotAuthorized from './Layouts/NotAuthorized';

const App = () => {
    return (
        <AuthProvider>
            <Router>
                <SessionTimeout/> 
                    <Layout>
                        <Routes>
                            <Route path="/login" element={<Login />} />
                            <Route
                                path="/home"
                                element={
                                    <ProtectedRoute requiredRoles={['Analyst']}>
                                        <Summary />
                                    </ProtectedRoute>
                                }
                            />
                            <Route
                                path="/detail"
                                element={
                                    <ProtectedRoute requiredRoles={['Admin']}>
                                        <Detail />
                                    </ProtectedRoute>
                                }
                            />
                            <Route path="/not-authorized" element={<NotAuthorized />} />
                            <Route path="/" element={<Login />} />
                            <Route path="*" element={<NotFound />} />              
                        </Routes>
                    </Layout>
            </Router>
        </AuthProvider>
    );
};

export default App;