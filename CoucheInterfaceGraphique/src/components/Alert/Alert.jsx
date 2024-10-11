import React from 'react';

const Alert = ({ variant = 'info', children }) => {
  const baseStyles = 'p-4 mb-4 rounded-lg';
  
  const variantStyles = {
    info: 'bg-blue-100 text-blue-800 border border-blue-300',
    success: 'bg-green-100 text-green-800 border border-green-300',
    warning: 'bg-yellow-100 text-yellow-800 border border-yellow-300',
    error: 'bg-red-100 text-red-800 border border-red-300',
  };

  return (
    <div className={`${baseStyles} ${variantStyles[variant]}`} role="alert">
      {children}
    </div>
  );
};

export default Alert;