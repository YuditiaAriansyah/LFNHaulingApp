// Demo seed: populate localStorage with sample fuel data
(function(){
  var k="lfn_fuel";
  var d=[
      {"id": "fuel01", "transactionDate": "2024-03-20", "vehicleId": "v01", "driverId": "drv01", "siteId": "s01", "fuelType": "DIESEL", "quantity": 45, "uomId": "uom04", "unitPrice": 18500, "totalPrice": 832500, "odometerReading": 45678, "pumpStationId": "PS-001", "operatorId": "u01", "status": "RECORDED"},
      {"id": "fuel02", "transactionDate": "2024-03-20", "vehicleId": "v02", "driverId": "drv02", "siteId": "s01", "fuelType": "DIESEL", "quantity": 42, "uomId": "uom04", "unitPrice": 18500, "totalPrice": 777000, "odometerReading": 32100, "pumpStationId": "PS-001", "operatorId": "u01", "status": "RECORDED"},
      {"id": "fuel03", "transactionDate": "2024-03-20", "vehicleId": "v03", "driverId": "drv03", "siteId": "s02", "fuelType": "DIESEL", "quantity": 38, "uomId": "uom04", "unitPrice": 18500, "totalPrice": 703000, "odometerReading": 12340, "pumpStationId": "PS-002", "operatorId": "u02", "status": "RECORDED"},
      {"id": "fuel04", "transactionDate": "2024-03-21", "vehicleId": "v01", "driverId": "drv01", "siteId": "s01", "fuelType": "DIESEL", "quantity": 46, "uomId": "uom04", "unitPrice": 18500, "totalPrice": 851000, "odometerReading": 45723, "pumpStationId": "PS-001", "operatorId": "u01", "status": "RECORDED"}
    ];
  try{
    if(!localStorage.getItem(k)){
      localStorage.setItem(k,JSON.stringify(d));
    }
  }catch(e){}
})();
