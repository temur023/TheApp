import React, {useState} from "react";
import axios from 'axios'
import { Link, useNavigate } from "react-router-dom";
function LoginPage() {
    const api_url = "https://believable-wisdom-production.up.railway.app"
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [error,setError] = useState("");
    const navigate = useNavigate();
    function handleEmailChange(event) {
        setEmail(event.target.value);
        setError("");
    }

    function handlePasswordChange(event) {
        setPassword(event.target.value);
        setError("");   
    }
const handleLogin = async () => {
    try {
        setError("");
        const loginData = { Email: email, Password: password };
        const response = await axios.post(`${api_url}/api/Auth/login`, loginData);
        
        const token = response.data.message || response.data.Message;

        localStorage.setItem("userToken", token);
        console.log("Token saved successfully!");
        navigate("/getall"); 
    
    } catch (error) {
        const data = error.response?.data;
        const serverMessage = data?.message || data?.Message || "Invalid email or password.";
        setError(serverMessage);
    }
}
    return (
        <>
            <div className="container font-family-Aerial">
                <div className="row">
                    <div className="col-lg-6 col-sm-12 d-flex flex-column gap-3 justify-content-center align-items-center d-flex vh-100">
                        <h1 className="mb-5">Sign in to The App</h1>
                        {
                            error&&(
                                <div className="alert alert-danger w-75 text-center" role="alert">
                                    {error}
                                </div>
                            )
                        }
                        <input value={email} onChange={handleEmailChange} type="email" id="inputEmail5" className="form-control-lg border-1" placeholder="Email" />
                        <input value={password} onChange={handlePasswordChange} type="password" id="inputPassword5" className="form-control-lg border-1" placeholder="Password" />
                        <button onClick={handleLogin} type="button" className="btn btn-primary btn-lg">Login</button>
                        <p className="mt-3">Don't have an account? <Link to="/register">Sign up</Link></p>
                    </div>
                    <div className="col-lg-6 d-none d-lg-block">
                        <img className="h-100" src="https://images.template.net/551106/Gradient-Background-edit-online.webp" alt="" />
                    </div>
                </div>
            </div>
        </>
    );
}
export default LoginPage;