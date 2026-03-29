// Demo seed: populate tyre problems data in localStorage
(function(){
  var k="lfn_tyre_problems";
  var d=[
    {"id":7,"no":5,"tanggal":"2026-03-25T00:00:00","site":"Sungai Dua","post":"POST 01","unitNo":"HINO-001","serialNumber":"SN-MICH-2026-001","merkType":"Michelin","size":"11R22.5","problem":"Tear/Cut - ban sobek akibat benda tajam","kerusakan":"Replace tyre","startHM":12450,"cost":1500000},
    {"id":8,"no":6,"tanggal":"2026-03-26T00:00:00","site":"Sebamban","post":"POST 02","unitNo":"HINO-002","serialNumber":"SN-BRID-2026-002","merkType":"Bridgestone","size":"12R22.5","problem":"Penetration - ban bocor karena paku","kerusakan":"Repair + inflate","startHM":9850,"cost":250000},
    {"id":9,"no":7,"tanggal":"2026-03-27T00:00:00","site":"Mandala","post":"POST 01","unitNo":"ISUZU-003","serialNumber":"SN-GOOD-2026-003","merkType":"Goodyear","size":"315/80R22.5","problem":"Uneven wear - tapak aus tidak rata","kerusakan":"Rotation","startHM":15200,"cost":0}
  ];
  try{
    // Force refresh localStorage with latest data every load
    localStorage.setItem(k,JSON.stringify(d));
  }catch(e){}
})();
