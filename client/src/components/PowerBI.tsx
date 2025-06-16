import React from 'react';

const PowerBIReport: React.FC = () => {
  return (
    <div style={{ width: '100%', height: '100%', padding: '1rem' }}>
      <iframe
        title="WinningNumbers"
        src="https://app.powerbi.com/view?r=eyJrIjoiN2I2NTJkYTAtZDNiNC00NzIyLTk3YjQtZTI0NzBiMmU5NWY4IiwidCI6ImQwY2U2YjEwLTcxMjctNGEyYi1iMWZmLTdlMjQ1MzI1MTFmYSJ9"
        frameBorder="0"
        allowFullScreen
        style={{
          width: '100%',
          height: '80vh',
          border: 'none'
        }}
      ></iframe>
    </div>
  );
};

export default PowerBIReport;
