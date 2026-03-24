import re
import os

BASE = r"c:\LFN\LFNHaulingApp\HaulingDemoApp\wwwroot"

# ─────────────────────────────────────────────
# CSS for datalist-fuel.html  (accent: #ef4444 red)
# ─────────────────────────────────────────────
CSS_FUEL_LIST = r"""
    <style>
        * { font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif; box-sizing: border-box; }
        :root {
            --navy: #0f172a;
            --navy-mid: #1e293b;
            --accent: #ef4444;
            --accent-dim: rgba(239, 68, 68, 0.15);
            --accent-light: rgba(239, 68, 68, 0.08);
            --bg-page: #F8FAFC;
            --bg-card: #ffffff;
            --border-color: #e2e8f0;
            --text-primary: #1e293b;
            --text-secondary: #64748b;
        }
        body { background: var(--bg-page); min-height: 100vh; color: var(--text-primary); }

        /* Navbar */
        .navbar { background: rgba(15, 23, 42, 0.97); backdrop-filter: blur(12px); border-bottom: 1px solid rgba(255,255,255,0.06); padding: 0.7rem 0; box-shadow: 0 1px 20px rgba(0,0,0,0.3); }
        .navbar-brand { font-weight: 700; font-size: 1.4rem; color: #fff !important; display: flex; align-items: center; gap: 10px; }
        .navbar-brand i { color: var(--accent); font-size: 1.8rem; }
        .navbar-nav { display: flex; align-items: center; gap: 2px; flex-wrap: wrap; }
        .nav-group { display: flex; align-items: center; gap: 2px; }
        .nav-group-label { font-size: 0.6rem; font-weight: 700; text-transform: uppercase; letter-spacing: 1.2px; color: rgba(255,255,255,0.25); padding: 0 10px 0 6px; margin-right: 2px; border-right: 1px solid rgba(255,255,255,0.12); white-space: nowrap; }
        .nav-link { color: rgba(255,255,255,0.6) !important; font-weight: 500; font-size: 0.82rem; padding: 0.45rem 0.7rem !important; border-radius: 8px; transition: all 0.2s ease; display: flex; align-items: center; gap: 5px; white-space: nowrap; }
        .nav-link i { font-size: 0.95rem; opacity: 0.7; }
        .nav-link:hover, .nav-link.active { color: #fff !important; background: var(--accent-dim); }
        .nav-link.active::before { content: ''; position: absolute; left: 0; top: 50%; transform: translateY(-50%); width: 3px; height: 60%; background: var(--accent); border-radius: 0 2px 2px 0; }

        /* Cards */
        .main-container { background: var(--bg-card); border-radius: 16px; box-shadow: 0 1px 4px rgba(0,0,0,0.06), 0 4px 16px rgba(0,0,0,0.04); padding: 28px; margin-top: 24px; border: 1px solid var(--border-color); }

        /* Page header */
        .page-header { background: linear-gradient(135deg, var(--navy), var(--navy-mid)); color: white; padding: 24px 28px; border-radius: 12px; margin-bottom: 28px; box-shadow: 0 4px 16px rgba(0,0,0,0.15); }
        .page-header h1 { margin: 0; font-size: 1.5rem; font-weight: 700; letter-spacing: -0.01em; }
        .page-header p { margin: 6px 0 0; opacity: 0.7; font-size: 0.875rem; }

        /* Stats */
        .stat-card { background: var(--bg-card); border-radius: 12px; border: 1px solid var(--border-color); border-top: 3px solid var(--accent); padding: 20px; box-shadow: 0 1px 3px rgba(0,0,0,0.05); transition: transform 0.2s, box-shadow 0.2s; }
        .stat-card:hover { transform: translateY(-2px); box-shadow: 0 8px 24px rgba(0,0,0,0.1); }
        .stat-card h6 { color: var(--text-secondary); font-size: 0.78rem; font-weight: 600; text-transform: uppercase; letter-spacing: 0.5px; margin-bottom: 8px; }
        .stat-card .number { font-size: 1.8rem; font-weight: 700; color: var(--text-primary); line-height: 1.1; }

        /* Filter */
        .filter-section { background: var(--bg-page); padding: 20px; border-radius: 10px; margin-bottom: 24px; border: 1px solid var(--border-color); border-left: 3px solid var(--accent); }
        .filter-section h5 { color: var(--text-primary); margin-bottom: 14px; font-weight: 600; font-size: 0.95rem; }

        /* Forms */
        .form-label { font-weight: 500; color: var(--text-primary); font-size: 0.85rem; }
        .form-select, .form-control { border-radius: 10px; border: 1.5px solid var(--border-color); padding: 10px 14px; font-size: 0.9rem; transition: border-color 0.2s, box-shadow 0.2s; background: #fff; color: var(--text-primary); }
        .form-select:focus, .form-control:focus { border-color: var(--accent); box-shadow: 0 0 0 3px var(--accent-dim); outline: none; }

        /* Buttons */
        .btn-primary { background: var(--accent); border: none; border-radius: 10px; padding: 10px 20px; font-weight: 600; font-size: 0.88rem; transition: all 0.2s; color: #fff; }
        .btn-primary:hover { background: #dc2626; transform: translateY(-1px); box-shadow: 0 4px 12px rgba(239,68,68,0.3); }
        .btn-outline-secondary { border-radius: 10px; border: 1.5px solid var(--border-color); color: var(--text-secondary); font-weight: 500; }
        .btn-outline-secondary:hover { background: var(--bg-page); border-color: #cbd5e1; }
        .btn-success { background: #22c55e; border: none; border-radius: 10px; font-weight: 600; }
        .btn-success:hover { background: #16a34a; }
        .btn-sm { border-radius: 8px; font-size: 0.8rem; padding: 6px 12px; }

        /* Table */
        .table { border-collapse: separate; border-spacing: 0; width: 100%; font-size: 0.875rem; }
        .table thead th { background: #f8fafc; color: var(--text-secondary); font-weight: 600; font-size: 0.78rem; text-transform: uppercase; letter-spacing: 0.5px; padding: 12px 14px; border-bottom: 2px solid var(--border-color); white-space: nowrap; }
        .table tbody td { padding: 13px 14px; border-bottom: 1px solid var(--border-color); vertical-align: middle; color: var(--text-primary); }
        .table tbody tr:hover { background: #f8fafc; }
        .table-responsive { border-radius: 10px; overflow: hidden; border: 1px solid var(--border-color); }

        /* Badges */
        .badge { border-radius: 50px; font-weight: 600; font-size: 0.72rem; padding: 4px 10px; letter-spacing: 0.3px; }
        .badge-success { background: rgba(34, 197, 94, 0.12); color: #15803d; }
        .badge-warning { background: rgba(245, 158, 11, 0.12); color: #b45309; }
        .badge-danger { background: rgba(239, 68, 68, 0.12); color: #b91c1c; }
        .badge-info { background: rgba(59, 130, 246, 0.12); color: #1d4ed8; }
        .badge-secondary { background: rgba(100, 116, 139, 0.12); color: #475569; }

        /* Card component */
        .card { border-radius: 12px; border: 1px solid var(--border-color); box-shadow: 0 1px 4px rgba(0,0,0,0.05); overflow: hidden; }
        .card-header { background: #fff; border-bottom: 1px solid var(--border-color); font-weight: 600; padding: 16px 20px; }
        .card-body { padding: 20px; }

        /* Tabs */
        .nav-tabs { border-bottom: 2px solid var(--border-color); gap: 4px; }
        .nav-tabs .nav-link { color: var(--text-secondary); border: none; background: transparent; padding: 10px 18px; border-radius: 8px 8px 0 0; font-weight: 500; font-size: 0.875rem; transition: all 0.2s; border-bottom: 2px solid transparent; margin-bottom: -2px; }
        .nav-tabs .nav-link:hover { color: var(--accent); background: var(--accent-light); }
        .nav-tabs .nav-link.active { color: var(--accent); border-bottom: 2px solid var(--accent); background: var(--accent-light); }
    </style>
"""

