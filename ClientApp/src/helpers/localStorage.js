export function getToken(key) {
    return localStorage.getItem(key);
}
export function setToken(key, token) {
    return localStorage.setItem(key, token);
}