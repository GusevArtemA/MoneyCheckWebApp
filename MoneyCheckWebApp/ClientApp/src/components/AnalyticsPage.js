import React, {useEffect, useState} from "react";
import {ChartComponent, Inject, LineSeries, Category, SplineSeries} from "@syncfusion/ej2-react-charts";
import {SeriesCollectionDirective, SeriesDirective} from "@syncfusion/ej2-react-charts/src/chart/series-directive";
import {Loader} from "../ui/Loader";
import {MCApi} from "../services/MCApi";
import {Box} from "../ui/Box";

import "../assets/scss/pages/analytics.scss";

export function AnalyticsPage() {
    const [splineDiagramData, setSplineDiagram] = useState(null);
    
    useEffect(() => {
        new MCApi().getStatsForYearAnalytics().then(data => setSplineDiagram(data));
    }, []);
    
    if(splineDiagramData == null) {
        return <div className="max justify-content-center align-items-center">
            <Loader/>
        </div>
    }
    
    return <SplineDiagramContainer data={splineDiagramData}/>
}

function SplineDiagramContainer(props) {
    const [janData, setJanData] = useState();
    
    useEffect(() => {
        setJanData(new MCApi().getStatsForYearAnalytics("month", 1).then(res => setJanData(res)));
    }, []);
    
    if(props.data.length === 1) {
        return <Box className="diagram-container">
            <p className="font-weight-bold">Ваши траты за январь</p>
            <ChartComponent title={props.title}
                            primaryXAxis={{valueType: "Category"}}>
                <Inject services={[LineSeries, Category, SplineSeries]}/>
                <SeriesCollectionDirective>
                    <SeriesDirective fill="#B122F4" type="Spline" dataSource={janData} xName="index" yName="amount"/>
                </SeriesCollectionDirective>
            </ChartComponent>
        </Box>
    }
    
    return <Box className="diagram-container">
        <ChartComponent title={props.title}
            primaryXAxis={{valueType: "Category"}}>
            <Inject services={[LineSeries, Category, SplineSeries]}/>
            <SeriesCollectionDirective>
                <SeriesDirective fill="#B122F4" type="Spline" dataSource={props.data} xName="index" yName="amount"/>
            </SeriesCollectionDirective>
        </ChartComponent>
    </Box>;
}