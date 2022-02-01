import React, {useEffect, useState} from "react";
import {MCApi} from "../services/MCApi";
import {Loader} from "../ui/Loader";

import "../assets/scss/pages/home.scss";
import {Box} from "../ui/Box";
import {NavLink} from "react-router-dom";
import {SelectBox} from "../ui/SelectBox";

import '../prototypes';
import {faAngleDown, faPen, faPlus, faTrash} from "@fortawesome/free-solid-svg-icons";
import Swal from "sweetalert2";
import {IconButton} from "../ui/IconButton";
import {FontAwesomeIcon} from "@fortawesome/react-fontawesome";
import withReactContent from 'sweetalert2-react-content'
import classNames from "classnames";
import {AutoComplete} from "../ui/AutoComplete";

const MySwal = withReactContent(Swal)

export function Home(props) {
    const api = new MCApi();
    const [userInfo, setUserInfo] = useState(null);
    const [categories, setCategories] = useState(null);
    const [debtors, setDebtors] = useState(null);
    const [transactions, setTransactions] = useState(null);
    const [balanceInfo, setBalanceInfo] = useState(null);
    const [refreshing, setRefreshing] = useState(false);
    
    useEffect(() => {
        api.getUserInfo().then(data =>
            setUserInfo(data));
        
        api.getCategories().then(data =>
            setCategories(data));

        api.getBalanceState().then(data =>
            setBalanceInfo(data));

        api.getPurchases().then(data =>
            setTransactions(data));

        api.getDebtors().then(data =>
            setDebtors(data)); 
        
        return () => {};
    }, []);
    
    if(userInfo === null ||
        categories === null ||
        debtors === null ||
        transactions === null ||
        balanceInfo === null ||
        refreshing) {
        return <div className="max d-flex justify-content-center align-items-center">
            <Loader/>
        </div>
    }

    
    
    return <div className="max container">
        <Greeter username={userInfo.username}/>
        <div className="main-wrapper d-flex flex-row justify-content-around">
            <div className='trans-debtors-wrapper d-flex flex-column align-items-center justify-content-around'>
                <TransactionsHandler
                    transactions={transactions}
                    categories={categories}
                    refresh={() => {setRefreshing(true)}}
                    filterValueChanged={(filter) => api.getPurchases(filter).then(data =>
                        setTransactions(data))}
                    itemAdded={() =>
                        api.getPurchases().then(data => {
                            setTransactions(data);
                            api.getBalanceState().then(data =>
                                setBalanceInfo(data));
                        })}/>
                <DebtorsHandler
                    debtors={debtors}
                    debtAdded={() => api.getDebtors().then(data => {
                        setDebtors(data);
                        api.getBalanceState().then(data =>
                            setBalanceInfo(data));
                    })}
                />
            </div>
            <div className="balance-info-wrapper">
                <BalanceInfo balanceInfo={balanceInfo}/>
            </div>
        </div>
    </div>;
}

function Greeter(props) {
    const now = new Date();

    let time = 'день';

    if(now.getHours() >= 17) {
        time = 'вечер';
    }

    return <div>
        <h1 id="greeter">Добрый {time}, {props.username}</h1>
    </div>
}

function BalanceInfo(props) {
    if(props.balanceInfo.balance == null ||
        props.balanceInfo.todaySpent == null ||
        props.balanceInfo.futureCash == null) {
        return <Loader/>
    }

    return <Box id="balance-status-wrapper" className="p-3 pt-4 pb-5 max-width">
        <div className="d-flex flex-column">
            <span className="prop">Ваш счет:</span>
            <span id="balance" className="prop-value">{props.balanceInfo.balance} руб</span>
            {props.balanceInfo.balance > 0 ? <NavLink to='/inflation' id="inflation-router">А сколько это будет в гамбургерах через месяц?</NavLink> : null}
        </div>
        <div className="d-flex flex-column">
            <span className="prop">Прогноз на следующий месяц:</span>
            <span id="future-balance" className="prop-value">{props.balanceInfo.futureCash} руб</span>
        </div>
        <div className="d-flex flex-column">
            <span className="prop">Сегодня потрачено:</span>
            <span id="today-spent" className="prop-value">{props.balanceInfo.todaySpent} руб</span>
            <NavLink to='/analytics' id="want-to-see-year-router">Хочу посмотреть, как я тратил деньги этот год</NavLink>
        </div>
    </Box>
}

function TransactionsHandler(props) {
    const selectBoxValueChangedHandler = function (value) {
        switch (value) {
            case 'день':
                props.filterValueChanged('day');
                break;
            case 'месяц':
                props.filterValueChanged('month');
                break;
            case 'год':
                props.filterValueChanged('year');
                break;
        }
    }
    
    return <div className="max-width">
        <div className="d-flex flex-row align-items-start" id="transactions-filter">
            <h1>Ваши транзакции за этот </h1>
            <SelectBox items={['день', 'месяц', 'год']} onValueChanged={selectBoxValueChangedHandler}/>
        </div>
        <TransactionsContainer transactions={props.transactions}
                               categories={props.categories}
                               itemAdded={() => {props.itemAdded();}}/>
    </div>
}

