import React from "react";
import {MCApi} from "../services/MCApi";
import {Loader} from "../ui/Loader";

import "../assets/scss/pages/home.scss";
import {Box} from "../ui/Box";
import {NavLink} from "react-router-dom";
import {SelectBox} from "../ui/SelectBox";

import '../prototypes';
import {faAngleDown, faArrowDown, faPlus, faTrash} from "@fortawesome/free-solid-svg-icons";
import Swal from "sweetalert2";
import {IconButton} from "../ui/IconButton";

export class Home extends React.Component {
    constructor(props) {
        super(props);

        this.api = new MCApi();

        this.state = {
            userInfo: null,
            categories: null,
            refreshing: false,
            debtors: null,
            transactions: null,
            balanceInfo: null
        }

        this.invokeInitPipeline();
    }

    render() {
        if(this.state.userInfo === null ||
            this.state.categories === null ||
            this.state.debtors === null ||
            this.state.transactions === null ||
            this.state.balanceInfo === null ||
            this.state.refreshing) {
            return <div className="max d-flex justify-content-center align-items-center">
                <Loader/>
            </div>
        }

        return <div className="max container">
            <Greeter username={this.state.userInfo.username}/>
            <div className="d-flex flex-row justify-content-around">
                <div className='d-flex flex-column align-items-center justify-content-around'>
                    <TransactionsHandler
                        transactions={this.state.transactions}    
                        categories={this.state.categories}
                        refresh={() => {this.setState({refreshing: true})}}
                        filterValueChanged={(filter) => this.api.getPurchases(filter).then(data =>
                            this.setPropFromApi(data, 'transactions'))}
                        itemAdded={() => 
                            this.api.getPurchases().then(data => {
                                this.setPropFromApi(data, 'transactions');
                            this.api.getBalanceState().then(data => 
                                this.setPropFromApi(data, 'balanceInfo'));
                        })}/>
                    <DebtorsHandler
                        debtors={this.state.debtors}
                        debtAdded={() => this.api.getDebtors().then(data =>
                            this.setPropFromApi(data, 'debtors'))}
                    />
                </div>
                <div>
                    <BalanceInfo balanceInfo={this.state.balanceInfo}/>
                </div>
            </div>
        </div>;
    }

    invokeInitPipeline() {
        this.api.getUserInfo().then(data => 
            this.setPropFromApi(data, 'userInfo'));

        this.api.getCategories().then(data => 
            this.setPropFromApi(data, 'categories'));

        this.api.getBalanceState().then(data => 
            this.setPropFromApi(data, 'balanceInfo'));
        
        this.api.getPurchases().then(data => 
            this.setPropFromApi(data, 'transactions'));
        
        this.api.getDebtors().then(data => 
            this.setPropFromApi(data, 'debtors'));
    }
    
    setPropFromApi(data, propName) {
        if(data === undefined) {
            this.errorLoadingMainInfo();
            return;
        }

        const obj = {};

        obj[propName] = data;
        
        this.setState(obj);
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
    }

    render() {
        if(this.props.balanceInfo.balance == null ||
            this.props.balanceInfo.todaySpent == null ||
            this.props.balanceInfo.futureCash == null) {
            return <Loader/>
        }

        return <Box id="balance-status-wrapper" className="p-3 pt-4 pb-5 max-width">
            <div className="d-flex flex-column">
                <span className="prop">Ваш счет:</span>
                <span id="balance" className="prop-value">{this.props.balanceInfo.balance} руб</span>
                {this.props.balanceInfo.balance > 0 ? <NavLink to='/inflation' id="inflation-router">А сколько это будет в гамбургерах через месяц?</NavLink> : null}
            </div>
            <div className="d-flex flex-column">
                <span className="prop">Прогноз на следующий месяц:</span>
                <span id="future-balance" className="prop-value">{this.props.balanceInfo.futureCash} руб</span>
            </div>
            <div className="d-flex flex-column">
                <span className="prop">Сегодня потрачено:</span>
                <span id="today-spent" className="prop-value">{this.props.balanceInfo.todaySpent} руб</span>
                <NavLink to='/stats' id="want-to-see-year-router">Хочу посмотреть, как я тратил деньги этот год</NavLink>
            </div>
        </Box>
    }
}

