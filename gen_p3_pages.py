import os

BASE = r"c:\LFN\LFNHaulingApp\HaulingDemoApp\wwwroot"
API_BASE = "https://lfn-hauling-903235665988.asia-southeast1.run.app"

PAGES = {
    "master-vehicle.html": {
        "accent": "#2563eb",
        "icon": "bi-truck",
        "title": "Vehicle Master",
        "subtitle": "Kelola data kendaraan (unit hauling, fuel tanker, support)",
        "api": "/api/vehicles/master",
        "fields": [
            {"name": "VehicleCode", "label": "Kode Vehicle", "col": 4, "type": "text", "required": True},
            {"name": "PoliceNumber", "label": "No. Polisi", "col": 4, "type": "text"},
            {"name": "VehicleType", "label": "Tipe Kendaraan", "col": 4, "type": "select", "options": ["HAULING", "SUPPORT", "FUEL_TANKER", "CRUSHER"], "required": True},
            {"name": "Brand", "label": "Merk", "col": 4, "type": "text"},
            {"name": "Model", "label": "Model", "col": 4, "type": "text"},
            {"name": "FuelType", "label": "Jenis BBM", "col": 4, "type": "select", "options": ["DIESEL", "SOLAR"]},
            {"name": "SiteCode", "label": "Site Code", "col": 4, "type": "text"},
            {"name": "CostCenter", "label": "Cost Center", "col": 4, "type": "text"},
            {"name": "CapacityVolume", "label": "Kapasitas Vol (m3)", "col": 3, "type": "number"},
            {"name": "CapacityWeight", "label": "Kapasitas Weight (ton)", "col": 3, "type": "number"},
            {"name": "YearMade", "label": "Tahun Buat", "col": 3, "type": "number"},
            {"name": "ChassisNumber", "label": "No. Chassis", "col": 6, "type": "text"},
            {"name": "MachineNumber", "label": "No. Machine", "col": 6, "type": "text"},
            {"name": "Remarks", "label": "Remarks", "col": 12, "type": "textarea"},
        ],
        "columns": [
            {"header": "Kode", "data": "vehicleCode"},
            {"header": "No. Polisi", "data": "policeNumber"},
            {"header": "Tipe", "data": "vehicleType"},
            {"header": "Merk/Model", "data": "brand"},
            {"header": "Site", "data": "siteCode"},
            {"header": "CC", "data": "costCenter"},
            {"header": "Cap (ton)", "data": "capacityWeight"},
            {"header": "Tahun", "data": "yearMade"},
            {"header": "Status", "data": "status", "badge": True},
        ],
        "stats": [
            {"id": "statTotal", "label": "Total Unit"},
            {"id": "statActive", "label": "Aktif"},
            {"id": "statHauling", "label": "Hauling"},
            {"id": "statFuel", "label": "Fuel Tanker"},
        ],
        "filter_extra": '''
                    <div class="col-md-3">
                        <label class="form-label">Tipe Kendaraan</label>
                        <select class="form-select" id="filterType" onchange="loadData()">
                            <option value="">Semua</option>
                            <option value="HAULING">HAULING</option>
                            <option value="SUPPORT">SUPPORT</option>
                            <option value="FUEL_TANKER">FUEL_TANKER</option>
                        </select>
                    </div>''',
        "extra_stat": "document.getElementById('statHauling').textContent = data.filter(d => d.vehicleType === 'HAULING').length;\n    document.getElementById('statFuel').textContent = data.filter(d => d.vehicleType === 'FUEL_TANKER').length;",
    },
    "master-driver.html": {
        "accent": "#7c3aed",
        "icon": "bi-person-badge",
        "title": "Driver Master",
        "subtitle": "Kelola data supir dan operator fleet hauling",
        "api": "/api/drivers/master",
        "fields": [
            {"name": "DriverCode", "label": "Kode Driver", "col": 4, "type": "text", "required": True},
            {"name": "FullName", "label": "Nama Lengkap", "col": 8, "type": "text", "required": True},
            {"name": "NIK", "label": "NIK / KTP", "col": 4, "type": "text"},
            {"name": "SIM", "label": "No. SIM", "col": 4, "type": "text"},
            {"name": "SIMType", "label": "Tipe SIM", "col": 4, "type": "select", "options": ["B1", "B2", "C"]},
            {"name": "Gender", "label": "Jenis Kelamin", "col": 4, "type": "select", "options": ["LAKI-LAKI", "PEREMPUAN"]},
            {"name": "DateOfBirth", "label": "Tanggal Lahir", "col": 4, "type": "date"},
            {"name": "Phone", "label": "No. HP", "col": 4, "type": "text"},
            {"name": "SiteCode", "label": "Site Code", "col": 4, "type": "text"},
            {"name": "DepartmentCode", "label": "Kode Dept", "col": 6, "type": "text"},
            {"name": "JoinDate", "label": "Tanggal Masuk", "col": 6, "type": "date"},
            {"name": "Remarks", "label": "Remarks", "col": 12, "type": "textarea"},
        ],
        "columns": [
            {"header": "Kode", "data": "driverCode"},
            {"header": "Nama Lengkap", "data": "fullName"},
            {"header": "NIK", "data": "nik"},
            {"header": "SIM", "data": "sim"},
            {"header": "Tipe SIM", "data": "simType"},
            {"header": "Site", "data": "siteCode"},
            {"header": "Dept", "data": "departmentCode"},
            {"header": "HP", "data": "phone"},
            {"header": "Status", "data": "status", "badge": True},
        ],
        "stats": [
            {"id": "statTotal", "label": "Total Driver"},
            {"id": "statActive", "label": "Aktif"},
            {"id": "statMale", "label": "Laki-Laki"},
            {"id": "statFemale", "label": "Perempuan"},
        ],
        "filter_extra": "",
        "extra_stat": "document.getElementById('statMale').textContent = data.filter(d => d.gender === 'LAKI-LAKI').length;\n    document.getElementById('statFemale').textContent = data.filter(d => d.gender === 'PEREMPUAN').length;",
    },
    "master-route.html": {
        "accent": "#0f766e",
        "icon": "bi-pin-map",
        "title": "Route Master",
        "subtitle": "Kelola rute hauling (Pit ke Stockpile, Overburden, dll)",
        "api": "/api/routes/master",
        "fields": [
            {"name": "RouteCode", "label": "Kode Rute", "col": 4, "type": "text", "required": True},
            {"name": "RouteName", "label": "Nama Rute", "col": 8, "type": "text", "required": True},
            {"name": "SiteCode", "label": "Site Code", "col": 4, "type": "text"},
            {"name": "OriginLocation", "label": "Lokasi Asal (Muat)", "col": 4, "type": "text"},
            {"name": "DestinationLocation", "label": "Lokasi Tujuan (Buang)", "col": 4, "type": "text"},
            {"name": "RouteType", "label": "Tipe Rute", "col": 4, "type": "select", "options": ["HAUL", "RETURN", "OVERBURDEN", "RECLAMATION"]},
            {"name": "DistanceKm", "label": "Jarak (km)", "col": 4, "type": "number"},
            {"name": "TravelTimeMin", "label": "Waktu Tempuh (menit)", "col": 4, "type": "number"},
            {"name": "HaulCostPerKm", "label": "Biaya/km (IDR)", "col": 6, "type": "number"},
            {"name": "FuelConsumptionPerKm", "label": "Konsumsi BBM/km (L)", "col": 6, "type": "number"},
            {"name": "RoadCondition", "label": "Kondisi Jalan", "col": 6, "type": "select", "options": ["GOOD", "MEDIUM", "POOR"]},
            {"name": "Remarks", "label": "Remarks", "col": 12, "type": "textarea"},
        ],
        "columns": [
            {"header": "Kode", "data": "routeCode"},
            {"header": "Nama Rute", "data": "routeName"},
            {"header": "Site", "data": "siteCode"},
            {"header": "Asal", "data": "originLocation"},
            {"header": "Tujuan", "data": "destinationLocation"},
            {"header": "Tipe", "data": "routeType"},
            {"header": "Jarak (km)", "data": "distanceKm", "currency": False},
            {"header": "Biaya/km", "data": "haulCostPerKm", "currency": True},
            {"header": "Status", "data": "status", "badge": True},
        ],
        "stats": [
            {"id": "statTotal", "label": "Total Rute"},
            {"id": "statActive", "label": "Aktif"},
            {"id": "statHaul", "label": "Haul"},
            {"id": "statOB", "label": "Overburden"},
        ],
        "filter_extra": '''
                    <div class="col-md-3">
                        <label class="form-label">Tipe Rute</label>
                        <select class="form-select" id="filterType" onchange="loadData()">
                            <option value="">Semua</option>
                            <option value="HAUL">HAUL</option>
                            <option value="OVERBURDEN">OVERBURDEN</option>
                            <option value="RETURN">RETURN</option>
                        </select>
                    </div>''',
        "extra_stat": "document.getElementById('statHaul').textContent = data.filter(d => d.routeType === 'HAUL').length;\n    document.getElementById('statOB').textContent = data.filter(d => d.routeType === 'OVERBURDEN').length;",
    },
}


