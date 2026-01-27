export const STRINGS = {
    mk: {
        uniKicker: "Универзитет „Св. Кирил и Методиј“ во Скопје",
        headerTitle: "Европска Студентска Картичка",

        login: "Најави се",
        subtitle: "UKIM • European Student Card (Demo)",

        afterLoginTitle: "Твој QR код",
        afterLoginHint:
            "Овој QR код (демо) води до верификација. Во реална верзија ќе се генерира по студент/токен.",
        scanHint: "Ако го скенираш, ќе отвори:",
        openVerify: "Отвори верификација",

        certified: "Сертифициран студент на УКИМ",
        statusValid: "VALID",
        cardTitle: "Студентска Картичка",

        demoPreviewTitle: "Преглед",
        demoHowTitle: "Како работи",
        demoPreviewItems: ["Responsive layout", "MK/EN label-и", "QR → verification page"],
        demoHowItems: [
            "QR кодот содржи URL до /verify/:token",
            "Scanner отвора web страница (оваа апликација)",
            "Страната прикажува “Certified” + студентска картичка",
        ],
        demoHowNote:
            "Подоцна: /verify/:token ќе повикува API за да земе податоци според токенот.",

        labels: {
            fullName: "Име и презиме",
            faculty: "Факултет",
            program: "Насока",
            index: "Индекс",
            credits: "Кредити",
            semester: "Семестар",
            enrollmentYear: "Година на упис",
            studyMode: "Редовен/Вонреден",
            address: "Адреса на живеење",
            email: "Е-пошта",
            gpa: "Просек",
            phone: "Телефон",
        },

        footerNote:
            "Демо интерфејс (хардкод). Податоците подоцна се вчитуваат од ASP.NET Core API.",
        notFoundStudent: "Непознат/невалиден QR токен.",
        backToHome: "Назад",
    },

    en: {
        uniKicker: 'Ss. Cyril and Methodius University in Skopje',
        headerTitle: "European Student Card",

        login: "Log in",
        subtitle: "UKIM • European Student Card (Demo)",

        afterLoginTitle: "Your QR code",
        afterLoginHint:
            "This demo QR code opens the verification page. In production it will be generated per student/token.",
        scanHint: "When scanned, it opens:",
        openVerify: "Open verification",

        certified: "Certified UKIM student",
        statusValid: "VALID",
        cardTitle: "Student Card",

        demoPreviewTitle: "Preview",
        demoHowTitle: "How it works",
        demoPreviewItems: ["Responsive layout", "MK/EN labels", "QR → verification page"],
        demoHowItems: [
            "The QR contains a URL to /verify/:token",
            "A scanner opens the web page (this app)",
            "The page shows “Certified” + a student card",
        ],
        demoHowNote:
            "Later: /verify/:token will call the API and fetch data by token.",

        labels: {
            fullName: "Full name",
            faculty: "Faculty",
            program: "Program",
            index: "Index",
            credits: "Credits",
            semester: "Semester",
            enrollmentYear: "Enrollment year",
            studyMode: "Full-time / Part-time",
            address: "Address",
            email: "Email",
            gpa: "GPA",
            phone: "Phone",
        },

        footerNote:
            "Demo UI (hardcoded). Later the data will be fetched from ASP.NET Core API.",
        notFoundStudent: "Unknown/invalid QR token.",
        backToHome: "Back",
    },
};
