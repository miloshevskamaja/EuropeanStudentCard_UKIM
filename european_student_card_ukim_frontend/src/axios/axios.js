import axios from "axios"

const axiosInstance = axios.create({
    baseURL: "http://localhost:5211/mock/iknow",
    headers: {
        "Content-Type": "application/json",
    },
});

export default axiosInstance;