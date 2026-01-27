import React from "react";
import {Navigate, Route, Routes} from "react-router-dom";
import Header from "./components/Header.jsx";
import LandingPage from "./pages/LandingPage.jsx";
import QrPage from "./pages/QrPage.jsx";
import StudentCardPage from "./pages/StudentCardPage.jsx";

export default function App() {
    return (
        <div className="app-shell">
            <Header/>
            <main className="app-main">
                <Routes>
                    <Route path="/" element={<LandingPage/>}/>
                    <Route path="/qr" element={<QrPage/>}/>
                    <Route path="/verify/:token" element={<StudentCardPage/>}/>
                    <Route path="*" element={<Navigate to="/" replace/>}/>
                </Routes>
            </main>
        </div>
    );
}
