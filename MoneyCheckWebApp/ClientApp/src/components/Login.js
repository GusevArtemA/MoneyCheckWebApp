import React from "react";
import {Box} from "../ui/Box";

import "../assets/scss/pages/login.scss";
import {LoginForm} from "./LoginForm";

export function Login(props) {
    return (
        <div className="max d-flex justify-content-center align-items-center">
            <Box className="login-form-wrapper d-flex flex-column justify-content-around align-items-center">
                <LoginForm withLogo={true}/>
            </Box>
        </div>
    );
}
