import axios from 'axios';
import { SensorData, SensorSummary } from '../sensorData';

/**
 * Получает данные за указанный период.
 */
export const fetchSensorData = (from: string, to: string) =>
    axios.get<SensorData[]>(`http://localhost:5177/api/data`, { params: { from, to } });

/**
 * Получает агрегированные данные.
 */
export const fetchSensorSummary = (from: string, to: string) =>
    axios.get<SensorSummary[]>(`http://localhost:5177/api/sensors/summary`, { params: { from, to } });