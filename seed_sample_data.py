"""
Audit all modules and inject 4 realistic sample records.
Strategy: seed localStorage, then patch loadData() to read from localStorage
when the API returns empty. Each module gets its own localStorage key.
"""
import os, re, json

wwwroot = "HaulingDemoApp/wwwroot"

# ─── SAMPLE DATA REGISTRY ────────────────────────────────────────────────────
# Each module: seedKey, variable name used in JS, sample records array

SAMPLES = {

    "master-site.html": {
        "seedKey": "lfn_sites",
        "varName": "allSites",
        "renderFn": ["updateStats", "renderTable"],
        "statsFn": "updateStats",
        "statCards": {"statTotal": 4, "statActive": 3, "statMining": 2, "statRegion": 3},
        "samples": [
            {"id":"s01","siteCode":"TNG","siteName":"Site Tanjung","region":"Kalimantan Selatan","siteType":"MINING","city":"Banjarmasin","province":"Kalimantan Selatan","address":"Jl. Poros Tanjung Km 12, Tabuk","timeZone":"Asia/Makassar","contactPerson":"Ahmad Hidayat","phone":"0812-3456-7890","remarks":"Site tambang batu bara utama","status":"ACTIVE"},
            {"id":"s02","siteCode":"KCM","siteName":"KTC Mining Camp","region":"Kalimantan Tengah","siteType":"MINING","city":"Palangka Raya","province":"Kalimantan Tengah","address":"KTC Camp Km 45, Kuala Kurun","timeZone":"Asia/Makassar","contactPerson":"Budi Santoso","phone":"0813-4567-8901","remarks":"Coal mining operation site","status":"ACTIVE"},
            {"id":"s03","siteCode":"PTB","siteName":"Port Boomangan","region":"Kalimantan Selatan","siteType":"PORT","city":"Pelaihari","province":"Kalimantan Selatan","address":"Jl. Pelaihari Boomangan No.5","timeZone":"Asia/Makassar","contactPerson":"Dewi Rahayu","phone":"0814-5678-9012","remarks":"Port loading batu bara","status":"ACTIVE"},
            {"id":"s04","siteCode":"GQS","siteName":"Graha Quarry Site","region":"Jawa Timur","siteType":"QUARRY","city":"Malang","province":"Jawa Timur","address":"Jl. Raya Quarry No.8, Tumpang","timeZone":"Asia/Jakarta","contactPerson":"Rudi Hermawan","phone":"0815-6789-0123","remarks":"Quarry batu kapur","status":"INACTIVE"},
        ]
    },

    "master-cost-center.html": {
        "seedKey": "lfn_costcenters",
        "varName": "allCC",
        "renderFn": ["renderStats", "renderTable"],
        "statCards": {"statTotal": 4, "statAlloc": 4, "statOps": 3, "statAdmin": 1},
        "samples": [
            {"id":"cc01","costCenterCode":"CC-OPS-001","costCenterName":"Operasi Hauling Site Tanjung","siteCode":"TNG","costCenterType":"OPERATIONAL","allocationAmount":15000000000,"currency":"IDR","budgetUsagePercent":68.5,"remarks":"Alokasi budget hauling bulanan","status":"ACTIVE"},
            {"id":"cc02","costCenterCode":"CC-MTN-001","costCenterName":"Maintenance Fleet Site Tanjung","siteCode":"TNG","costCenterType":"OPERATIONAL","allocationAmount":3500000000,"currency":"IDR","budgetUsagePercent":42.0,"remarks":"Perawatan rutin dan perbaikan unit","status":"ACTIVE"},
            {"id":"cc03","costCenterCode":"CC-ADM-001","costCenterName":"Administrasi Site KTC","siteCode":"KCM","costCenterType":"ADMIN","allocationAmount":1800000000,"currency":"IDR","budgetUsagePercent":31.2,"remarks":"Biaya admin dan umum site","status":"ACTIVE"},
            {"id":"cc04","costCenterCode":"CC-PRJ-001","costCenterName":"Proyek Ekspansi Port Boomangan","siteCode":"PTB","costCenterType":"PROJECT","allocationAmount":85000000000,"currency":"IDR","budgetUsagePercent":15.0,"remarks":"Pengembangan infrastruktur port","status":"ACTIVE"},
        ]
    },

    "master-user.html": {
        "seedKey": "lfn_users",
        "varName": "allUsers",
        "renderFn": ["renderStats", "renderTable"],
        "statCards": {"statTotal": 4, "statActive": 3, "statAdmin": 1, "statManager": 1, "statUser": 1, "statViewer": 1},
        "samples": [
            {"id":"u01","username":"yuditia.ariansyah","fullName":"Yuditia Ariansyah","email":"yuditia@lfmining.co.id","role":"ADMIN","department":"IT","site":"HO","phone":"0811-2233-4455","lastLogin":"2026-03-24T08:15:00Z","status":"ACTIVE"},
            {"id":"u02","username":"heru.wijaya","fullName":"Heru Wijaya","email":"heru.wijaya@lfmining.co.id","role":"MANAGER","department":"Operations","site":"TNG","phone":"0812-3344-5566","lastLogin":"2026-03-23T17:45:00Z","status":"ACTIVE"},
            {"id":"u03","username":"siti.nurhaliza","fullName":"Siti Nurhaliza","email":"siti.nurhaliza@lfmining.co.id","role":"USER","department":"Finance","site":"HO","phone":"0813-4455-6677","lastLogin":"2026-03-22T10:30:00Z","status":"ACTIVE"},
            {"id":"u04","username":"andi.pratama","fullName":"Andi Pratama","email":"andi.pratama@lfmining.co.id","role":"VIEWER","department":"Procurement","site":"KCM","phone":"0814-5566-7788","lastLogin":"2026-03-21T14:00:00Z","status":"INACTIVE"},
        ]
    },

    "master-department.html": {
        "seedKey": "lfn_departments",
        "varName": "data",
        "renderFn": ["renderStats", "renderTable"],
        "statCards": {"statTotal": 4, "statActive": 3, "statDivision": 1, "statDept": 3},
        "samples": [
            {"id":"d01","deptCode":"DEPT-OPS","deptName":"Operations","deptLevel":"DIVISION","parentDept":"","siteCode":"TNG","costCenterCode":"CC-OPS-001","remarks":"Divisi operasi tambang","status":"ACTIVE"},
            {"id":"d02","deptCode":"DEPT-MTN","deptName":"Maintenance","deptLevel":"DEPARTMENT","parentDept":"DEPT-OPS","siteCode":"TNG","costCenterCode":"CC-MTN-001","remarks":"Departemen pemeliharaan fleet","status":"ACTIVE"},
            {"id":"d03","deptCode":"DEPT-FIN","deptName":"Finance","deptLevel":"DEPARTMENT","parentDept":"","siteCode":"HO","costCenterCode":"CC-ADM-001","remarks":"Departemen keuangan dan akuntansi","status":"ACTIVE"},
            {"id":"d04","deptCode":"DEPT-HRD","deptName":"Human Resources","deptLevel":"DEPARTMENT","parentDept":"","siteCode":"HO","costCenterCode":"CC-ADM-001","remarks":"Departemen SDM dan umum","status":"ACTIVE"},
        ]
    },

    "master-driver.html": {
        "seedKey": "lfn_drivers",
        "varName": "data",
        "renderFn": ["renderStats", "renderTable"],
        "statCards": {"statTotal": 4, "statActive": 3, "statMale": 3, "statFemale": 1},
        "samples": [
            {"id":"dr01","driverCode":"DRV-001","nik":"3174051208900001","fullName":"Joko Susilo","birthDate":"1990-08-12","gender":"Male","phone":"0856-1234-5678","email":"joko.susilo@email.com","simType":"B2","simExpiry":"2027-05-15","siteCode":"TNG","deptCode":"DEPT-OPS","licensePlate":"DA 1234 OT","status":"ACTIVE"},
            {"id":"dr02","driverCode":"DRV-002","nik":"3174052307910002","fullName":"Asep Saepudin","birthDate":"1991-07-23","gender":"Male","phone":"0856-2345-6789","email":"asep.saepudin@email.com","simType":"B2","simExpiry":"2026-11-20","siteCode":"TNG","deptCode":"DEPT-OPS","licensePlate":"DA 2345 OT","status":"ACTIVE"},
            {"id":"dr03","driverCode":"DRV-003","nik":"6271050808890003","fullName":"Rizky Ramadhan","birthDate":"1989-08-08","gender":"Male","phone":"0857-3456-7890","email":"rizky.ramadhan@email.com","simType":"B1","simExpiry":"2028-03-10","siteCode":"KCM","deptCode":"DEPT-OPS","licensePlate":"KT 1111 PT","status":"ACTIVE"},
            {"id":"dr04","driverCode":"DRV-004","nik":"3271051505920004","fullName":"Siti Aminah","birthDate":"1992-05-15","gender":"Female","phone":"0858-4567-8901","email":"siti.aminah@email.com","simType":"B2","simExpiry":"2026-08-25","siteCode":"TNG","deptCode":"DEPT-OPS","licensePlate":"DA 3456 OT","status":"INACTIVE"},
        ]
    },

    "master-vehicle.html": {
        "seedKey": "lfn_vehicles",
        "varName": "data",
        "renderFn": ["renderStats", "renderTable"],
        "statCards": {"statTotal": 4, "statActive": 3, "statHauling": 2, "statFuel": 1},
        "samples": [
            {"id":"v01","vehicleCode":"VH-001","licensePlate":"DA 1234 OT","vehicleType":"HAULING","brand":"Hino","model":"Hino 500 FM 260 JD","engineNumber":"E001-HM500-2023","chassisNumber":"CH001-HM500-2023","manufactureYear":2023,"siteCode":"TNG","costCenterCode":"CC-OPS-001","capacityTon":20,"fuelType":"Diesel","status":"ACTIVE","remarks":"Unit primary hauling Tanjung"},
            {"id":"v02","vehicleCode":"VH-002","licensePlate":"DA 2345 OT","vehicleType":"HAULING","brand":"Hino","model":"Hino 500 FM 260 JD","engineNumber":"E002-HM500-2023","chassisNumber":"CH002-HM500-2023","manufactureYear":2023,"siteCode":"TNG","costCenterCode":"CC-OPS-001","capacityTon":20,"fuelType":"Diesel","status":"ACTIVE","remarks":"Unit secondary hauling Tanjung"},
            {"id":"v03","vehicleCode":"VS-001","licensePlate":"DA 8888 OT","vehicleType":"SUPPORT","brand":"Toyota","model":"Hilux DPL 2.4","engineNumber":"E003-HLX-2022","chassisNumber":"CH003-HLX-2022","manufactureYear":2022,"siteCode":"TNG","costCenterCode":"CC-OPS-001","capacityTon":1.5,"fuelType":"Diesel","status":"ACTIVE","remarks":"Unit support lapangan"},
            {"id":"v04","vehicleCode":"VF-001","licensePlate":"DA 9999 OT","vehicleType":"FUEL_TANKER","brand":"Mitsubishi","model":"Mitsubishi Fuso 120 PS","engineNumber":"E004-FSO-2021","chassisNumber":"CH004-FSO-2021","manufactureYear":2021,"siteCode":"TNG","costCenterCode":"CC-MTN-001","capacityTon":5,"fuelType":"Diesel","status":"MAINTENANCE","remarks":"Tangki BBM kapasitas 5000 liter"},
        ]
    },

    "master-route.html": {
        "seedKey": "lfn_routes",
        "varName": "data",
        "renderFn": ["renderStats", "renderTable"],
        "statCards": {},
        "samples": [
            {"id":"r01","routeCode":"RT-TNG-01","routeName":"Tanjung - Port Boomangan","siteCode":"TNG","originLocation":"Tanjung Mining Pit A","destinationLocation":"Port Boomangan","routeType":"HAUL","distanceKm":18.5,"travelTimeMin":35,"haulCostPerKm":45000,"fuelConsumptionPerKm":0.38,"roadCondition":"MEDIUM","remarks":"Rute utama hauling batu bara","status":"ACTIVE"},
            {"id":"r02","routeCode":"RT-TNG-02","routeName":"Tanjung - Dump Area B","siteCode":"TNG","originLocation":"Tanjung Mining Pit A","destinationLocation":"Dump Area Overburden B","routeType":"OVERBURDEN","distanceKm":6.2,"travelTimeMin":15,"haulCostPerKm":38000,"fuelConsumptionPerKm":0.42,"roadCondition":"POOR","remarks":"Rute pembuangan overburden","status":"ACTIVE"},
            {"id":"r03","routeCode":"RT-KCM-01","routeName":"KTC - Stockpile Utara","siteCode":"KCM","originLocation":"KTC Mining Pit North","destinationLocation":"Stockpile Utara","routeType":"HAUL","distanceKm":12.0,"travelTimeMin":25,"haulCostPerKm":52000,"fuelConsumptionPerKm":0.35,"roadCondition":"GOOD","remarks":"Rute hauling site KTC","status":"ACTIVE"},
            {"id":"r04","routeCode":"RT-TNG-03","routeName":"Return - Port ke Pit","siteCode":"TNG","originLocation":"Port Boomangan","destinationLocation":"Tanjung Mining Pit A","routeType":"RETURN","distanceKm":17.8,"travelTimeMin":30,"haulCostPerKm":28000,"fuelConsumptionPerKm":0.30,"roadCondition":"MEDIUM","remarks":"Rute kosong kembali ke pit","status":"ACTIVE"},
        ]
    },

    "master-material.html": {
        "seedKey": "lfn_materials",
        "varName": "data",
        "renderFn": ["renderStats", "renderTable"],
        "statCards": {},
        "samples": [
            {"id":"m01","materialCode":"MAT-FUEL-001","materialName":"Solar (Diesel)","materialGroup":"FUEL","materialType":"ITEM","uomCode":"LTR","brand":"Pertamina","spec":"DEX 50 ppm","unitPrice":15150,"currency":"IDR","siteCode":"TNG","remarks":"Bahan bakar utama kendaraan hauling","status":"ACTIVE"},
            {"id":"m02","materialCode":"MAT-TYRE-001","materialName":"Ban Dump Truck 1200R20","materialGroup":"TYRE","materialType":"ITEM","uomCode":"PCS","brand":"Bridgestone","spec":"1200R20 V-Steer","unitPrice":5800000,"currency":"IDR","siteCode":"TNG","remarks":"Ban untuk unit dump truck 20 ton","status":"ACTIVE"},
            {"id":"m03","materialCode":"MAT-PART-001","materialName":"Oli Mesin Heavy Duty 15W-40","materialGroup":"PART","materialType":"ITEM","uomCode":"LTR","brand":"Shell","spec":"Rimula R4 X 15W-40","unitPrice":85000,"currency":"IDR","siteCode":"TNG","remarks":"Oli mesin untuk Hino 500 dan Mitsubishi Fuso","status":"ACTIVE"},
            {"id":"m04","materialCode":"MAT-SVC-001","materialName":"Jasa Service Periodik 5000km","materialGroup":"SERVICE","materialType":"SERVICE","uomCode":"UNIT","brand":"-","spec":"Includes filter, oil, inspection","unitPrice":3500000,"currency":"IDR","siteCode":"TNG","remarks":"Service rutin setiap 5000 km","status":"ACTIVE"},
        ]
    },

    "master-uom.html": {
        "seedKey": "lfn_uoms",
        "varName": "data",
        "renderFn": ["renderStats", "renderTable"],
        "statCards": {},
        "samples": [
            {"id":"u01","uomCode":"LTR","uomName":"Liter","uomCategory":"VOLUME","remarks":"Satuan volume cairan","status":"ACTIVE"},
            {"id":"u02","uomCode":"KG","uomName":"Kilogram","uomCategory":"WEIGHT","remarks":"Satuan berat","status":"ACTIVE"},
            {"id":"u03","uomCode":"PCS","uomName":"Pieces","uomCategory":"COUNT","remarks":"Satuan jumlah barang","status":"ACTIVE"},
            {"id":"u04","uomCode":"TON","uomName":"Metric Ton","uomCategory":"WEIGHT","remarks":"Satuan ton metrik","status":"ACTIVE"},
        ]
    },

    "master-coa.html": {
        "seedKey": "lfn_coa",
        "varName": "data",
        "renderFn": ["renderStats", "renderTable"],
        "statCards": {},
        "samples": [
            {"id":"c01","coaCode":"4-100","coaName":"Pendapatan Hauling","coaLevel":"HEADER","parentCode":"4","coaType":"REVENUE","remarks":"Akun pendapatan utama operasi hauling","status":"ACTIVE"},
            {"id":"c02","coaCode":"4-110","coaName":"Pendapatan Angkutan Batu Bara","coaLevel":"DETAIL","parentCode":"4-100","coaType":"REVENUE","remarks":"Pendapatan dari jasa angkut batu bara","status":"ACTIVE"},
            {"id":"c03","coaCode":"5-100","coaName":"Beban Operasional Hauling","coaLevel":"HEADER","parentCode":"5","coaType":"EXPENSE","remarks":"Akun beban operasi utama","status":"ACTIVE"},
            {"id":"c04","coaCode":"5-110","coaName":"Beban BBM (Solar)","coaLevel":"DETAIL","parentCode":"5-100","coaType":"EXPENSE","remarks":"Bahan bakar dump truck dan kendaraan","status":"ACTIVE"},
        ]
    },

    "master-tax.html": {
        "seedKey": "lfn_tax",
        "varName": "data",
        "renderFn": ["renderStats", "renderTable"],
        "statCards": {},
        "samples": [
            {"id":"t01","taxCode":"PPN","taxName":"Pajak Pertambahan Nilai","taxRate":11,"taxType":"OUTPUT","remarks":"PPN 11% sesuai PMK terbaru","status":"ACTIVE"},
            {"id":"t02","taxCode":"PPh21","taxName":"PPh Pasal 21","taxRate":5,"taxType":"OUTPUT","remarks":"PPh 21 atas gaji karyawan","status":"ACTIVE"},
            {"id":"t03","taxCode":"PPh23","taxName":"PPh Pasal 23","taxRate":2,"taxType":"OUTPUT","remarks":"PPh 23 atas jasa sebesar 2%","status":"ACTIVE"},
            {"id":"t04","taxCode":"PPh26","taxName":"PPh Pasal 26","taxRate":20,"taxType":"OUTPUT","remarks":"PPh 26 atas penghasilan luar negeri","status":"INACTIVE"},
        ]
    },

    "master-workflow.html": {
        "seedKey": "lfn_workflow",
        "varName": "data",
        "renderFn": ["renderStats", "renderTable"],
        "statCards": {},
        "samples": [
            {"id":"w01","workflowCode":"WF-PR-001","workflowName":"Approval Purchase Request","entityType":"PURCHASE_REQUEST","minAmount":0,"maxAmount":50000000,"approvalLevels":2,"level1Role":"MANAGER","level2Role":"DIRECTOR","remarks":"PR sampai 50jt只需要 manager approve","status":"ACTIVE"},
            {"id":"w02","workflowCode":"WF-PR-002","workflowName":"Approval PR di atas 50jt","entityType":"PURCHASE_REQUEST","minAmount":50000001,"maxAmount":999999999999,"approvalLevels":3,"level1Role":"MANAGER","level2Role":"DIRECTOR","level3Role":"FINANCE_DIRECTOR","remarks":"PR di atas 50jt还需要 finance director","status":"ACTIVE"},
            {"id":"w03","workflowCode":"WF-RM-001","workflowName":"Approval Repair & Maintenance","entityType":"WORK_ORDER","minAmount":0,"maxAmount":999999999999,"approvalLevels":2,"level1Role":"SUPERVISOR","level2Role":"MANAGER","remarks":"Work order maintenance langsung ke manager","status":"ACTIVE"},
            {"id":"w04","workflowCode":"WF-HR-001","workflowName":"Approval Cuti Karyawan","entityType":"LEAVE","minAmount":0,"maxAmount":0,"approvalLevels":2,"level1Role":"SUPERVISOR","level2Role":"HR_MANAGER","remarks":"Approval cuti karyawan tetap dan kontrak","status":"ACTIVE"},
        ]
    },

    "vendor.html": {
        "seedKey": "lfn_vendors",
        "varName": "data",
        "renderFn": ["renderStats", "renderTable"],
        "statCards": {},
        "samples": [
            {"id":"v01","vendorCode":"VND-001","vendorName":"PT Pertamina (persero) Lubricants","category":"FUEL","contactPerson":"Dian Pratama","phone":"021-5789-1234","email":"sales.lube@pertamina.co.id","address":"Jl. Medan Merdeka Timur No.1A, Jakarta","npwp":"01.001.001.2-001.000","bankAccount":"130001234567","bankName":"Bank BRI","remarks":"Supplier oli dan pelumas resmi","status":"ACTIVE"},
            {"id":"v02","vendorCode":"VND-002","vendorName":"CV Borneo Tyre Mart","category":"TYRE","contactPerson":"Hendra Gunawan","phone":"0511-3344-5566","email":"borneo.tyre@gmail.com","address":"Jl. A. Yani Km 6, Banjarmasin","npwp":"02.002.002.3-002.000","bankAccount":"0011223344","bankName":"Bank Kalteng","remarks":"Supplier ban segala ukuran","status":"ACTIVE"},
            {"id":"v03","vendorCode":"VND-003","vendorName":"PT Hino Motors Sales Indonesia","category":"PART","contactPerson":"Ferry Tanoto","phone":"021-3456-7890","email":"hino.sales@hino.co.id","address":"Jl. Gaya Motor III No.5, Jakarta Utara","npwp":"03.003.003.4-003.000","bankAccount":"8812345678","bankName":"Bank BCA","remarks":"Sparepart resmi Hino","status":"ACTIVE"},
            {"id":"v04","vendorCode":"VND-004","vendorName":"PT Vale Indonesia TBK","category":"MINING","contactPerson":"Ahmad Dahlan","phone":"021-5678-9012","email":"procurement@vale.com","address":"Menara Vale, Jl. Jend. Sudirman Kav.52-53","npwp":"04.004.004.5-004.000","bankAccount":"1199008877","bankName":"Bank CIMB","remarks":"Rekanan mining operation","status":"INACTIVE"},
        ]
    },

    "purchase-request.html": {
        "seedKey": "lfn_pr",
        "varName": "data",
        "renderFn": ["renderStats", "renderTable"],
        "statCards": {},
        "samples": [
            {"id":"pr01","prNumber":"PR-2026-0001","siteCode":"TNG","department":"DEPT-OPS","requestedBy":"Heru Wijaya","requestDate":"2026-03-10","itemDescription":"Solar Dex 5000 Liter untuk unit hauling","quantity":5000,"uomCode":"LTR","estimatedPrice":75750000,"currency":"IDR","status":"APPROVED","remarks":"Stok BBM hampir habis, urgent"},
            {"id":"pr02","prNumber":"PR-2026-0002","siteCode":"TNG","department":"DEPT-MTN","requestedBy":"Joko Susilo","requestDate":"2026-03-15","itemDescription":"Ban Bridgestone 1200R20 V-Steer - 8 pcs","quantity":8,"uomCode":"PCS","estimatedPrice":46400000,"currency":"IDR","status":"PENDING","remarks":""},
            {"id":"pr03","prNumber":"PR-2026-0003","siteCode":"KCM","department":"DEPT-OPS","requestedBy":"Rizky Ramadhan","requestDate":"2026-03-18","itemDescription":"Jasa service periodik 5000km Hino 500","quantity":2,"uomCode":"UNIT","estimatedPrice":7000000,"currency":"IDR","status":"APPROVED","remarks":""},
            {"id":"pr04","prNumber":"PR-2026-0004","siteCode":"TNG","department":"DEPT-HRD","requestedBy":"Siti Nurhaliza","requestDate":"2026-03-20","itemDescription":"Alat tulis kantor (ATK) bulanan","quantity":1,"uomCode":"PACK","estimatedPrice":2500000,"currency":"IDR","status":"REJECTED","remarks":"Budget tidak tersedia"},
        ]
    },

    "purchase-order.html": {
        "seedKey": "lfn_po",
        "varName": "data",
        "renderFn": ["renderStats", "renderTable"],
        "statCards": {},
        "samples": [
            {"id":"po01","poNumber":"PO-2026-0001","vendorCode":"VND-001","vendorName":"PT Pertamina","siteCode":"TNG","orderDate":"2026-03-11","deliveryDate":"2026-03-14","itemDescription":"Solar Dex 5000 Liter","quantity":5000,"uomCode":"LTR","unitPrice":15150,"totalAmount":75750000,"currency":"IDR","status":"RECEIVED","remarks":""},
            {"id":"po02","poNumber":"PO-2026-0002","vendorCode":"VND-002","vendorName":"CV Borneo Tyre Mart","siteCode":"TNG","orderDate":"2026-03-16","deliveryDate":"2026-03-23","itemDescription":"Ban Bridgestone 1200R20","quantity":8,"uomCode":"PCS","unitPrice":5800000,"totalAmount":46400000,"currency":"IDR","status":"IN_TRANSIT","remarks":""},
            {"id":"po03","poNumber":"PO-2026-0003","vendorCode":"VND-003","vendorName":"PT Hino Motors Sales","siteCode":"TNG","orderDate":"2026-03-12","deliveryDate":"2026-03-25","itemDescription":"Filter oli Hino 500 (set 10pcs)","quantity":10,"uomCode":"SET","unitPrice":450000,"totalAmount":4500000,"currency":"IDR","status":"APPROVED","remarks":""},
            {"id":"po04","poNumber":"PO-2026-0004","vendorCode":"VND-001","vendorName":"PT Pertamina","siteCode":"KCM","orderDate":"2026-03-19","deliveryDate":"2026-03-22","itemDescription":"Oli Shell Rimula R4 X 15W-40","quantity":100,"uomCode":"LTR","unitPrice":85000,"totalAmount":8500000,"currency":"IDR","status":"PENDING","remarks":""},
        ]
    },

    "good-receipt.html": {
        "seedKey": "lfn_gr",
        "varName": "data",
        "renderFn": ["renderStats", "renderTable"],
        "statCards": {},
        "samples": [
            {"id":"gr01","grNumber":"GR-2026-0001","poNumber":"PO-2026-0001","vendorCode":"VND-001","vendorName":"PT Pertamina","siteCode":"TNG","receivedDate":"2026-03-14","receivedBy":"Joko Susilo","itemDescription":"Solar Dex","quantityReceived":4980,"uomCode":"LTR","condition":"GOOD","warehouseLocation":"Tangki BBM Site Tanjung","status":"COMPLETED"},
            {"id":"gr02","grNumber":"GR-2026-0002","poNumber":"PO-2026-0002","vendorCode":"VND-002","vendorName":"CV Borneo Tyre Mart","siteCode":"TNG","receivedDate":"2026-03-23","receivedBy":"Asep Saepudin","itemDescription":"Ban Bridgestone 1200R20","quantityReceived":8,"uomCode":"PCS","condition":"GOOD","warehouseLocation":"Gudang Ban Site Tanjung","status":"IN_TRANSIT"},
            {"id":"gr03","grNumber":"GR-2026-0003","poNumber":"PO-2026-0003","vendorCode":"VND-003","vendorName":"PT Hino Motors Sales","siteCode":"TNG","receivedDate":"2026-03-25","receivedBy":"Heru Wijaya","itemDescription":"Filter oli Hino 500","quantityReceived":10,"uomCode":"SET","condition":"GOOD","warehouseLocation":"Gudang Sparepart","status":"PENDING"},
            {"id":"gr04","grNumber":"GR-2026-0004","poNumber":"PO-2026-0004","vendorCode":"VND-001","vendorName":"PT Pertamina","siteCode":"KCM","receivedDate":"2026-03-22","receivedBy":"Rizky Ramadhan","itemDescription":"Oli Shell Rimula R4 X","quantityReceived":95,"uomCode":"LTR","condition":"PARTIAL","warehouseLocation":"Tangki BBM KTC Camp","status":"PARTIAL"},
        ]
    },

    "good-issue.html": {
        "seedKey": "lfn_gi",
        "varName": "data",
        "renderFn": ["renderStats", "renderTable"],
        "statCards": {},
        "samples": [
            {"id":"gi01","giNumber":"GI-2026-0001","siteCode":"TNG","issuedDate":"2026-03-15","issuedBy":"Heru Wijaya","department":"DEPT-OPS","itemDescription":"Solar Dex untuk unit VH-001","quantity":350,"uomCode":"LTR","purpose":"Operasi hauling harian","recipient":"VH-001 (DA 1234 OT)","status":"COMPLETED"},
            {"id":"gi02","giNumber":"GI-2026-0002","siteCode":"TNG","issuedDate":"2026-03-16","issuedBy":"Joko Susilo","department":"DEPT-MTN","itemDescription":"Oli mesin Shell Rimula R4 X","quantity":20,"uomCode":"LTR","purpose":"Service rutin VH-002","recipient":"VH-002 (DA 2345 OT)","status":"COMPLETED"},
            {"id":"gi03","giNumber":"GI-2026-0003","siteCode":"KCM","issuedDate":"2026-03-18","issuedBy":"Rizky Ramadhan","department":"DEPT-OPS","itemDescription":"Solar Dex untuk unit VH-003","quantity":280,"uomCode":"LTR","purpose":"Operasi hauling site KTC","recipient":"VH-003 (KT 1111 PT)","status":"PENDING"},
            {"id":"gi04","giNumber":"GI-2026-0004","siteCode":"TNG","issuedDate":"2026-03-20","issuedBy":"Asep Saepudin","department":"DEPT-MTN","itemDescription":"Filter oli + filter udara Hino","quantity":2,"uomCode":"SET","purpose":"PM berkala VH-001","recipient":"VH-001 (DA 1234 OT)","status":"CANCELLED"},
        ]
    },

    "fleet.html": {
        "seedKey": "lfn_fleet",
        "varName": "data",
        "renderFn": ["renderTable"],
        "statCards": {},
        "samples": [
            {"id":"f01","unitNo":"VH-001","licensePlate":"DA 1234 OT","vehicleType":"HAULING","siteCode":"TNG","driverName":"Joko Susilo","status":"ACTIVE","category":"HAULING","hmValue":8450,"fuelLevel":78.5,"odometer":124500,"remarks":"Unit primary hauling Tanjung"},
            {"id":"f02","unitNo":"VH-002","licensePlate":"DA 2345 OT","vehicleType":"HAULING","siteCode":"TNG","driverName":"Asep Saepudin","status":"ACTIVE","category":"HAULING","hmValue":6120,"fuelLevel":45.2,"odometer":98200,"remarks":"Unit secondary hauling Tanjung"},
            {"id":"f03","unitNo":"VS-001","licensePlate":"DA 8888 OT","vehicleType":"SUPPORT","siteCode":"TNG","driverName":"Belum Ditugaskan","status":"IDLE","category":"SUPPORT","hmValue":3200,"fuelLevel":92.0,"odometer":45000,"remarks":"Kendaraan support supervisor"},
            {"id":"f04","unitNo":"VF-001","licensePlate":"DA 9999 OT","vehicleType":"FUEL_TANKER","siteCode":"TNG","driverName":"Siti Aminah","status":"MAINTENANCE","category":"FUEL_TANKER","hmValue":5600,"fuelLevel":20.0,"odometer":76000,"remarks":"Tangki BBM - sedang service rem"},
        ]
    },

    "rm.html": {
        "seedKey": "lfn_rm",
        "varName": "data",
        "renderFn": ["renderTable"],
        "statCards": {},
        "samples": [
            {"id":"rm01","workOrderNo":"WO-2026-0001","siteCode":"TNG","unitNo":"VH-001","woType":"PREVENTIVE","category":"PM","description":"Service rutin 5000km - ganti oli, filter","assignedTo":"Tim MTN-01","woDate":"2026-03-10","dueDate":"2026-03-12","status":"COMPLETED"},
            {"id":"rm02","workOrderNo":"WO-2026-0002","siteCode":"TNG","unitNo":"VF-001","woType":"CORRECTIVE","category":"CORRECTIVE","description":"Perbaikan sistem rem - kampas rem aus","assignedTo":"Tim MTN-02","woDate":"2026-03-18","dueDate":"2026-03-20","status":"IN_PROGRESS"},
            {"id":"rm03","workOrderNo":"WO-2026-0003","siteCode":"KCM","unitNo":"VH-003","woType":"PREVENTIVE","category":"PM","description":"PM 10000km - inspeksi keseluruhan","assignedTo":"Tim MTN-KCM","woDate":"2026-03-15","dueDate":"2026-03-17","status":"COMPLETED"},
            {"id":"rm04","workOrderNo":"WO-2026-0004","siteCode":"TNG","unitNo":"VS-001","woType":"CORRECTIVE","category":"CORRECTIVE","description":"Perbaikan AC - refrigerant habis","assignedTo":"Tim MTN-01","woDate":"2026-03-22","dueDate":"2026-03-24","status":"PENDING"},
        ]
    },

    "inventory.html": {
        "seedKey": "lfn_inventory",
        "varName": "data",
        "renderFn": ["renderTable"],
        "statCards": {},
        "samples": [
            {"id":"i01","itemCode":"MAT-FUEL-001","itemName":"Solar (Diesel) Dex","category":"FUEL","siteCode":"TNG","location":"Tangki BBM Utama","uomCode":"LTR","quantity":8500,"minStock":3000,"maxStock":20000,"unitPrice":15150,"remarks":"Stok aman untuk 2 minggu","status":"ACTIVE"},
            {"id":"i02","itemCode":"MAT-TYRE-001","itemName":"Ban Bridgestone 1200R20","category":"TYRE","siteCode":"TNG","location":"Gudang Ban A","uomCode":"PCS","quantity":24,"minStock":10,"maxStock":50,"unitPrice":5800000,"remarks":"Terakhir restock 2026-03-16","status":"ACTIVE"},
            {"id":"i03","itemCode":"MAT-PART-001","itemName":"Oli Shell Rimula R4 X 15W-40","category":"PART","siteCode":"TNG","location":"Gudang Sparepart","uomCode":"LTR","quantity":45,"minStock":20,"maxStock":200,"unitPrice":85000,"remarks":"Stok menipis - perlu reorder","status":"LOW_STOCK"},
            {"id":"i04","itemCode":"MAT-SVC-001","itemName":"Filter Oli Hino 500","category":"PART","siteCode":"TNG","location":"Gudang Sparepart","uomCode":"PCS","quantity":8,"minStock":5,"maxStock":30,"unitPrice":225000,"remarks":"Set filter lengkap Hino","status":"ACTIVE"},
        ]
    },

    "haul-trip.html": {
        "seedKey": "lfn_haul",
        "varName": "data",
        "renderFn": ["renderTable"],
        "statCards": {},
        "samples": [
            {"id":"ht01","tripNumber":"HT-2026-0001","siteCode":"TNG","tripDate":"2026-03-24","routeCode":"RT-TNG-01","driverCode":"DRV-001","vehicleCode":"VH-001","materialType":"COAL","loadWeight":19.8,"uom":"TON","originLocation":"Tanjung Pit A","destLocation":"Port Boomangan","distanceKm":18.5,"fuelUsed":7.3,"hmStart":8450,"hmEnd":8453,"status":"COMPLETED"},
            {"id":"ht02","tripNumber":"HT-2026-0002","siteCode":"TNG","tripDate":"2026-03-24","routeCode":"RT-TNG-01","driverCode":"DRV-002","vehicleCode":"VH-002","materialType":"COAL","loadWeight":20.1,"uom":"TON","originLocation":"Tanjung Pit A","destLocation":"Port Boomangan","distanceKm":18.5,"fuelUsed":7.5,"hmStart":6120,"hmEnd":6123,"status":"COMPLETED"},
            {"id":"ht03","tripNumber":"HT-2026-0003","siteCode":"TNG","tripDate":"2026-03-24","routeCode":"RT-TNG-02","driverCode":"DRV-001","vehicleCode":"VH-001","materialType":"OVERBURDEN","loadWeight":22.0,"uom":"TON","originLocation":"Tanjung Pit A","destLocation":"Dump Area B","distanceKm":6.2,"fuelUsed":2.7,"hmStart":8453,"hmEnd":8455,"status":"COMPLETED"},
            {"id":"ht04","tripNumber":"HT-2026-0004","siteCode":"KCM","tripDate":"2026-03-24","routeCode":"RT-KCM-01","driverCode":"DRV-003","vehicleCode":"VH-003","materialType":"COAL","loadWeight":19.5,"uom":"TON","originLocation":"KTC Pit North","destLocation":"Stockpile Utara","distanceKm":12.0,"fuelUsed":4.4,"hmStart":3200,"hmEnd":3203,"status":"IN_PROGRESS"},
        ]
    },

    "weighbridge.html": {
        "seedKey": "lfn_wb",
        "varName": "data",
        "renderFn": ["renderTable"],
        "statCards": {},
        "samples": [
            {"id":"wb01","ticketNo":"WB-2026-0001","siteCode":"TNG","weighbridgeId":"WB-01","vehicleCode":"VH-001","licensePlate":"DA 1234 OT","weighingType":"LOADED","firstWeight":38.2,"secondWeight":18.4,"netWeight":19.8,"uom":"TON","materialType":"COAL","weighingDate":"2026-03-24T06:45:00Z","weighingOfficer":"Siti Aminah","remarks":"Berat bersih muatan batu bara","status":"CONFIRMED"},
            {"id":"wb02","ticketNo":"WB-2026-0002","siteCode":"TNG","weighbridgeId":"WB-01","vehicleCode":"VH-002","licensePlate":"DA 2345 OT","weighingType":"LOADED","firstWeight":38.6,"secondWeight":18.5,"netWeight":20.1,"uom":"TON","materialType":"COAL","weighingDate":"2026-03-24T07:10:00Z","weighingOfficer":"Andi Pratama","remarks":"Berat bersih muatan batu bara","status":"CONFIRMED"},
            {"id":"wb03","ticketNo":"WB-2026-0003","siteCode":"TNG","weighbridgeId":"WB-02","vehicleCode":"VF-001","licensePlate":"DA 9999 OT","weighingType":"EMPTY","firstWeight":18.4,"secondWeight":0,"netWeight":18.4,"uom":"TON","materialType":"EMPTY","weighingDate":"2026-03-24T07:30:00Z","weighingOfficer":"Siti Aminah","remarks":"Timbangan kosong setelah bongkar","status":"CONFIRMED"},
            {"id":"wb04","ticketNo":"WB-2026-0004","siteCode":"KCM","weighbridgeId":"WB-KCM-01","vehicleCode":"VH-003","licensePlate":"KT 1111 PT","weighingType":"LOADED","firstWeight":37.0,"secondWeight":17.5,"netWeight":19.5,"uom":"TON","materialType":"COAL","weighingDate":"2026-03-24T08:00:00Z","weighingOfficer":"Hendra Gunawan","remarks":"Weighing KTC site","status":"PENDING"},
        ]
    },

    "finance.html": {
        "seedKey": "lfn_finance",
        "varName": "data",
        "renderFn": ["renderTable"],
        "statCards": {},
        "samples": [
            {"id":"fn01","journalNo":"JN-2026-0001","journalDate":"2026-03-10","period":"2026-03","description":"Pendapatan hauling HT-2026-0001","accountCode":"4-110","accountName":"Pendapatan Angkutan Batu Bara","debit":0,"credit":157500000,"currency":"IDR","status":"POSTED"},
            {"id":"fn02","journalNo":"JN-2026-0002","journalDate":"2026-03-12","period":"2026-03","description":"Beban BBM Solar - VH-001, VH-002","accountCode":"5-110","accountName":"Beban BBM (Solar)","debit":10125000,"credit":0,"currency":"IDR","status":"POSTED"},
            {"id":"fn03","journalNo":"JN-2026-0003","journalDate":"2026-03-15","period":"2026-03","description":"Beban maintenance VH-001 - service 5000km","accountCode":"5-120","accountName":"Beban Perawatan Kendaraan","debit":3500000,"credit":0,"currency":"IDR","status":"POSTED"},
            {"id":"fn04","journalNo":"JN-2026-0004","journalDate":"2026-03-20","period":"2026-03","description":"Gaji Karyawan Operations Maret 2026","accountCode":"6-100","accountName":"Beban Gaji Karyawan","debit":185000000,"credit":0,"currency":"IDR","status":"PENDING"},
        ]
    },

    "hr.html": {
        "seedKey": "lfn_hr",
        "varName": "data",
        "renderFn": ["renderTable"],
        "statCards": {},
        "samples": [
            {"id":"hr01","employeeId":"EMP-001","nik":"3174051208900001","fullName":"Joko Susilo","position":"Driver Hauling","department":"DEPT-OPS","siteCode":"TNG","joinDate":"2021-03-15","employmentType":"PKWT","status":"ACTIVE","basicSalary":8500000,"allowance":2500000,"phone":"0856-1234-5678","remarks":"Driver utama site Tanjung"},
            {"id":"hr02","employeeId":"EMP-002","nik":"3174052307910002","fullName":"Asep Saepudin","position":"Driver Hauling","department":"DEPT-OPS","siteCode":"TNG","joinDate":"2022-01-10","employmentType":"PKWT","status":"ACTIVE","basicSalary":8500000,"allowance":2000000,"phone":"0856-2345-6789","remarks":"Driver secondary site Tanjung"},
            {"id":"hr03","employeeId":"EMP-003","nik":"6271050808890003","fullName":"Rizky Ramadhan","position":"Driver Hauling","department":"DEPT-OPS","siteCode":"KCM","joinDate":"2022-06-20","employmentType":"PKWT","status":"ACTIVE","basicSalary":9000000,"allowance":3000000,"phone":"0857-3456-7890","remarks":"Driver site KTC"},
            {"id":"hr04","employeeId":"EMP-004","nik":"3271051505920004","fullName":"Siti Aminah","position":"Weighbridge Officer","department":"DEPT-OPS","siteCode":"TNG","joinDate":"2023-02-01","employmentType":"PKWT","status":"ACTIVE","basicSalary":7500000,"allowance":1500000,"phone":"0858-4567-8901","remarks":"Weighbridge officer shift pagi"},
        ]
    },

    "kpi-dashboard.html": {
        "seedKey": "lfn_kpi",
        "varName": "data",
        "renderFn": ["renderTable"],
        "statCards": {},
        "samples": [
            {"id":"k01","period":"Maret 2026","siteCode":"TNG","totalTrips":1248,"totalMaterial":24950,"avgCycleTime":48.5,"avgPayload":19.9,"fuelEfficiency":0.38,"availability":92.5,"utilization":88.3,"oee":81.2,"status":"PUBLISHED"},
            {"id":"k02","period":"Februari 2026","siteCode":"TNG","totalTrips":1156,"totalMaterial":23120,"avgCycleTime":50.2,"avgPayload":20.0,"fuelEfficiency":0.39,"availability":90.1,"utilization":85.7,"oee":77.3,"status":"PUBLISHED"},
            {"id":"k03","period":"Maret 2026","siteCode":"KCM","totalTrips":892,"totalMaterial":17840,"avgCycleTime":52.0,"avgPayload":20.0,"fuelEfficiency":0.37,"availability":88.0,"utilization":82.5,"oee":72.6,"status":"PUBLISHED"},
            {"id":"k04","period":"Maret 2026","siteCode":"ALL","totalTrips":2140,"totalMaterial":42790,"avgCycleTime":49.8,"avgPayload":19.99,"fuelEfficiency":0.375,"availability":90.8,"utilization":86.1,"oee":78.2,"status":"PENDING"},
        ]
    },

    "datalist-fuel.html": {
        "seedKey": "lfn_fuel",
        "varName": "fuelData",
        "renderFn": ["renderTable"],
        "statCards": {},
        "samples": [
            {"id":"fl01","transactionDate":"2026-03-24","siteCode":"TNG","vehicleCode":"VH-001","licensePlate":"DA 1234 OT","driverName":"Joko Susilo","fuelType":"Solar Dex","quantity":350,"uom":"LTR","odometer":124500,"hmValue":8453,"fuelRate":0.38,"transactionType":"USAGE","remarks":"Trip Hauling 8x PP","status":"POSTED"},
            {"id":"fl02","transactionDate":"2026-03-24","siteCode":"TNG","vehicleCode":"VH-002","licensePlate":"DA 2345 OT","driverName":"Asep Saepudin","fuelType":"Solar Dex","quantity":320,"uom":"LTR","odometer":98200,"hmValue":6123,"fuelRate":0.38,"transactionType":"USAGE","remarks":"Trip Hauling 7x PP","status":"POSTED"},
            {"id":"fl03","transactionDate":"2026-03-24","siteCode":"KCM","vehicleCode":"VH-003","licensePlate":"KT 1111 PT","driverName":"Rizky Ramadhan","fuelType":"Solar Dex","quantity":295,"uom":"LTR","odometer":45600,"hmValue":3203,"fuelRate":0.37,"transactionType":"USAGE","remarks":"Trip Hauling site KTC","status":"POSTED"},
            {"id":"fl04","transactionDate":"2026-03-24","siteCode":"TNG","vehicleCode":"VF-001","licensePlate":"DA 9999 OT","driverName":"Siti Aminah","fuelType":"Solar Dex","quantity":180,"uom":"LTR","odometer":76000,"hmValue":5601,"fuelRate":0.45,"transactionType":"USAGE","remarks":"Fuel tanker delivery","status":"PENDING"},
        ]
    },

    "datalist- tyre.html": {
        "seedKey": "lfn_tyre",
        "varName": "data",
        "renderFn": ["renderTable"],
        "statCards": {},
        "samples": [
            {"id":"tr01","tyreSerialNo":"TYR-001-BR","vehicleCode":"VH-001","licensePlate":"DA 1234 OT","tyreSize":"1200R20","brand":"Bridgestone","pattern":"V-Steer","position":"FL","mountDate":"2025-09-15","totalHours":3250,"remainingTreadMm":18.5,"remarks":"Ban depan kiri","status":"IN_USE"},
            {"id":"tr02","tyreSerialNo":"TYR-002-BR","vehicleCode":"VH-002","licensePlate":"DA 2345 OT","tyreSize":"1200R20","brand":"Bridgestone","pattern":"V-Steer","position":"FR","mountDate":"2025-08-20","totalHours":3100,"remainingTreadMm":15.2,"remarks":"Tread menipis, monitor ketat","status":"IN_USE"},
            {"id":"tr03","tyreSerialNo":"TYR-003-GD","vehicleCode":"VH-001","licensePlate":"DA 1234 OT","tyreSize":"1200R20","brand":"Goodyear","pattern":"RLS","position":"RL","mountDate":"2025-07-10","totalHours":3450,"remainingTreadMm":8.0,"remarks":"Segera diganti - tread habis","status":"CRITICAL"},
            {"id":"tr04","tyreSerialNo":"TYR-004-CT","vehicleCode":"VH-003","licensePlate":"KT 1111 PT","tyreSize":"1200R20","brand":"Continental","pattern":"HCS","position":"FL","mountDate":"2025-11-05","totalHours":2100,"remainingTreadMm":22.0,"remarks":"Ban baru di VH-003","status":"IN_USE"},
        ]
    },

    "data-Upload-Fuel.html": {
        "seedKey": "lfn_fuel_upload",
        "varName": "data",
        "renderFn": [],
        "statCards": {},
        "samples": [
            {"fileName":"Fuel_Transaction_TNG_2026-03-01.csv","uploadDate":"2026-03-01","uploadedBy":"Siti Aminah","totalRecords":245,"status":"SUCCESS","remarks":"Upload harian fuel site Tanjung"},
            {"fileName":"Fuel_Transaction_TNG_2026-03-15.csv","uploadDate":"2026-03-15","uploadedBy":"Joko Susilo","totalRecords":312,"status":"SUCCESS","remarks":"Upload harian fuel site Tanjung"},
            {"fileName":"Fuel_Transaction_KCM_2026-03-10.csv","uploadDate":"2026-03-10","uploadedBy":"Rizky Ramadhan","totalRecords":189,"status":"SUCCESS","remarks":"Upload harian fuel site KTC"},
            {"fileName":"Fuel_Transaction_TNG_2026-03-20.csv","uploadDate":"2026-03-20","uploadedBy":"Asep Saepudin","totalRecords":298,"status":"PARTIAL","remarks":"3 record duplikat di-skip"},
        ]
    },

    "data-Upload-Tyre.html": {
        "seedKey": "lfn_tyre_upload",
        "varName": "data",
        "renderFn": [],
        "statCards": {},
        "samples": [
            {"fileName":"Tyre_PO_TNG_2026-03-01.csv","uploadDate":"2026-03-01","uploadedBy":"Heru Wijaya","totalRecords":48,"status":"SUCCESS","remarks":"Upload PO ban Bridgestone"},
            {"fileName":"Tyre_Problem_TNG_2026-03-15.csv","uploadDate":"2026-03-15","uploadedBy":"Joko Susilo","totalRecords":12,"status":"SUCCESS","remarks":"Report ban bermasalah"},
            {"fileName":"Tyre_PO_KCM_2026-03-10.csv","uploadDate":"2026-03-10","uploadedBy":"Rizky Ramadhan","totalRecords":36,"status":"SUCCESS","remarks":"Upload PO ban site KTC"},
            {"fileName":"Tyre_Problem_TNG_2026-03-20.csv","uploadDate":"2026-03-20","uploadedBy":"Asep Saepudin","totalRecords":8,"status":"PARTIAL","remarks":"2 record invalid format - perlu koreksi"},
        ]
    },

}


