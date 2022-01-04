import React from "react";
import {CookieHelper} from "../services/CookieHelper";
import {Redirect} from "react-router-dom";
import {Loader} from "../ui/Loader";
import {ReactComponent as Logo} from "../assets/images/logo.svg";
import {TextInput} from "../ui/TextInput";
import {SubmitButton} from "../ui/Button";
import md5 from "crypto-js/md5";

export class RegistrationForm extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            redirect: false,
            loading: false
        }

        this.onSubmitted = this.onSubmitted.bind(this);
        this.setError = this.setError.bind(this);
        this.errorLoginRef = React.createRef();
        this.errorPasswordRef = React.createRef();
        this.errorRetryPasswordRef = React.createRef();
    }

    render() {
        if (this.state.redirect) {
            return <Redirect to='/home'/>
        }

        return (
            <>
                {this.state.loading ? <Loader/> : this.renderForm()}
            </>
        );
    }

    renderForm() {
        return <>
            <form className="d-flex justify-content-center flex-column align-items-center" onSubmit={this.onSubmitted}>
                <TextInput errorRef={this.errorLoginRef} placeholder="Логин"/>
                <TextInput errorRef={this.errorPasswordRef} placeholder="Пароль"/>
                <TextInput errorRef={this.errorRetryPasswordRef} placeholder="Повторите пароль"/>
                <SubmitButton value="Регистрация"/>
            </form>
        </>;
    }

    onSubmitted(e) {
        e.preventDefault();
        if (this.getLogin() === '') {
            this.setErrorLogin('Это поле обязательно');
            return false;
        } else {
            this.setRightLogin();
        }

        if (this.getPassword() === '') {
            this.setErrorPassword('Это поле обязательно');
            return false;
        } else {
            this.setRightPassword();
        }

        if (this.getRetryPassword() === '') {
            this.setErrorRetryPassword('Это поле обязательно');
            return false;
        } else {
            this.setRightRetryPassword();
        }

        if(this.getPassword() !== this.getRetryPassword()) {
            this.setErrorRetryPassword('Пароли не совпадают');
            this.setErrorPassword('Пароли не совпадают');
            return false;
        } else {
            this.setRightRetryPassword();
            this.setRightPassword();
        }

        this.sendCreds();
    }

    sendCreds() {
        const password = this.getPassword();
        const username = this.getLogin();

        let creds = {
            username: username,
            password: password
        };

        fetch('/auth/api/log-up', {
            method: 'POST',
            body: JSON.stringify(creds),
            headers: {
                'Content-Type': 'application/json',
                'Content-Encoding': 'UTF-8'
            }
        }).then(result => {
            this.setState({
                loading: false
            });

            if(result.status === 400) {
                this.setErrorLogin('Пользователь с таким именем уже создан');
                return;
            }

            this.setInitialState();
        });

        this.setState({
            loading: true
        });
    }

    setErrorLogin(content = 'Проверьте данные') {
        this.errorLoginRef.current.innerText = content;
    }

    setErrorPassword(content = 'Проверьте данные') {
        this.errorPasswordRef.current.innerText = content;
    }

    setErrorRetryPassword(content = 'Проверьте данные') {
        this.errorRetryPasswordRef.current.innerText = content;
    }

    setError() {
        this.setErrorLogin();
        this.setErrorPassword();
    }

    setRightLogin() {
        this.errorLoginRef.current.innerText = '';
    }

    setRightPassword() {
        this.errorPasswordRef.current.innerText = '';
    }

    setRightRetryPassword() {
        this.errorRetryPasswordRef.current.innerText = '';
    }

    setAllRight() {
        this.setRightLogin();
        this.setRightPassword();
    }

    setInitialState() {
        this.setState({
            redirect: true
        });
    }

    getLogin() {
        return this.errorLoginRef.current.parentNode.querySelector('input').value;
    }

    getPassword() {
        return this.errorPasswordRef.current.parentNode.querySelector('input').value;
    }

    getRetryPassword() {
        return this.errorRetryPasswordRef.current.parentNode.querySelector('input').value;
    }
}