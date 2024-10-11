import React, { useState, useEffect } from 'react';

const LoadingSpinner = () => {
  const [elapsedTime, setElapsedTime] = useState(0);

  useEffect(() => {
    const timer = setInterval(() => {
      setElapsedTime((prevTime) => prevTime + 1);
    }, 1000);

    return () => {
      clearInterval(timer);
    };
  }, []);

  return (
    <div className="flex flex-col items-center justify-center h-64 mt-4">
      <div className="w-32 h-32 border-t-2 border-b-2 rounded-full animate-spin border-atoli_blue"></div>
      <div className="mt-4 text-lg font-semibold text-atoli_blue">Chargement</div>
      <div className="mt-2 text-sm text-gray-600">
        Temps écoulé: {elapsedTime} secondes
      </div>
    </div>
  );
};

export default LoadingSpinner;