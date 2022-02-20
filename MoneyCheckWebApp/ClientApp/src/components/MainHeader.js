import {useEffect, useState} from "react";
import React from "react";
import {Link, Redirect} from "react-router-dom";
import {CookieHelper} from "../services/CookieHelper";
import classNames from "classnames";
import {IconButton} from "../ui/IconButton";
import {
    faChartPie,
    faCloudSun,
    faCoins,
    faDoorOpen,
    faHome,
    faMoon,
    faPen,
    faSun
} from "@fortawesome/free-solid-svg-icons";
import {AnimatedLogo} from "../ui/AnimatedLogo";
import {FontAwesomeIcon} from "@fortawesome/react-fontawesome";
import Swal from "sweetalert2";
import {PageLoader} from "../ui/PageLoader";
import {MCApi} from "../services/MCApi";

export function MainHeader() {
    const [authGrant, setGrant] = useState('LOADING_GRANT');
    const cookieHelper = new CookieHelper();
    
    useEffect(() => {
        cookieHelper.checkIfCookieCanUseForAuth().then(res => {
            if(res) {
                setGrant("GRANTED");
            } else {
                setGrant("GRANT_FAILED");
            }
        })
    }, []);
    
    if(!cookieHelper.canAuthByCookie()) {
        return <Redirect to="/welcome"/>
    }
    
    if(authGrant === 'LOADING_GRANT') {
        return <PageLoader/>
    } else if(authGrant === 'GRANT_FAILED') {
        cookieHelper.deleteAllCookies();
        return <Redirect to="/welcome"/>
    }
    
    return <div className="d-flex flex-row justify-content-between align-items-center mt-2">
        <div className="d-flex flex-row align-items-center">
            <QuitButton/>
            <NavPanel/>
        </div>
        <Greeter/>
        <AnimatedLogo/>
    </div>;
}

function NavPanel() {
    return <div className="d-flex flex-row ml-2 justify-content-around nav-bar flex-wrap">
        <Link to="/home" title="Домой"><FontAwesomeIcon icon={faHome}/></Link>
        <Link to="/analytics" title="Анализ финансов"><FontAwesomeIcon icon={faChartPie}/></Link>
        <Link to="/inflation" title="Прогноз инфляции"><FontAwesomeIcon icon={faCoins}/></Link>
    </div>
}

function QuitButton() {
    const [logout, setLogout] = useState(false);
    const [redirect, setRedirect] = useState(false);
    const [loading, setLoading] = useState(false);
    const cookieHelper = new CookieHelper();

    if(redirect) {
        return <Redirect to="/welcome"/>;
    }
    
    if(loading) {
        return <PageLoader/>;
    }

    if(logout) {
        Swal.fire({
            icon: 'question',
            title: "Вы уверены, что хотите выйти?",
            confirmButtonText: 'Да',
            showCancelButton: true,
            confirmButtonColor: '#DE3842',
            cancelButtonText: "Нет"
        }).then((res) => {
            if(res.isConfirmed) {
                setLoading(true);
                fetch('/auth/api/logout?token=' + cookieHelper.getCookie('cmAuthToken'), {
                    method: 'POST'
                }).then(res => {
                    if(res.status === 200) {
                        setRedirect(true);
                        cookieHelper.deleteAllCookies();
                    }  else {
                        Swal.fire({
                            title: 'Не получилось выйти...',
                            text: 'Попробуйте позже',
                            showConfirmButton: false,
                            timer: 1000,
                            icon: 'error'
                        });
                    }
                });
            }
            setLogout(false);
        })
    }
    
    return <FontAwesomeIcon className="quit-but" onClick={() => {setLogout(true)}} icon={faDoorOpen}/>;
}

function Greeter() {
    let time = '';
    
    let now = new Date();
    let hours = now.getHours();
    
    if(hours >= 0 && hours <= 3 || hours >= 21 && hours <= 23) {
        return <span className="font-weight-bold text-center">
            Доброй ночи
            <FontAwesomeIcon className="ml-1" icon={faMoon}/>
        </span>
    } else if(hours < 21 && hours >= 17) {
        return <span className="font-weight-bold text-center">
            Добрый вечер
            <FontAwesomeIcon className="ml-1" icon={faCloudSun} />
        </span>
    } else {
        return <span className="font-weight-bold text-center">
            Добрый день
            <FontAwesomeIcon className="ml-1" icon={faSun} />
        </span>
    }
}