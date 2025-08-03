import React, { useEffect, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import LoadingScreen from '../LoadingScreen/LoadingScreen';

const Detail = () => {
  const [showModal, setShowModal] = useState(false);
  const [showModalLinks, setModalLinks] = useState(false);
  const [userLinks, setUserLinks] = useState([]); // For user links
  const [flaggedLinks, setFlaggedLinks] = useState([]); // For flagged links
  const location = useLocation();
  const navigate = useNavigate();
  const { userId } = location.state || {};
  const [updatedReasons, setUpdatedReasons] = useState({});
  const [user, setUser] = useState(null);
  const [userApps, setUserApps] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [loading, setLoading] = useState(true);


  useEffect(() => {
    const fetchUserData = async () => {
      try {
        if (userId) {
          const userResponse = await fetch(
            `${process.env.REACT_APP_API_BASE_URL}api/Details/userdetails/${userId}`,
            {
              method: "GET",
              headers: {
                "Content-Type": "application/json",
                "X-API-KEY": process.env.REACT_APP_API_KEY,
              },
            }
          );
          const userData = await userResponse.json();
          setUser(userData);

          const appsResponse = await fetch(
            `${process.env.REACT_APP_API_BASE_URL}api/Details/userapps/${userId}`,
            {
              method: "GET",
              headers: {
                "Content-Type": "application/json",
                "X-API-KEY": process.env.REACT_APP_API_KEY,
              },
            }
          );
          const appsData = await appsResponse.json();
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

  const handleAddDescription = (app) => {
    navigate("/update-description", { state: { userId, app } });
  };

  const handleReasonChange = (index, newReason) => {
  setUpdatedReasons(prev => ({
    ...prev,
    [index]: newReason
  }));
};

const saveUpdatedReason = async (url, index) => {
  const reason = updatedReasons[index];

  if (!reason) {
    alert("Please enter a reason before saving.");
    return;
  }

  try {
    const response = await fetch(`${process.env.REACT_APP_API_BASE_URL}api/Details/updateReasoning`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        "X-API-KEY": process.env.REACT_APP_API_KEY,
      },
      body: JSON.stringify({
        userId: userId,
        url: url,
        reason: reason,
      }),
    });

    if (!response.ok) {
      throw new Error("Failed to update reason.");
    }

    alert("Reason updated successfully.");
  } catch (error) {
    console.error("Error updating reason:", error);
    alert("Could not update reason.");
  }
};


  const handleSubmit = async () => {
    await fetch(`${process.env.REACT_APP_API_BASE_URL}api/Details/updateUserAppStatus`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        "X-API-KEY": process.env.REACT_APP_API_KEY,
      },
      body: JSON.stringify({ userId }),
    });
    navigate(-1);
  };

  const logClickAccept = async () => {
    try {
      const payload = {
        id: userId,
        result: "accepted",
        reason: "",
        links: [],
      };

      const res = await fetch(`${process.env.REACT_APP_API_BASE_URL}api/Details/send-webhook-accept`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          "X-API-KEY": process.env.REACT_APP_API_KEY,
        },
        body: JSON.stringify(payload),
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

  const logClickReject = async () => {
    try {
      const payload = {
        id: userId,
        result: "rejected",
        reason: "",
        links: [],
      };

      const res = await fetch(`${process.env.REACT_APP_API_BASE_URL}api/Details/send-webhook-reject`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          "X-API-KEY": process.env.REACT_APP_API_KEY,
        },
        body: JSON.stringify(payload),
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

  const fetchUserLinks = async () => {
  try {
    const response = await fetch(
      `${process.env.REACT_APP_API_BASE_URL}api/Details/automated-links/${userId}`,
      {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          "X-API-KEY": process.env.REACT_APP_API_KEY,
        },
      }
    );
    if (!response.ok) throw new Error("Failed to fetch user links");
    const data = await response.json();

    // Convert array of strings to array of objects with `url` key
    const linksWithUrls = data.map(url => ({ url }));

    setUserLinks(linksWithUrls);
    setFlaggedLinks([]); // Clear flagged links when showing user links
    setModalLinks(true);
  } catch (error) {
    console.error("Error fetching user links:", error);
    alert("Could not load user links");
  }
};


  const fetchFlaggedLinks = async () => {
  try {
    const response = await fetch(
      `${process.env.REACT_APP_API_BASE_URL}api/Details/flagged-automated-links/details/${userId}`,
      {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          "X-API-KEY": process.env.REACT_APP_API_KEY,
        },
      }
    );
    const data = await response.json();

    setFlaggedLinks(data); // set entire object, not just .links
    setUserLinks([]);
    setModalLinks(true);
  } catch (error) {
    console.error("Error fetching flagged links:", error);
    alert("Could not load flagged links");
  }
};



  const disabledStyle = {
    backgroundColor: "#b3d7ff",
    borderColor: "#b3d7ff",
    color: "#ffffff",
    opacity: 1,
    cursor: "not-allowed",
  };

  const disabledStyle2 = {
    backgroundColor: "#f8bcbc",
    borderColor: "#f8bcbc",
    color: "#ffffff",
    opacity: 1,
    cursor: "not-allowed",
  };

  const hasAnyDescriptions = userApps.some((app) => app.descriptionCount > 0);

  if (loading) {
    return <LoadingScreen />;
  }

  const deleteLink = async (linkToDelete) => {
    if (!window.confirm("Are you sure you want to delete this link?")) return;

    try {
      const response = await fetch(`${process.env.REACT_APP_API_BASE_URL}api/Details/delete-automated-link`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          "X-API-KEY": process.env.REACT_APP_API_KEY,
        },
        body: JSON.stringify({
          userId: userId,
          link: linkToDelete,
        }),
      });

      if (!response.ok) {
        throw new Error("Failed to delete link");
      }

      // Remove link from whichever list is currently displayed
      if (flaggedLinks.length > 0) {
        setFlaggedLinks((prevLinks) => prevLinks.filter((link) => link.url !== linkToDelete));
      } else {
        setUserLinks((prevLinks) => prevLinks.filter((link) => link.url !== linkToDelete));
      }
    } catch (error) {
      console.error("Error deleting link:", error);
      alert("Could not delete link.");
    }
  };

  const getFullLink = (platform, link) => {
    if (!link) return "";
    const isFull = link.startsWith("http");
    if (isFull) return link;

    const baseUrls = {
      Facebook: "https://facebook.com/",
      Instagram: "https://instagram.com/",
      Twitter: "https://twitter.com/",
      Linkedin: "https://linkedin.com/in/",
      TikTok: "https://tiktok.com/@",
      YouTube: "https://youtube.com/@",
    };

    const base = baseUrls[platform] || "https://";
    const cleaned = link.replace(/^@+/, ""); // remove leading @ if any
    return `${base}${cleaned}`;
  };

  return (
    <div className="container mt-5">
      <h2>User Detail</h2>
      {user ? (
        <div>
          <p>
            <strong>Full Name:</strong> {user.fullName}
          </p>
          <p>
            <strong>Title:</strong> {user.title}
          </p>
          <p>
            <strong>Status:</strong> {user.status ? "Submitted" : "Pending"}
          </p>
          <br />
          <button className="btn btn-secondary mb-2" onClick={() => setShowModal(true)}>
            Details
          </button>
          &nbsp;
          <button className="btn btn-secondary mb-2" onClick={fetchUserLinks}>
            Links
          </button>
          &nbsp;
          <button className="btn btn-secondary mb-2" onClick={fetchFlaggedLinks}>
            Flagged Links
          </button>
        </div>
      ) : null}

      {/* Social Media Links */}
      <h3 className="mt-4">Social Media Links</h3>
      <br />
      {isLoading ? (
        <p>Loading social media links...</p>
      ) : userApps.length > 0 ? (
        <ul className="list-group mt-3">
          {userApps.map((app, index) => (
            <div className="row" key={app.appId || index}>
              <div className="col-sm-3">
                <span>
                  <strong>{app.appName}</strong>
                </span>
              </div>
              <div className="col-sm-4">
                <a href={getFullLink(app.appName, app.link)} target="_blank" rel="noopener noreferrer">
                  {getFullLink(app.appName, app.link)}
                </a>
              </div>
              <div className="col-sm-3">
                <span>{app.descriptionCount}</span>
              </div>
              <div className="col-sm-2">
                <button className="btn btn-secondary mb-2" onClick={() => handleAddDescription(app)} disabled={app.status}>
                  Add Hate Speech
                </button>
              </div>
            </div>
          ))}
        </ul>
      ) : (
        <p>No social media links found for this user.</p>
      )}

      <br />
      <button className="btn btn-secondary mt-3 ms-2" onClick={() => navigate(-1)}>
        Back
      </button>
      <button
        className="btn btn-primary mt-3 ms-2"
        onClick={logClickAccept}
        disabled={hasAnyDescriptions}
        style={hasAnyDescriptions ? disabledStyle : {}}
      >
        Accept
      </button>
      <button
        className="btn btn-danger mt-3 ms-2"
        onClick={logClickReject}
        disabled={!hasAnyDescriptions}
        style={!hasAnyDescriptions ? disabledStyle2 : {}}
      >
        Reject
      </button>

      {/* User Details Modal */}
      {showModal && (
        <div className="modal show fade d-block" tabIndex="-1" style={{ backgroundColor: "rgba(0,0,0,0.5)" }}>
          <div className="modal-dialog modal-lg" style={{ maxWidth: "90vw", width: "auto" }}>
            <div className="modal-content">
              <div className="modal-header">
                <h5 className="modal-title">User Details</h5>
                <button type="button" className="btn-close" onClick={() => setShowModal(false)}></button>
              </div>
              <div className="modal-body">
                <p>
                  <strong>Full Name:</strong> {user?.fullName}
                </p>
                <p>
                  <strong>Title:</strong> {user?.title}
                </p>
                <p>
                  <strong>Biography:</strong> {user?.biography}
                </p>
                <p>
                  <strong>City:</strong> {user?.city}
                </p>
                <p>
                  <strong>Country:</strong> {user?.country}
                </p>
                <p>
                  <strong>Expertise:</strong> {user?.expertise}
                </p>
                <p>
                  <strong>Language:</strong> {user?.language}
                </p>
                <p>
                  <strong>Life Experience:</strong> {user?.lifeExperience}
                </p>
                <p>
                  <strong>Media Expertise:</strong> {user?.mediaExpertise}
                </p>
                <p>
                  <strong>Organization:</strong> {user?.organization}
                </p>
              </div>
              <div className="modal-footer">
                <button type="button" className="btn btn-secondary" onClick={() => setShowModal(false)}>
                  Close
                </button>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Automated Links Modal */}
     {showModalLinks && (
  <div
    className="modal show fade d-block"
    tabIndex={-1}
    style={{ backgroundColor: "rgba(0,0,0,0.5)" }}
  >
    <div className="modal-dialog modal-lg" style={{ maxWidth: "90vw", width: "auto" }}>
      <div className="modal-content">
        <div className="modal-header">
          <h5 className="modal-title">
            {flaggedLinks.length > 0 ? "Flagged Automated Links" : "Automated Links"}
          </h5>
          <button
            type="button"
            className="btn-close"
            onClick={() => setModalLinks(false)}
          />
        </div>
        <div className="modal-body" style={{ overflowX: "auto" }}>
          {flaggedLinks.length > 0 ? (
            // Flagged links with reason and hate speech
            flaggedLinks.map((link, index) => (
              <div key={index} style={{ marginBottom: "2rem" }}>
                <p>
                  <strong>URL:</strong>{" "}
                  <a href={link.url} target="_blank" rel="noopener noreferrer">
                    {link.url}
                  </a>
                </p>
                <div className="mb-2">
                  <strong>Reason:</strong>
                  <textarea
                    className="form-control"
                    rows={6}
                    value={updatedReasons[index] ?? link.reason}
                    onChange={(e) => handleReasonChange(index, e.target.value)}
                  />
                </div>
                <p>
                  <strong>Hate Speech:</strong> {link.hateSpeech}
                </p>
                <div className="d-flex gap-2">
                  <button
                    className="btn btn-sm btn-primary"
                    onClick={() => saveUpdatedReason(link.url, index)}
                  >
                    Save
                  </button>
                  <button
                    className="btn btn-sm btn-danger"
                    onClick={() => deleteLink(link.url)}
                  >
                    Delete
                  </button>
                </div>
                <hr />
              </div>
            ))
          ) : userLinks.length > 0 ? (
            // Normal links from userLinks
            <ul className="list-group">
              {userLinks.map((linkObj, index) => (
                <li key={index} className="list-group-item">
                  <a href={linkObj.url} target="_blank" rel="noopener noreferrer">
                    {linkObj.url}
                  </a>
                  <button
                    className="btn btn-sm btn-danger float-end"
                    onClick={() => deleteLink(linkObj.url)}
                  >
                    Delete
                  </button>
                </li>
              ))}
            </ul>
          ) : (
            <p>No links found.</p>
          )}
        </div>

        <div className="modal-footer">
          <button
            type="button"
            className="btn btn-secondary"
            onClick={() => setModalLinks(false)}
          >
            Close
          </button>
        </div>
      </div>
    </div>
  </div>
)}




    </div>
  );
};

export default Detail;