# ─────────────────────────────────────────────
# CSS for datalist- tyre.html  (accent: #f59e0b amber)
# ─────────────────────────────────────────────
CSS_TYRE_LIST = r"""
    <style>
        * { font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif; box-sizing: border-box; }
        :root {
            --navy: #0f172a;
            --navy-mid: #1e293b;
            --accent: #f59e0b;
            --accent-dim: rgba(245, 158, 11, 0.15);
            --accent-light: rgba(245, 158, 11, 0.08);
            --bg-page: #F8FAFC;
            --bg-card: #ffffff;
            --border-color: #e2e8f0;
            --text-primary: #1e293b;
            --text-secondary: #64748b;
        }
        body { background: var(--bg-page); min-height: 100vh; padding-top: 76px; color: var(--text-primary); }

        /* Navbar */
        .navbar { background: rgba(15, 23, 42, 0.97); backdrop-filter: blur(12px); border-bottom: 1px solid rgba(255,255,255,0.06); padding: 0.7rem 0; box-shadow: 0 1px 20px rgba(0,0,0,0.3); }
        .navbar-brand { font-weight: 700; font-size: 1.4rem; color: #fff !important; display: flex; align-items: center; gap: 10px; }
        .navbar-brand i { color: var(--accent); font-size: 1.8rem; }
        .navbar-nav { display: flex; align-items: center; gap: 2px; }
        .nav-link { color: rgba(255,255,255,0.6) !important; font-weight: 500; font-size: 0.82rem; padding: 0.45rem 0.7rem !important; border-radius: 8px; transition: all 0.2s; display: flex; align-items: center; gap: 5px; white-space: nowrap; }
        .nav-link i { font-size: 0.95rem; opacity: 0.7; }
        .nav-link:hover, .nav-link.active { color: #fff !important; background: var(--accent-dim); }
        .nav-link.active::before { content: ''; position: absolute; left: 0; top: 50%; transform: translateY(-50%); width: 3px; height: 60%; background: var(--accent); border-radius: 0 2px 2px 0; }

        /* Stats */
        .stats-card { background: var(--bg-card); border-radius: 12px; border: 1px solid var(--border-color); border-top: 3px solid var(--accent); padding: 20px; box-shadow: 0 1px 3px rgba(0,0,0,0.05); transition: transform 0.2s, box-shadow 0.2s; }
        .stats-card:hover { transform: translateY(-2px); box-shadow: 0 8px 24px rgba(0,0,0,0.1); }
        .stats-card h6 { color: var(--text-secondary); font-size: 0.78rem; font-weight: 600; text-transform: uppercase; letter-spacing: 0.5px; margin-bottom: 8px; }
        .stats-card .number { font-size: 1.8rem; font-weight: 700; color: var(--text-primary); line-height: 1.1; }

        /* Card */
        .card { border-radius: 12px; border: 1px solid var(--border-color); box-shadow: 0 1px 4px rgba(0,0,0,0.05); overflow: hidden; }
        .card-header { background: #fff; border-bottom: 1px solid var(--border-color); font-weight: 600; padding: 16px 20px; }
        .card-body { padding: 20px; }

        /* Forms */
        .form-label { font-weight: 500; color: var(--text-primary); font-size: 0.85rem; }
        .form-select, .form-control { border-radius: 10px; border: 1.5px solid var(--border-color); padding: 10px 14px; font-size: 0.9rem; transition: border-color 0.2s, box-shadow 0.2s; background: #fff; color: var(--text-primary); }
        .form-select:focus, .form-control:focus { border-color: var(--accent); box-shadow: 0 0 0 3px var(--accent-dim); outline: none; }

        /* Buttons */
        .btn-primary { background: var(--accent); border: none; border-radius: 10px; padding: 10px 20px; font-weight: 600; font-size: 0.88rem; transition: all 0.2s; color: #fff; }
        .btn-primary:hover { background: #d97706; transform: translateY(-1px); box-shadow: 0 4px 12px rgba(245,158,11,0.3); }

        /* Table */
        .table { border-collapse: separate; border-spacing: 0; width: 100%; font-size: 0.875rem; }
        .table thead th { background: #f8fafc; color: var(--text-secondary); font-weight: 600; font-size: 0.78rem; text-transform: uppercase; letter-spacing: 0.5px; padding: 12px 14px; border-bottom: 2px solid var(--border-color); white-space: nowrap; }
        .table tbody td { padding: 13px 14px; border-bottom: 1px solid var(--border-color); vertical-align: middle; color: var(--text-primary); }
        .table tbody tr:hover { background: #f8fafc; }
        .table-responsive { border-radius: 10px; overflow: hidden; border: 1px solid var(--border-color); }

        /* Badges */
        .badge { border-radius: 50px; font-weight: 600; font-size: 0.72rem; padding: 4px 10px; letter-spacing: 0.3px; }
        .badge-po { background: rgba(20, 184, 166, 0.12); color: #0f766e; }
        .badge-problem { background: rgba(239, 68, 68, 0.12); color: #b91c1c; }

        /* Tabs */
        .nav-tabs { border-bottom: 2px solid var(--border-color); gap: 4px; }
        .nav-tabs .nav-link { color: var(--text-secondary); border: none; background: transparent; padding: 10px 18px; border-radius: 8px 8px 0 0; font-weight: 500; font-size: 0.875rem; transition: all 0.2s; border-bottom: 2px solid transparent; margin-bottom: -2px; }
        .nav-tabs .nav-link:hover { color: var(--accent); background: var(--accent-light); }
        .nav-tabs .nav-link.active { color: var(--accent); border-bottom: 2px solid var(--accent); background: var(--accent-light); }
    </style>
"""

