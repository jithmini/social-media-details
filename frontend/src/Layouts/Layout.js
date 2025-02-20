import React from 'react';
import { useLocation } from 'react-router-dom';
import Header from '../Header/Header';
import Footer from '../Footer/Footer'; // Assuming you have a Footer component

const Layout = ({ children }) => {
    const location = useLocation();
    const hideHeader = location.pathname === '/login';

    return (
        <div className="d-flex flex-column min-vh-100">
            {!hideHeader && <Header />}
            <main className="flex-grow-1">
                {children}
            </main>
            <Footer />
        </div>
    );
};

export default Layout;