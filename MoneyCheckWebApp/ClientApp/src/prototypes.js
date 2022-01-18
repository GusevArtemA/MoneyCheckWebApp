Date.prototype.getShortTime = function () {
  const hours = this.getHours();
  const minutes = this.getMinutes();
  
  function convertInt(a) {
   return `${a < 9 ? '0' : ''}${a}`; 
  }
  
  return `${convertInt(hours)}:${convertInt(minutes)}`;
}