# ─────────────────────────────────────────────
# CSS for data-Upload-Fuel.html  (accent: #ef4444 red)
# ─────────────────────────────────────────────
CSS_UPLOAD_FUEL = r"""
    <style>
        * { font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif; box-sizing: border-box; }
        :root {
            --navy: #0f172a;
            --navy-mid: #1e293b;
            --accent: #ef4444;
            --accent-dim: rgba(239, 68, 68, 0.15);
            --accent-light: rgba(239, 68, 68, 0.08);
            --bg-page: #F8FAFC;
            --bg-card: #ffffff;
            --border-color: #e2e8f0;
            --text-primary: #1e293b;
            --text-secondary: #64748b;
        }
        body { background: var(--bg-page); min-height: 100vh; color: var(--text-primary); }

        /* Navbar */
        .navbar { background: rgba(15, 23, 42, 0.97); backdrop-filter: blur(12px); border-bottom: 1px solid rgba(255,255,255,0.06); padding: 0.7rem 0; box-shadow: 0 1px 20px rgba(0,0,0,0.3); }
        .navbar-brand { font-weight: 700; font-size: 1.4rem; color: #fff !important; display: flex; align-items: center; gap: 10px; }
        .navbar-brand i { color: var(--accent); font-size: 1.8rem; }
        .navbar-nav { display: flex; align-items: center; gap: 2px; flex-wrap: wrap; }
        .nav-group { display: flex; align-items: center; gap: 2px; }
        .nav-group-label { font-size: 0.6rem; font-weight: 700; text-transform: uppercase; letter-spacing: 1.2px; color: rgba(255,255,255,0.25); padding: 0 10px 0 6px; margin-right: 2px; border-right: 1px solid rgba(255,255,255,0.12); white-space: nowrap; }
        .nav-link { color: rgba(255,255,255,0.6) !important; font-weight: 500; font-size: 0.82rem; padding: 0.45rem 0.7rem !important; border-radius: 8px; transition: all 0.2s; display: flex; align-items: center; gap: 5px; white-space: nowrap; }
        .nav-link i { font-size: 0.95rem; opacity: 0.7; }
        .nav-link:hover, .nav-link.active { color: #fff !important; background: var(--accent-dim); }
        .nav-link.active::before { content: ''; position: absolute; left: 0; top: 50%; transform: translateY(-50%); width: 3px; height: 60%; background: var(--accent); border-radius: 0 2px 2px 0; }

        /* Card */
        .card { border-radius: 16px; border: 1px solid var(--border-color); box-shadow: 0 1px 4px rgba(0,0,0,0.05), 0 4px 16px rgba(0,0,0,0.04); overflow: hidden; }
        .card-header { background: #fff; border-bottom: 1px solid var(--border-color); font-weight: 600; padding: 16px 24px; font-size: 0.95rem; }
        .card-body { padding: 24px; }

        /* Upload Zone */
        .upload-zone { border: 2px dashed var(--border-color); border-radius: 12px; padding: 48px 32px; text-align: center; transition: all 0.3s; cursor: pointer; background: var(--bg-page); }
        .upload-zone:hover { border-color: var(--accent); background: var(--accent-light); }
        .upload-zone.dragover { border-color: var(--accent); background: var(--accent-dim); }
        .upload-icon { font-size: 3rem; color: var(--text-secondary); margin-bottom: 12px; }
        .upload-zone h5 { color: var(--text-primary); font-weight: 600; margin-bottom: 6px; }
        .upload-zone p { color: var(--text-secondary); font-size: 0.875rem; margin: 0; }

        /* Buttons */
        .btn-primary { background: var(--accent); border: none; border-radius: 10px; padding: 10px 24px; font-weight: 600; font-size: 0.88rem; transition: all 0.2s; color: #fff; }
        .btn-primary:hover { background: #dc2626; transform: translateY(-1px); box-shadow: 0 4px 12px rgba(239,68,68,0.3); }
        .btn-outline-secondary { border-radius: 10px; border: 1.5px solid var(--border-color); color: var(--text-secondary); font-weight: 500; }
        .btn-outline-secondary:hover { background: var(--bg-page); border-color: #cbd5e1; }

        /* Forms */
        .form-label { font-weight: 500; color: var(--text-primary); font-size: 0.85rem; }
        .form-select, .form-control { border-radius: 10px; border: 1.5px solid var(--border-color); padding: 10px 14px; font-size: 0.9rem; transition: border-color 0.2s, box-shadow 0.2s; background: #fff; color: var(--text-primary); }
        .form-select:focus, .form-control:focus { border-color: var(--accent); box-shadow: 0 0 0 3px var(--accent-dim); outline: none; }

        /* Table */
        .table { border-collapse: separate; border-spacing: 0; width: 100%; font-size: 0.875rem; }
        .table thead th { background: #f8fafc; color: var(--text-secondary); font-weight: 600; font-size: 0.78rem; text-transform: uppercase; letter-spacing: 0.5px; padding: 12px 14px; border-bottom: 2px solid var(--border-color); white-space: nowrap; }
        .table tbody td { padding: 13px 14px; border-bottom: 1px solid var(--border-color); vertical-align: middle; color: var(--text-primary); }
        .table tbody tr:hover { background: #f8fafc; }
        .table-responsive { border-radius: 10px; overflow: hidden; border: 1px solid var(--border-color); }

        /* Badges */
        .badge { border-radius: 50px; font-weight: 600; font-size: 0.72rem; padding: 4px 10px; letter-spacing: 0.3px; }
        .badge-success { background: rgba(34, 197, 94, 0.12); color: #15803d; }
        .badge-warning { background: rgba(245, 158, 11, 0.12); color: #b45309; }
        .badge-danger { background: rgba(239, 68, 68, 0.12); color: #b91c1c; }
    </style>
"""

