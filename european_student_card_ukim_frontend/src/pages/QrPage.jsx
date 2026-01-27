import React, {useMemo} from "react";
import {QRCodeCanvas} from "qrcode.react";
import {useLang} from "../context/LangContext.jsx";
import {STRINGS} from "../i18n/strings.js";

export default function QrPage() {
    const {lang} = useLang();
    const t = STRINGS[lang];

    const token = "221133";

    const verifyUrl = useMemo(() => {
        const origin = window.location.origin;
        return `${origin}/verify/${encodeURIComponent(token)}`;
    }, [token]);

    return (
        <div className="container">
            <div className="grid-2">
                <section className="panel">
                    <h2 className="panel-title">{t.afterLoginTitle}</h2>
                    <p className="panel-text">{t.afterLoginHint}</p>

                    <div className="qr-wrap">
                        <div className="qr-card">
                            <QRCodeCanvas value={verifyUrl} size={220} includeMargin/>
                        </div>

                        <div className="qr-meta">
                            <div className="meta-label">{t.scanHint}</div>
                            <code className="mono url-box">{verifyUrl}</code>

                            <a className="secondary-btn" href={`/verify/${encodeURIComponent(token)}`}>
                                {t.openVerify}
                            </a>
                        </div>
                    </div>
                </section>

                <aside className="panel subtle">
                    <div className="mini-title">{t.demoHowTitle}</div>
                    <ul className="nice-list">
                        {t.demoHowItems.map((x) => (
                            <li key={x}>{x}</li>
                        ))}
                    </ul>
                    <div className="note">{t.demoHowNote}</div>
                </aside>
            </div>
        </div>
    );
}
