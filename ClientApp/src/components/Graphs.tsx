import * as React from "react";
import * as Chart from "chart.js";
import { connect } from "react-redux";

import { IState } from "../store/store";
import { Graph, DataPoints } from "../store/data-blocks";


interface GraphsProps {
  graphs: Array<Graph>
}

class Graphs extends React.Component<GraphsProps, {}> {
  render() {
    const graphs = this.props.graphs.map<JSX.Element>((graph: Graph, index: number) => {
      return (
        <li key={index} className="graphs-list-item">
            <canvas id={graph.graphId}></canvas>
        </li>
      );
    });

    return (
      <ul id="graphs" className={graphs.length > 0 ? "graphs-list" : "hide"}>
        {graphs}
      </ul>
    );
  }

  componentDidUpdate() {
    this.props.graphs.forEach(graph => {
      let context = document.getElementById(graph.graphId) as HTMLCanvasElement;
      let labels = graph.labels.map<string>((label, index) => {
        let tmp = label.split(" ")[1];
        return tmp.substring(0, tmp.length-3);
      });
      let config: Chart.ChartConfiguration = ChartDataConfigurationBuilder("line", graph.dataset, labels);

      config.options = {
        maintainAspectRatio: false
      };

      config.options.title = {
        display: true,
        fontFamily: "'Lato', sans-serif",
        text: graph.company.name
      };
      let chart = new Chart(context, config);
    })
  }
}

function ChartDataConfigurationBuilder(type: Chart.ChartType, dataPoints: Array<number>, labels: Array<string>): Chart.ChartConfiguration {

  let chartDataSets: Chart.ChartDataSets[] = [{
    data: dataPoints,
    label: "Stock Price in USD",
    backgroundColor: "#00BCD4",
    borderColor: "#0097A7",
    borderWidth: 5
  }];

  let chartData: Chart.ChartData = {
    datasets: chartDataSets,
    labels: labels
  };

  return {
    type: type,
    data: chartData
  } as Chart.ChartConfiguration;
}

function mapStateToProps(state: IState) {
  return {
    graphs: state.graphs
  }
}

export default connect(mapStateToProps)(Graphs);