function TransactionsContainer(props) {
    const formAlertToAddTransaction = function() {
        const itemAdded = props.itemAdded;
        
        let selectBoxValue = props.categories[0];
        
        MySwal.fire({
            title: 'Добавление транзакции',
            confirmButtonColor: '#2EC321',
            html: (<form>
                <input type="number" placeholder="Сумма в рублях" className="swal2-input" id="amount"/>
                <SelectBox items={props.categories.map(a => a.name)}
                           className="modal-select-box"
                           onValueChanged={(a) => selectBoxValue = a}
                />
            </form>),
            async preConfirm(inputValue) {
                Swal.showLoading();
                let amount = document.getElementById('amount');

                if(amount.value === 0) {
                    Swal.showValidationMessage('Сумма обязательна');
                    return null;
                }

                if(selectBoxValue === '') {
                    Swal.showValidationMessage('Категория обязательна');
                    return null;
                }

                if(!props.categories.some(a => a.name === selectBoxValue)) {
                    Swal.showValidationMessage('Категория не найдена');
                    return null;
                }

                const obj = {
                    amount: amount.value,
                    categoryId: props.categories.filter(a => a.name === selectBoxValue)[0].id,
                }

                let res = await fetch('/api/transactions/add-purchase', {
                    body: JSON.stringify(obj),
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    }
                });

                if(res.status === 200) {
                    fireAdded().then(() => {
                        itemAdded();
                    });
                } else {
                    fireSomethingWentWrong();
                }
            }
        });
    }
    
    if(props.transactions == null) {
        return <Box className="max-width d-flex flex-column transactions-container justify-content-center align-items-center loading-transactions list-container">
            <Loader/>
        </Box>;
    }

    if(props.transactions.length === 0) {
        return <EmptyBox rightButton={faPlus} onRightButtonClick={formAlertToAddTransaction}/>
    }

    return <Box className="max-width d-flex flex-column transactions-container list-container"
                rightButton={faPlus}
                onRightButtonClick={formAlertToAddTransaction}>
        {props.transactions.map(x => <TransactionContainer key={x.id} onDelete={props.itemAdded} transaction={x}/>)}
    </Box>
}

function TransactionContainer(props) {
    const [canBeEdit, setEdit] = useState(false);
    
    const deleteTransaction = function () {
        fetch('/api/transactions/remove-purchase?id=' + props.transaction.id, {
            method: 'DELETE'
        }).then(result => {
           if(result.status === 200) {
               fireDeleted().then(() => props.onDelete());
           } else {
               fireSomethingWentWrong();  
           }
        });
    };
    
    const editTransaction = function () {
        MySwal.fire({
            title: "Изменение транзакции",
            html: <div>
                <input className="swal2-input" type="number" id="change-amount" placeholder={`${props.transaction.amount} руб`}/>
            </div>,
            preConfirm(inputValue) {
                Swal.showLoading();
                
                let apiObj = {
                    id: props.transaction.id,
                    amount: null
                }
                let amount = document.getElementById("change-amount").value;
                
                if(amount === '') {
                    return;
                }
                
                amount = parseInt(amount);
                
                if(amount <= 0) {
                    Swal.showValidationMessage("Сумма должна быть больше нуля");
                    return;
                }
                
                if(amount !== '' && amount !== props.transaction.amount.toString() && parseInt(amount) > 0) {
                    apiObj.amount = amount;
                    
                    fetch("/api/transactions/edit-purchase", {
                        method: 'PATCH',
                        body: JSON.stringify(apiObj),
                        headers: {
                            "Content-Type": "application/json"
                        }
                    }).then(result => {
                        if(result.status === 200) {
                            fireEdited().then(() => props.onDelete());
                        } else {
                            fireSomethingWentWrong();
                        }
                    })
                }
            }
        })
    }
    
    return (
        <DeletableContainer onDelete={deleteTransaction}>
            <div onMouseEnter={() => setEdit(true)}
                 onMouseLeave={() => setEdit(false)}
                 className="d-flex flex-row justify-content-between align-items-center p-1 position-relative">
                <div>
                    <img src={props.transaction.iconUrl} width='65' alt="*Иконка*" className="transaction-icon"/>
                </div>
                <div className='transaction-description'>
                    {props.transaction.description}
                </div>
                <div className="d-flex flex-row justify-content-end">
                    <IconButton
                        onClick={editTransaction}
                        className={classNames("can-hide", canBeEdit ? "shown" : "hidden")}
                        icon={faPen}/>
                    <div>
                        <span className='transaction-cost'>{props.transaction.amount} руб</span>
                    </div>
                </div>
            </div>
        </DeletableContainer>
    )
}

