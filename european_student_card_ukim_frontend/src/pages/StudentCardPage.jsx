import React, {useEffect, useMemo, useState} from "react";
import {useNavigate, useParams} from "react-router-dom";
import {useLang} from "../context/LangContext.jsx";
import {STRINGS} from "../i18n/strings.js";
import useIKnow from "../hooks/useIKnow.js";

export default function StudentCardPage() {
    const {lang} = useLang();
    const t = STRINGS[lang];
    const nav = useNavigate();
    const {token} = useParams();

    const {getStudentByIndex, loading} = useIKnow();

    const [student, setStudent] = useState(null);
    const [notFound, setNotFound] = useState(false);

    useEffect(() => {
        if (!token) return;

        // eslint-disable-next-line react-hooks/set-state-in-effect
        setNotFound(false);
        setStudent(null);

        getStudentByIndex(token)
            .then((data) => {
                if (!data) {
                    setNotFound(true);
                    return;
                }
                setStudent(data);
            })
            .catch(() => setNotFound(true));
    }, [token, getStudentByIndex]);

    const viewModel = useMemo(() => {
        if (!student) return null;

        const isRegular = student.status === 1;
        const isExtraordinary = student.status === -1;

        return {
            fullName: `${student.name ?? ""} ${student.surname ?? ""}`.trim(),
            index: student.index ?? "",
            program: student.programmeName ?? "",
            enrollmentYear: student.enrollmentYear ?? "",
            credits: student.ects ?? "",
            gpa: student.gpa ?? "",
            email: student.email ?? "",
            phone: student.phone ?? "",
            address: student.address ?? "",
            studyMode: isRegular ? "Редовен" : isExtraordinary ? "Вонреден" : "",
            isValid: isRegular,
        };
    }, [student]);

    const isValid = viewModel?.isValid === true;

    return (
        <div className="container">
            <div className="verify-top">
                <div className={`status-pill ${isValid ? "" : "invalid"}`}>
                    <span className="status-dot"/>
                    <span className="status-text">
            {loading
                ? t.loading ?? "Loading..."
                : viewModel
                    ? (isValid ? t.statusValid : (t.statusInvalid ?? "NOT VALID"))
                    : (notFound ? t.notFoundStudent : "")}
          </span>
                </div>

                <button className="ghost-btn" onClick={() => nav("/")}>
                    ← {t.backToHome}
                </button>
            </div>

            <section className={`verify-hero ${isValid ? "" : "invalid"}`}>
                <h2 className="verify-title">{t.certified}</h2>
                <p className="verify-sub">
                    {loading
                        ? (t.loading ?? "Loading...")
                        : viewModel
                            ? (isValid
                                ? (t.validStudent)
                                : (t.notEligibleEsc ?? t.notValidStudent))
                            : (notFound ? t.notFoundStudent : "")}
                </p>
            </section>

            {viewModel && (
                <section className="card-wrap">
                    <div className={`student-card ${isValid ? "" : "card-invalid"}`}>
                        <div className="card-head">
                            <div className="card-mark">
                                <div className="ukim-logo small"/>
                            </div>

                            <div className="card-head-text">
                                <div className="card-title">{t.cardTitle}</div>
                                <div className="card-subtitle">{t.subtitle}</div>
                            </div>

                            <div className={`chip-valid ${isValid ? "" : "chip-invalid"}`}>
                                {isValid ? t.statusValid : (t.statusInvalid ?? "NOT VALID")}
                            </div>
                        </div>

                        <div className="card-body">
                            <div className="row">
                                <Field label={t.labels.fullName} value={viewModel.fullName}/>
                                <Field label={t.labels.index} value={viewModel.index}/>
                            </div>

                            <div className="row">
                                <Field label={t.labels.program} value={viewModel.program}/>
                                <Field label={t.labels.enrollmentYear} value={viewModel.enrollmentYear}/>
                            </div>

                            <div className="row">
                                <Field label={t.labels.credits} value={viewModel.credits}/>
                                <Field label={t.labels.gpa} value={viewModel.gpa}/>
                            </div>

                            <div className="row">
                                <Field label={t.labels.studyMode} value={viewModel.studyMode}/>
                                <Field label={t.labels.email} value={viewModel.email}/>
                            </div>

                            <div className="row">
                                <Field className="span-2" label={t.labels.address} value={viewModel.address}/>
                            </div>

                            <div className="row">
                                <Field className="span-2" label={t.labels.phone} value={viewModel.phone}/>
                            </div>
                        </div>

                        <div className="card-foot">
                            <div className="foot-note">
                                {isValid
                                    ? t.footerNote
                                    : (t.footerNoteInvalid ?? "This student card is not valid for ESC benefits.")}
                            </div>
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
