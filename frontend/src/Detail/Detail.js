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

    const handleSubmit = async () => {
            
            const response = await fetch(`${process.env.REACT_APP_API_BASE_URL}/api/Details/updateUserAppStatus`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "X-API-KEY": process.env.REACT_APP_API_KEY
                },
                body: JSON.stringify({ userId}),
            });
            navigate(-1);
    };
    const logClickAccept = async () => {
        try {
            const payload = {
                id: userId,
                result: "accepted",
                reason: "",
                links: []
            };

            const res = await fetch(`${process.env.REACT_APP_API_BASE_URL}/api/Details/send-webhook-accept`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "X-API-KEY": process.env.REACT_APP_API_KEY
                },
                body: JSON.stringify(payload)
            });

            if (res.ok) {
                alert("Webhook sent successfully");
                await handleSubmit();
            } else {
                alert("Failed to send webhook.");
            }
        } catch (error) {
                alert(`Webhook failed: ${error.message}`);
        }
    };
    const disabledStyle = {
                backgroundColor: '#b3d7ff',
                borderColor: '#b3d7ff',
                color: '#ffffff',
                opacity: 1,
                cursor: 'not-allowed'
    };
    const disabledStyle2 = {
                backgroundColor: '#f8bcbc',
                borderColor: '#f8bcbc',
                color: '#ffffff',
                opacity: 1,
                cursor: 'not-allowed'
    };
    
    const logClickReject = async () => {
        try {
            const payload = {
                id: userId,
                result: "rejected",
                reason: "",
                links: []
            };

            const res = await fetch(`${process.env.REACT_APP_API_BASE_URL}/api/Details/send-webhook-reject`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "X-API-KEY": process.env.REACT_APP_API_KEY
                },
                body: JSON.stringify(payload)
            });

            if (res.ok) {
                alert("Webhook sent successfully");
                await handleSubmit();
            } else {
                alert("Failed to send webhook.");
            }
        } catch (error) {
                alert(`Webhook failed: ${error.message}`);
        }
    };

    if (loading) {
        return <LoadingScreen />; 
    }
    const hasAnyDescriptions = userApps.some(app => app.descriptionCount > 0);


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
                              {/* <span>{app.status ? "Submitted" : "Pending"}</span> */}
                              <span>{app.descriptionCount}</span>
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
            <button className="btn btn-secondary mt-3 ms-2" onClick={() => navigate(-1)}>Back</button>
            <button className="btn btn-primary mt-3 ms-2" onClick={async () => {await logClickAccept()}} disabled={hasAnyDescriptions} style={hasAnyDescriptions ? disabledStyle : {}}> Accept </button>
            <button className="btn btn-danger mt-3 ms-2" onClick={async () => {await logClickReject()}} disabled={!hasAnyDescriptions} style={!hasAnyDescriptions ? disabledStyle2 : {}}> Reject </button>
        </div>
    );
};

export default Detail;
