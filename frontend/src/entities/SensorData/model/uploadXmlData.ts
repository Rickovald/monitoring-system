import axios from 'axios';

/**
 * Отправляет XML-файл на API для валидации.
 * @param file XML-файл.
 */
export const uploadXml = (file: File) => {
    const formData = new FormData();
    formData.append('file', file);
    return axios.post(`http://localhost:5177/api/upload-xml`, formData, {
        headers: { 'Content-Type': 'multipart/form-data' }
    });
};