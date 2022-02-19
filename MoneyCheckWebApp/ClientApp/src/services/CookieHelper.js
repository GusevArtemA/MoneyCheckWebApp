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
    
    deleteAllCookies() {
        const cookies = document.cookie.split(";");

        for (let i = 0; i < cookies.length; i++) {
            const cookie = cookies[i];
            const eqPos = cookie.indexOf("=");
            const name = eqPos > -1 ? cookie.substr(0, eqPos) : cookie;
            document.cookie = name + "=;expires=Thu, 01 Jan 1970 00:00:00 GMT";
        }
    }
}