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
    }

    async getBalanceState() {
        return await this._fetchJson('/api/web/user-balance-stats');
    }
    
    async getPurchases(filter = 'today') {
        return await this._fetchJson('/api/user-stats/get-transactions?filter=' + filter);
    }
    
    async getDebtors() {
        return await this._fetchJson('/api/web/get-debtors');
    }
    
    async getCategories(includeDefaultCategories = true)
    {
        return await this._fetchJson('/api/web/get-categories?includeDefaultCategories=' + includeDefaultCategories);
    }
    
    async getCategoriesData() {
        return await this._fetchJson('/api/user-stats/get-categories-stats');
    }
    
    async getStatsForYearAnalytics(filter = "year", index = null) {
        return await this._fetchJson(`/api/user-stats/get-trace?filter=${filter}&index=${index ?? ''}`);
    }
    
    async getCatLogos() {
        return await this._fetchJson('/api/web/get-def-logos');
    }
    
    async getInflationPredication(nowCost, index) {
        return await this._fetchJson(`/api/inflation-predication/predict?priceNow=${nowCost}&index=${index}`)
    }
    
    async getInflationForYear() {
        return await this._fetchJson('/api/web/get-inflation-for-year');
    }
    
    async getDayStats() {
        return await this._fetchJson('/api/user-stats/get-categories-stats-day');    
    }
    
    async isTokenActive(token) {
        let res = await fetch('/api/token-ensurer/ensure', {
            method: 'GET'
        });
        
        return res.ok;
    }
    
    /**
     * Fetches json with following url
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