# ─────────────────────────────────────────────
# CSS for data-Upload-Tyre.html  (accent: #f59e0b amber)
# ─────────────────────────────────────────────
CSS_UPLOAD_TYRE = r"""
    <style>
        * { font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif; box-sizing: border-box; }
        :root {
            --navy: #0f172a;
            --navy-mid: #1e293b;
            --accent: #f59e0b;
            --accent-dim: rgba(245, 158, 11, 0.15);
            --accent-light: rgba(245, 158, 11, 0.08);
            --bg-page: #F8FAFC;
            --bg-card: #ffffff;
            --border-color: #e2e8f0;
            --text-primary: #1e293b;
            --text-secondary: #64748b;
        }
        body { background: var(--bg-page); min-height: 100vh; color: var(--text-primary); }

        /* Navbar */
        .navbar { background: rgba(15, 23, 42, 0.97); backdrop-filter: blur(12px); border-bottom: 1px solid rgba(255,255,255,0.06); padding: 0.7rem 0; box-shadow: 0 1px 20px rgba(0,0,0,0.3); }
        .navbar-brand { font-weight: 700; font-size: 1.4rem; color: #fff !important; display: flex; align-items: center; gap: 10px; }
        .navbar-brand i { color: var(--accent); font-size: 1.8rem; }
        .navbar-nav { display: flex; align-items: center; gap: 2px; flex-wrap: wrap; }
        .nav-group { display: flex; align-items: center; gap: 2px; }
        .nav-group-label { font-size: 0.6rem; font-weight: 700; text-transform: uppercase; letter-spacing: 1.2px; color: rgba(255,255,255,0.25); padding: 0 10px 0 6px; margin-right: 2px; border-right: 1px solid rgba(255,255,255,0.12); white-space: nowrap; }
        .nav-link { color: rgba(255,255,255,0.6) !important; font-weight: 500; font-size: 0.82rem; padding: 0.45rem 0.7rem !important; border-radius: 8px; transition: all 0.2s; display: flex; align-items: center; gap: 5px; white-space: nowrap; }
        .nav-link i { font-size: 0.95rem; opacity: 0.7; }
        .nav-link:hover, .nav-link.active { color: #fff !important; background: var(--accent-dim); }
        .nav-link.active::before { content: ''; position: absolute; left: 0; top: 50%; transform: translateY(-50%); width: 3px; height: 60%; background: var(--accent); border-radius: 0 2px 2px 0; }

        /* Card */
        .card { border-radius: 16px; border: 1px solid var(--border-color); box-shadow: 0 1px 4px rgba(0,0,0,0.05), 0 4px 16px rgba(0,0,0,0.04); overflow: hidden; }
        .card-header { background: #fff; border-bottom: 1px solid var(--border-color); font-weight: 600; padding: 16px 24px; font-size: 0.95rem; }
        .card-body { padding: 24px; }

        /* Upload Zone */
        .upload-zone { border: 2px dashed var(--border-color); border-radius: 12px; padding: 48px 32px; text-align: center; transition: all 0.3s; cursor: pointer; background: var(--bg-page); }
        .upload-zone:hover { border-color: var(--accent); background: var(--accent-light); }
        .upload-zone.dragover { border-color: var(--accent); background: var(--accent-dim); }
        .upload-icon { font-size: 3rem; color: var(--text-secondary); margin-bottom: 12px; }
        .upload-zone h5 { color: var(--text-primary); font-weight: 600; margin-bottom: 6px; }
        .upload-zone p { color: var(--text-secondary); font-size: 0.875rem; margin: 0; }

        /* Buttons */
        .btn-primary { background: var(--accent); border: none; border-radius: 10px; padding: 10px 24px; font-weight: 600; font-size: 0.88rem; transition: all 0.2s; color: #fff; }
        .btn-primary:hover { background: #d97706; transform: translateY(-1px); box-shadow: 0 4px 12px rgba(245,158,11,0.3); }
        .btn-outline-secondary { border-radius: 10px; border: 1.5px solid var(--border-color); color: var(--text-secondary); font-weight: 500; }
        .btn-outline-secondary:hover { background: var(--bg-page); border-color: #cbd5e1; }

        /* Forms */
        .form-label { font-weight: 500; color: var(--text-primary); font-size: 0.85rem; }
        .form-select, .form-control { border-radius: 10px; border: 1.5px solid var(--border-color); padding: 10px 14px; font-size: 0.9rem; transition: border-color 0.2s, box-shadow 0.2s; background: #fff; color: var(--text-primary); }
        .form-select:focus, .form-control:focus { border-color: var(--accent); box-shadow: 0 0 0 3px var(--accent-dim); outline: none; }

        /* Table */
        .table { border-collapse: separate; border-spacing: 0; width: 100%; font-size: 0.875rem; }
        .table thead th { background: #f8fafc; color: var(--text-secondary); font-weight: 600; font-size: 0.78rem; text-transform: uppercase; letter-spacing: 0.5px; padding: 12px 14px; border-bottom: 2px solid var(--border-color); white-space: nowrap; }
        .table tbody td { padding: 13px 14px; border-bottom: 1px solid var(--border-color); vertical-align: middle; color: var(--text-primary); }
        .table tbody tr:hover { background: #f8fafc; }
        .table-responsive { border-radius: 10px; overflow: hidden; border: 1px solid var(--border-color); }

        /* Badges */
        .badge { border-radius: 50px; font-weight: 600; font-size: 0.72rem; padding: 4px 10px; letter-spacing: 0.3px; }
        .badge-success { background: rgba(34, 197, 94, 0.12); color: #15803d; }
        .badge-warning { background: rgba(245, 158, 11, 0.12); color: #b45309; }
    </style>
"""


