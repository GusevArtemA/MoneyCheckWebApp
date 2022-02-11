import React, {useEffect, useRef, useState} from "react";
import {TextInput} from "../ui/TextInput";
import {Container} from "reactstrap";
import {Loader} from "../ui/Loader";
import {MCApi} from "../services/MCApi";
import "../assets/scss/pages/inflation.scss";
import LogoSvg from "../assets/images/logo.svg";
import {ChartComponent, Inject, Category,
    Legend, SplineSeries, ColumnSeries,} from "@syncfusion/ej2-react-charts";
import {SeriesCollectionDirective, SeriesDirective} from "@syncfusion/ej2-react-charts/src/chart/series-directive";
import {Box} from "../ui/Box";
import {DataLabel, Tooltip} from "@syncfusion/ej2-react-charts";
import AnimatedLogo from "../assets/images/animated/animated-logo.svg";
import {NavLink, Redirect} from "react-router-dom";
import {CookieHelper} from "../services/CookieHelper";

export function InflationPage() {
    const [inflations, setInflations] = useState(null);
    let api = new MCApi();

    useEffect(() => {
        api.getInflationForYear().then(data => setInflations(data));
    }, []);
    
    if(!new CookieHelper().canAuthByCookie()) {
        return <Redirect to="/welcome"/>
    }
    
    if(inflations === null) {
        return <Loader/>;
    }
    
    return <Container className="max">
        <div className="d-flex flex-row justify-content-between align-items-center mt-1">
            <h1>Сколько это будет стоить?</h1>
            <NavLink to="/home"><object width="75" type="image/svg+xml" data={AnimatedLogo}>Animated Logo</object></NavLink>
        </div>
        <InflationForm/>
        <div>
            <h1>Прогноз инфляции с помощью нейросети <span className="colored-in-brand-purple money-check-infl-label">Money Check</span></h1>
            <Box>
                <SplineDiagramContainer data={inflations}/>
            </Box>
        </div>
    </Container>    
}

function InflationForm() {
    const [inflationFutureCash, setFutureInflationCash] = useState(-1);
    const [cachedIndex, setCachedIndex] = useState(1);
    const [cachedCost, setCachedCost] = useState(1000);

    const nowCostRef = useRef();
    const indexRef = useRef();

    let api = new MCApi();
    
    useEffect(() => {
        nowCostRef.current.value = 1000;
        indexRef.current.value = 1;
        refreshData();
    }, []);

    function refreshData() {
        if(Number.parseInt(nowCostRef.current.value) <= 0) {
            nowCostRef.current.value = 1000;
        }

        if(Number.parseInt(indexRef.current.value) <= 0) {
            indexRef.current.value = 1;
        }
        
        refreshCost(Number.parseInt(nowCostRef.current.value), Number.parseInt(indexRef.current.value));
    }
    
    function refreshCost(cost, index) {
        if(cost !== cachedCost || index !== cachedIndex || inflationFutureCash === -1) {
            setCachedCost(cost);
            setCachedIndex(index);
            api.getInflationPredication(cost, index).then(data => setFutureInflationCash(data));
        }
    }
    
    return <div className="d-flex flex-row justify-content-around align-items-center inflation-form-container">
        <div>
            <div>
                <p>Сколько это стоит сейчас?</p>
                <TextInput onBlur={refreshData}  placeholder="В рублях" type="number" inputRef={nowCostRef} />
            </div>
            <div>
                <p>На сколько месяцев Вы хотите рассчитать?</p>
                <TextInput onBlur={refreshData} placeholder="Количество месяцев" type="number" inputRef={indexRef} />
            </div>
        </div>
        <div>
            <p>Оно будет стоить в переводе на наши деньги:</p>
            {inflationFutureCash > 0 ? <span className="amount-label">{inflationFutureCash} руб</span> : <Loader/>}
        </div>
    </div>;
}

function SplineDiagramContainer(props) {
    return <Box className="diagram-container">
        <ChartComponent
            primaryXAxis={{valueType: "Category"}}
            tooltip={{ enable: true }}>
            <Inject services={[Category, SplineSeries, ColumnSeries, Tooltip, DataLabel, Legend]}/>
            <SeriesCollectionDirective>
                <SeriesDirective fill="#B122F4"
                                 type="Spline"
                                 dataSource={props.data}
                                 xName="index"
                                 yName="value"
                                 name="Прогноз инфляции"
                                 marker={{ visible: true, width: 10, height: 10 }}
                                 animation={{enable: true, duration: 1200}}/>
            </SeriesCollectionDirective>
        </ChartComponent>
    </Box>;
}