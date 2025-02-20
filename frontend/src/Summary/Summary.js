import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import '@fortawesome/fontawesome-free/css/all.min.css'; // Import Font Awesome CSS
import '../App.css'; // Import the CSS file

const Summary = () => {
    const [userDetails, setUserDetails] = useState([]);
    const [sortConfig, setSortConfig] = useState({ key: 'status', direction: 'ascending' });
    const [searchQuery, setSearchQuery] = useState('');
    const [currentPage, setCurrentPage] = useState(1);
    const [itemsPerPage] = useState(10); // Number of items per page
    const navigate = useNavigate();

    useEffect(() => {
        fetch(`${process.env.REACT_APP_API_BASE_URL}/api/Summary/userdetails`)
            .then(response => response.json())
            .then(data => setUserDetails(data));
    }, []);

    const sortedUserDetails = [...userDetails].sort((a, b) => {
        if (a[sortConfig.key] < b[sortConfig.key]) {
            return sortConfig.direction === 'ascending' ? -1 : 1;
        }
        if (a[sortConfig.key] > b[sortConfig.key]) {
            return sortConfig.direction === 'ascending' ? 1 : -1;
        }
        return 0;
    });

    const filteredUserDetails = sortedUserDetails.filter(user =>
        user.fullName.toLowerCase().includes(searchQuery.toLowerCase()) ||
        user.title.toLowerCase().includes(searchQuery.toLowerCase())
    );

    // Pagination logic
    const indexOfLastItem = currentPage * itemsPerPage;
    const indexOfFirstItem = indexOfLastItem - itemsPerPage;
    const currentItems = filteredUserDetails.slice(indexOfFirstItem, indexOfLastItem);

    const totalPages = Math.ceil(filteredUserDetails.length / itemsPerPage);

    const requestSort = key => {
        let direction = 'ascending';
        if (sortConfig.key === key && sortConfig.direction === 'ascending') {
            direction = 'descending';
        }
        setSortConfig({ key, direction });
    };

    const handleViewClick = (userId) => {
        navigate('/detail', { state: { userId } });
    };

    const handlePageChange = (pageNumber) => {
        setCurrentPage(pageNumber);
    };

    return (
        <div className="container mt-5">
            <h2>User Details</h2>
            <div className="row mb-3">
                <div className="col-4">
                    <input
                        type="text"
                        placeholder="Search by Full Name or Title"
                        className="form-control"
                        value={searchQuery}
                        onChange={e => setSearchQuery(e.target.value)}
                    />
                </div>
            </div>
            <table className="table table-bordered">
                <thead>
                    <tr>
                        <th className="text-center">Full Name</th>
                        <th className="text-center">Title</th>
                        <th className="text-center" onClick={() => requestSort('status')} style={{ cursor: 'pointer' }}>
                            Status {sortConfig.key === 'status' && (sortConfig.direction === 'ascending' ? '▲' : '▼')}
                        </th>
                        <th className="text-center">Action</th>
                    </tr>
                </thead>
                <tbody>
                    {currentItems.map(user => (
                        <tr key={user.userId}>
                            <td className="text-center">{user.fullName}</td>
                            <td className="text-center">{user.title}</td>
                            <td className="text-center">{user.status ? 'Submitted' : 'Pending'}</td>
                            <td className="text-center">
                                <i className="fas fa-eye" style={{ cursor: 'pointer' }} onClick={() => handleViewClick(user.userId)}></i>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
            <nav>
                <ul className="pagination justify-content-center">
                    {Array.from({ length: totalPages }, (_, index) => (
                        <li key={index + 1} className={`page-item ${currentPage === index + 1 ? 'active' : ''}`}>
                            <button className="page-link" onClick={() => handlePageChange(index + 1)}>
                                {index + 1}
                            </button>
                        </li>
                    ))}
                </ul>
            </nav>
        </div>
    );
};

export default Summary;