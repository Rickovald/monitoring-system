import React, { useEffect, useState } from 'react';
import { Button, TextField, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Paper } from '@mui/material';
import { Line } from 'react-chartjs-2';
import axios from 'axios';

interface SensorData {
    id: number;
    sensorId: number;
    value: number;
    timestamp: string;
}

interface Dataset {
    label: string;
    data: number[];
    fill: boolean;
    backgroundColor: string;
    borderColor: string;
}
  
interface ChartData {
    labels: string[];
    datasets: Dataset[];
}
const HomePage: React.FC = () => {
    const [startDate, setStartDate] = useState('');
    const [endDate, setEndDate] = useState('');
    const [data, setData] = useState<SensorData[]>([]);
    const [chartData, setChartData] = useState<ChartData | null>();

    useEffect(() => {
        const fetchData = async () => {
            try {
                const response = await axios.get<SensorData[]>('http://localhost:5001/api/SensorData', {
                    params: { startDate, endDate }
                });
                setData(response.data);

                const labels = response.data.map(d => new Date(d.timestamp).toLocaleString("ru-RU", { timeZone: "Europe/Moscow" }));
                const datasets = Array.from({ length: 3 }, (_, i) => ({
                    label: `Sensor ${i + 1}`,
                    data: response.data.filter(d => d.sensorId === i + 1).map(d => d.value),
                    fill: false,
                    backgroundColor: `rgba(${Math.floor(Math.random() * 256)}, ${Math.floor(Math.random() * 256)}, ${Math.floor(Math.random() * 256)}, 0.6)`,
                    borderColor: `rgba(${Math.floor(Math.random() * 256)}, ${Math.floor(Math.random() * 256)}, ${Math.floor(Math.random() * 256)}, 1)`
                }));

                setChartData({
                    labels,
                    datasets
                });
            } catch (error) {
                console.error(error);
            }
        };

        fetchData();
        const interval = setInterval(fetchData, 5000);

        return () => clearInterval(interval);
    }, [startDate, endDate]);

    const handleExport = async () => {
        const response = await axios.get<Blob>('http://localhost:5001/api/SensorData/export', { responseType: 'blob' });
        const url = window.URL.createObjectURL(new Blob([response.data]));
        const link = document.createElement('a');
        link.href = url;
        link.setAttribute('download', 'data.xml');
        document.body.appendChild(link);
        link.click();
    };

    return (
        <div>
            <h1>Sensor Data Monitoring</h1>
            <div>
                <TextField
                    id="start-date"
                    label="Start Date"
                    type="datetime-local"
                    value={startDate}
                    onChange={(e) => setStartDate(e.target.value)}
                />
                <TextField
                    id="end-date"
                    label="End Date"
                    type="datetime-local"
                    value={endDate}
                    onChange={(e) => setEndDate(e.target.value)}
                />
            </div>
            <Button variant="contained" color="primary" onClick={handleExport}>
                Export XML
            </Button>
            <TableContainer component={Paper}>
                <Table aria-label="simple table">
                    <TableHead>
                        <TableRow>
                            <TableCell>ID</TableCell>
                            <TableCell>Sensor ID</TableCell>
                            <TableCell>Value</TableCell>
                            <TableCell>Timestamp</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {data.map((row) => (
                            <TableRow key={row.id}>
                                <TableCell component="th" scope="row">
                                    {row.id}
                                </TableCell>
                                <TableCell>{row.sensorId}</TableCell>
                                <TableCell>{row.value}</TableCell>
                                <TableCell>{new Date(row.timestamp).toLocaleString("ru-RU", { timeZone: "Europe/Moscow" })}</TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
                </Table>
            </TableContainer>
            {chartData && <Line data={chartData} />}
        </div>
    );
};

export default HomePage;