import React from 'react';
import './LoadingOverlay.css'; 
import loadingGif from './loading-load.gif';

const LoadingScreen = () => {
    return (
        <div className="loading-overlay">
            <img src={loadingGif} alt="Loading..." />
        </div>
    );
};

export default LoadingScreen;