function DebtorsHandler(props) {
    const formAlertToAddDebtor = () => {
        const debtAdded = props.debtAdded;

        Swal.fire({
            title: 'Добавление должника',
            input: 'text',
            inputPlaceholder: 'Имя должника',
            preConfirm(inputValue) {
                Swal.showLoading();
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
                        fireAdded();
                        debtAdded();
                    } else {
                        fireSomethingWentWrong();
                    }
                });
            }
        });
    }
    
    if(props.debtors.length === 0) {
        return <div className="mt-3 max-width">
            <h1>Ваши должники</h1>
            <EmptyBox rightButton={faPlus}
                      onRightButtonClick={formAlertToAddDebtor}/>
        </div>;  
    }
    
    return <div className="mt-3 max-width">
        <h1>Ваши должники</h1>
        <Box className="d-flex flex-column transactions-container list-container"
             rightButton={faPlus}
             onRightButtonClick={formAlertToAddDebtor}>
            {props.debtors.map(d => <DebtorContainer debtAdded={props.debtAdded} debtor={d} key={d.id}/>)}
        </Box>
    </div>;
}

function DebtorContainer(props) {
    const [isOpened, setOpened] = useState(false); 
    let debtorSum = 0;

    props.debtor.debts.map(d => d.amount).forEach(d => debtorSum += d);

    const addDebt = function() {
        const debtorId = props.debtor.id;
        const debtAdded = props.debtAdded;

        Swal.fire({
            title: `Добавление долга ${props.debtor.name}`,
            html: `
                <input type='number' placeholder='Сумма долга' id="debt-sum" class="swal2-input"/>
                <input type="text" placeholder="Описание" id="debt-description" class="swal2-input"/>
            `,
            preConfirm(inputValue) {
                Swal.showLoading();
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
    
    const removeDebtor =  function() {

        fetch(`/api/debtors/remove?id=${props.debtor.id}`, {
            method: 'DELETE'
        }).then(result => {
            if(result.status === 200) {
                fireDeleted();
                props.debtAdded();
            } else {
                fireSomethingWentWrong();
            }
        });
    }
    
    
    return <div className={(isOpened ? 'opened-debtor-list' : "debtor-list") + ' p-1'}>
        <DeletableContainer onDelete={removeDebtor}>
            <Box className="d-flex flex-row justify-content-between align-items-center">
                <span className="font-weight-bolder debtor-name">{props.debtor.name}</span>
                <div className="d-flex flex-row">
                    <span>{debtorSum} руб</span>
                    <IconButton onClick={addDebt} icon={faPlus}/>
                    <IconButton icon={faAngleDown}
                                onClick={() => setOpened(!isOpened)}/>
                </div>
            </Box>    
        </DeletableContainer>
        <div className="debts-container">
            {
                props.debtor.debts.length > 0 ?
                    props.debtor.debts.map(debt => <DebtContainer debtAdded={props.debtAdded} debt={debt}/>) :
                    <div className="d-flex justify-content-center">
                        <span>Здесь ничего нет</span>
                    </div>
            }
        </div>
    </div>;
}

function DebtContainer(props) {
    const removeDebt = function() {
        fetch(`/api/debts/remove-debt?id=${props.debt.debtId}`, {
            method: 'DELETE'
        }).then(result => {
            if(result.status === 200) {
                fireDeleted();
                props.debtAdded();
            } else {
                fireSomethingWentWrong();
            }
        });
    }
    
    return <DeletableContainer onDelete={removeDebt}>
        <div key={props.debt.id} className="d-flex flex-row justify-content-between mt-1">
            <span className={props.debt.amount > 0 ? 'def-debt-span' : 'return-debt-span'}>{props.debt.description}</span>
            <div>
                <span className="font-weight-bolder">
                    {Math.abs(props.debt.amount)} руб
                </span>
            </div>
        </div>
    </DeletableContainer>;
}

function DeletableContainer(props) {
    const [deleteDialogTime, setDialog] = useState(false)
    
    return <div
            className="deletable-container"
            onDoubleClick={() => setDialog(true)}
            onMouseLeave={() => {setDialog(false)}}
            onClick={(e) => {
                if(deleteDialogTime) {
                    Swal.fire({
                        title: "Вы точно хотите удалить?",
                        icon: "question",
                        confirmButtonColor: "#DE3842",
                        confirmButtonText: "Да",
                        showCancelButton: true,
                        cancelButtonText: "Нет"
                    }).then(result => {
                        if(result.isConfirmed) {
                            props.onDelete();
                        }
                    })
                }
            }}>
        {props.children}
        <div className={classNames("max can-hide delete-dialog", deleteDialogTime ? "shown" : "hidden")}>
            <FontAwesomeIcon icon={faTrash}/>
        </div>
    </div>
}

function EmptyBox(props) {
    return <Box className="d-flex flex-column transactions-container justify-content-center align-items-center none-transactions list-container"
            {...props}>
        <span className="colored-in-brand-purple">Здесь ничего нет</span>
    </Box>
}

function fireSuccess(title) {
    return Swal.fire({
       title: title,
       showConfirmButton: false,
       timer: 1000,
       icon: 'success' 
    });
}

function fireAdded() {
    return fireSuccess('Добавлено');
}

function fireDeleted() {
    return fireSuccess('Удалено')
}

function fireEdited() {
    return fireSuccess('Изменено')
}

function fireSomethingWentWrong() {
    return Swal.fire({
        title: "Что-то пошло не так",
        showConfirmButton: false,
        timer: 1000,
        icon: 'error'
    });
}
