import React from "react";
import {LoginForm} from "./LoginForm";
import {Box} from "../ui/Box";

import "../assets/scss/pages/welcome.scss";
import {Loader} from "../ui/Loader";
import {RegistrationForm} from "./RegistrationForm";

import {ReactComponent as Logo} from "../assets/images/logo.svg";
import {CookieHelper} from "../services/CookieHelper";
import {Redirect} from "react-router-dom";

export class Welcome extends React.Component {
    render() {
        const cookieHelper = new CookieHelper();

        if(cookieHelper.canAuthByCookie()) {
            return <Redirect to="/login"/> //Login pipline
        }

        return (
            <div className="max">
                <div className="main-wrapper d-flex flex-row justify-content-around align-items-center">
                    <StatisticsBox/>
                    <div className="forms-wrapper max-height d-flex align-items-center justify-content-center flex-column form-pre-wrapper">
                        <Box className="form-wrapper">
                            <LoginForm withLogo={false}/>
                        </Box>
                        <Box className="form-wrapper">
                            <RegistrationForm/>
                        </Box>
                    </div>
                </div>
                <div className="logo-wrapper d-flex justify-content-center align-items-center">
                    <Logo id="logo"/>
                </div>
            </div>

        );
    }
}

class IndicatorWithValue extends React.Component {
    render() {
        return <div className="d-flex flex-column indicator">
            <span className="statistics-indicator">{this.props.indicator}</span>
            <span className="statistics-value">{this.props.value}</span>
        </div>
    }
}

class StatisticsBox extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            statistics: null
        };
        setInterval(() => this.fetchStatisticAndUpdate(), 2000);
    }

    render() {
        let displayElement = this.state.statistics == null ?
            <div className="m-5">
                <Loader/>
            </div> :
            <Box className="d-flex flex-column statistics justify-content-center align-items-start pl-5">
                <IndicatorWithValue indicator="Транзакций за сегодня" value={this.state.statistics.purchasesCountToday}/>
                <IndicatorWithValue indicator="Последняя транзакция" value={this.state.statistics.lastTransaction}/>
                <IndicatorWithValue indicator="Пользователей с нами" value={this.state.statistics.peopleWithUs}/>
            </Box>;

        return <div className="statistics-wrapper">
            <h1>Статистика:</h1>
            {displayElement}
        </div>;
    }

    async fetchStatisticAndUpdate() {
        fetch('/stats/get', {
            method: 'GET'
        }).then(response => response.json()).then(json => this.setState({
            statistics: json
        }));
    }
}
