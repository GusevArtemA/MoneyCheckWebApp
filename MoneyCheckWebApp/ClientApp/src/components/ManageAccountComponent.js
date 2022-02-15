import {useEffect, useState} from "react";
import React from "react";
import {MCApi} from "../services/MCApi";
import {Loader} from "../ui/Loader";
import {Button} from "../ui/Button";
import {Redirect} from "react-router-dom";
import {CookieHelper} from "../services/CookieHelper";
import classNames from "classnames";
import {Box} from "../ui/Box";
import {IconButton} from "../ui/IconButton";
import {faPen} from "@fortawesome/free-solid-svg-icons";
import {
    AccumulationChartComponent,
    AccumulationDataLabel,
    AccumulationLegend, AccumulationSeriesCollectionDirective, AccumulationSeriesDirective,
    Inject,
    PieSeries
} from "@syncfusion/ej2-react-charts";

export function ManageAccountComponent() {
    const [isOpened, setOpen] = useState(false);
    
    return <div className={classNames("manage-account-wrapper", isOpened && 'fill-screen-account')}>
        <BurgerButton onClick={() => setOpen(!isOpened)} isClosed={!isOpened}/>
        <AccountSettings isOpen={isOpened}/>
    </div>;
}

function AccountSettings({isOpen, ...props}) {
    const [userInfo, setInfo] = useState(null);
    const [dayStats, setDayStats] = useState(null);
    const [logout, setLogout] = useState(false);
    const api = new MCApi();
    
    useEffect(() => {
        api.getDayStats().then(data => setDayStats(data));
        api.getUserInfo().then(data => setInfo(data));     
    }, []);
    
    if(logout) {
        const cookieHelper = new CookieHelper();

        cookieHelper.deleteAllCookies();
        
        return <Redirect to="/welcome"/>
    }
    
    if(userInfo == null || dayStats == null) {
        return <div className="max d-flex justify-content-center align-items-center">
            <Loader/>
        </div>
    }
    
    return <div className="account-settings-wrapper">
        <Box className="d-flex flex-column">
            <NameChanger name={userInfo.username}/>
            <Button onClick={() => {setLogout(true)}}>
                Выйти
            </Button>    
        </Box>  
    </div>
}

function NameChanger({name, ...props}) {
    const [editable, setEdit] = useState(false);
    
    return <div onMouseEnter={() => setEdit(true)}
                onMouseLeave={() => setEdit(false)}>
        <span>{name}</span>
        <IconButton icon={faPen} className={classNames('can-hide', editable ? 'shown' : 'hidden')}/>
    </div>
}

function BurgerButton(props) {
    return <div {...props} className="burger">
        <div className={props.isClosed ? 'burger-1-closed' : 'burger-1-opened'}/>
        <div className={props.isClosed ? 'burger-2-closed' : 'burger-2-opened'}/>
        <div className={props.isClosed ? 'burger-3-closed' : 'burger-3-opened'}/>
    </div>
}