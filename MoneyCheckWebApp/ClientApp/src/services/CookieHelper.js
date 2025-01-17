import {MCApi} from "./MCApi";

export class CookieHelper {
    getCookie(cookieName) {
        let matches = document.cookie.match(new RegExp(
            "(?:^|; )" + cookieName.replace(/([.$?*|{}()[\]\\/+^])/g, '\\$1') + "=([^;]*)"
        ));
        return matches ? decodeURIComponent(matches[1]) : undefined;
    }

    canAuthByCookie() {
        let cookie = this.getCookie('cmAuthToken');
        
        return !(cookie === undefined || cookie === null || cookie === '');
    }
    
    async checkIfCookieCanUseForAuth() {
        const api = new MCApi();
        
        return await api.isTokenActive(this.getCookie("cmAuthToken"));
    }
    
    deleteAllCookies() {
        const cookies = document.cookie.split(";");

        for (let i = 0; i < cookies.length; i++) {
            const cookie = cookies[i];
            const eqPos = cookie.indexOf("=");
            const name = eqPos > -1 ? cookie.substr(0, eqPos) : cookie;
            document.cookie = name + "=;Max-Age=0; path = /; domain=" + window.location.hostname;
        }
    }
}
