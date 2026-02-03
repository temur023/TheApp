import axios from "axios";
import React, {useEffect, useState} from "react";
function GetAll(){
    const [users, setUsers] = useState([]);
    const fetchUsers = async () => {
    try {
        const token = localStorage.getItem("userToken");
        const response = await axios.get("http://localhost:5017/api/User/get-all", {
            headers: {
                Authorization: `Bearer ${token}`
            }
        });

        console.log("Full API Response:", response.data);

        const userList = response.data.data; 

    } catch (error) {
        console.error("Fetching users failed! ", error.response?.data || error.message);
    }
}
    useEffect(() => {
        fetchUsers();
    }, []);
    return(<>
        <div className="container-fluid">
            <div className="row  align-items-center mt-5">
                <div className="col-12 d-flex flex-column align-items-center"> 
                        <table className="table table-striped table-hover w-75">
                            <thead>
                            <tr>
                                <th>Name</th>
                                <th>Email</th>
                                <th>Status</th>
                                <th>Last seen</th>
                            </tr>
                            </thead>
                            <tbody>
                                    {users.map((user) => (
                                <tr key={user.id || user._id}>
                                    <td>{user.fullName}</td>
                                    <td>{user.email}</td>
                                    <td>
                                        <span className="badge bg-success">
                                            {user.status || "Active"}
                                        </span>
                                    </td>
                                    <td>{user.lastSeen || "Just now"}</td>
                                </tr>
                            ))}
                            </tbody>
                        </table>
                </div>    
            </div>
        </div>
    </>);
}
export default GetAll