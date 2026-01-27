import React, {useMemo} from "react";
import {useNavigate, useParams} from "react-router-dom";
import {useLang} from "../context/LangContext.jsx";
import {STRINGS} from "../i18n/strings.js";

export default function StudentCardPage() {
    const {lang} = useLang();
    const t = STRINGS[lang];
    const nav = useNavigate();
    const {token} = useParams();

    const studentsByToken = useMemo(
        () => ({
            "ukim-demo-token-123": {
                fullName: "Јован Јовановски",
                faculty:
                    "Факултет за информатички науки и компјутерско инженерство (ФИНКИ)",
                program: "Софтверско инженерство и информациски системи",
                index: "221133",
                credits: "162",
                semester: "7",
                enrollmentYear: "2022",
                studyMode: "Редовен",
                address: "ул. Партизански Одреди бр. 120",
                email: "jovan.jovanovsk@gmail.com",
                gpa: "9.10",
                phone: "070 123 456",
            },
        }),
        []
    );

    const student = studentsByToken[token];

    return (
        <div className="container">
            <div className="verify-top">
                <div className="status-pill">
                    <span className="status-dot"/>
                    <span className="status-text">{t.statusValid}</span>
                </div>

                <button className="ghost-btn" onClick={() => nav("/")}>
                    ← {t.backToHome}
                </button>
            </div>

            <section className="verify-hero">
                <h2 className="verify-title">{t.certified}</h2>
                <p className="verify-sub">
                    {student ? "Identity verified via student-specific QR token." : t.notFoundStudent}
                </p>
            </section>

            {student && (
                <section className="card-wrap">
                    <div className="student-card">
                        <div className="card-head">
                            <div className="card-mark">
                                <div className="ukim-logo small"/>
                            </div>

                            <div className="card-head-text">
                                <div className="card-title">{t.cardTitle}</div>
                                <div className="card-subtitle">
                                    UKIM • European Student Card
                                </div>
                            </div>

                            <div className="chip-valid">{t.statusValid}</div>
                        </div>

                        <div className="card-body">
                            <div className="row">
                                <Field label={t.labels.fullName} value={student.fullName}/>
                                <Field label={t.labels.index} value={student.index}/>
                            </div>

                            <div className="row">
                                <Field label={t.labels.faculty} value={student.faculty}/>
                                <Field label={t.labels.program} value={student.program}/>
                            </div>

                            <div className="row">
                                <Field label={t.labels.credits} value={student.credits}/>
                                <Field label={t.labels.semester} value={student.semester}/>
                            </div>

                            <div className="row">
                                <Field
                                    label={t.labels.enrollmentYear}
                                    value={student.enrollmentYear}
                                />
                                <Field label={t.labels.studyMode} value={student.studyMode}/>
                            </div>

                            <div className="row">
                                <Field
                                    className="span-2"
                                    label={t.labels.address}
                                    value={student.address}
                                />
                            </div>

                            <div className="row">
                                <Field label={t.labels.email} value={student.email}/>
                                <Field label={t.labels.gpa} value={student.gpa}/>
                            </div>

                            <div className="row">
                                <Field
                                    className="span-2"
                                    label={t.labels.phone}
                                    value={student.phone}
                                />
                            </div>
                        </div>

                        <div className="card-foot">
                            <div className="foot-note">{t.footerNote}</div>
                        </div>
                    </div>
                </section>
            )}
        </div>
    );
}

function Field({label, value, className = ""}) {
    return (
        <div className={`field ${className}`}>
            <div className="field-label">{label}</div>
            <div className="field-value">{value}</div>
        </div>
    );
}