class TransactionsHandler extends React.Component {
    constructor(props) {
        super(props);
        this.selectBoxValueChangedHandler = this.selectBoxValueChangedHandler.bind(this);
    }
    
    render() {
        return <div>
            <div className="d-flex flex-row align-items-start" id="transactions-filter">
                <h1>Ваши транзакции за этот </h1>
                <SelectBox items={['день', 'месяц', 'год']} onValueChanged={this.selectBoxValueChangedHandler}/>
            </div>
            <TransactionsContainer transactions={this.props.transactions}
                                   categories={this.props.categories}
                                   itemAdded={() => {this.props.itemAdded();}}/>
        </div>
    }
    
    selectBoxValueChangedHandler(value) {
        this.state = {
            transactions: []
        };
        
        switch (value) {
            case 'день':
                this.props.filterValueChanged('day');
                break;
            case 'месяц':
                this.props.filterValueChanged('month');
                break;
            case 'год':
                this.props.filterValueChanged('year');
                break;
        }
    }
    
    fetchPipeline(filter = 'today') {
        new MCApi().getPurchases(filter).then(result => {
            this.setState({
                transactions: result.map(x => {
                    return {
                        logoUrl: x.iconUrl,
                        description: x.description,
                        boughtAt: new Date(x.timeStamp).getShortTime(),
                        cost: x.amount
                    };
                })
            });
        });
    }
}

class TransactionsContainer extends React.Component {
    constructor(props) {
        super(props);
        this.formAlertToAddTransaction = this.formAlertToAddTransaction.bind(this);
    }
    
    render() {
        if(this.props.transactions == null) {
            return <Box className="d-flex flex-column transactions-container justify-content-center align-items-center loading-transactions list-container">
                <Loader/>
            </Box>;
        }
        
        if(this.props.transactions.length === 0) {
            return <Box className="d-flex flex-column transactions-container justify-content-center align-items-center none-transactions list-container"
                        rightButton={faPlus}
                        rightButtonClick={this.formAlertToAddTransaction}>
                <span className="colored-in-brand-purple">Здесь ничего нет</span>
            </Box> 
        }
        
        return <Box className="d-flex flex-column transactions-container list-container"
                    rightButton={faPlus}
                    rightButtonClick={this.formAlertToAddTransaction}>
            {this.props.transactions.map(x => <TransactionContainer transaction={x}/>)}
        </Box>
    }
    
    formAlertToAddTransaction() {
        var options = '';
        
        console.log(this.props.categories);
        
        this.props.categories.forEach((a) => options += `<option>${a.name}</option>\n`);
        
        const cat = this.props.categories;
        const itemAdded = this.props.itemAdded;
        
        Swal.fire({
            title: 'Добавление транзакции',
            confirmButtonColor: '#2EC321',
            html: `                           
                <input type="number" placeholder="Сумма в рублях" class="swal2-input" id="amount"/>
                <input type="text" placeholder="Категория" class="swal2-input" id="categoryName" list="category">
                <datalist id="category">
                  ${options}
                </datalist>            
            `,
            async preConfirm(inputValue) {
                Swal.showLoading();
                let amount = document.getElementById('amount');
                let categoryName = document.getElementById('categoryName');
                
                if(amount.value === 0) {
                    Swal.showValidationMessage('Сумма обязательна');
                    return null;
                }
                
                if(categoryName.value === '') {
                    Swal.showValidationMessage('Категория обязательна');
                    return null;
                }
                
                if(!cat.some(a => a.name === categoryName.value)) {
                    Swal.showValidationMessage('Категория не найдена');
                    return null;
                }
                
                const obj = {
                    amount: amount.value,
                    categoryId: cat.filter(a => a.name === categoryName.value)[0].id,
                }
                
                let res = await fetch('/api/transactions/add-purchase', {
                    body: JSON.stringify(obj),
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    }
                });
                
                if(res.status === 200) {
                    Swal.fire({
                        title: 'Добавлено',
                        showConfirmButton: false,
                        icon: 'success',
                        timer: 1000
                    }).then(() => {
                        itemAdded();
                    });
                } else {
                    Swal.fire({
                        title: 'Что-то пошло не так...',
                        showConfirmButton: false,
                        icon: 'error',
                        timer: 1000
                    });
                }
            }
        });
    }
}

