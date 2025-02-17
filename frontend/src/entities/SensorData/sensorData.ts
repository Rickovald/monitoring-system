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
