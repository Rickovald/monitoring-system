import axios from 'axios';

const API_BASE = 'http://localhost:5177/api';

export interface SensorData {
  id: number;
  sensorId: number;
  value: number;
  timestamp: string;
}

export interface SensorSummary {
  sensorId: number;
  average: number;
  maximum: number;
  minimum: number;
}

/**
 * Получает данные за указанный период.
 */
export const fetchSensorData = (from: string, to: string) =>
  axios.get<SensorData[]>(`${API_BASE}/data`, { params: { from, to } });

/**
 * Получает агрегированные данные.
 */
export const fetchSensorSummary = (from: string, to: string) =>
  axios.get<SensorSummary[]>(`${API_BASE}/sensors/summary`, { params: { from, to } });

/**
 * Отправляет XML-файл на API для валидации.
 * @param file XML-файл.
 */
export const uploadXml = (file: File) => {
    const formData = new FormData();
    formData.append('file', file);
    return axios.post(`${API_BASE}/upload-xml`, formData, {
      headers: { 'Content-Type': 'multipart/form-data' }
    });
  };
  