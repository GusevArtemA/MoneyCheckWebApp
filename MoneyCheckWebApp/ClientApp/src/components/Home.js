import React from "react";
import {MCApi} from "../services/MCApi";
import {Loader} from "../ui/Loader";

import "../assets/scss/pages/home.scss";
import {Box} from "../ui/Box";
import {Link, NavLink} from "react-router-dom";

export class Home extends React.Component {
    constructor(props) {
        super(props);

        this.api = new MCApi();

        this.state = {
            userInfo: null
        }

        this.invokeInitPipeline();
    }

    render() {
        if(this.state.userInfo === null) {
            return <div className="max d-flex justify-content-center align-items-center">
                <Loader/>
            </div>
        }

        return <div className="max">
            <Greeter username={this.state.userInfo.username}/>
            <div>
                <BalanceInfo/>
            </div>
        </div>;
    }

    invokeInitPipeline() {
        this.api.getUserInfo().then(data => {
            if(data === undefined) {
                this.errorLoadingMainInfo();
                return;
            }

            this.setState({
                userInfo: data
            })
        });
    }

    errorLoadingMainInfo() {

    }
}

class Greeter extends React.Component {
    render() {
        const now = new Date();

        let time = 'день';

        if(now.getHours() >= 17) {
            time = 'вечер';
        }

        return <div>
            <h1 id="greeter">Добрый {time}, {this.props.username}</h1>
        </div>
    }
}

class BalanceInfo extends React.Component {
    constructor(props) {
        super(props);

        this.api = new MCApi();
        this.state = {
            balance: null,
            todaySpent: null
        }

        this.api.getUserInfo().then(result => {
            if(result === undefined) {
                return;
            }

            this.setState({
                balance: result.balance
            });
        })

        this.api.getPurchases().then(result => {
            let total = 0;

            if(result === undefined) {
                return;
            }

            console.log(result);

            if(result.length === 0) {
                this.setState({
                    todaySpent: 0
                });
                return;
            }

            result.map(x => x.amount).each(x => total += x);

            this.setState({
                todaySpent: total
            });
        })
    }

    render() {
        if(this.state.balance == null || this.state.todaySpent == null) {
            return <Loader/>
        }

        return <Box id="balance-status-wrapper" className="p-3">
            <div className="d-flex flex-column">
                <span className="prop">Ваш счет:</span>
                <span id="balance" className="prop-value">{this.state.balance} руб</span>
                <NavLink to='/inflation' id="inflation-router">А сколько это будет в гамбургерах через месяц?</NavLink>
            </div>
            <div className="d-flex flex-column">
                <span className="prop">Прогноз на следующий месяц:</span>
                <span id="future-balance" className="prop-value">1000 руб</span>
            </div>
            <div className="d-flex flex-column">
                <span className="prop">Сегодня потрачено:</span>
                <span id="today-spent" className="prop-value">{this.state.todaySpent} руб</span>
                <NavLink to='/stats' id="want-to-see-year-router">Хочу посмотреть, как я тратил деньги этот год</NavLink>
            </div>
        </Box>
    }
}