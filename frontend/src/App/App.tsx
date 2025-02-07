import React from 'react';
import { BrowserRouter, Routes, Route, Link } from 'react-router-dom';
import { Container, AppBar, Toolbar, Button } from '@mui/material';
import DataPage from '../features/Data/DataPage';
import UploadPage from '../features/Upload/UploadPage';

const App: React.FC = () => {
  return (
    <BrowserRouter>
      <AppBar position="static">
        <Toolbar>
          <Button color="inherit" component={Link} to="/data">Данные</Button>
          <Button color="inherit" component={Link} to="/upload">Загрузка XML</Button>
        </Toolbar>
      </AppBar>
      <Container style={{ marginTop: '20px' }}>
        <Routes>
          <Route path="/data" element={<DataPage />} />
          <Route path="/upload" element={<UploadPage />} />
          <Route path="*" element={<DataPage />} />
        </Routes>
      </Container>
    </BrowserRouter>
  );
};

export default App;
