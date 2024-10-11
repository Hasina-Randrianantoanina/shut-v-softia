export const getColor = (index) => {
    const colors = [
      "#5470C6", "#91CC75", "#FAC858", "#EE6666", "#73C0DE",
      "#3BA272", "#FC8452", "#9A60B4", "#EA7CCC"
    ];
    return colors[index % colors.length];
  };
  