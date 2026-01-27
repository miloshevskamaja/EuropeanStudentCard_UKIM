import axiosInstance from "../axios/axios.js";

const iKnowRepository = {
    getStudentByIndex: async (studentId) => {
        return await axiosInstance.get(`/studentsByIndex/${studentId}`);
    },
    getStudentEligibleForEsc: async (studentId) => {
        return await axiosInstance.get(`/students/${studentId}`);
    },
    getActiveStudents: async () => {
        return await axiosInstance.get(`/students`);
    },
};

export default iKnowRepository;