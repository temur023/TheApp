import axios from "axios";
import React, {useEffect, useState, useCallback} from "react";
import { useNavigate } from "react-router";
import { Link } from "react-router";
function GetAll(){
    const [users, setUsers] = useState([]);
    const [loading, setLoading] = useState(false);
    const [total, setTotal] = useState(0);
    const [checkedUsers, setCheckedUsers] = useState([]);
    const navigate = useNavigate();
    const [filter, setFilter] = useState({
        name: "",
        status: "",
        pageNumber: 1,
        pageSize: 10
})

    const logout = async ()=>{
        localStorage.removeItem("userToken");
        navigate("/login")
    }
    const unblockSelected = async ()=>{
        try{
         if (!window.confirm("Are you sure you want to unblock all selected users?")) return;
            setLoading(true)
            const token = localStorage.getItem("userToken")
            if(checkedUsers.length>0){
                const response = await axios.put("http://localhost:5017/api/User/unblock-selected",checkedUsers,{
                    headers: {
                        Authorization: `Bearer ${token}`
                    }
                })
                window.alert("Selected users have been unblocked!");
                setCheckedUsers([]);
                fetchUsers();
                setLoading(false)
            }
            else{
                window.alert("No users selected")
            }
        }
        catch(error){
            console.error("Error: ", error.response?.data || error.message)
        }
    }
    const blockSelected = async ()=>{
        try{
         if (!window.confirm("Are you sure you want to block all selected users?")) return;
            setLoading(true)
            const token = localStorage.getItem("userToken")
            if(checkedUsers.length>0){
                const response = await axios.put("http://localhost:5017/api/User/block-selected",checkedUsers,{
                    headers: {
                        Authorization: `Bearer ${token}`
                    }
                })
                window.alert("Selected users have been blocked!");
                setCheckedUsers([]);
                fetchUsers();
                setLoading(false)
            }
            else{
                window.alert("No users selected")
            }
        }
        catch(error){
            console.error("Error: ", error.response?.data || error.message)
        }
    }
    const deleteSelected = async ()=>{
        try{
            if (!window.confirm("Are you sure you want to delete all selected users?")) return;
            setLoading(true)
            const token = localStorage.getItem("userToken")
            if(checkedUsers.length>0){
                const response = await axios.delete("http://localhost:5017/api/User/delete-selected",{
                    headers: {
                        Authorization: `Bearer ${token}`
                    }
                    ,data: checkedUsers
                })
                window.alert("Selected users have been deleted!");
                setCheckedUsers([]);
                fetchUsers();
                setLoading(false)
            }
            else{
                window.alert("No users selected")
            }
        }
        catch(error){
            console.error("Error: ", error.response?.data || error.message)
        }
    }
    function handleCheckingUsers(id){
        setCheckedUsers(c=>c.includes(id)?c.filter((userId)=>userId!==id):[...c, id]);
    }

    function handleCheckingAllUsers(){
        if(checkedUsers.length===users.length&&users.length>0){
            setCheckedUsers([]);
        }
        else{
           const allIds = users.map(u => u.id);
            setCheckedUsers(allIds);
        }
    }
const fetchUsers = useCallback(async () => {
    try {
        const token = localStorage.getItem("userToken");

        const requestParams = {
            PageNumber: filter.pageNumber,
            PageSize: filter.pageSize
        };

        if (filter.name?.trim()) {
            requestParams.Name = filter.name.trim();
        }

        if (filter.status && filter.status !== "") {
            requestParams.Status = filter.status; 
        }

        const response = await axios.get("http://localhost:5017/api/User/get-all", {
            headers: { Authorization: `Bearer ${token}` },
            params: requestParams
        });

        setUsers(response.data.data || []);
        setTotal(response.data.totalRecords || 0);

    } catch (error) {
        console.error("Fetching users failed!", error);
        setUsers([]);
    }
}, [filter]); 
useEffect(() => {
        const delayDebounceFn = setTimeout(() => {
            fetchUsers();
        }, 500);
        return () => clearTimeout(delayDebounceFn);
    }, [fetchUsers]);
useEffect(() => {
    setCheckedUsers([]);
}, [users]);

    const deleteUnverifiedUsers = async () =>{
        try{
            if (!window.confirm("Are you sure you want to delete all unverified users?")) return;
            setLoading(true)
            const token = localStorage.Item("userToken")
            const response = await axios.delete("http://localhost:5017/api/User/delete-unverified",{
                headers: {
                    Authorization: `Bearer ${token}`
                }
            });
            alert(response.data.message || "Unverified users deleted successfully!");
            setLoading(false)
        }
        catch(error){
            console.error("Error: ", error .response?.data || error.message)
        }
    }
return (
        <div className="container-fluid mt-5">
            <div className="row justify-content-center">
                <div className="col-12 w-75">
                    <div className="d-flex justify-content-end mb-2">
                    <button 
                        onClick={logout} 
                        className="btn btn-outline-dark btn-sm fw-bold px-4"
                    >
                        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" className="bi bi-box-arrow-right me-2" viewBox="0 0 16 16">
                          <path fillRule="evenodd" d="M10 12.5a.5.5 0 0 1-.5.5h-8a.5.5 0 0 1-.5-.5v-9a.5.5 0 0 1 .5-.5h8a.5.5 0 0 1 .5.5v2a.5.5 0 0 0 1 0v-2A1.5 1.5 0 0 0 9.5 2h-8A1.5 1.5 0 0 0 0 3.5v9A1.5 1.5 0 0 0 1.5 14h8a1.5 1.5 0 0 0 1.5-1.5v-2a.5.5 0 0 0-1 0z"/>
                          <path fillRule="evenodd" d="M15.854 8.354a.5.5 0 0 0 0-.708l-3-3a.5.5 0 0 0-.708.708L14.293 7.5H5.5a.5.5 0 0 0 0 1h8.793l-2.147 2.146a.5.5 0 0 0 .708.708z"/>
                        </svg>
                        Logout
                    </button>
                </div>
                    <div className="row mb-3 bg-light p-3 rounded border mx-0">
        
        <div className="col-md-6 d-flex gap-2">
            
            <button 
                className="btn btn-danger"
                onClick={deleteSelected}
                disabled={loading || checkedUsers.length === 0}
                title="delete selected users"
            >
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" clasname="bi bi-trash3-fill" viewBox="0 0 16 16">
                  <path d="M11 1.5v1h3.5a.5.5 0 0 1 0 1h-.538l-.853 10.66A2 2 0 0 1 11.115 16h-6.23a2 2 0 0 1-1.994-1.84L2.038 3.5H1.5a.5.5 0 0 1 0-1H5v-1A1.5 1.5 0 0 1 6.5 0h3A1.5 1.5 0 0 1 11 1.5m-5 0v1h4v-1a.5.5 0 0 0-.5-.5h-3a.5.5 0 0 0-.5.5M4.5 5.029l.5 8.5a.5.5 0 1 0 .998-.06l-.5-8.5a.5.5 0 1 0-.998.06m6.53-.528a.5.5 0 0 0-.528.47l-.5 8.5a.5.5 0 0 0 .998.058l.5-8.5a.5.5 0 0 0-.47-.528M8 4.5a.5.5 0 0 0-.5.5v8.5a.5.5 0 0 0 1 0V5a.5.5 0 0 0-.5-.5"/>
                </svg>
            </button>
            <button 
                className="btn btn-secondary"
                onClick={blockSelected}
                disabled={loading || checkedUsers.length === 0}
                title="block selected users"
            >
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" clasname="bi bi-dash-circle-fill" viewBox="0 0 16 16">
                  <path d="M16 8A8 8 0 1 1 0 8a8 8 0 0 1 16 0M4.5 7.5a.5.5 0 0 0 0 1h7a.5.5 0 0 0 0-1z"/>
                </svg> Block
            </button>
            <button 
                className="btn btn-outline-secondary"
                onClick={unblockSelected}
                disabled={loading || checkedUsers.length === 0}
                title="unblock selected users"
            >
               <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" clasname="bi bi-dash-circle" viewBox="0 0 16 16">
                  <path d="M8 15A7 7 0 1 1 8 1a7 7 0 0 1 0 14m0 1A8 8 0 1 0 8 0a8 8 0 0 0 0 16"/>
                  <path d="M4 8a.5.5 0 0 1 .5-.5h7a.5.5 0 0 1 0 1h-7A.5.5 0 0 1 4 8"/>
                </svg>
            </button>
            
            <button 
                className="btn btn-outline-danger" 
                onClick={deleteUnverifiedUsers} 
                disabled={loading}
                title="delete unverified users"
            >
               <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" clasname  ="bi bi-eraser-fill" viewBox="0 0 16 16">
                  <path d="M8.086 2.207a2 2 0 0 1 2.828 0l3.879 3.879a2 2 0 0 1 0 2.828l-5.5 5.5A2 2 0 0 1 7.879 15H5.12a2 2 0 0 1-1.414-.586l-2.5-2.5a2 2 0 0 1 0-2.828zm.66 11.34L3.453 8.254 1.914 9.793a1 1 0 0 0 0 1.414l2.5 2.5a1 1 0 0 0 .707.293H7.88a1 1 0 0 0 .707-.293z"/>
                </svg>
            </button>
        </div>

        <div className="col-md-6">
            <div className="row g-2 justify-content-end">
                <div className="col-auto">
                    <input 
                        type="text" 
                        className="form-control" 
                        placeholder="Filter by Name"
                        value={filter.name}
                        onChange={(e) => setFilter({...filter, name: e.target.value, pageNumber: 1})} 
                    />
                </div>
                <div className="col-auto">
                    <select 
                        className="form-select"
                        value={filter.status ?? ""} 
                        onChange={(e) => setFilter({
                            ...filter, 
                            status: e.target.value === "" ? null : e.target.value, 
                            pageNumber: 1
                        })}
                    >
                        <option value="">All Statuses</option>
                        <option value="Unverified">Unverified</option>
                        <option value="Active">Active</option>
                        <option value="Blocked">Blocked</option>
                    </select>
                </div>
            </div>
        </div>
    </div>
                    <table className="table table-striped table-hover">
                        <thead>
                            <tr>
                                <th>
                                    <input 
                                        type="checkbox" 
                                        className="form-check-input" 
                                        onChange={handleCheckingAllUsers}
                                        checked={users.length > 0 && checkedUsers.length === users.length}
                                    />
                                </th>
                                <th>Name</th>
                                <th>Email</th>
                                <th>Status</th>
                                <th>Last seen</th>
                            </tr>
                        </thead>
                        <tbody>
                            {users.map((user) => (
                                <tr key={user.id}>
                                    <td>
                                        <input 
                                            type="checkbox" 
                                            className="form-check-input" 
                                            checked={checkedUsers.includes(user.id)}
                                            onChange={() => handleCheckingUsers(user.id)}
                                        />
                                    </td> 
                                    <td>{user.fullName}</td>
                                    <td>{user.email}</td>
                                    <td>
                                        <span className={`badge ${
                                            user.status==="Active" ? "bg-success" :
                                            user.status==="Unverified" ? "bg-warning" :
                                            "bg-danger"
                                        }` }  >
                                            {user.status}
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
    );
}
export default GetAll