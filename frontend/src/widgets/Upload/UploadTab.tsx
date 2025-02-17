import React, { useState } from 'react';
import { Button, Typography } from '@mui/material';
import { uploadXml } from '../../entities/SensorData/model/uploadXmlData';

const UploadPage: React.FC = () => {
  const [file, setFile] = useState<File | null>(null);
  const [message, setMessage] = useState<string>("");

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files.length > 0) {
      setFile(e.target.files[0]);
    }
  };


  const handleUpload = () => {
    if (!file) return;
    uploadXml(file)
      .then(() => setMessage("Файл успешно загружен и валидирован"))
      .catch(err => setMessage("Ошибка: " + err.response?.data));
  };

  return (
    <div>
      <Typography variant="h4" style={{ color: 'black' }}>Загрузка XML</Typography>
      <input type="file" accept=".xml" onChange={handleFileChange} />
      <Button variant="contained" onClick={handleUpload} style={{ marginLeft: '10px' }}>
        Загрузить
      </Button>
      {message && <Typography variant="body1" style={{ marginTop: '20px', color: 'green' }}>{message}</Typography>}
    </div>
  );
};

export default UploadPage;
