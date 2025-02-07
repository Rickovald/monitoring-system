import React, { useEffect, useState, useCallback, useRef } from 'react';
import { TextField, Button } from '@mui/material';
import DataChart from '../../shared/DataChart';
import DataTable from '../../shared/DataTable';
import { fetchSensorData, fetchSensorSummary, SensorData, SensorSummary } from '../../shared/api';

const DataPage: React.FC = () => {
  const [data, setData] = useState<SensorData[]>([]);
  const [summary, setSummary] = useState<SensorSummary[]>([]);
  
  // Храним даты в формате "YYYY-MM-DDTHH:mm" для элемента input
  const [from, setFrom] = useState<string>(() => {
    const d = new Date(Date.now());
    return d.toLocaleString("sv-SE", { timeZone: "Europe/Moscow" }).slice(0, 16);
  });
  const [to, setTo] = useState<string>(() => {
    const d = new Date(Date.now() + 3600000);
    return d.toLocaleString("sv-SE", { timeZone: "Europe/Moscow" }).slice(0, 16);
  });
  
  // Используем useRef для хранения актуальных данных для сравнения
  const dataRef = useRef(data);
  const summaryRef = useRef(summary);

  useEffect(() => {
    dataRef.current = data;
  }, [data]);
  
  useEffect(() => {
    summaryRef.current = summary;
  }, [summary]);

  // Функция загрузки данных с проверкой на изменения
  const loadData = useCallback(() => {
    const fromISO = new Date(from).toISOString();
    const toISO = new Date(to).toISOString();

    fetchSensorData(fromISO, toISO)
      .then(res => {
        // Преобразуем массив в строку для простого сравнения.
        // Если данные изменились, обновляем состояние.
        if (JSON.stringify(res.data) !== JSON.stringify(dataRef.current)) {
          setData(res.data);
        }
      })
      .catch(err => console.error(err));
  
    fetchSensorSummary(fromISO, toISO)
      .then(res => {
        if (JSON.stringify(res.data) !== JSON.stringify(summaryRef.current)) {
          setSummary(res.data);
        }
      })
      .catch(err => console.error(err));
  }, [from, to]);

  // При изменении диапазона сразу загружаем данные
  useEffect(() => {
    loadData();
  }, [loadData]);

  // Интервал для автоматического обновления каждые 5 секунд
  useEffect(() => {
    const interval = setInterval(() => {
      loadData();
    }, 5000);
    return () => clearInterval(interval);
  }, [loadData]);

  return (
    <div>
      <h2>Данные с датчиков</h2>
      <div style={{ marginBottom: '20px' }}>
        <TextField
          label="From"
          type="datetime-local"
          value={from}
          onChange={e => setFrom(e.target.value)}
        />
        <TextField
          label="To"
          type="datetime-local"
          value={to}
          onChange={e => setTo(e.target.value)}
          style={{ marginLeft: '10px' }}
        />
        <Button
          variant="contained"
          onClick={loadData}
          style={{ marginLeft: '10px' }}
        >
          Обновить
        </Button>
      </div>
      <DataChart data={data} />
      <DataTable data={data} />
      <h3>Агрегированные данные</h3>
      <ul>
        {summary.map(s => (
          <li key={s.sensorId}>
            Датчик {s.sensorId}: Среднее = {s.average.toFixed(2)}, Макс = {s.maximum.toFixed(2)}, Мин = {s.minimum.toFixed(2)}
          </li>
        ))}
      </ul>
    </div>
  );
};

export default DataPage