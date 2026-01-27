import {useCallback, useEffect, useState} from "react";
import iKnowRepository from "../repository/iKnowRepository.js";

const initialState = {
    students: [],
    loading: false,
};

const useIKnow = () => {
    const [state, setState] = useState(initialState);

    const getStudentByIndex = useCallback((studentId) => {
        return iKnowRepository
            .getStudentByIndex(studentId)
            .then((response) => {
                console.log(`Fetched student by index: ${studentId}`);
                return response.data;
            })
            .catch((error) => {
                console.log(error);
            });
    }, []);

    const getStudentEligibleForEsc = useCallback((studentId) => {
        return iKnowRepository
            .getStudentEligibleForEsc(studentId)
            .then((response) => {
                console.log(`Fetched ESC eligibility for student: ${studentId}`);
                return response.data;
            })
            .catch((error) => {
                console.log(error);
            });
    }, []);

    const getActiveStudents = useCallback(() => {
        return iKnowRepository
            .getActiveStudents()
            .then((response) => {
                console.log("Fetched active students.");
                setState({
                    "students": response.data,
                    "loading": false,
                });
            })
            .catch((error) => {
                console.log(error);
            });
    }, []);

    useEffect(() => {
        getActiveStudents();
    }, [getActiveStudents]);

    return {
        ...state,
        getStudentByIndex,
        getStudentEligibleForEsc,
        getActiveStudents,
    };
};

export default useIKnow;