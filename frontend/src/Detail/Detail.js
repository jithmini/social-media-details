import React, { useEffect, useState } from 'react';
import { useLocation } from 'react-router-dom';

const Detail = () => {
    const location = useLocation();
    const { userId } = location.state || {};
    const [user, setUser] = useState(null);

    useEffect(() => {
        if (userId) {
            fetch(`${process.env.REACT_APP_API_BASE_URL}/api/Details/userdetails/${userId}`)
                .then(response => response.json())
                .then(data => setUser(data));
        }
    }, [userId]);

    return (
        <div className="container mt-5">
            <h2>User Detail</h2>
            {user ? (
                <div>
                    <p><strong>Full Name:</strong> {user.fullName}</p>
                    <p><strong>Title:</strong> {user.title}</p>
                    <p><strong>Status:</strong> {user.status ? 'Submitted' : 'Pending'}</p>
                </div>
            ) : (
                <p>Loading user details...</p>
            )}
        </div>
    );
};

export default Detail;