def make_seed_block(samples, seed_key):
    """Build the seed IIFE as a string."""
    payload = json.dumps(samples, ensure_ascii=False)
    return f"""
        // ── DEMO SEED: auto-populate {len(samples)} sample records ──
        (function() {{
            var key = "{seed_key}";
            try {{
                if (!localStorage.getItem(key)) {{
                    localStorage.setItem(key, JSON.stringify({payload}));
                }}
            }} catch(e) {{}}
        }})();
    """


def find_load_block(content, var_name):
    """Find the async function block that assigns to the var_name."""
    # Pattern: async function loadData() { ... varName = await res.json() ... }
    patterns = [
        # Pattern A: async function loadData() { ... var = await res.json() ...
        re.compile(r'(async\s+function\s+(\w+)\s*\([^)]*\)\s*\{[\s\S]*?)(await\s+res\.json\(\))'),
        # Pattern B: loadData = async () => { ... var = await res.json() ...
        re.compile(r'(async\s+function\s+(\w+)\s*\([^)]*\)\s*\{[\s\S]*?)(const\s+\w+\s*=\s*await\s+res\.json\(\))'),
    ]
    for p in patterns:
        m = p.search(content)
        if m:
            fn_name = m.group(2)
            return fn_name, m.start(), m.end()
    return None, -1, -1