def get_nav_html():
    return r'''
    <!-- Navigation -->
    <nav class="navbar navbar-expand-lg sticky-top">
        <div class="container">
            <a class="navbar-brand" href="/">
                <i class="bi bi-truck"></i>
                LFN
            </a>
            <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
                <span class="navbar-toggler-icon"></span>
            </button>
            <div class="collapse navbar-collapse" id="navbarNav">
                <ul class="navbar-nav ms-auto flex-wrap">
                    <li class="nav-item">
                        <a class="nav-link" href="/"><i class="bi bi-house"></i> Home</a>
                    </li>
                    <li class="nav-group">
                        <span class="nav-group-label">Operasi</span>
                        <a class="nav-link" href="data-Upload-Fuel.html"><i class="bi bi-fuel-pump"></i> Fuel</a>
                        <a class="nav-link" href="rm.html"><i class="bi bi-tools"></i> R&M</a>
                        <a class="nav-link" href="fleet.html"><i class="bi bi-car-front"></i> Fleet</a>
                        <a class="nav-link" href="data-Upload-Tyre.html"><i class="bi bi-disc"></i> Tyre</a>
                        <a class="nav-link" href="inventory.html"><i class="bi bi-boxes"></i> Inventory</a>
                    </li>
                    <li class="nav-group">
                        <span class="nav-group-label">Procurement</span>
                        <a class="nav-link" href="purchase-request.html"><i class="bi bi-file-text"></i> PR</a>
                        <a class="nav-link" href="purchase-order.html"><i class="bi bi-file-earmark"></i> PO</a>
                        <a class="nav-link" href="good-receipt.html"><i class="bi bi-box-seam"></i> GR</a>
                        <a class="nav-link" href="good-issue.html"><i class="bi bi-box-arrow-right"></i> GI</a>
                        <a class="nav-link" href="vendor.html"><i class="bi bi-building"></i> Vendor</a>
                    </li>
                    <li class="nav-group">
                        <span class="nav-group-label">Keuangan</span>
                        <a class="nav-link" href="finance.html"><i class="bi bi-wallet2"></i> Finance</a>
                        <a class="nav-link" href="hr.html"><i class="bi bi-people"></i> HR</a>
                    </li>
                </ul>
            </div>
        </div>
    </nav>
'''


