import React, {useEffect, useRef, useState} from "react";
import {useLang} from "../context/LangContext.jsx";
import {useTheme} from "../context/ThemeContext.jsx";
import {STRINGS} from "../i18n/strings.js";
import ukimLogo from "../assets/ukim-logo-white.svg"

export default function Header() {
    const {lang, setLang} = useLang();
    const {theme, toggleTheme} = useTheme();
    const t = STRINGS[lang];

    const [open, setOpen] = useState(false);
    const menuRef = useRef(null);

    useEffect(() => {
        const onDown = (e) => {
            if (!menuRef.current) return;
            if (!menuRef.current.contains(e.target)) setOpen(false);
        };
        document.addEventListener("mousedown", onDown);
        return () => document.removeEventListener("mousedown", onDown);
    }, []);

    return (
        <header className="topbar">
            <div className="topbar-inner">
                <div className="topbar-left">
                    <div className="ukim-logo" aria-label="UKIM logo"><img src={ukimLogo} alt="UKIM Logo"/></div>
                </div>

                <div className="topbar-center">
                    <div className="topbar-kicker">{t.uniKicker}</div>
                    <div className="topbar-title">{t.headerTitle}</div>
                </div>

                <div className="topbar-right" ref={menuRef}>
                    <button
                        className="icon-btn"
                        type="button"
                        aria-label="Profile menu"
                        aria-expanded={open ? "true" : "false"}
                        onClick={() => setOpen((v) => !v)}
                    >
                        <span className="icon-user"/>
                    </button>

                    {open && (
                        <div className="dropdown">
                            <div className="dropdown-section">
                                <div className="dropdown-title">Language</div>
                                <div className="dropdown-row">
                                    <button
                                        type="button"
                                        className={lang === "mk" ? "pill-btn active" : "pill-btn"}
                                        onClick={() => setLang("mk")}
                                    >
                                        MK
                                    </button>
                                    <button
                                        type="button"
                                        className={lang === "en" ? "pill-btn active" : "pill-btn"}
                                        onClick={() => setLang("en")}
                                    >
                                        EN
                                    </button>
                                </div>
                            </div>

                            <div className="dropdown-divider"/>

                            <div className="dropdown-section">
                                <div className="dropdown-title">Theme</div>
                                <button type="button" className="theme-toggle" onClick={toggleTheme}>
                                    <span className="theme-label">{theme === "dark" ? "Dark" : "Light"}</span>
                                    <span className={theme === "dark" ? "switch on" : "switch"}>
                    <span className="knob"/>
                  </span>
                                </button>
                            </div>
                        </div>
                    )}
                </div>
            </div>
        </header>
    );
}