def find_render_stats_block(content, var_name):
    """Find renderStats function to understand stat card ids."""
    m = re.search(r'function\s+renderStats\s*\([^)]*\)\s*\{([^}]+(?:\{[^}]*\}[^}]*)*)\}', content, re.DOTALL)
    return m


def patch_file(filepath, info):
    """Patch one HTML file: inject seed + modify loadData to use localStorage."""
    with open(filepath, "r", encoding="utf-8") as f:
        content = f.read()

    seed_key = info["seedKey"]
    var_name = info["varName"]
    samples = info["samples"]

    if seed_key in content:
        return False, "already seeded"

    if not samples:
        return False, "no samples"

    seed_block = make_seed_block(samples, seed_key)

    # ── Step 1: Inject seed IIFE before </body> ──────────────────────────
    body_end = content.rfind("</body>")
    if body_end == -1:
        return False, "no </body>"
    content = content[:body_end] + seed_block + "\n    " + content[body_end:]

    # ── Step 2: Patch loadData to read from localStorage when API empty ───
    # Find the fetch+json block
    fetch_patterns = [
        re.compile(r'(const\s+res\s*=\s*await\s+fetch\([^;]+;[\s\S]{0,200}?)(\w+\s*=\s*await\s+res\.json\(\);?)'),
        re.compile(r'(const\s+response\s*=\s*await\s+fetch\([^;]+;[\s\S]{0,200}?)(\w+\s*=\s*await\s+response\.json\(\);?)'),
    ]
    patched = False
    for fp in fetch_patterns:
        m = fp.search(content)
        if m:
            # Determine var name from the match
            full_match = m.group(0)
            assign_match = re.search(r'(\w+)\s*=\s*await\s+(?:res|response)\.json\(\)', full_match)
            if assign_match:
                data_var = assign_match.group(1)
            else:
                data_var = var_name

            inject = f'''
            // Demo: use localStorage seed if API returns empty
            var _seedKey_ = "{seed_key}";
            var _apiData_ = {data_var} || [];
            if (_apiData_.length === 0) {{
                try {{ var _seed_ = JSON.parse(localStorage.getItem(_seedKey_)); if (_seed_) {data_var} = _seed_; }} catch(e) {{}}
            }}
            '''
            # Replace the assignment with assignment + inject
            old_assign = m.group(2)
            new_assign = old_assign + inject
            content = content[:m.start()] + m.group(1) + new_assign + content[m.end():]
            patched = True
            break

    with open(filepath, "w", encoding="utf-8") as f:
        f.write(content)

    return True, f"SEEDED {len(samples)} records" + (" (loadData patched)" if patched else " (fetch block not found)")


def main():
    results = []
    total = 0
    total_recs = 0

    for filename, info in SAMPLES.items():
        filepath = os.path.join(wwwroot, filename)
        if not os.path.exists(filepath):
            results.append(f"SKIP (not found): {filename}")
            continue

        ok, msg = patch_file(filepath, info)
        n = len(info["samples"])
        total_recs += n
        if ok:
            total += 1
            results.append(f"OK   [{n} recs] {filename}  -- {msg}")
        else:
            results.append(f"SKIP {filename}  ({msg})")

    print("\n".join(results))
    print(f"\nTotal: {total} modules patched, {total_recs} sample records injected")


if __name__ == "__main__":
    main()
