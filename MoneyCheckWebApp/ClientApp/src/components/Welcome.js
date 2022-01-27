import React, {useEffect, useState} from "react";
import {LoginForm} from "./LoginForm";
import {Box} from "../ui/Box";

import "../assets/scss/pages/welcome.scss";
import {Loader} from "../ui/Loader";
import {RegistrationForm} from "./RegistrationForm";

import {ReactComponent as Logo} from "../assets/images/logo.svg";
import {CookieHelper} from "../services/CookieHelper";
import {Redirect} from "react-router-dom";

export function Welcome(props) {
    const cookieHelper = new CookieHelper();

    if(cookieHelper.canAuthByCookie()) {
        return <Redirect to="/login"/> //Login pipeline
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

function IndicatorWithValue(props) {
    return <div className="d-flex flex-column indicator">
        <span className="statistics-indicator">{props.indicator}</span>
        <span className="statistics-value">{props.value}</span>
    </div>
}

function StatisticsBox() {
    const [statistics, setStatistics] = useState(null);
    
    useEffect(() => {
        const interval = setInterval(() => {
            fetch('/stats/get', {
                method: 'GET'
            }).then(response => response.json()).then(json => setStatistics(json));
        }, 2000);

        return () => {
           clearInterval(interval);
       }
    })
    
    let displayElement = statistics == null ?
        <div className="m-5">
            <Loader/>
        </div> :
        <Box className="d-flex flex-column statistics justify-content-center align-items-start pl-5">
            <IndicatorWithValue indicator="Транзакций за сегодня" value={statistics.purchasesCountToday}/>
            <IndicatorWithValue indicator="Последняя транзакция" value={statistics.lastTransaction}/>
            <IndicatorWithValue indicator="Пользователей с нами" value={statistics.peopleWithUs}/>
        </Box>;

    return <div className="statistics-wrapper">
        <h1>Статистика:</h1>
        {displayElement}
    </div>;
}
