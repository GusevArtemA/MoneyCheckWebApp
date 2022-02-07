import React from "react";
import {TextInput} from "../ui/TextInput";
import {Container} from "reactstrap";

export function InflationPage() {
    return <Container className="max">
        <h1>Сколько это будет стоить?</h1>
        <div className="d-flex flex-row justify-content-around align-items-center">
            <div>
                <div>
                    <p>Сколько это стоит сейчас?</p>
                    <TextInput placeholder="В рублях" type="number"/>
                </div>
                <div>
                    <p>На сколько месяцев Вы хотите рассчитать?</p>
                    <TextInput placeholder="Количество месяцев" type="number"/>
                </div>
            </div>
            <div>
                <p>Оно будет стоить в переводе на наши деньги:</p>
                <span className="amount-label">1024 руб</span>
            </div>
        </div>
    </Container>    
}