def hex_darken(hex_color):
    r = max(0, int(hex_color[1:3], 16) - 25)
    g = max(0, int(hex_color[3:5], 16) - 25)
    b = max(0, int(hex_color[5:7], 16) - 25)
    return f"#{r:02x}{g:02x}{b:02x}"


def build_field(f):
    t = f["type"]
    col = f["col"]
    name = f["name"]
    label = f["label"]
    req = "required" if f.get("required") else ""
    if t == "textarea":
        return f'<div class="col-md-{col}"><label class="form-label">{label}</label><textarea class="form-control" id="f{name}" {req} rows="2"></textarea></div>'
    elif t == "select":
        opts = "".join(f'<option value="{o}">{o}</option>' for o in f["options"])
        return f'<div class="col-md-{col}"><label class="form-label">{label}</label><select class="form-select" id="f{name}" {req}><option value="">-- Pilih --</option>{opts}</select></div>'
    elif t == "number":
        return f'<div class="col-md-{col}"><label class="form-label">{label}</label><input type="number" class="form-control" id="f{name}" step="any"></div>'
    elif t == "date":
        return f'<div class="col-md-{col}"><label class="form-label">{label}</label><input type="date" class="form-control" id="f{name}"></div>'
    else:
        return f'<div class="col-md-{col}"><label class="form-label">{label}</label><input type="text" class="form-control" id="f{name}" {req}></div>'


