import React, { useEffect, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import LoadingScreen from '../LoadingScreen/LoadingScreen';

const Detail = () => {
    const location = useLocation();
    const navigate = useNavigate();
    const { userId } = location.state || {};

    const [user, setUser] = useState(null);
    const [userApps, setUserApps] = useState([]);
    const [isLoading, setIsLoading] = useState(true);
    const [loading, setLoading] = useState(true); 

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
                        // submissionStatus: localStorage.getItem(`submissionStatus_${userId}_${app.appId}`) || "pending",
                    }));

                    setUserApps(appsData);
                    setLoading(false);
                }
            } catch (error) {
                console.error("Error fetching data:", error);
                setUserApps([]);
            } finally {
                setIsLoading(false);
                setLoading(false);
            }
        };

        fetchUserData();
    }, [userId]);

    // Navigate to UpdateDescription page
    const handleAddDescription = (app) => {
        navigate("/update-description", { state: { userId, app } });
    };

    if (loading) {
        return <LoadingScreen />; 
    }

    return (
        <div className="container mt-5">
            <h2>User Detail</h2>
            {user ? (
                <div>
                    <p><strong>Full Name:</strong> {user.fullName}</p>
                    <p><strong>Title:</strong> {user.title}</p>
                    <p><strong>Status:</strong> {user.status ? "Submitted" : "Pending"}</p>
                </div>
            ) : (
                <p></p>
            )}

            <h3 className="mt-4">Social Media Links</h3>
            <br></br>
            {isLoading ? (
                <p>Loading social media links...</p>
            ) : userApps.length > 0 ? (
                <ul className="list-group mt-3">
                    {userApps.map((app, index) => (
                        // <li key={index} className="list-group-item d-flex justify-content-between align-items-center">
                          <div class="row">
                            <div class="col-sm-3">
                              <span><strong>{app.appName}</strong></span>
                            </div>
                            <div class="col-sm-4">
                              <a href={app.link} target="_blank" rel="noopener noreferrer">{app.link}</a>
                            </div>
                            <div class="col-sm-3">
                              <span>{app.status ? "Submitted" : "Pending"}</span>
                            </div>
                            <div class="col-sm-2">
                            <button
                                className="btn btn-secondary mb-2"
                                onClick={() => handleAddDescription(app)}
                                disabled={app.status}
                            >
                                Add Hate Speech
                            </button>
                            </div>
                          </div>
                          
                            
                        // </li>
                    ))}
                    <br></br>
                </ul>
            ) : (
                <p>No social media links found for this user.</p>
            )}
            <br></br>
            <button className="btn btn-secondary" onClick={() => navigate(-1)}>Back</button>
        </div>
    );
};

export default Detail;
