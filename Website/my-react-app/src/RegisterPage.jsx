import { data, Link, useNavigate } from "react-router-dom";
import React, {useState} from "react";
import axios from "axios";
function RegisterPage() {
    const api_url = "https://believable-wisdom-production.up.railway.app"
    const [fullName,setFullName] = useState("");
    const [email,setEmail] = useState("");
    const [password,setPassword] = useState("");
    const [checkPassword, setCheckPassword] = useState("")
    const [error, setError] = useState("");
    const navigate = useNavigate()
    function handleFullName(e){
        setFullName(e.target.value)
    }
    function handleEmail(e){
        setEmail(e.target.value)
        setError("")
    }
    function handlePassword(e){
        setPassword(e.target.value)
        setError("")
    }
    function handleCheckPassword(e){
        setCheckPassword(e.target.value)
        setError("")
    }
   const handleRegister = async () => {
    try {
        setError("");
        const response = await axios.post(`${api_url}/api/Auth/create`, registerData);
        
        if (response.data.statusCode === 200 || response.data.StatusCode === 200) {
            navigate("/login"); 
        } else {
            setError(response.data.message || response.data.Message);
        }
    } catch (error) {
    const msg = error.response?.data?.message || 
                error.response?.data?.Message || 
                error.message || 
                "Connection Error";
    setError(msg);
}
}
    return(
    <>
       <div className="container font-family-Aerial justify-content-center align-items-center vh-100">
           <div className="row">
               <div className="col-lg-6 flex-column gap-3 justify-content-center align-items-center d-flex vh-100">
                   <h1 className="mb-5">Create an Account</h1>
                   {
                        error&&(
                            <div className="alert alert-danger w-75 text-center" role="alert">
                                {error}
                            </div>
                        )
                    }
                   <input type="text" value={fullName} onChange={handleFullName} id="inputFullName" className="form-control-lg border-1" placeholder="Full Name" />
                   <input type="email" value={email} onChange={handleEmail} id="inputEmail" className="form-control-lg border-1" placeholder="Email" />
                   <input type="password" value={password} onChange={handlePassword} id="inputPassword" className="form-control-lg border-1" placeholder="Password" />
                   <input type="password" value={checkPassword} onChange={handleCheckPassword} id="inputPassword" className="form-control-lg border-1" placeholder="Check the Password" />
                   <button type="button" onClick={handleRegister} className="btn btn-primary btn-lg">Register</button>
                   <p className="mt-3">Already have an account? <Link to="/login">Sign in</Link></p>
               </div>
               <div className="col-lg-6 d-none d-lg-block">
                   <img className="h-100 w-auto" src="https://images.template.net/551106/Gradient-Background-edit-online.webp" alt="" />
               </div>
           </div>
       </div>
               
    </>
);
}
export default RegisterPage;