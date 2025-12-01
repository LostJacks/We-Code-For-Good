import React, { useState } from "react";
import EmailInput from "../components/EmailInput";
import PasswordInput from "../components/PasswordInput";
import LoginButton from "../components/LoginButton";
import { useNavigate } from "react-router-dom";
import "../styles/main.css";
import "../styles/login.css";

const API_BASE = import.meta.env.VITE_API_URL || "";

export default function LoginPage() {
    const [showPassword, setShowPassword] = useState(false);
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");
    const togglePassword = () => setShowPassword((v) => !v);
    const navigate = useNavigate();

    const handleLogin = async (e) => {
        e.preventDefault();
        setError("");
        try {
            const payload = {
                email,
                password
            };
            const res = await fetch(`${API_BASE}/api/auth/login`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(payload),
            });
            if (!res.ok) {
                const txt = await res.text();
                throw new Error(txt || "Login failed");
            }
            const data = await res.json();
            console.log("API Response:", data);
            console.log("accessToken:", data.accessToken);
            console.log("refreshToken:", data.refreshToken);

            if (!data.accessToken) {
                console.error("No accessToken in response!");
                throw new Error("No token received from server");
            }

            localStorage.setItem("accessToken", data.accessToken);
            localStorage.setItem("refreshToken", data.refreshToken);

            console.log("Tokens stored in localStorage");
            console.log("localStorage contents:", localStorage);
            
            navigate("/main");
        } catch (err) {
            console.error("Login error:", err);
            setError(err.message || "Login failed");
        }
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
                        {error && <div style={{ color: "red", marginTop: 8 }}>{error}</div>}
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