import React from 'react';

const PowerBIReport: React.FC = () => {
  return (
    <div style={{
      position: 'fixed',
      top: 0,
      left: 0,
      width: '100vw',
      height: '100vh',
      margin: 0,
      padding: 0,
      zIndex: 9999,
      backgroundColor: 'white'
    }}>
      <iframe
        title="WinningNumbers"
        src="https://app.powerbi.com/view?r=eyJrIjoiODY2ZDViNzQtODNlNi00YjQ1LTlhMmYtZmM0NGVlM2E4ZjFmIiwidCI6ImQwY2U2YjEwLTcxMjctNGEyYi1iMWZmLTdlMjQ1MzI1MTFmYSJ9"
        frameBorder="0"
        allowFullScreen
        style={{
          width: '100%',
          height: '100%',
          border: 'none'
        }}
      ></iframe>
    </div>
  );
};

 export default PowerBIReport;
