import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import LoginPage from "./LoginPage"
import RegisterPage from "./RegisterPage"
import GetAll from "./GetAll";

function App() {
  return (
    <>
    <BrowserRouter>
        <Routes>
            <Route path="/login" element={<LoginPage />}></Route>
            <Route path="/register" element={<RegisterPage />}></Route>
            <Route path="/getall" element={<GetAll />}></Route> 
            <Route path="/" element={<Navigate to={"/login"} />}></Route>
        </Routes>
        
    </BrowserRouter>
        
    </>
      
  )
}

export default App