def replace_style_and_nav(filepath, new_css, page_active=None):
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()

    # Replace <style> block
    style_pattern = re.compile(r'<style>.*?</style>', re.DOTALL)
    content = style_pattern.sub(new_css.strip(), content, count=1)

    # Add Google Fonts if missing
    if 'fonts.googleapis.com' not in content:
        gf_link = '<link href="https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700&display=swap" rel="stylesheet">'
        # Insert before the closing </head>
        content = content.replace('</head>', gf_link + '\n    </head>')

    # Replace navbar
    nav_pattern = re.compile(r'<nav class="navbar[^>]*>.*?</nav>', re.DOTALL)
    content = nav_pattern.sub(get_nav_html().strip(), content, count=1)

    # Add Bootstrap JS before </body>
    if 'bootstrap.bundle.min.js' not in content:
        content = content.replace('</body>', '<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>\n</body>')

    with open(filepath, 'w', encoding='utf-8') as f:
        f.write(content)
    print(f"  Updated: {os.path.basename(filepath)}")


def main():
    print("Updating datalist-fuel.html...")
    replace_style_and_nav(
        os.path.join(BASE, 'datalist-fuel.html'),
        CSS_FUEL_LIST
    )

    print("Updating datalist- tyre.html...")
    replace_style_and_nav(
        os.path.join(BASE, 'datalist- tyre.html'),
        CSS_TYRE_LIST
    )

    print("Updating data-Upload-Fuel.html...")
    replace_style_and_nav(
        os.path.join(BASE, 'data-Upload-Fuel.html'),
        CSS_UPLOAD_FUEL
    )

    print("Updating data-Upload-Tyre.html...")
    replace_style_and_nav(
        os.path.join(BASE, 'data-Upload-Tyre.html'),
        CSS_UPLOAD_TYRE
    )

    print("\nAll files updated!")


if __name__ == '__main__':
    main()
