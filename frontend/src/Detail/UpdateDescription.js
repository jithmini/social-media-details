import React, { useEffect, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import LoadingScreen from '../LoadingScreen/LoadingScreen';

const UpdateDescription = () => {
    const location = useLocation();
    const navigate = useNavigate();
    const { userId, app } = location.state || {};
    const [loading, setLoading] = useState(true); 
    const [description, setDescription] = useState("");
    const [descriptionList, setDescriptions] = useState([]);
    const [isSubmitted, setIsSubmitted] = useState(
        localStorage.getItem(`submissionStatus_${userId}_${app.appId}`) === "submitted"
    );

    // Fetch existing descriptions
    const fetchDescriptions = async () => {
        try {
            const response = await fetch(
                `${process.env.REACT_APP_API_BASE_URL}/api/Details/getDescriptions/${userId}/${app.appId}`,
                {
                    method: 'GET',
                    headers: {
                        'Content-Type': 'application/json',
                        'X-API-KEY': process.env.REACT_APP_API_KEY
                    }
                }
            );
            setLoading(false);
            if (response.ok) {
                const data = await response.json();
                setDescriptions(data);
            } else {
                setDescriptions([]);
            }
        } catch (error) {
            console.error("Error fetching descriptions:", error);
        }
    };

    useEffect(() => {
        fetchDescriptions();
    }, []);

    // Add new description
    const handleSubmitDescription = async () => {
        if (!description.trim()) {
            alert("Please enter a description.");
            return;
        }

        try {
            const response = await fetch(`${process.env.REACT_APP_API_BASE_URL}/api/Details/updateDescription`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "X-API-KEY": process.env.REACT_APP_API_KEY
                },
                body: JSON.stringify({ userId, appId: app.appId, description }),
            });

            
            if (response.ok) {
                alert("Description added successfully!");
                setDescription("");
                fetchDescriptions();
            } else {
                alert("Failed to add description.");
            }
        } catch (error) {
            console.error("Error adding description:", error);
            alert("An error occurred.");
        }
    };

    // Submit descriptions (Final action)
    const handleSubmit = async () => {
        // setIsSubmitted(true);
        // localStorage.setItem(`submissionStatus_${userId}_${app.appId}`, "submitted");

        if (descriptionList.length > 0){
            
            const response = await fetch(`${process.env.REACT_APP_API_BASE_URL}/api/Details/updateUserAppStatus`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "X-API-KEY": process.env.REACT_APP_API_KEY
                },
                body: JSON.stringify({ userId, appId: app.appId, description }),
            });
            navigate(-1);
        }
        else{
            alert("No data to submit!");
        }
        
    };

    // Delete description
    const handleDeleteDescription = async (id) => {
        if (!window.confirm("Are you sure you want to delete this description?")) return;

        try {
            const response = await fetch(`${process.env.REACT_APP_API_BASE_URL}/api/Details/deleteDescription/${id}`, {
                method: "DELETE",
                headers: {
                    "Content-Type": "application/json",
                    "X-API-KEY": process.env.REACT_APP_API_KEY
                }
            });

            if (response.ok) {
                alert("Description deleted successfully!");
                fetchDescriptions();
            } else {
                alert("Failed to delete description.");
            }
        } catch (error) {
            console.error("Error deleting description:", error);
            alert("An error occurred.");
        }
    };

    if (loading) {
        return <LoadingScreen />; 
    }

    return (
        <div className="container mt-5">
            <h2>Hate Speech Entry Form</h2>
            <h5>Application: {app?.appName}</h5>
            
            <textarea
                className="form-control"
                rows="4"
                placeholder="Enter description..."
                value={description}
                onChange={(e) => setDescription(e.target.value)}
            />
            <div className="mt-3">
                <button className="btn btn-secondary mt-3 ms-2" onClick={handleSubmitDescription} disabled={isSubmitted}>
                    Add
                </button>
            </div>

            {/* Display added descriptions */}
            <h3 className="mt-4">Added Hate Speeches</h3>
            {descriptionList.length > 0 ? (
                <table className="table mt-3">
                    <thead>
                        <tr>
                            <th>Hate Speech</th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
                        {descriptionList.map((desc) => (
                            <tr key={desc.id}>
                                <td>{desc.description}</td>
                                {/* <td>
                                    <button className="bt" onClick={() => handleDeleteDescription(desc.id)} disabled={isSubmitted}>
                                        Delete
                                    </button>
                                </td> */}
                                <td className="text-center">
                                    <i className="fa fa-times" style={{ cursor: 'pointer', color: 'red' }} onClick={() => handleDeleteDescription(desc.id)} disabled={isSubmitted}></i>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            ) : (
                <p>No Hate Speeches found.</p>
            )}
            
            <button className="btn btn-secondary mt-3 ms-2" onClick={() => navigate(-1)}>Back</button>
            <button className="btn btn-secondary mt-3 ms-2" onClick={handleSubmit} disabled={isSubmitted}>Submit</button>
        </div>
    );
};

export default UpdateDescription;
