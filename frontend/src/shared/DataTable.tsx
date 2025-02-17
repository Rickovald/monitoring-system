import React from 'react';
import { Table, TableBody, TableCell, TableHead, TableRow } from '@mui/material';
import { SensorData } from '../entities/SensorData/sensorData';

interface DataTableProps {
  data: SensorData[];
}

const DataTable: React.FC<DataTableProps> = ({ data }) => {
  return (
    <Table>
      <TableHead>
        <TableRow>
          <TableCell>ID</TableCell>
          <TableCell>Sensor ID</TableCell>
          <TableCell>Value</TableCell>
          <TableCell>Timestamp</TableCell>
        </TableRow>
      </TableHead>
      <TableBody>
        {data.map((d) => (
          <TableRow key={d.id}>
            <TableCell>{d.id}</TableCell>
            <TableCell>{d.sensorId}</TableCell>
            <TableCell>{d.value.toFixed(2)}</TableCell>
            <TableCell>{new Date(d.timestamp).toLocaleString()}</TableCell>
          </TableRow>
        ))}
      </TableBody>
    </Table>
  );
};

export default DataTable;
