import { Link, useNavigate } from "react-router-dom";
import React, {useState} from "react";
import axios from "axios";
function RegisterPage() {
    const [fullName,setFullName] = useState("");
    const [email,setEmail] = useState("");
    const [password,setPassword] = useState("");
    const navigate = useNavigate()
    function handleFullName(e){
        setFullName(e.target.value)
    }
    function handleEmail(e){
        setEmail(e.target.value)
    }
    function handlePassword(e){
        setPassword(e.target.value)
    }
    const handleRegister = async () => {
        try{
            const registerData = {
            FullName: fullName,
            Email: email,
            Password: password
        }
        const response = await axios.post("http://localhost:5017/api/Auth/create", registerData)
        alert("Registration successful")
        navigate("/login")
        console.log("Registration successful!", response.data)
        }
        catch(error){
            console.error("Registration failed! ", error.response?.data || error.message)
        }
        
    }
    return(
    <>
       <div className="container font-family-Aerial justify-content-center align-items-center vh-100">
           <div className="row">
               <div className="col-6 d-flex flex-column gap-3 justify-content-center align-items-center d-flex vh-100">
                   <h1 className="mb-5">Create an Account</h1>
                   <input type="text" value={fullName} onChange={handleFullName} id="inputFullName" className="form-control-lg border-1" placeholder="Full Name" />
                   <input type="email" value={email} onChange={handleEmail} id="inputEmail" className="form-control-lg border-1" placeholder="Email" />
                   <input type="password" value={password} onChange={handlePassword} id="inputPassword" className="form-control-lg border-1" placeholder="Password" />
                   <button type="button" onClick={handleRegister} className="btn btn-primary btn-lg">Register</button>
                   <p className="mt-3">Already have an account? <Link to="/login">Sign in</Link></p>
               </div>
               <div className="col-6">
                   <img className="h-100" src="https://images.template.net/551106/Gradient-Background-edit-online.webp" alt="" />
               </div>
           </div>
       </div>
               
    </>
);
}
export default RegisterPage;