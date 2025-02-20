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
                                    <ProtectedRoute>
                                        <Summary />
                                    </ProtectedRoute>
                                }
                            />
                            <Route
                                path="/detail"
                                element={
                                    <ProtectedRoute>
                                        <Detail />
                                    </ProtectedRoute>
                                }
                            />
                            <Route path="/" element={<Login />} />
                            <Route path="*" element={<NotFound />} />              
                        </Routes>
                    </Layout>
            </Router>
        </AuthProvider>
    );
};

export default App;