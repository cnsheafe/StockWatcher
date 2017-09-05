import * as React from "react";
import * as Chart from "chart.js";
import { connect } from "react-redux";

import { IState } from "../store/store";
import { Graph } from "../store/data-blocks";


interface GraphsProps {
    graphs: Array<Graph>
}

class Graphs extends React.Component<GraphsProps, {}> {
    render() {
        const graphs = this.props.graphs.map<JSX.Element>((graph: Graph,index: number) => {
            return (
                <li key={index}><canvas id={graph.graphId}></canvas></li>
            );
        });

        return (
            <ul id="graphs">
                {graphs}
            </ul>
        );
    }

    componentDidUpdate() {
        this.props.graphs.forEach(graph => {
            let context = document.getElementById(graph.graphId) as HTMLCanvasElement;
            let config: Chart.ChartConfiguration = ChartConfigurationBuilder<number>("line", graph.dataset);

            let chart = new Chart(context, config);
        })
    }
}

function ChartConfigurationBuilder<T>(type: Chart.ChartType, dataPoints: Array<T>): Chart.ChartConfiguration {

    let chartDataSets: Chart.ChartDataSets[] = [{
        data: dataPoints
    }];

    let chartData: Chart.ChartData = {
        datasets: chartDataSets
    };

    return {
        type: type,
        data: chartData
    };
}

function mapStateToProps(state: IState) {
    return {
        graphs: state.graphs
    }
}

export default connect(mapStateToProps)(Graphs);