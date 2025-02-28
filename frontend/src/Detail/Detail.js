import React, { useEffect, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";

const Detail = () => {
    const location = useLocation();
    const navigate = useNavigate();
    const { userId } = location.state || {};

    const [user, setUser] = useState(null);
    const [userApps, setUserApps] = useState([]);
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
        const fetchUserData = async () => {
            try {
                if (userId) {
                    const userResponse = await fetch(
                        `${process.env.REACT_APP_API_BASE_URL}/api/Details/userdetails/${userId}`,
                        {
                            method: "GET",
                            headers: {
                                "Content-Type": "application/json",
                                "X-API-KEY": process.env.REACT_APP_API_KEY
                            }
                        }
                    );
                    const userData = await userResponse.json();
                    setUser(userData);

                    const appsResponse = await fetch(
                        `${process.env.REACT_APP_API_BASE_URL}/api/Details/userapps/${userId}`,
                        {
                            method: "GET",
                            headers: {
                                "Content-Type": "application/json",
                                "X-API-KEY": process.env.REACT_APP_API_KEY
                            }
                        }
                    );
                    const appsData = await appsResponse.json();

                    // Check for submission status in localStorage
                    const updatedApps = appsData.map((app) => ({
                        ...app,
                        submissionStatus: localStorage.getItem(`submissionStatus_${userId}_${app.appId}`) || "pending",
                    }));

                    setUserApps(updatedApps);
                }
            } catch (error) {
                console.error("Error fetching data:", error);
            } finally {
                setIsLoading(false);
            }
        };

        fetchUserData();
    }, [userId]);

    // Navigate to UpdateDescription page
    const handleAddDescription = (app) => {
        navigate("/update-description", { state: { userId, app } });
    };

    return (
        <div className="container mt-5">
            <h2>User Detail</h2>
            {user ? (
                <div>
                    <p><strong>Full Name:</strong> {user.fullName}</p>
                    <p><strong>Title:</strong> {user.title}</p>
                    {/* <p><strong>Status:</strong> {user.status ? "Submitted" : "Pending"}</p> */}
                </div>
            ) : (
                <p>Loading user details...</p>
            )}

            <h3 className="mt-4">Social Media Links</h3>
            {isLoading ? (
                <p>Loading social media links...</p>
            ) : userApps.length > 0 ? (
                <ul className="list-group mt-3">
                    {userApps.map((app, index) => (
                        <li key={index} className="list-group-item d-flex justify-content-between align-items-center">
                            <span><strong>{app.appName}</strong></span>
                            <a href={app.link} target="_blank" rel="noopener noreferrer">{app.link}</a>
                            <span>{app.submissionStatus === "submitted" ? "Submitted" : "Pending"}</span>
                            <button
                                className="b"
                                onClick={() => handleAddDescription(app)}
                                disabled={app.submissionStatus === "submitted"}
                            >
                                Add Hate Speech
                            </button>
                        </li>
                    ))}
                </ul>
            ) : (
                <p>No social media links found for this user.</p>
            )}
            
            <button className="floating-button" onClick={() => navigate(-1)}>Back</button>
        </div>
    );
};

export default Detail;
