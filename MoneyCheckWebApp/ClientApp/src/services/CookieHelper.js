export class CookieHelper {
    getCookie(cookieName) {
        let matches = document.cookie.match(new RegExp(
            "(?:^|; )" + cookieName.replace(/([\.$?*|{}\(\)\[\]\\\/\+^])/g, '\\$1') + "=([^;]*)"
        ));
        return matches ? decodeURIComponent(matches[1]) : undefined;
    }

    canAuthByCookie() {
        if(this.getCookie('cmAuthToken') === undefined) {
            return false;
        }

        return true;
    }
}