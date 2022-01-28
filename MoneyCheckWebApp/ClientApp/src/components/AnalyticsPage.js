import React, {useEffect, useState} from "react";
import {ChartComponent, Inject, Category,
        Legend, SplineSeries, ColumnSeries,
        Tooltip, DataLabel} from "@syncfusion/ej2-react-charts";
import {SeriesCollectionDirective, SeriesDirective} from "@syncfusion/ej2-react-charts/src/chart/series-directive";
import {Loader} from "../ui/Loader";
import {MCApi} from "../services/MCApi";
import {Box} from "../ui/Box";

import "../assets/scss/pages/analytics.scss";
import {Container} from "reactstrap";
import Logo from "../assets/images/logo.svg";

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
    
    return <Container>
        <div className="d-flex justify-content-between align-items-center mb-2 mt-2">
            <h1>Как я тратил деньги в этом году</h1>
            <img src={Logo} alt="Logotype" width="75"/>
        </div>
        <SplineDiagramContainer data={splineDiagramData}/>
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