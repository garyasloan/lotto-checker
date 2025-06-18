import React from 'react';

const PowerBIReport: React.FC = () => {
  return (
    <div style={{ width: '100%', height: '100%', padding: '1rem' }}>
      <iframe
        title="WinningNumbers"
        src="https://public.tableau.com/views/NumberOccurrences/Sheet1?:embed=true&:showVizHome=no"
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