def build_th(col):
    return f"<th>{col['header']}</th>"


def build_td(col):
    d = col["data"]
    if col.get("currency"):
        return f'<td>{{r.{d} != null ? "Rp " + Number(r.{d}).toLocaleString("id-ID") : "-"}}</td>'
    elif col.get("badge"):
        return f'<td><span class="badge" :class="badgeClass(r.{d})">{{r.{d}}}</span></td>'
    else:
        return f"<td>{{r.{d} ?? '-'}}</td>"


def badge_logic():
    lines = []
    lines.append("if (val === 'ACTIVE') return 'badge-active';")
    lines.append("if (val === 'INACTIVE') return 'badge-inactive';")
    lines.append("if (val === 'MAINTENANCE') return 'badge bg-warning text-dark';")
    lines.append("if (val === 'IDLE') return 'badge bg-secondary';")
    lines.append("if (val === 'SCRAP') return 'badge bg-danger';")
    lines.append("if (val === 'OFF_DUTY') return 'badge bg-secondary';")
    lines.append("if (val === 'TERMINATED') return 'badge bg-danger';")
    return "".join(lines)


def build_page(fname, info):
    accent = info["accent"]
    r = int(accent[1:3], 16)
    g = int(accent[3:5], 16)
    b = int(accent[5:7], 16)
    accent_dim = f"rgba({r},{g},{b},0.15)"
    accent_dark = hex_darken(accent)

    fields_html = "\n            ".join(build_field(f) for f in info["fields"])
    col_headers = "".join(build_th(c) for c in info["columns"]) + "<th style='width:110px'>Aksi</th>"
    col_cells = "".join(build_td(c) for c in info["columns"]) + '<td><button class="btn btn-sm btn-outline-primary me-1" onclick="editRow(r.id)"><i class="bi bi-pencil"></i></button><button class="btn btn-sm btn-outline-danger" onclick="deleteRow(r.id)"><i class="bi bi-trash"></i></button></td>'
    stat_cards = "".join(
        f'<div class="col-md-3"><div class="stat-card"><h6>{s["label"]}</h6><div class="number" id="{s["id"]}">-</div></div></div>'
        for s in info["stats"]
    )
    badge_fn = badge_logic()
    extra_stat = info["extra_stat"]
    filter_extra = info.get("filter_extra", "")
    field_loads = "\n            ".join(
        ("document.getElementById('f" + f["name"] + "').value = r." + f["name"] + " ?? '';")
        for f in info["fields"]
    )
    field_body = "\n            ".join(
        ("body." + f["name"] + " = document.getElementById('f" + f["name"] + "').value || null;")
        for f in info["fields"]
    )

    html = f'''<!DOCTYPE html>
<html lang="id">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>{info["title"]} - LFN</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.1/font/bootstrap-icons.css" rel="stylesheet">
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700&display=swap" rel="stylesheet">
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <style>
        * {{ font-family: 'Inter', sans-serif; box-sizing: border-box; }}
        :root {{
            --navy: #0f172a; --navy-mid: #1e293b;
            --accent: {accent}; --accent-dim: {accent_dim};
            --bg-page: #F8FAFC; --bg-card: #ffffff; --border-color: #e2e8f0;
            --text-primary: #1e293b; --text-secondary: #64748b;
        }}
        body {{ background: var(--bg-page); min-height: 100vh; color: var(--text-primary); }}
        .navbar {{ background: rgba(15,23,42,0.97); backdrop-filter: blur(12px); border-bottom: 1px solid rgba(255,255,255,0.06); padding: 0.7rem 0; box-shadow: 0 1px 20px rgba(0,0,0,0.3); }}
        .navbar-brand {{ font-weight: 700; font-size: 1.4rem; color: #fff !important; display: flex; align-items: center; gap: 10px; }}
        .navbar-brand i {{ color: var(--accent); font-size: 1.8rem; }}
        .navbar-nav {{ display: flex; align-items: center; gap: 2px; flex-wrap: wrap; }}
        .nav-group {{ display: flex; align-items: center; gap: 2px; }}
        .nav-group-label {{ font-size: 0.6rem; font-weight: 700; text-transform: uppercase; letter-spacing: 1.2px; color: rgba(255,255,255,0.25); padding: 0 10px 0 6px; margin-right: 2px; border-right: 1px solid rgba(255,255,255,0.12); white-space: nowrap; }}
        .nav-link {{ color: rgba(255,255,255,0.6) !important; font-weight: 500; font-size: 0.82rem; padding: 0.45rem 0.7rem !important; border-radius: 8px; transition: all 0.2s; display: flex; align-items: center; gap: 5px; white-space: nowrap; }}
        .nav-link i {{ font-size: 0.95rem; opacity: 0.7; }}
        .nav-link:hover, .nav-link.active {{ color: #fff !important; background: var(--accent-dim); }}
        .page-header {{ background: linear-gradient(135deg, var(--navy), var(--navy-mid)); color: white; padding: 24px 28px; border-radius: 12px; margin-bottom: 24px; }}
        .page-header h1 {{ margin: 0; font-size: 1.4rem; font-weight: 700; letter-spacing: -0.01em; display: flex; align-items: center; gap: 10px; }}
        .page-header p {{ margin: 4px 0 0; opacity: 0.6; font-size: 0.875rem; }}
        .card {{ border-radius: 16px; border: 1px solid var(--border-color); box-shadow: 0 1px 4px rgba(0,0,0,0.05); overflow: hidden; }}
        .card-header {{ background: #fff; border-bottom: 1px solid var(--border-color); padding: 16px 24px; font-weight: 600; font-size: 0.95rem; display: flex; align-items: center; justify-content: space-between; }}
        .card-body {{ padding: 24px; }}
        .stat-card {{ background: var(--bg-card); border-radius: 12px; border: 1px solid var(--border-color); border-top: 3px solid var(--accent); padding: 18px 20px; box-shadow: 0 1px 3px rgba(0,0,0,0.05); transition: transform 0.2s; }}
        .stat-card:hover {{ transform: translateY(-2px); box-shadow: 0 8px 24px rgba(0,0,0,0.1); }}
        .stat-card h6 {{ color: var(--text-secondary); font-size: 0.72rem; font-weight: 600; text-transform: uppercase; letter-spacing: 0.5px; margin-bottom: 6px; }}
        .stat-card .number {{ font-size: 1.8rem; font-weight: 700; color: var(--text-primary); line-height: 1.1; }}
        .form-label {{ font-weight: 500; color: var(--text-primary); font-size: 0.85rem; }}
        .form-control, .form-select {{ border-radius: 10px; border: 1.5px solid var(--border-color); padding: 10px 14px; font-size: 0.9rem; transition: border-color 0.2s, box-shadow 0.2s; background: #fff; color: var(--text-primary); }}
        .form-control:focus, .form-select:focus {{ border-color: var(--accent); box-shadow: 0 0 0 3px var(--accent-dim); outline: none; }}
        .btn-primary {{ background: var(--accent); border: none; border-radius: 10px; padding: 10px 20px; font-weight: 600; font-size: 0.88rem; transition: all 0.2s; color: #fff; }}
        .btn-primary:hover {{ background: {accent_dark}; transform: translateY(-1px); box-shadow: 0 4px 12px {accent_dim.replace("0.15","0.4")}; }}
        .btn-outline-primary {{ border-radius: 10px; border: 1.5px solid var(--accent); color: var(--accent); font-weight: 500; font-size: 0.82rem; padding: 8px 16px; }}
        .btn-outline-primary:hover {{ background: var(--accent-dim); }}
        .btn-outline-secondary {{ border-radius: 10px; border: 1.5px solid var(--border-color); color: var(--text-secondary); font-weight: 500; }}
        .btn-outline-secondary:hover {{ background: var(--bg-page); }}
        .btn-outline-danger {{ border-radius: 10px; border: 1.5px solid #ef4444; color: #ef4444; font-size: 0.82rem; padding: 8px 14px; }}
        .btn-outline-danger:hover {{ background: #fef2f2; }}
        .btn-sm {{ border-radius: 8px; font-size: 0.8rem; padding: 6px 12px; }}
        .table {{ border-collapse: separate; border-spacing: 0; width: 100%; font-size: 0.875rem; }}
        .table thead th {{ background: #f8fafc; color: var(--text-secondary); font-weight: 600; font-size: 0.75rem; text-transform: uppercase; letter-spacing: 0.5px; padding: 12px 14px; border-bottom: 2px solid var(--border-color); white-space: nowrap; }}
        .table tbody td {{ padding: 13px 14px; border-bottom: 1px solid var(--border-color); vertical-align: middle; color: var(--text-primary); }}
        .table tbody tr:hover {{ background: #f8fafc; }}
        .table-responsive {{ border-radius: 10px; overflow: hidden; border: 1px solid var(--border-color); }}
        .badge {{ border-radius: 50px; font-weight: 600; font-size: 0.72rem; padding: 4px 10px; letter-spacing: 0.3px; }}
        .badge-active {{ background: rgba(34,197,94,0.12); color: #15803d; }}
        .badge-inactive {{ background: rgba(100,116,139,0.12); color: #475569; }}
        .badge.bg-warning {{ background: rgba(234,179,8,0.15) !important; color: #a16207 !important; }}
        .badge.bg-danger {{ background: rgba(239,68,68,0.12) !important; color: #b91c1c !important; }}
        .badge.bg-secondary {{ background: rgba(100,116,139,0.12) !important; color: #475569 !important; }}
        .modal-content {{ border-radius: 16px; border: none; box-shadow: 0 20px 60px rgba(0,0,0,0.3); }}
        .modal-header {{ border-bottom: 1px solid var(--border-color); padding: 20px 24px; }}
        .modal-body {{ padding: 24px; }}
        .modal-footer {{ border-top: 1px solid var(--border-color); padding: 16px 24px; }}
        .loading-overlay {{ position: fixed; top: 0; left: 0; right: 0; bottom: 0; background: rgba(255,255,255,0.85); z-index: 9999; display: flex; align-items: center; justify-content: center; flex-direction: column; gap: 12px; }}
        .spinner {{ width: 40px; height: 40px; border: 4px solid var(--border-color); border-top-color: var(--accent); border-radius: 50%; animation: spin 0.8s linear infinite; }}
        @keyframes spin {{ to {{ transform: rotate(360deg); }} }}
        .no-data {{ text-align: center; padding: 48px; color: var(--text-secondary); }}
        .no-data i {{ font-size: 3rem; opacity: 0.3; margin-bottom: 12px; }}
    </style>
</head>
<body>
    <div id="loadingOverlay" class="loading-overlay" style="display:none">
        <div class="spinner"></div>
        <span style="color:var(--text-secondary);font-weight:500">Memuat...</span>
    </div>

    <nav class="navbar navbar-expand-lg sticky-top">
        <div class="container">
            <a class="navbar-brand" href="/"><i class="bi bi-diagram-3"></i> LFN</a>
            <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav"><span class="navbar-toggler-icon"></span></button>
            <div class="collapse navbar-collapse" id="navbarNav">
                <ul class="navbar-nav ms-auto flex-wrap">
                    <li class="nav-item"><a class="nav-link" href="/"><i class="bi bi-house"></i> Home</a></li>
                    <li class="nav-group">
                        <span class="nav-group-label">P1 Master</span>
                        <a class="nav-link" href="master-site.html"><i class="bi bi-geo-alt"></i> Site</a>
                        <a class="nav-link" href="master-cost-center.html"><i class="bi bi-diagram-3"></i> Cost Center</a>
                        <a class="nav-link" href="master-user.html"><i class="bi bi-people"></i> User</a>
                    </li>
                    <li class="nav-group">
                        <span class="nav-group-label">P2 Master</span>
                        <a class="nav-link" href="master-department.html"><i class="bi bi-diagram-3"></i> Dept</a>
                        <a class="nav-link" href="master-material.html"><i class="bi bi-box-seam"></i> Material</a>
                        <a class="nav-link" href="master-uom.html"><i class="bi bi-rulers"></i> UOM</a>
                        <a class="nav-link" href="master-workflow.html"><i class="bi bi-arrow-right-circle"></i> Workflow</a>
                    </li>
                    <li class="nav-group">
                        <span class="nav-group-label">P3 Master</span>
                        <a class="nav-link" href="master-vehicle.html"><i class="bi bi-truck"></i> Vehicle</a>
                        <a class="nav-link" href="master-driver.html"><i class="bi bi-person-badge"></i> Driver</a>
                        <a class="nav-link" href="master-route.html"><i class="bi bi-pin-map"></i> Route</a>
                    </li>
                    <li class="nav-group">
                        <span class="nav-group-label">Operasi</span>
                        <a class="nav-link" href="data-Upload-Fuel.html"><i class="bi bi-fuel-pump"></i> Fuel</a>
                        <a class="nav-link" href="rm.html"><i class="bi bi-tools"></i> R&amp;M</a>
                    </li>
                </ul>
            </div>
        </div>
    </nav>

    <div class="container py-4">
        <div class="page-header">
            <h1><i class="bi {info["icon"]}" style="color:{accent}"></i> {info["title"]}</h1>
            <p>{info["subtitle"]}</p>
        </div>

        <div class="row mb-4 g-3">
            {stat_cards}
        </div>

        <div class="card mb-4">
            <div class="card-header">
                <span>Filter</span>
                <button class="btn btn-primary" onclick="openForm()"><i class="bi bi-plus-lg"></i> Tambah Baru</button>
            </div>
            <div class="card-body pb-2">
                <div class="row g-3 align-items-end">
                    <div class="col-md-3">
                        <label class="form-label">Status</label>
                        <select class="form-select" id="filterStatus" onchange="loadData()">
                            <option value="">Semua</option>
                            <option value="ACTIVE">ACTIVE</option>
                            <option value="INACTIVE">INACTIVE</option>
                        </select>
                    </div>
                    {filter_extra}
                    <div class="col-md-auto">
                        <button class="btn btn-outline-secondary" onclick="resetFilters()"><i class="bi bi-x-circle"></i> Reset</button>
                    </div>
                </div>
            </div>
        </div>

        <div class="card">
            <div class="card-body p-0">
                <div class="table-responsive">
                    <table class="table mb-0">
                        <thead><tr>{col_headers}</tr></thead>
                        <tbody id="tableBody"></tbody>
                    </table>
                    <div id="noData" class="no-data" style="display:none">
                        <i class="bi bi-inbox"></i>
                        <p>Belum ada data. Klik "Tambah Baru" untuk menambahkan.</p>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="formModal" tabindex="-1">
        <div class="modal-dialog modal-lg modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="modalTitle">Tambah Baru</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <form id="mainForm">
                        <input type="hidden" id="fId">
                        <div class="row g-3">
                            {fields_html}
                        </div>
                    </form>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">Batal</button>
                    <button type="button" class="btn btn-primary" onclick="saveForm()"><i class="bi bi-check-lg"></i> Simpan</button>
                </div>
            </div>
        </div>
    </div>

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
    <script>
        const API = "{API_BASE}{info["api"]}";
        const MODAL = new bootstrap.Modal(document.getElementById("formModal"));
        let editingId = null;

        function showLoading(show) {{
            document.getElementById("loadingOverlay").style.display = show ? "flex" : "none";
        }}

        function badgeClass(val) {{
            {badge_fn}
            return "";
        }}

        function resetFilters() {{
            document.getElementById("filterStatus").value = "";
            const ft = document.getElementById("filterType");
            if (ft) ft.value = "";
            loadData();
        }}

        async function loadData() {{
            showLoading(true);
            try {{
                const params = new URLSearchParams();
                const status = document.getElementById("filterStatus").value;
                if (status) params.append("status", status);
                const ft = document.getElementById("filterType");
                if (ft && ft.value) {{
                    const typeParam = "{fname}" === "master-vehicle.html" ? "vehicleType" : ("{fname}" === "master-route.html" ? "routeType" : "");
                    if (typeParam) params.append(typeParam, ft.value);
                }}
                const res = await fetch(API + (params.toString() ? "?" + params : ""));
                if (!res.ok) throw new Error(await res.text());
                const data = await res.json();
                renderTable(data);
                renderStats(data);
            }} catch (e) {{
                console.error(e);
                Swal.fire("Error", "Gagal memuat data: " + e.message, "error");
            }} finally {{
                showLoading(false);
            }}
        }}

        function renderTable(data) {{
            const tbody = document.getElementById("tableBody");
            const noData = document.getElementById("noData");
            if (!data || data.length === 0) {{
                tbody.innerHTML = "";
                noData.style.display = "block";
                return;
            }}
            noData.style.display = "none";
            tbody.innerHTML = data.map(r => `<tr>{col_cells}</tr>`).join("");
        }}

        function renderStats(data) {{
            if (!data) return;
            document.getElementById("statTotal").textContent = data.length;
            document.getElementById("statActive").textContent = data.filter(d => d.status === "ACTIVE").length;
            {extra_stat}
        }}

        function openForm(id) {{
            editingId = id;
            document.getElementById("fId").value = "";
            document.getElementById("modalTitle").textContent = id ? "Edit Data" : "Tambah Baru";
            document.querySelectorAll("#mainForm input, #mainForm select, #mainForm textarea").forEach(el => {{
                if (el.type === "hidden") return;
                el.value = "";
            }});
            if (id) loadRow(id);
            MODAL.show();
        }}

        async function loadRow(id) {{
            const res = await fetch(API + "/" + id);
            if (!res.ok) return;
            const r = await res.json();
            {field_loads}
        }}

        async function editRow(id) {{ openForm(id); }}

        async function deleteRow(id) {{
            const ok = await Swal.fire({{ title: "Hapus Data?", text: "Data yang dihapus tidak bisa dikembalikan.", icon: "warning", showCancelButton: true, confirmButtonColor: "#ef4444", confirmButtonText: "Ya, Hapus!" }});
            if (!ok.isConfirmed) return;
            const res = await fetch(API + "/" + id, {{ method: "DELETE" }});
            if (res.ok) {{ Swal.fire("Berhasil","Data dihapus","success"); loadData(); }}
            else {{ const e = await res.json(); Swal.fire("Error", e.error || "Gagal hapus","error"); }}
        }}

        async function saveForm() {{
            const body = {{}};
            {field_body}
            Object.keys(body).forEach(k => {{ if (body[k] === "") body[k] = null; }});
            const method = editingId ? "PUT" : "POST";
            const url = editingId ? API + "/" + editingId : API;
            const res = await fetch(url, {{ method, headers: {{"Content-Type":"application/json"}}, body: JSON.stringify(body) }});
            if (res.ok) {{ Swal.fire("Berhasil","Data disimpan","success"); MODAL.hide(); loadData(); }}
            else {{ const e = await res.json(); Swal.fire("Error", e.error || "Gagal menyimpan","error"); }}
        }}

        loadData();
    </script>
</body>
</html>'''

    with open(os.path.join(BASE, fname), "w", encoding="utf-8") as f:
        f.write(html)
    print(f"Created: {fname}")


for fname, info in PAGES.items():
    build_page(fname, info)

print("All P3 pages generated!")
