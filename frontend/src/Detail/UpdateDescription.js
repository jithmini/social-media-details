import React, { useEffect, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";

const UpdateDescription = () => {
    const location = useLocation();
    const navigate = useNavigate();
    const { userId, app } = location.state || {};
    
    const [description, setDescription] = useState("");
    const [descriptions, setDescriptions] = useState([]);
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
    const handleSubmit = () => {
        setIsSubmitted(true);
        localStorage.setItem(`submissionStatus_${userId}_${app.appId}`, "submitted");
        alert("Submission successful!");
        navigate(-1); // Go back to Detail page
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
                <button className="a" onClick={handleSubmitDescription} disabled={isSubmitted}>
                    Add
                </button>
            </div>

            {/* Display added descriptions */}
            <h3 className="mt-4">Added Hate Speeches</h3>
            {descriptions.length > 0 ? (
                <table className="table mt-3">
                    <thead>
                        <tr>
                            <th>Hate Speech</th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
                        {descriptions.map((desc) => (
                            <tr key={desc.id}>
                                <td>{desc.description}</td>
                                <td>
                                    <button className="bt" onClick={() => handleDeleteDescription(desc.id)} disabled={isSubmitted}>
                                        Delete
                                    </button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            ) : (
                <p>No Hate Speeches found.</p>
            )}
            
            <button className="floating-button" onClick={() => navigate(-1)}>Back</button>
            <button onClick={handleSubmit} disabled={isSubmitted}>Submit</button>
        </div>
    );
};

export default UpdateDescription;
