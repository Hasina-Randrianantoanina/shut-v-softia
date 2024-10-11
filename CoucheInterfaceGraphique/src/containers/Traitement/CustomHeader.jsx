import React, { useState } from 'react';

export const CustomHeader = (props) => {
  const [isTooltipVisible, setIsTooltipVisible] = useState(false);

  return (
    <div 
      className="custom-header-cell"
      onMouseEnter={() => setIsTooltipVisible(true)}
      onMouseLeave={() => setIsTooltipVisible(false)}
    >
      <div className="header-cell-text">
        {props.displayName}
      </div>
      {isTooltipVisible && (
        <div className="header-cell-tooltip">
          {props.tooltip}
        </div>
      )}
    </div>
  );
};