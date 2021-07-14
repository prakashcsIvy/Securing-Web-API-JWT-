import axios from "axios";
import { getToken, setToken } from "./localStorage";


//# api client using axios
const GetAPIUrl = (url) => {
    //let domain = process.env.REACT_APP_SERVER_URL;
    let domain = "https://localhost:4001/";
    if (!domain) { return ""; }
    return domain + url;
};

// ** All http get operation managed here
export function httpGet(config) {
    const { url } = config;
    return commonFetch({ url: GetAPIUrl(url), method: "get" });
}
// ** all http delete operation managed here
export function httpDelete(config) {
    const { url } = config;
    return commonFetch({ url: GetAPIUrl(url), method: "delete" });
}

// ** All http post operation managed here
export function httpPut(config) {
    const { url } = config;
    return commonFetch({ url: GetAPIUrl(url), method: "put" });
}

export function httpPost(config) {
    const { url, data } = config;
    return commonFetch({ url: GetAPIUrl(url), method: "post", data: data });
}

//**  All http put operation managed here
// ** function returns common headers
function getCommonHeaders() {
    return {
        "Content-Type": "application/json",
        "Access-Control-Allow-Origin": "*",
        "G-Auth": ""
    };
}

// * * function calls api and get the result
const commonFetch = (config) => {
    const { url, data, method } = config;
    if (!url) {
        console.log("There is no URL available to call API's.");
        return;
    }
    return axios({
        url: url,
        method: method,
        headers: getCommonHeaders(),
        data: data ? data : null,
        withCredentials: true
    }).then(parseResponseJSON)
        .catch((err) => { return Promise.reject(err); });
};

//** function take the response validate and returns
function parseResponseJSON(response) {
    console.log("parseResponseJSON: ", response);
    if (!response) { return; }
    return response.data;
}

// ** request Interceptor
axios.interceptors.request.use(
    function (config) {
        let token = getToken("Access_Token");
        console.log("token from ls: ", token);
        if (token) { config.headers.Authorization = "Bearer " + token; }
        return config;
    },
    function (error) {
        //? Do something with request error
        return Promise.reject(error);
    }
);
let isRefreshing = false;
let failedQueue = [];


// ** Response Interceptor
axios.interceptors.response.use(
    function (response) {
        console.log("response: ", response);
        return response;
    },
    function (error) {
        console.log("error: ", error);
        const originalRequest = error.config;
        if (!error.response) {
            alert(`Requested server is unavailable`);
            return;
        }
        if ((error.response.status === 400 || error.response.status === 401) && !originalRequest._retry) {
            if (error.config.url.endsWith('/Home/refresh-token')) {
                return Promise.reject(error);
            }

            if (isRefreshing) {
                return new Promise(function (resolve, reject) {
                    failedQueue.push({ resolve, reject })
                }).then(token => {
                    originalRequest.headers['Authorization'] = 'Bearer ' + token;
                    return axios(originalRequest);
                }).catch(err => {
                    return Promise.reject(err);
                })
            }

            const processQueue = (error, token = null) => {
                failedQueue.forEach(prom => {
                    if (error) { prom.reject(error); }
                    else { prom.resolve(token); }
                })
                failedQueue = [];
            }

            originalRequest._retry = true;
            isRefreshing = true;
            return new Promise(function (resolve, reject) {
                axios({
                    url: GetAPIUrl('Home/refresh-token'),
                    method: "POST",
                    headers: getCommonHeaders(),
                    data: null,
                    withCredentials: true
                }).then((result) => {
                    if (!result || !result.data) { return; }
                    var data = result.data;
                    setToken("Access_Token", data.jwtToken);
                    originalRequest.headers['Authorization'] = 'Bearer ' + data.jwtToken;
                    processQueue(null, data.token);
                    resolve(axios(originalRequest));
                }).catch((err) => {
                    processQueue(err, null);
                    alert("User Session Timedout");
                    reject(err);
                }).finally(() => { isRefreshing = false })
            })
        }
    });
//*/