import React, { useState } from "react";
import EmailInput from "../components/EmailInput";
import PasswordInput from "../components/PasswordInput";
import LoginButton from "../components/LoginButton";
import { useNavigate } from "react-router-dom";
import "../styles/main.css";
import "../styles/login.css";



export default function LoginPage() {
    const [showPassword, setShowPassword] = useState(false);
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const togglePassword = () => setShowPassword((v) => !v);
    const navigate = useNavigate();

    const handleLogin = (e) => {
        e.preventDefault();
    
    };

    return (
        <div className="login-page">
            <img src="/assets/background.jpg" alt="Background" className="background-image" />
            <div className="page-wrapper">
                <img src="/assets/Rope.png" alt="Rope" className="Rope" />
                <div className="auth-box" style={{ position: "relative" }}>
                    <div className="User-login" >
                        <div className="welcome-back" >Welcome Back!</div>
                        <div className="no-account">Don't have an account?</div>
                        <button className="register" title="register" onClick={() => navigate("/register")}>Register</button>
                        <EmailInput value={email} onChange={e => setEmail(e.target.value)} />
                        <PasswordInput value={password} onChange={e => setPassword(e.target.value)} showPassword={showPassword} togglePassword={togglePassword} />
                        <LoginButton onClick={handleLogin} />
                    </div>
                    <div className="circle"></div>
                </div>
            </div>
            <div className="diagram">
                <div className="box box-labeling">
                    <div className="tab">Labeling</div>
                </div>
                <div className="box box-tool">
                    <div className="tab">Tool</div>
                </div>
            </div>
            <img src="/assets/Rope.png" alt="Rope2" className="Rope2" />
            <img src="/assets/LabelCard.png" alt="LabelCard" className="LabelCard" />
        </div>
    );
}