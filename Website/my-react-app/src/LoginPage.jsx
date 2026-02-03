import React, {useState} from "react";
import axios from 'axios'
import { Link, useNavigate } from "react-router-dom";
function LoginPage() {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const navigate = useNavigate();
    function handleEmailChange(event) {
        setEmail(event.target.value);
    }

    function handlePasswordChange(event) {
        setPassword(event.target.value);
    }
const handleLogin = async () => {
    try {
        const loginData = { Email: email, Password: password };
        const response = await axios.post("http://localhost:5017/api/Auth/login", loginData);
        
        const token = response.data.message;

        if (token) {
            localStorage.setItem("userToken", token);
            console.log("Token saved successfully!");
            navigate("/getall"); 
        } else {
            console.error("Token missing in 'message' field:", response.data);
        }
    } catch (error) {
        console.error("Logging failed", error.response?.data || error.message);
    }
}
    return (
        <>
            <div className="container font-family-Aerial">
                
                <div className="row">
                    <div className="col-6 d-flex flex-column gap-3 justify-content-center align-items-center d-flex vh-100">
                        <h1 className="mb-5">Sign in to The App</h1>
                        <input value={email} onChange={handleEmailChange} type="email" id="inputEmail5" className="form-control-lg border-1" placeholder="Email" />
                        <input value={password} onChange={handlePasswordChange} type="password" id="inputPassword5" className="form-control-lg border-1" placeholder="Password" />
                        <button onClick={handleLogin} type="button" className="btn btn-primary btn-lg">Login</button>
                        <p className="mt-3">Don't have an account? <Link to="/register">Sign up</Link></p>
                    </div>
                    <div className="col-lg-6">
                        <img className="h-100" src="https://images.template.net/551106/Gradient-Background-edit-online.webp" alt="" />
                    </div>
                </div>
            </div>
        </>
    );
}
export default LoginPage;