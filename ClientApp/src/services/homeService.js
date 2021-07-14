import { httpGet, httpPost } from "../helpers/apiClients";

//# returns all lovlist
export function getResult() {
    return httpGet({ url: "Home" })
}

export function getAccessToken(param) {
    return httpPost({ url: "Home/authenticate", data: param })
}

export function getRefreshToken() {
    return httpPost({ url: "Home/refresh-token" })
}

export function revokeRefreshToken() {
    return httpPost({ url: "Home/revoke-token" })
}