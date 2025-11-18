import React, { useState } from "react";
import FirstNameInput from "../components/FirstNameInput";
import LastNameInput from "../components/LastNameInput";
import EmailInput from "../components/EmailInput";
import PasswordInput from "../components/PasswordInput";
import RegisterButton from "../components/RegisterButton";
import ConfirmPassword from "../components/ConfirmPassword";
import { useNavigate } from "react-router-dom";
import "../styles/main.css";
import "../styles/login.css";

export default function RegisterPage() {

    const [firstName, setFirstName] = useState("");
    const [lastName, setLastName] = useState("");
    const [email, setEmail] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const [password, setPassword] = useState("");
    const [showPassword, setShowPassword] = useState(false);
    const togglePassword = () => setShowPassword((v) => !v);

    const navigate = useNavigate();

    const handleRegister = (e) => {
        e.preventDefault();

    };

    return (
        <div className="login-page">
            <img src="/assets/background.jpg" alt="Background" className="background-image" />
            <div className="page-wrapper">
                <img src="/assets/Rope.png" alt="Rope" className="Rope" />
                <div className="auth-box" style={{ position: "relative" }}>
                    <div className="User-login" >
                        <div className="create-new-account" >Create New Account</div>
                        <div className="already-have-account">Already have an account?</div>
                        <button className="login" title="login" onClick={() => navigate("/login")}>Login</button>
                        <br /><br />
                        <FirstNameInput value={firstName} onChange={e => setFirstName(e.target.value)} />
                        <LastNameInput value={lastName} onChange={e => setLastName(e.target.value)} />
                        <EmailInput value={email} onChange={e => setEmail(e.target.value)} />
                        <PasswordInput value={password} onChange={e => setPassword(e.target.value)} showPassword={showPassword} togglePassword={togglePassword} />
                        <ConfirmPassword value={confirmPassword} onChange={e => setConfirmPassword(e.target.value)} />
                        <RegisterButton onClick={handleRegister} />
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