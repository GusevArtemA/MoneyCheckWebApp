import {CookieHelper} from "./CookieHelper";

export class MCApi {
    constructor() {
        this.cookieHelper = new CookieHelper();

        if(!this.cookieHelper.canAuthByCookie()) {
            console.error("Can't get cookie token");
        }
    }

    async getUserInfo() {
        return await this._fetchJson('/api/user-data/get-data');
        let response = await fetch('/api/user-data/get-data', {
           method: 'GET'
        });

        if(response.ok) {
            return await response.json();
        } else {
            return undefined;
        }
    }

    async getPurchases(filter = 'by_today') {
        return await this._fetchJson('/api/transactions/get-purchases?filter=' + filter);
    }

    async getDebts() {
        return await this._fetchJson('/api/debts/get-bets');
    }

    /**
     * Fetches json from followed url
     * @private
     */
    async _fetchJson(url, method = 'GET') {
        let response = await fetch(url, {
            method: 'GET'
        });

        if(!response.ok) {
            return undefined;
        }

        return await response.json();
    }
}