class TransactionContainer extends React.Component {
    constructor(props) {
        super(props);
        this.deltaMove = 0;
        this.clicked = false;
    }
    
    render() {
        return (
            <div>
                <div className="d-flex flex-row justify-content-between align-items-center p-1 position-relative">
                    <div>
                        <img src={this.props.transaction.iconUrl} width='65' alt="*Иконка*"/>
                    </div>
                    <div className='transaction-description'>
                        {this.props.transaction.description}
                    </div>
                    <div className="d-flex flex-row justify-content-end">
                        <div>
                            <span className='transaction-cost'>{this.props.transaction.amount} руб</span>
                        </div>
                    </div>
                </div>    
            </div>
        )
    }
}

class DebtorsHandler extends React.Component {
    constructor(props) {
        super(props);
        this.formAlertToAddDebtor = this.formAlertToAddDebtor.bind(this);
    }
    
    render() {
        return<div className="mt-3">
            <h1>Ваши должники</h1>
            <Box className="d-flex flex-column transactions-container list-container"
                 rightButton={faPlus}
                 rightButtonClick={this.formAlertToAddDebtor}>
                {this.props.debtors.length > 0 ?  this.props.debtors.map(d => <DebtorContainer debtAdded={this.props.debtAdded} debtor={d} key={d.id}/>) :
                    <div className="d-flex justify-content-center align-items-center">
                        <span className="colored-in-brand-purple">Здесь ничего нет</span>
                    </div>}
            </Box>
        </div> 
    }
    
    formAlertToAddDebtor() {
        const debtAdded = this.props.debtAdded;
        
        Swal.fire({
            title: 'Добавление должника',
            input: 'text', 
            inputPlaceholder: 'Имя должника',
            preConfirm(inputValue) {
                if(inputValue === '') {
                    Swal.showValidationMessage('Имя обязательно');
                } 
                
                fetch('/api/debtors/add', {
                   method: 'POST',
                   body: JSON.stringify({
                       name: inputValue
                   }),
                   headers: {
                       'Content-Type': 'application/json'
                   } 
                }).then(result => {
                    if(result.status === 200) {
                        Swal.fire({
                            title: 'Должник добавлен',
                            icon: 'success',
                            showConfirmButton: false,
                            timer: 1000
                        });
                        debtAdded();
                    } else {
                        Swal.fire({
                            title: 'Что-то пошло не так...',
                            icon: 'error',
                            showConfirmButton: false,
                            timer: 1000
                        });
                    }
                });
            }
        });
    }
}

class DebtorContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            isOpened: false
        }
        
        this.removeDebtor = this.removeDebtor.bind(this);
        this.addDebt = this.addDebt.bind(this);
    }
    
    render() {
        let debtorSum = 0;

        this.props.debtor.debts.map(d => d.amount).forEach(d => debtorSum += d);
        
        return <div className={(this.state.isOpened ? 'opened-debtor-list' : "debtor-list") + ' p-1'}>
            <div className="d-flex flex-row justify-content-between align-items-center">
                <span className="font-weight-bolder debtor-name">{this.props.debtor.name}</span>
                <div>
                    <span>{debtorSum} руб</span>
                    <IconButton icon={faTrash} onClick={this.removeDebtor}/>
                    <IconButton onClick={this.addDebt} icon={faPlus}/>
                    <IconButton icon={faAngleDown}
                                onClick={() => this.setState({isOpened: !this.state.isOpened})}/>
                </div>
            </div>
            <div className="debts-container">
            {
                this.props.debtor.debts.length > 0 ?
                    this.props.debtor.debts.map(debt => (
                        <div key={debt.id} className="d-flex flex-row justify-content-between mt-1">
                            <span className={debt.amount > 0 ? 'def-debt-span' : 'return-debt-span'}>{debt.description}</span>
                            <div>
                                <span className="font-weight-bolder">
                                    {Math.abs(debt.amount)} руб
                                </span>
                                <IconButton onClick={() => this.removeDebt(debt.debtId)} icon={faTrash}/>
                            </div>
                        </div>
                    )) :
                    <div className="d-flex justify-content-center">
                        <span>Здесь ничего нет</span>
                    </div>    
            }
            </div>
        </div>
    }
    
    addDebt() {
        const debtorId = this.props.debtor.id;
        const debtAdded = this.props.debtAdded;
        
        Swal.fire({
            title: `Добавление долга ${this.props.debtor.name}`,
            html: `
                <input type='number' placeholder='Сумма долга' id="debt-sum" class="swal2-input"/>
                <input type="text" placeholder="Описание" id="debt-description" class="swal2-input"/>
            `,
            preConfirm(inputValue) {
                const debtSum = document.getElementById('debt-sum');
                const debtDescription = document.getElementById('debt-description');
                
                if(debtSum.value === '') {
                    Swal.showValidationMessage('Сумма обязательна');
                }
                
                if(debtDescription.value === '') {
                    Swal.showValidationMessage('Описание обязательно');
                }
                
                fetch('/api/debts/add-debt', {
                    method: 'POST',
                    body: JSON.stringify({
                       amount: debtSum.value,
                       description: debtDescription.value,
                       debtorId: debtorId
                    }), 
                    headers: {
                       'Content-Type': 'application/json'
                    }
                }).then(result => {
                    if(result.status === 200) {
                        Swal.fire({
                            title: 'Добавлено',
                            showConfirmButton: false,
                            timer: 1000,
                            icon: "success"
                        });
                        debtAdded();
                    } else {
                        Swal.fire({
                            title: 'Что-то пошло не так...',
                            showConfirmButton: false,
                            timer: 1000,
                            icon: "error"
                        })
                    }
                });
            }
        });
    }
    
    removeDebt(id) {
        const debtAdded = this.props.debtAdded;
        
        Swal.fire({
            title: 'Вы уверены, что хотите удалить долг?',
            icon: 'question',
            showCancelButton: true
        }).then(res => {
            if(res.isConfirmed) {
                fetch(`/api/debts/remove-debt?id=${id}`, {
                    method: 'DELETE'
                }).then(result => {
                    if(result.status === 200) {
                        Swal.fire({
                            title: 'Долг удален',
                            icon: 'success',
                            showConfirmButton: false,
                            timer: 1000
                        });
                        debtAdded();
                    } else {
                        Swal.fire({
                            title: 'Что-то пошло не так...',
                            icon: 'error',
                            showConfirmButton: false,
                            timer: 1000
                        });
                    }
                });
            }
        })
    }
    
    removeDebtor() {
        const debtAdded = this.props.debtAdded;
        
        Swal.fire({
            title: `Вы уверены, что хотите удалить ${this.props.debtor.name}`,
            icon: "question",
            showCancelButton: true
        }).then(result => {
            if(result.isConfirmed) {
                fetch(`/api/debtors/remove?id=${this.props.debtor.id}`, {
                   method: 'DELETE' 
                }).then(result => {
                    if(result.status === 200) {
                        Swal.fire({
                            title: 'Должник удален',
                            icon: 'success',
                            showConfirmButton: false,
                            timer: 1000
                        });
                        debtAdded();
                    } else {
                        Swal.fire({
                            title: 'Что-то пошло не так...',
                            icon: 'error',
                            showConfirmButton: false,
                            timer: 1000
                        });
                    }
                });
            }
        })
    }
}
