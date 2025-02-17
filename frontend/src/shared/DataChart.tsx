import React from 'react';
import { Line } from 'react-chartjs-2';
import { Chart, CategoryScale, LinearScale, PointElement, LineElement, Title, Tooltip, Legend } from 'chart.js';
import { SensorData } from '../entities/SensorData/sensorData';

Chart.register(CategoryScale, LinearScale, PointElement, LineElement, Title, Tooltip, Legend);

interface DataChartProps {
  data: SensorData[];
}

const DataChart: React.FC<DataChartProps> = ({ data }) => {
  const chartData = {
    labels: data.map(d => new Date(d.timestamp).toLocaleTimeString()),
    datasets: [
      {
        label: 'Sensor Data',
        data: data.map(d => d.value),
        fill: false,
        borderColor: 'rgb(75, 192, 192)'
      }
    ]
  };

  return <Line data={chartData} />;
};

export default DataChart;
