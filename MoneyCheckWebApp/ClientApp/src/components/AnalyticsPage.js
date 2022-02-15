import React, {useEffect, useState} from "react";
import {ChartComponent, Inject, Category,
        Legend, SplineSeries, ColumnSeries,
        Tooltip, DataLabel,
        AccumulationChartComponent, AccumulationSeriesCollectionDirective, AccumulationSeriesDirective,
        AccumulationLegend, PieSeries, AccumulationDataLabel} from "@syncfusion/ej2-react-charts";
import {SeriesCollectionDirective, SeriesDirective} from "@syncfusion/ej2-react-charts/src/chart/series-directive";
import {MCApi} from "../services/MCApi";
import {Box} from "../ui/Box";

import "../assets/scss/pages/analytics.scss";
import {Container} from "reactstrap";
import {NavLink, Redirect} from "react-router-dom";
import {CookieHelper} from "../services/CookieHelper";
import {PageLoader} from "../ui/PageLoader";
import {AnimatedLogo} from "../ui/AnimatedLogo";

export function AnalyticsPage() {
    const [splineDiagramData, setSplineDiagram] = useState(null);
    const [pieData, setPieData] = useState(null);
    
    useEffect(() => {
        const api = new MCApi();
        
        api.getStatsForYearAnalytics().then(data => setSplineDiagram(data));
        api.getCategoriesData().then(data => setPieData(data));
    }, []);

    if(!new CookieHelper().canAuthByCookie()) {
        return <Redirect to="/welcome"/>
    }
    
    if(splineDiagramData == null || pieData == null) {
        return <PageLoader/>
    }

    if(splineDiagramData.length === 0 || pieData.length === 0) {
        return <SpendMoneyToGetStats/>
    }
    
    return <Container>
        <div className="d-flex justify-content-between align-items-center mb-2 mt-2">
            <h1>Как я тратил деньги в этом году</h1>
            <NavLink to="/home"><AnimatedLogo/></NavLink>
        </div>
        <SplineDiagramContainer data={splineDiagramData}/>
        <div className="d-flex flex-row mt-2 justify-content-between align-items-center pie-and-export-block">
            <Box className="half-fill-x pie-chart-wrapper">
                <PieDiagram data={pieData}/>
            </Box>
            <Box className="d-flex flex-column half-fill-x export-container">
                <ExportAsCSVBlock/>
            </Box>
        </div>
    </Container>
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
                                 yName="amount"
                                 name="Сумма трат"
                                 marker={{ visible: true, width: 10, height: 10 }}
                                animation={{enable: true, duration: 1200}}/>
            </SeriesCollectionDirective>
        </ChartComponent>
    </Box>;
}

function ExportAsCSVBlock() {
    return <a href="/api/exports/csv-purchases" className="brand-button" download>
        Экспорт в CSV
    </a>
}

function PieDiagram(props) {
    return <AccumulationChartComponent
                useGroupingSeparator={true} enableSmartLabels={true} enableAnimation={true}>
        <Inject services={[AccumulationLegend, PieSeries, AccumulationDataLabel]}/>
        <AccumulationSeriesCollectionDirective>
            <AccumulationSeriesDirective dataSource={props.data} xName='categoryName' yName='categoryAmount' innerRadius='20%' dataLabel={{
                visible: true, position: 'Outside', name: 'x'
            }}/>
        </AccumulationSeriesCollectionDirective>
    </AccumulationChartComponent>
}

function SpendMoneyToGetStats() {
    return <div className="position-absolute d-flex justify-content-center align-items-center max text-center">
        <span className="colored-in-brand-purple font-weight-bold spend-money-to-get-stats">Совершайте покупки, чтобы получать статистику 💸</span>
    </div>
}