// Demo seed: populate localStorage with sample data for rm.html
(function(){
  var k="lfn_rm";
  var d=[
      {"id":"rm01","woNumber":"WO-2024-001","woType":"PREVENTIVE","vehicleId":"v01","siteId":"s01","scheduledDate":"2024-03-25","description":"Service rutin 5000 km - ganti oli, filter solar, cek rem","assignedMechanicId":"u01","status":"SCHEDULED","priority":"MEDIUM","estimatedHours":4},
      {"id":"rm02","woNumber":"WO-2024-002","woType":"CORRECTIVE","vehicleId":"v04","siteId":"s03","scheduledDate":"2024-03-20","description":"Perbaikan gearbox - bunyi tidak normal saat mundur","assignedMechanicId":"u01","status":"IN_PROGRESS","priority":"HIGH","estimatedHours":16},
      {"id":"rm03","woNumber":"WO-2024-003","woType":"PREVENTIVE","vehicleId":"v03","siteId":"s02","scheduledDate":"2024-03-28","description":"Inspeksi hydraulic system dan ganti filter","assignedMechanicId":"u02","status":"SCHEDULED","priority":"LOW","estimatedHours":3},
      {"id":"rm04","woNumber":"WO-2024-004","woType":"PREVENTIVE","vehicleId":"v02","siteId":"s01","scheduledDate":"2024-03-30","description":"Service rutin 5000 km - oli mesin, filter udaranya","assignedMechanicId":"u01","status":"SCHEDULED","priority":"MEDIUM","estimatedHours":4}
    ];
  try{
    if(!localStorage.getItem(k)){
      localStorage.setItem(k,JSON.stringify(d));
    }
  }catch(e){}
})();
