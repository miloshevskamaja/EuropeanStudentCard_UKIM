import React from "react";
import {useNavigate} from "react-router-dom";
import {useLang} from "../context/LangContext.jsx";
import {STRINGS} from "../i18n/strings.js";

export default function LandingPage() {
    const nav = useNavigate();
    const {lang} = useLang();
    const t = STRINGS[lang];

    return (
        <div className="container">
            <section className="hero">
                <div className="hero-card">
                    <div className="hero-badge">{t.subtitle}</div>
                    <h1 className="hero-title">{t.headerTitle}</h1>
                    <p className="hero-text">
                        Demo front-end со 3 чекори: login → QR → верификација/картичка.
                    </p>

                    <div className="hero-actions">
                        <button className="primary-btn" onClick={() => nav("/qr")} type="button">
                            {t.login}
                        </button>
                    </div>

                    <div className="hero-footnote">{t.footerNote}</div>
                </div>

                <div className="hero-side">
                    <div className="glass-card">
                        <div className="glass-title">{t.demoPreviewTitle}</div>
                        {t.demoPreviewItems.map((x) => (
                            <div className="glass-row" key={x}>
                                <span className="dot"/>
                                <span>{x}</span>
                            </div>
                        ))}
                    </div>
                </div>
            </section>
        </div>
    );
}
