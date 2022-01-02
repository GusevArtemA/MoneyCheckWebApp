import React from "react";
import {CookieHelper} from "../services/CookieHelper";
import {Redirect} from "react-router-dom";
import {Loader} from "../ui/Loader";
import {ReactComponent as Logo} from "../assets/images/logo.svg";
import {TextInput} from "../ui/TextInput";
import {SubmitButton} from "../ui/Button";
import md5 from "crypto-js/md5";

export class LoginForm extends React.Component {
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
    }

    componentDidMount() {
        let cookieHelper = new CookieHelper();

        if (cookieHelper.canAuthByCookie()) {
            this.setInitialState();
        }
    }

    render() {
        if (this.state.redirect) {
            return <Redirect to='/profile'/>
        }

        return (
            <>
                {this.state.loading ? <Loader/> : this.renderForm()}
            </>
        );
    }

    renderForm() {
        return <>
            {this.props.withLogo ? <Logo/> : null}
            <form className="d-flex justify-content-center flex-column align-items-center" onSubmit={this.onSubmitted}>
                <TextInput errorRef={this.errorLoginRef} placeholder="Логин"/>
                <TextInput errorRef={this.errorPasswordRef} placeholder="Пароль" type="password"/>
                <SubmitButton value="Войти"/>
            </form>
        </>;
    }

    onSubmitted(e) {
        e.preventDefault();
        if (this.getLogin() === '') {
            this.setErrorLogin();
            return false;
        } else {
            this.setRightLogin();
        }

        if (this.getPassword() === '') {
            this.setErrorPassword();
            return false;
        } else {
            this.setRightPassword();
        }

        this.sendCreds();
    }

    sendCreds() {
        const md5PasswordHash = md5(this.getPassword());
        const username = this.getLogin();

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
            this.setState({
                loading: false
            });

            if (result.status === 401) {
                this.setErrorLogin('Неправильный логин или пароль');
                return;
            }

            if (result.status === 200) {
                this.setAllRight();
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
}