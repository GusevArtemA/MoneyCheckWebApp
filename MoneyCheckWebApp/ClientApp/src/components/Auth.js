import React from "react";
import {TextInput} from "../ui/inputs/TextInput";
import {SubmitInput} from "../ui/inputs/SubmitInput";
import "../assets/scss/pages/auth-style.scss";
import {ReactComponent as LogoSvg} from "../assets/images/logo-without-label.svg";

import md5 from "crypto-js/md5"

export class Auth extends React.Component {
    constructor(props) {
        super(props);

        this.onSubmitted = this.onSubmitted.bind(this);
        this.loginRef = React.createRef();
        this.passwordRef = React.createRef();
    }

    render() {
        return (
            <div className={"d-flex flex-row justify-content-around align-items-center main-wrapper"}>
                <div>
                    <form onSubmit={this.onSubmitted} className="d-flex flex-column justify-content-center align-items-center p-5">
                        <h1 className={"mb-3"}>Вход</h1>
                        <TextInput inputRef={this.loginRef} name='login' placeholder={"Логин"}/>
                        <TextInput inputRef={this.passwordRef} name='password' placeholder={"Пароль"} type={"password"}/>
                        <SubmitInput>
                            Войти
                        </SubmitInput>
                    </form>
                </div>
                <div>
                    <LogoSvg/>
                </div>
            </div>
        );
    }

    onSubmitted(e) {
        e.preventDefault();

        if(this.loginRef.current.value === '') {
            this.loginRef.current.className = 'error-float-input';
            return false;
        } else {
            this.loginRef.current.className = '';
        }

        if(this.passwordRef.current.value === '') {
            this.passwordRef.current.className = 'error-float-input';
            return false;
        } else {
            this.passwordRef.current.className = '';
        }

        this.sendCreds();
    }

    sendCreds() {
        const md5PasswordHash = md5(this.passwordRef.current.value);
        const username = this.loginRef.current.value;

        let creds = {
            username: username,
            passwordHash: md5PasswordHash.toString()
        };

        fetch('/auth/api/login', {
            method: 'POST',
            body: JSON.stringify(creds),
            headers: {
                'Content-Type': 'application/json',
                'Content-Encoding': 'UTF-8'
            }
        }).then(result => {
           if(result.status === 401) {
               alert('Неправильные данные');
           }

           if(result.status === 200) {
               alert('OK');
           }
        });
    }
}