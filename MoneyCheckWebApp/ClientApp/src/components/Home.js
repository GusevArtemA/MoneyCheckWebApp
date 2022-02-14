import React, {useEffect, useRef, useState} from "react";
import {MCApi} from "../services/MCApi";
import {Loader} from "../ui/Loader";

import "../assets/scss/pages/home.scss";
import {Box} from "../ui/Box";
import {NavLink, Redirect} from "react-router-dom";
import {SelectBox} from "../ui/SelectBox";

import '../prototypes';
import {faAngleDown, faMoneyBill, faPen, faPlus, faTrash} from "@fortawesome/free-solid-svg-icons";
import Swal from "sweetalert2";
import {IconButton} from "../ui/IconButton";
import {FontAwesomeIcon} from "@fortawesome/react-fontawesome";
import withReactContent from 'sweetalert2-react-content'
import classNames from "classnames";
import {AutoComplete} from "../ui/AutoComplete";
import {ImagesSlider} from "../ui/ImagesSlider";
import {TextInput} from "../ui/TextInput";
import {CookieHelper} from "../services/CookieHelper";
import {PageLoader} from "../ui/PageLoader";
import {AnimatedLogo} from "../ui/AnimatedLogo";

const MySwal = withReactContent(Swal)

export function Home(props) {
    const api = new MCApi();
    const [userInfo, setUserInfo] = useState(null);
    const [categories, setCategories] = useState(null);
    const [customCategories, setCustomCategories] = useState(null);
    const [debtors, setDebtors] = useState(null);
    const [transactions, setTransactions] = useState(null);
    const [balanceInfo, setBalanceInfo] = useState(null);
    const [refreshing, setRefreshing] = useState(false);
    const [availableCatIcons, setCatIcons] = useState(null);
    
    useEffect(() => {
        api.getUserInfo().then(data =>
            setUserInfo(data));
        
        api.getCategories().then(data =>
            setCategories(data));
        
        api.getCategories(false).then(data =>
            setCustomCategories(data));
        
        api.getBalanceState().then(data =>
            setBalanceInfo(data));

        api.getPurchases().then(data =>
            setTransactions(data));

        api.getDebtors().then(data =>
            setDebtors(data));
        api.getCatLogos().then(data => 
            setCatIcons(data));
        return () => {};
    }, []);

    if(!new CookieHelper().canAuthByCookie()) {
        return <Redirect to="/welcome "/>
    }
    
    if(userInfo === null ||
        categories === null ||
        debtors === null ||
        transactions === null ||
        balanceInfo === null ||
        availableCatIcons === null ||
        refreshing) {
        return <PageLoader/>
    }

    return <div className="max container">
        <Greeter username={userInfo.username}/>
        <div className="main-home-wrapper d-flex flex-row justify-content-around max-height">
            <div className='trans-debtors-wrapper d-flex flex-column align-items-center'>
                <TransactionsHandler
                    transactions={transactions}
                    categories={categories}
                    refresh={() => {setRefreshing(true)}}
                    filterValueChanged={(filter) => api.getPurchases(filter).then(data =>
                        setTransactions(data))}
                    itemAdded={(filter) =>
                        api.getPurchases(filter).then(data => {
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
                <CategoriesHandler needUpdate={() => 
                    api.getCategories(false).then(data =>
                        setCustomCategories(data))} 
                   availableIcons={availableCatIcons} categories={customCategories}/>
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

    return <div className="d-flex flex-row justify-content-between align-items-center mt-1">
        <h1 id="greeter">Добрый {time}, {props.username}</h1>
        <NavLink to="/home"><AnimatedLogo/></NavLink>
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
            <NavLink to='/inflation' id="inflation-router">Расчет инфляции</NavLink>
        </div>
        <div className="d-flex flex-column">
            <span className="prop">Прогноз на конец месяца:</span>
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
    const [timeSpan, setTimeSpan] = useState("день");
    const [filter, setFilter] = useState("нет");
    const [orderFilter, setOrder] = useState("возрастания");
    
    const selectBoxValueChangedHandler = function (value) {
        let f = 'ERR';
        
        switch (value) {
            case 'день':
                f = 'day';
                break;
            case 'месяц':
                f = 'month';
                break;
            case 'год':
                f = 'year';
                break;
        }

        setTimeSpan(f);
        props.filterValueChanged(f);
    }
    
    const filterValueChanged = function(value) {
        setFilter(value);
    }
    
    const orderFilterValueChanged = function (value) {
        setOrder(value);
    }
    
    let arrayOfTransactions = [...props.transactions];
    
    switch (filter) {
        case "цене":
            switch (orderFilter) {
                case "возрастания":
                    arrayOfTransactions.sort((a, b) => a.amount - b.amount);
                    break;
                case "убывания":
                    arrayOfTransactions.sort((a, b) => b.amount - a.amount);
                    break;
            }
            break;
        case "дате":
            switch (orderFilter) {
                case "возрастания":
                    arrayOfTransactions.sort((a, b) => Date.parse(a.timeStamp) - Date.parse(b.timeStamp));
                    break;
                case "убывания":
                    arrayOfTransactions.sort((a, b) => Date.parse(b.timeStamp) - Date.parse(a.timeStamp));
                    break;
            }
            break;
    }
    
    return <div className="max-width">
        <div>
            <div className="d-flex flex-row align-items-start flex-wrap" id="transactions-filter">
                <h1>Ваши транзакции за этот </h1>
                <SelectBox items={['день', 'месяц', 'год']} onValueChanged={selectBoxValueChangedHandler}/>
            </div>
            <div className="d-flex flex-row align-items-center justify-content-start mb-1 flex-wrap">
                <p className="mb-0">Фильтровать по</p>
                <SelectBox
                    onValueChanged={filterValueChanged}
                    items={['нет', 'цене', 'дате']}/>
                {filter !== 'нет' && <>
                    <p className="mb-0">в порядке</p>
                    <SelectBox
                        items={["возрастания", "убывания"]}
                        onValueChanged={orderFilterValueChanged}/>
                </>}
            </div>
        </div>
        <TransactionsContainer transactions={arrayOfTransactions}
                               categories={props.categories}
                               itemAdded={() => {props.itemAdded(timeSpan);}}/>
    </div>
}

function TransactionsContainer(props) {
    const [capacityId, setCapacityId] = useState(-1);
    const [capacity, setCapacity] = useState(-1);
    
    const formAlertToAddTransaction = function() {
        const itemAdded = props.itemAdded;
        MySwal.fire({
            title: 'Добавление транзакции',
            confirmButtonColor: '#2EC321',
            html: (<form className="swal-form d-flex justify-content-center flex-column align-items-center">
                <input type="number" placeholder="Сумма в рублях" className="swal2-input" id="amount"/>
                <AutoComplete items={props.categories.map(a => a.name)} id="category-name"/>
            </form>),
            async preConfirm(inputValue) {
                Swal.showLoading();
                let amount = document.getElementById('amount');
                let selectBoxValue = document.getElementById('category-name').value;
                
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
    
    const capacityChangedHandler = function (id, newCap) {
        setCapacityId(id);
        setCapacity(newCap);
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
        {props.transactions.map(x => <TransactionContainer onSetAsBaseCapacity={capacityChangedHandler}
                                                           capacity={capacity}
                                                           capacityId={capacityId}
                                                           key={x.id}
                                                           onItemDelete={props.itemAdded}
                                                           transaction={x}/>)}
    </Box>
}

function TransactionContainer(props) {
    const [canBeEdit, setEdit] = useState(false);
    
    let cost = Math.round(props.transaction.amount / (props.capacity > 0 ? props.capacity : 1) * 100) / 100;
    
    const deleteTransaction = function () {
        fetch('/api/transactions/remove-purchase?id=' + props.transaction.id, {
            method: 'DELETE'
        }).then(result => {
           if(result.status === 200) {
               fireDeleted().then(() => props.onItemDelete());
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
                            fireEdited().then(() => props.onItemDelete());
                        } else {
                            fireSomethingWentWrong();
                        }
                    })
                }
            }
        })
    }
    
    const capacityChangerHandler = function () {
        if(props.capacityId === props.transaction.id) {
            props.onSetAsBaseCapacity(-1, -1);
            return;
        }
        
        props.onSetAsBaseCapacity(props.transaction.id, props.transaction.amount);
    }
    
    return (
        <DeletableContainer
            onMouseEnter={() => setEdit(true)}
            onMouseLeave={() => setEdit(false)}
            className="d-flex flex-row justify-content-between align-items-center p-1 position-relative ч"
            onItemDelete={deleteTransaction}>
            <div>
                <img src={props.transaction.iconUrl} width='65' alt="*Иконка*" className="transaction-icon"/>
            </div>
            <div className="d-flex justify-content-between cat-cost-combo align-items-center">
                <div className='transaction-description cat-cost-component'>
                    {props.transaction.description}
                </div>
                <div className="d-flex flex-row justify-content-end cat-cost-component">
                    <div className="transaction-operations-wrapper">
                        <IconButton
                            onClick={editTransaction}
                            className={classNames("can-hide", canBeEdit ? "shown" : "hidden")}
                            icon={faPen}/>
                        <IconButton
                            onClick={capacityChangerHandler}
                            className={classNames("can-hide", canBeEdit || props.capacityId === props.transaction.id ? "shown" : "hidden")}
                            icon={faMoneyBill}/>
                    </div>
                    <div className="d-flex flex-column justify-content-end align-items-end">
                        <span className='transaction-cost'>{cost} {props.capacityId !== -1 ? "шт" : "руб"}</span>
                        <DateSpan className="ml-2" date={props.transaction.timeStamp}/>
                    </div>
                </div>
            </div>
        </DeletableContainer>
    )
}

function DateSpan(props) {
    let dateTime = new Date(props.date);
    
    const [month, day, hours, minutes] = [dateTime.getMonth(), dateTime.getDate(), dateTime.getHours(), dateTime.getMinutes()];
    
    let monthString;
    
    switch (month) {
        case 0:
            monthString = "янв";
            break;
        case 1: 
            monthString = "фев";
            break;
        case 2: 
            monthString = "мрт";
            break;
        case 3:
            monthString= "апр";
            break;
        case 4:
            monthString = "май";
            break;
        case 5:
            monthString = "июн";
            break;
        case 6:
            monthString = "июл";
            break;
        case 7: 
            monthString = "авг";
            break;
        case 8: 
            monthString = "сен";
            break;
        case 9: 
            monthString = "окт";
            break;
        case 10:
            monthString = "ноя";
            break;
        case 11:
            monthString = "дек";
            break;
    }
    
    return <span className={classNames("date-time-label", props.className)}>
        {day} {monthString}, {hours > 9 ? hours : "0" + hours}:{minutes > 9 ? minutes : "0" + minutes}
    </span>;
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
        <DeletableContainer onItemDelete={removeDebtor} className="d-flex flex-row justify-content-between align-items-center">
            <span className="font-weight-bolder debtor-name">{props.debtor.name}</span>
            <div className="d-flex flex-row">
                <span>{debtorSum} руб</span>
                <IconButton onClick={addDebt} icon={faPlus}/>
                <IconButton icon={faAngleDown}
                            onClick={() => setOpened(!isOpened)}/>
            </div>
        </DeletableContainer>
        <div className="debts-container">
            {
                props.debtor.debts.length > 0 ?
                    props.debtor.debts.map(debt => <DebtContainer debtAdded={props.debtAdded} debt={debt} key={debt.debtId}/>) :
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
    
    return <DeletableContainer onItemDelete={removeDebt} key={props.debt.id} className="d-flex flex-row justify-content-between mt-1">
        <span className={props.debt.amount > 0 ? 'def-debt-span' : 'return-debt-span'}>{props.debt.description}</span>
        <div>
            <span className="font-weight-bolder">
                {Math.abs(props.debt.amount)} руб
            </span>
        </div>
    </DeletableContainer>;
}

function CategoriesHandler(props) {
    return <div className="max-width">
        <h1>Ваши собственные категории</h1>
        <CategoriesContainer needUpdate={props.needUpdate} availableIcons={props.availableIcons} categories={props.categories}/>
    </div>
}

function CategoriesContainer(props) {
    if(props.categories.length === 0) {
        return <EmptyBox/>
    }
    
    const addNewCategoryHandler = function () {
        let icoIndex = 0;
        MySwal.fire({
            html: <form>
                <TextInput id="category-name" type="text" placeholder="Название категории"/>
                <ImagesSlider items={props.availableIcons} onValueChanged={(a) => icoIndex = a}/>
            </form>,
            preConfirm() {
                Swal.showLoading();
                let catName = document.getElementById("category-name").value;
                
                if(catName === '') {
                    Swal.showValidationMessage("Название категории обязательно");
                    return;
                }
                
                fetch('/api/categories/add-category', {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify({
                        categoryName: catName,
                        logoId: props.availableIcons[icoIndex].id
                    })
                }).then(response => {
                    if(response.ok) {
                        fireAdded();
                        props.needUpdate();
                    } else {
                        fireSomethingWentWrong();
                    }
                });
            }
        });
    }
    
    return <Box className="box max-width d-flex flex-column transactions-container list-container"
            rightButton={faPlus}
            onRightButtonClick={addNewCategoryHandler}>
        {props.categories.map(x => <CategoryContainer needUpdate={props.needUpdate} key={x.id} category={x}/>)}
    </Box>
}

function CategoryContainer(props) {
    function deleteCategory() {
        fetch(`/api/categories/delete-category?id=${props.category.id}`, {
            method: "DELETE"
        }).then(res => {
            if(res.ok) {
                fireDeleted();
                props.needUpdate();   
            } else {
                fireSomethingWentWrong();
            }
        })
    }
    
    return <DeletableContainer className="d-flex justify-content-between align-items-center mb-1" onItemDelete={deleteCategory}>
        <img className="transaction-icon" src={"/api/media/get-category-media-logo?categoryId=" + props.category.id} alt="Логотип категории"/>
        <span>{props.category.name}</span>
    </DeletableContainer>
}

function DeletableContainer(props) {
    const [deleteDialogTime, setDialog] = useState(false)
    const containerRef = useRef();
    
    return <div
            {...props}
            ref={containerRef}
            className={classNames("deletable-container", props.className)}
            onDoubleClick={(e) => {
                if(e.target === containerRef.current || e.target.tagName === 'DIV') {
                    setDialog(true);
                }       
            }}
            onMouseLeave={(e) => {
                setDialog(false); 
                if(props.onMouseLeave) {
                    props.onMouseLeave(e);
                }
            }}
            onClick={(e) => {
                if(deleteDialogTime && e.target === containerRef.current.querySelector('.delete-dialog')) {
                    Swal.fire({
                        title: "Вы точно хотите удалить?",
                        icon: "question",
                        confirmButtonColor: "#DE3842",
                        confirmButtonText: "Да",
                        showCancelButton: true,
                        cancelButtonText: "Нет"
                    }).then(result => {
                        if(result.isConfirmed) {
                            props.onItemDelete();
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
