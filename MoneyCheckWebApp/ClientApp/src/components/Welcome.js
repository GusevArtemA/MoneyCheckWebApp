import React, {useEffect, useState} from "react";
import {LoginForm} from "./LoginForm";
import {Box} from "../ui/Box";

import "../assets/scss/pages/welcome.scss";
import {Loader} from "../ui/Loader";
import {RegistrationForm} from "./RegistrationForm";

import {ReactComponent as Logo} from "../assets/images/logo.svg";
import {CookieHelper} from "../services/CookieHelper";
import {Redirect} from "react-router-dom";
import {AnimatedLogo} from "../ui/AnimatedLogo";

export function Welcome(props) {
    const cookieHelper = new CookieHelper();

    if(cookieHelper.canAuthByCookie()) {
        return <Redirect to="/login"/> //Login pipeline
    }
    
    return (
        <div className="max">            
            <div className="main-wrapper d-flex flex-row justify-content-around align-items-center">
                <AnimatedLogo width="300" className="logo"/>
                <div className="forms-wrapper max-height d-flex align-items-center justify-content-center flex-column form-pre-wrapper">
                    <Box className="form-wrapper">
                        <LoginForm withLogo={false}/>
                    </Box>
                    <Box className="form-wrapper">
                        <RegistrationForm/>
                    </Box>
                </div>
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

