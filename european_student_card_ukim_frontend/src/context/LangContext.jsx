import React, {createContext, useContext, useEffect, useMemo, useState} from "react";

const LangContext = createContext(null);

export function LangProvider({children}) {
    const [lang, setLang] = useState(() => localStorage.getItem("lang") || "mk");

    useEffect(() => {
        localStorage.setItem("lang", lang);
    }, [lang]);

    const value = useMemo(() => ({lang, setLang}), [lang]);
    return <LangContext.Provider value={value}>{children}</LangContext.Provider>;
}

// eslint-disable-next-line react-refresh/only-export-components
export function useLang() {
    const ctx = useContext(LangContext);
    if (!ctx) throw new Error("useLang must be used within LangProvider");
    return ctx;
}
