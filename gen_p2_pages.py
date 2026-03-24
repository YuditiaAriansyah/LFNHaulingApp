import os

BASE = r"c:\LFN\LFNHaulingApp\HaulingDemoApp\wwwroot"
API_BASE = "https://lfn-hauling-903235665988.asia-southeast1.run.app"

PAGES = {
    "master-department.html": {
        "accent": "#7c3aed",
        "icon": "bi-diagram-3",
        "title": "Department Master",
        "subtitle": "Kelola struktur organisasi (Divisi, Departemen, Section)",
        "api": "/api/departments/master",
        "fields": [
            {"name": "DeptCode", "label": "Kode Dept", "col": 4, "type": "text", "required": True},
            {"name": "DeptName", "label": "Nama Dept", "col": 8, "type": "text", "required": True},
            {"name": "Level", "label": "Level", "col": 4, "type": "select", "options": ["DIVISION", "DEPARTMENT", "SECTION"]},
            {"name": "ParentCode", "label": "Parent Code", "col": 4, "type": "text"},
            {"name": "SiteCode", "label": "Site Code", "col": 4, "type": "text"},
            {"name": "CostCenter", "label": "Cost Center", "col": 6, "type": "text"},
            {"name": "HeadName", "label": "Nama Kepala", "col": 6, "type": "text"},
            {"name": "HeadTitle", "label": "Jabatan Kepala", "col": 6, "type": "text"},
            {"name": "Remarks", "label": "Remarks", "col": 12, "type": "textarea"},
        ],
        "columns": [
            {"header": "Code", "data": "deptCode"},
            {"header": "Nama Dept", "data": "deptName"},
            {"header": "Level", "data": "level", "badge_key": True},
            {"header": "Parent", "data": "parentCode"},
            {"header": "Site", "data": "siteCode"},
            {"header": "Cost Center", "data": "costCenter"},
            {"header": "Status", "data": "status", "badge": True},
        ],
        "stats": [
            {"id": "statTotal", "label": "Total Dept"},
            {"id": "statActive", "label": "Dept Aktif"},
            {"id": "statDivision", "label": "Divisi"},
            {"id": "statDept", "label": "Departemen"},
        ],
    },
    "master-material.html": {
        "accent": "#ea580c",
        "icon": "bi-box-seam",
        "title": "Material / Service Master",
        "subtitle": "Kelola daftar material, spare part, dan jasa",
        "api": "/api/materials/master",
        "fields": [
            {"name": "MaterialCode", "label": "Kode Material", "col": 4, "type": "text", "required": True},
            {"name": "MaterialName", "label": "Nama Material", "col": 8, "type": "text", "required": True},
            {"name": "MaterialGroup", "label": "Group", "col": 4, "type": "select", "options": ["FUEL", "TYRE", "PART", "SERVICE", "OTHERS"]},
            {"name": "MaterialType", "label": "Tipe", "col": 4, "type": "select", "options": ["ITEM", "SERVICE"]},
            {"name": "UomCode", "label": "Satuan (UOM)", "col": 4, "type": "text"},
            {"name": "Brand", "label": "Merk", "col": 6, "type": "text"},
            {"name": "Spec", "label": "Spesifikasi", "col": 6, "type": "text"},
            {"name": "UnitPrice", "label": "Harga Satuan", "col": 4, "type": "number"},
            {"name": "Currency", "label": "Mata Uang", "col": 4, "type": "text"},
            {"name": "SiteCode", "label": "Site Code", "col": 4, "type": "text"},
            {"name": "Remarks", "label": "Remarks", "col": 12, "type": "textarea"},
        ],
        "columns": [
            {"header": "Kode", "data": "materialCode"},
            {"header": "Nama Material", "data": "materialName"},
            {"header": "Group", "data": "materialGroup"},
            {"header": "Tipe", "data": "materialType"},
            {"header": "UOM", "data": "uomCode"},
            {"header": "Harga", "data": "unitPrice", "currency": True},
            {"header": "Status", "data": "status", "badge": True},
        ],
        "stats": [
            {"id": "statTotal", "label": "Total Material"},
            {"id": "statActive", "label": "Aktif"},
            {"id": "statItem", "label": "Item"},
            {"id": "statService", "label": "Service"},
        ],
    },
    "master-uom.html": {
        "accent": "#0f766e",
        "icon": "bi-rulers",
        "title": "Unit of Measure (UOM) Master",
        "subtitle": "Kelola satuan ukuran (Volume, Weight, Count, dll)",
        "api": "/api/uom/master",
        "fields": [
            {"name": "UomCode", "label": "Kode UOM", "col": 4, "type": "text", "required": True},
            {"name": "UomName", "label": "Nama UOM", "col": 8, "type": "text", "required": True},
            {"name": "UomType", "label": "Tipe", "col": 4, "type": "select", "options": ["VOLUME", "WEIGHT", "LENGTH", "COUNT", "TIME", "CURRENCY"]},
            {"name": "BaseUomCode", "label": "UOM Dasar", "col": 4, "type": "text"},
            {"name": "ConversionFactor", "label": "Faktor Konversi", "col": 4, "type": "number"},
            {"name": "Remarks", "label": "Remarks", "col": 12, "type": "textarea"},
        ],
        "columns": [
            {"header": "Kode UOM", "data": "uomCode"},
            {"header": "Nama UOM", "data": "uomName"},
            {"header": "Tipe", "data": "uomType"},
            {"header": "UOM Dasar", "data": "baseUomCode"},
            {"header": "Faktor", "data": "conversionFactor"},
        ],
        "stats": [
            {"id": "statTotal", "label": "Total UOM"},
            {"id": "statVolume", "label": "Volume"},
            {"id": "statWeight", "label": "Weight"},
            {"id": "statCount", "label": "Count"},
        ],
    },
    "master-workflow.html": {
        "accent": "#2563eb",
        "icon": "bi-arrow-right-circle",
        "title": "Approval Workflow Master",
        "subtitle": "Kelola alur persetujuan untuk PR, PO, GR, dan modul lainnya",
        "api": "/api/workflows/master",
        "fields": [
            {"name": "WorkflowName", "label": "Nama Workflow", "col": 6, "type": "text", "required": True},
            {"name": "ModuleType", "label": "Modul", "col": 3, "type": "select", "options": ["PR", "PO", "GR", "FUEL_USAGE", "RM"], "required": True},
            {"name": "ApprovalOrder", "label": "Step ke-", "col": 3, "type": "number"},
            {"name": "SiteCode", "label": "Site Code", "col": 4, "type": "text"},
            {"name": "CostCenter", "label": "Cost Center", "col": 4, "type": "text"},
            {"name": "ApproverRole", "label": "Role Approver", "col": 4, "type": "select", "options": ["ADMIN", "MANAGER", "USER"], "required": True},
            {"name": "ApproverName", "label": "Nama Approver", "col": 6, "type": "text"},
            {"name": "ApprovalLevel", "label": "Level", "col": 3, "type": "select", "options": ["REQUIRED", "OPTIONAL", "SKIP"]},
            {"name": "MinAmount", "label": "Min Amount", "col": 3, "type": "number"},
            {"name": "MaxAmount", "label": "Max Amount", "col": 6, "type": "number"},
            {"name": "Remarks", "label": "Remarks", "col": 12, "type": "textarea"},
        ],
        "columns": [
            {"header": "Nama Workflow", "data": "workflowName"},
            {"header": "Modul", "data": "moduleType"},
            {"header": "Step", "data": "approvalOrder"},
            {"header": "Role", "data": "approverRole"},
            {"header": "Nama Approver", "data": "approverName"},
            {"header": "Level", "data": "approvalLevel"},
            {"header": "Min Amt", "data": "minAmount", "currency": True},
            {"header": "Max Amt", "data": "maxAmount", "currency": True},
            {"header": "Status", "data": "status", "badge": True},
        ],
        "stats": [
            {"id": "statTotal", "label": "Total Workflow"},
            {"id": "statActive", "label": "Aktif"},
            {"id": "statPR", "label": "PR Flow"},
            {"id": "statPO", "label": "PO Flow"},
        ],
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


def badge_logic(info):
    has_level = any(c.get("badge_key") for c in info["columns"])
    lines = []
    if has_level:
        lines.append("if (val === 'DIVISION') return 'bg-primary';")
        lines.append("if (val === 'DEPARTMENT') return 'bg-info';")
        lines.append("if (val === 'SECTION') return 'bg-secondary';")
    lines.append("if (val === 'ACTIVE') return 'badge-active';")
    lines.append("if (val === 'INACTIVE') return 'badge-inactive';")
    return "".join(lines)


def extra_stat_render(info):
    name = info["title"].split(" ")[0].lower()
    if "Department" in info["title"]:
        return """
    document.getElementById("statDivision").textContent = data.filter(d => d.level === "DIVISION").length;
    document.getElementById("statDept").textContent = data.filter(d => d.level === "DEPARTMENT").length;
"""
    if "Material" in info["title"]:
        return """
    document.getElementById("statItem").textContent = data.filter(d => d.materialType === "ITEM").length;
    document.getElementById("statService").textContent = data.filter(d => d.materialType === "SERVICE").length;
"""
    if "UOM" in info["title"]:
        return """
    document.getElementById("statVolume").textContent = data.filter(d => d.uomType === "VOLUME").length;
    document.getElementById("statWeight").textContent = data.filter(d => d.uomType === "WEIGHT").length;
    document.getElementById("statCount").textContent = data.filter(d => d.uomType === "COUNT").length;
"""
    if "Workflow" in info["title"]:
        return """
    document.getElementById("statPR").textContent = data.filter(d => d.moduleType === "PR").length;
    document.getElementById("statPO").textContent = data.filter(d => d.moduleType === "PO").length;
"""
    return ""


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
    badge_fn = badge_logic(info)
    extra_stat = extra_stat_render(info)
    field_loads = "\n            ".join(
        f"document.getElementById('f{f['name']}').value = r.{f['name']} ?? '';"
        for f in info["fields"]
    )
    field_body = "\n            ".join(
        f'body.{f["name"]} = document.getElementById("f{f["name"]}").value || null;'
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
        .badge.bg-primary {{ background: rgba(37,99,235,0.1) !important; color: #1d4ed8 !important; }}
        .badge.bg-info {{ background: rgba(14,165,233,0.1) !important; color: #0284c7 !important; }}
        .badge.bg-secondary {{ background: rgba(100,116,139,0.1) !important; color: #475569 !important; }}
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
                    <div class="col-md-auto">
                        <button class="btn btn-outline-secondary" onclick="document.getElementById('filterStatus').value='';loadData()"><i class="bi bi-x-circle"></i> Reset</button>
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

        async function loadData() {{
            showLoading(true);
            try {{
                const params = new URLSearchParams();
                const status = document.getElementById("filterStatus").value;
                if (status) params.append("status", status);
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
            tbody.innerHTML = data.map(r => `
                <tr>
                    {col_cells}
                </tr>
            `).join("");
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

print("All P2 pages generated!")
