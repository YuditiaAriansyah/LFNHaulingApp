#!/usr/bin/env python3
import re, os

# CSS templates for each file - accent color varies per module
CSS_TEMPLATES = {
    "fleet.html": """    <style>
        :root {
            --primary-color: #1e3a5f;
            --secondary-color: #3b82f6;
            --success-color: #10b981;
            --warning-color: #f59e0b;
            --danger-color: #ef4444;
            --bg-page: #F8FAFC;
            --bg-card: #ffffff;
            --border-color: #e2e8f0;
            --text-primary: #1e293b;
            --text-secondary: #64748b;
            --accent-color: #3b82f6;
            --accent-light: #eff6ff;
            --navy: #1e3a5f;
        }
        .navbar { background: var(--navy); padding: 0.75rem 0; border-bottom: 1px solid rgba(255,255,255,0.08); box-shadow: 0 1px 3px rgba(0,0,0,0.15); }
        .navbar-brand { font-weight: 700; font-size: 1.35rem; color: #fff !important; display: flex; align-items: center; gap: 8px; }
        .navbar-brand i { color: #38bdf8; font-size: 1.6rem; }
        .nav-group { display: flex; align-items: center; gap: 2px; }
        .nav-group-label { font-size: 0.65rem; font-weight: 700; text-transform: uppercase; letter-spacing: 1px; color: rgba(255,255,255,0.35); padding: 0 8px; margin-right: 4px; border-right: 1px solid rgba(255,255,255,0.15); }
        .nav-link { color: rgba(255,255,255,0.65) !important; font-weight: 500; font-size: 0.875rem; padding: 0.5rem 0.75rem !important; border-radius: 8px; transition: all 0.2s ease; display: flex; align-items: center; gap: 6px; position: relative; }
        .nav-link i { font-size: 1rem; opacity: 0.8; }
        .nav-link:hover, .nav-link.active { color: #fff !important; background: rgba(59,130,246,0.25); }
        .nav-link.active::before { content: ''; position: absolute; left: 0; top: 50%; transform: translateY(-50%); width: 3px; height: 60%; background: #38bdf8; border-radius: 0 2px 2px 0; }
        body { background-color: var(--bg-page); min-height: 100vh; padding-top: 76px; font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif; color: var(--text-primary); line-height: 1.6; }
        .main-container { background: var(--bg-card); border-radius: 16px; box-shadow: 0 1px 4px rgba(0,0,0,0.06), 0 4px 12px rgba(0,0,0,0.04); border: 1px solid var(--border-color); padding: 28px 32px; margin-top: 20px; }
        .page-header { background: linear-gradient(135deg, #1e3a5f 0%, #2563eb 100%); color: white; padding: 24px 28px; border-radius: 12px; margin-bottom: 28px; box-shadow: 0 4px 12px rgba(30,58,95,0.3); }
        .page-header h1 { margin: 0 0 4px 0; font-size: 1.5rem; font-weight: 700; letter-spacing: -0.01em; }
        .page-header p { margin: 0; opacity: 0.75; font-size: 0.875rem; }
        .stats-card { background: var(--bg-card); border-radius: 12px; padding: 20px 16px; margin-bottom: 16px; box-shadow: 0 1px 3px rgba(0,0,0,0.06), 0 2px 8px rgba(0,0,0,0.04); border: 1px solid var(--border-color); text-align: center; transition: transform 0.2s ease, box-shadow 0.2s ease; }
        .stats-card:hover { transform: translateY(-2px); box-shadow: 0 4px 12px rgba(0,0,0,0.1); }
        .stats-card h6 { color: var(--text-secondary); margin-bottom: 10px; font-weight: 600; font-size: 0.7rem; text-transform: uppercase; letter-spacing: 0.5px; }
        .stats-card .number { font-size: 1.625rem; font-weight: 700; line-height: 1.2; }
        .stats-card.unit { border-top: 3px solid var(--secondary-color); }
        .stats-card.unit .number { color: var(--secondary-color); }
        .stats-card.hm { border-top: 3px solid var(--success-color); }
        .stats-card.hm .number { color: var(--success-color); }
        .stats-card.km { border-top: 3px solid var(--accent-color); }
        .stats-card.km .number { color: var(--accent-color); }
        .stats-card.fuel { border-top: 3px solid var(--warning-color); }
        .stats-card.fuel .number { color: var(--warning-color); }
        .stats-card.ratio { border-top: 3px solid var(--danger-color); }
        .stats-card.ratio .number { color: var(--danger-color); }
        .summary-section { background: var(--bg-page); padding: 20px; border-radius: 12px; margin-bottom: 24px; border: 1px solid var(--border-color); }
        .summary-section h5 { color: var(--text-primary); margin-bottom: 16px; font-weight: 600; font-size: 0.95rem; }
        .summary-table { margin-bottom: 0; }
        .summary-table th { background: var(--bg-page); color: var(--text-secondary); font-size: 0.72rem; font-weight: 600; text-transform: uppercase; letter-spacing: 0.5px; border-bottom: 2px solid var(--border-color) !important; }
        .summary-table td, .summary-table th { padding: 11px 14px; vertical-align: middle; font-size: 0.85rem; }
        .summary-table .site-name { font-weight: 600; color: var(--text-primary); }
        .summary-table .overall { background: var(--accent-light) !important; }
        .summary-table .overall td { font-weight: 700; color: var(--accent-color); }
        .filter-section { background: var(--bg-page); padding: 20px 24px; border-radius: 12px; margin-bottom: 24px; border: 1px solid var(--border-color); border-left: 4px solid var(--accent-color); }
        .form-label { font-weight: 600; color: var(--text-secondary); font-size: 0.8rem; text-transform: uppercase; letter-spacing: 0.4px; margin-bottom: 6px; }
        .form-select, .form-control { border-radius: 10px; border: 1.5px solid var(--border-color); padding: 10px 14px; font-size: 0.875rem; background: #fff; color: var(--text-primary); transition: border-color 0.2s ease, box-shadow 0.2s ease; }
        .form-select:focus, .form-control:focus { border-color: var(--accent-color); box-shadow: 0 0 0 3px rgba(59,130,246,0.1); outline: none; }
        .btn { border-radius: 10px; padding: 9px 22px; font-weight: 600; font-size: 0.875rem; transition: all 0.2s ease; border: none; display: inline-flex; align-items: center; gap: 6px; }
        .btn-primary { background: var(--accent-color); color: white; }
        .btn-primary:hover { background: #2563eb; transform: translateY(-1px); box-shadow: 0 4px 12px rgba(59,130,246,0.35); }
        .btn-success { background: var(--success-color); color: white; }
        .btn-success:hover { background: #059669; transform: translateY(-1px); box-shadow: 0 4px 12px rgba(16,185,129,0.35); }
        .btn-danger { background: var(--danger-color); color: white; }
        .btn-danger:hover { background: #dc2626; transform: translateY(-1px); box-shadow: 0 4px 12px rgba(239,68,68,0.35); }
        .btn-warning { background: var(--warning-color); color: white; }
        .table-responsive { border-radius: 12px; overflow: hidden; }
        .table { margin-bottom: 0; font-size: 0.875rem; }
        .table thead th { background: var(--bg-page); color: var(--text-secondary); font-weight: 600; font-size: 0.72rem; text-transform: uppercase; letter-spacing: 0.5px; border-bottom: 2px solid var(--border-color) !important; padding: 12px 16px; white-space: nowrap; }
        .table tbody tr { transition: background-color 0.15s ease; border-bottom: 1px solid var(--border-color); }
        .table tbody tr:last-child { border-bottom: none; }
        .table tbody tr:hover { background-color: #f8fafc; }
        .table tbody td { padding: 13px 16px; vertical-align: middle; color: var(--text-primary); }
        .badge { padding: 4px 12px; border-radius: 50px; font-weight: 600; font-size: 0.7rem; letter-spacing: 0.3px; text-transform: uppercase; }
        .badge-active { background: #d1fae5; color: #065f46; }
        .badge-inactive { background: #fee2e2; color: #991b1b; }
        .badge-pending { background: #fef3c7; color: #92400e; }
        .badge-approved { background: #d1fae5; color: #065f46; }
        .badge-rejected { background: #fee2e2; color: #991b1b; }
        .badge-open { background: #dbeafe; color: #1e40af; }
        .badge-completed { background: #d1fae5; color: #065f46; }
        .badge-draft { background: #f1f5f9; color: #475569; }
        .badge-posted { background: #d1fae5; color: #065f46; }
        .nav-tabs { border-bottom: 2px solid var(--border-color); margin-bottom: 24px; gap: 4px; }
        .nav-tabs .nav-link { border: none; color: var(--text-secondary); font-weight: 500; font-size: 0.875rem; padding: 10px 20px; border-radius: 8px 8px 0 0; background: transparent; margin-right: 0; border-bottom: 2px solid transparent; margin-bottom: -2px; transition: all 0.2s ease; }
        .nav-tabs .nav-link:hover { color: var(--accent-color); background: var(--accent-light); }
        .nav-tabs .nav-link.active { color: var(--accent-color); background: transparent; border-bottom-color: var(--accent-color); font-weight: 600; }
        .tab-content { display: none; }
        .tab-content.active { display: block; }
        .action-buttons .btn { padding: 5px 10px; font-size: 0.75rem; border-radius: 8px; }
        .modal-header { background: var(--navy); color: white; border-radius: 12px 12px 0 0; padding: 20px 24px; }
        .modal-header .btn-close { filter: invert(1); }
        .upload-zone { border: 2px dashed var(--border-color); border-radius: 12px; padding: 40px; text-align: center; background: var(--bg-page); transition: all 0.2s ease; }
        .upload-zone:hover { border-color: var(--accent-color); background: var(--accent-light); }
        .upload-zone i { font-size: 48px; color: var(--text-secondary); margin-bottom: 16px; }
        .vehicle-icon { width: 38px; height: 38px; border-radius: 10px; display: flex; align-items: center; justify-content: center; background: var(--accent-light); color: var(--accent-color); font-size: 1.1rem; }
    </style>""",

    "rm.html": """    <style>
        :root {
            --primary-color: #1e3a5f;
            --secondary-color: #0f766e;
            --success-color: #10b981;
            --warning-color: #f59e0b;
            --danger-color: #ef4444;
            --bg-page: #F8FAFC;
            --bg-card: #ffffff;
            --border-color: #e2e8f0;
            --text-primary: #1e293b;
            --text-secondary: #64748b;
            --accent-color: #0f766e;
            --accent-light: #f0fdfa;
            --navy: #1e3a5f;
        }
        .navbar { background: var(--navy); padding: 0.75rem 0; border-bottom: 1px solid rgba(255,255,255,0.08); box-shadow: 0 1px 3px rgba(0,0,0,0.15); }
        .navbar-brand { font-weight: 700; font-size: 1.35rem; color: #fff !important; display: flex; align-items: center; gap: 8px; }
        .navbar-brand i { color: #38bdf8; font-size: 1.6rem; }
        .nav-group { display: flex; align-items: center; gap: 2px; }
        .nav-group-label { font-size: 0.65rem; font-weight: 700; text-transform: uppercase; letter-spacing: 1px; color: rgba(255,255,255,0.35); padding: 0 8px; margin-right: 4px; border-right: 1px solid rgba(255,255,255,0.15); }
        .nav-link { color: rgba(255,255,255,0.65) !important; font-weight: 500; font-size: 0.875rem; padding: 0.5rem 0.75rem !important; border-radius: 8px; transition: all 0.2s ease; display: flex; align-items: center; gap: 6px; position: relative; }
        .nav-link i { font-size: 1rem; opacity: 0.8; }
        .nav-link:hover, .nav-link.active { color: #fff !important; background: rgba(15,118,110,0.25); }
        .nav-link.active::before { content: ''; position: absolute; left: 0; top: 50%; transform: translateY(-50%); width: 3px; height: 60%; background: #38bdf8; border-radius: 0 2px 2px 0; }
        body { background-color: var(--bg-page); min-height: 100vh; padding-top: 76px; font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif; color: var(--text-primary); line-height: 1.6; }
        .main-container { background: var(--bg-card); border-radius: 16px; box-shadow: 0 1px 4px rgba(0,0,0,0.06), 0 4px 12px rgba(0,0,0,0.04); border: 1px solid var(--border-color); padding: 28px 32px; margin-top: 20px; }
        .page-header { background: linear-gradient(135deg, #1e3a5f 0%, #0f766e 100%); color: white; padding: 24px 28px; border-radius: 12px; margin-bottom: 28px; box-shadow: 0 4px 12px rgba(15,118,110,0.3); }
        .page-header h1 { margin: 0 0 4px 0; font-size: 1.5rem; font-weight: 700; letter-spacing: -0.01em; }
        .page-header p { margin: 0; opacity: 0.75; font-size: 0.875rem; }
        .stats-card { background: var(--bg-card); border-radius: 12px; padding: 20px 16px; margin-bottom: 16px; box-shadow: 0 1px 3px rgba(0,0,0,0.06), 0 2px 8px rgba(0,0,0,0.04); border: 1px solid var(--border-color); text-align: center; transition: transform 0.2s ease, box-shadow 0.2s ease; }
        .stats-card:hover { transform: translateY(-2px); box-shadow: 0 4px 12px rgba(0,0,0,0.1); }
        .stats-card h6 { color: var(--text-secondary); margin-bottom: 10px; font-weight: 600; font-size: 0.7rem; text-transform: uppercase; letter-spacing: 0.5px; }
        .stats-card .number { font-size: 1.625rem; font-weight: 700; line-height: 1.2; }
        .stats-card.wo { border-top: 3px solid var(--secondary-color); }
        .stats-card.wo .number { color: var(--secondary-color); }
        .stats-card.pm { border-top: 3px solid var(--success-color); }
        .stats-card.pm .number { color: var(--success-color); }
        .stats-card.cm { border-top: 3px solid var(--danger-color); }
        .stats-card.cm .number { color: var(--danger-color); }
        .stats-card.downtime { border-top: 3px solid var(--warning-color); }
        .stats-card.downtime .number { color: var(--warning-color); }
        .stats-card.cost { border-top: 3px solid #7c3aed; }
        .stats-card.cost .number { color: #7c3aed; }
        .filter-section { background: var(--bg-page); padding: 20px 24px; border-radius: 12px; margin-bottom: 24px; border: 1px solid var(--border-color); border-left: 4px solid var(--secondary-color); }
        .form-label { font-weight: 600; color: var(--text-secondary); font-size: 0.8rem; text-transform: uppercase; letter-spacing: 0.4px; margin-bottom: 6px; }
        .form-select, .form-control { border-radius: 10px; border: 1.5px solid var(--border-color); padding: 10px 14px; font-size: 0.875rem; background: #fff; color: var(--text-primary); transition: border-color 0.2s ease, box-shadow 0.2s ease; }
        .form-select:focus, .form-control:focus { border-color: var(--secondary-color); box-shadow: 0 0 0 3px rgba(15,118,110,0.1); outline: none; }
        .btn { border-radius: 10px; padding: 9px 22px; font-weight: 600; font-size: 0.875rem; transition: all 0.2s ease; border: none; display: inline-flex; align-items: center; gap: 6px; }
        .btn-primary { background: var(--secondary-color); color: white; }
        .btn-primary:hover { background: #0d5e56; transform: translateY(-1px); box-shadow: 0 4px 12px rgba(15,118,110,0.35); }
        .btn-success { background: var(--success-color); color: white; }
        .btn-success:hover { background: #059669; transform: translateY(-1px); box-shadow: 0 4px 12px rgba(16,185,129,0.35); }
        .btn-danger { background: var(--danger-color); color: white; }
        .btn-danger:hover { background: #dc2626; transform: translateY(-1px); box-shadow: 0 4px 12px rgba(239,68,68,0.35); }
        .btn-warning { background: var(--warning-color); color: white; }
        .table-responsive { border-radius: 12px; overflow: hidden; }
        .table { margin-bottom: 0; font-size: 0.875rem; }
        .table thead th { background: var(--bg-page); color: var(--text-secondary); font-weight: 600; font-size: 0.72rem; text-transform: uppercase; letter-spacing: 0.5px; border-bottom: 2px solid var(--border-color) !important; padding: 12px 16px; white-space: nowrap; }
        .table tbody tr { transition: background-color 0.15s ease; border-bottom: 1px solid var(--border-color); }
        .table tbody tr:last-child { border-bottom: none; }
        .table tbody tr:hover { background-color: #f8fafc; }
        .table tbody td { padding: 13px 16px; vertical-align: middle; color: var(--text-primary); }
        .badge { padding: 4px 12px; border-radius: 50px; font-weight: 600; font-size: 0.7rem; letter-spacing: 0.3px; text-transform: uppercase; }
        .badge-open { background: #dbeafe; color: #1e40af; }
        .badge-inprogress { background: #fef3c7; color: #92400e; }
        .badge-completed { background: #d1fae5; color: #065f46; }
        .badge-cancelled { background: #f1f5f9; color: #475569; }
        .badge-scheduled { background: #dbeafe; color: #1e40af; }
        .badge-overdue { background: #fee2e2; color: #991b1b; }
        .badge-reported { background: #f3e8ff; color: #6b21a8; }
        .badge-low { background: #f1f5f9; color: #475569; }
        .badge-medium { background: #fef3c7; color: #92400e; }
        .badge-high { background: #fed7aa; color: #9a3412; }
        .badge-critical { background: #fee2e2; color: #991b1b; }
        .badge-breakdown { background: #fee2e2; color: #991b1b; }
        .badge-corrective { background: #f3e8ff; color: #6b21a8; }
        .badge-emergency { background: #fee2e2; color: #991b1b; animation: pulse 1.5s infinite; }
        @keyframes pulse { 0%, 100% { opacity: 1; } 50% { opacity: 0.6; } }
        .nav-tabs { border-bottom: 2px solid var(--border-color); margin-bottom: 24px; gap: 4px; }
        .nav-tabs .nav-link { border: none; color: var(--text-secondary); font-weight: 500; font-size: 0.875rem; padding: 10px 20px; border-radius: 8px 8px 0 0; background: transparent; margin-right: 0; border-bottom: 2px solid transparent; margin-bottom: -2px; transition: all 0.2s ease; }
        .nav-tabs .nav-link:hover { color: var(--secondary-color); background: var(--accent-light); }
        .nav-tabs .nav-link.active { color: var(--secondary-color); background: transparent; border-bottom-color: var(--secondary-color); font-weight: 600; }
        .tab-content { display: none; }
        .tab-content.active { display: block; }
        .action-buttons .btn { padding: 5px 10px; font-size: 0.75rem; border-radius: 8px; }
        .modal-header { background: var(--secondary-color); color: white; border-radius: 12px 12px 0 0; padding: 20px 24px; }
        .modal-header .btn-close { filter: invert(1); }
        .form-section { background: var(--bg-page); padding: 20px 24px; border-radius: 12px; margin-bottom: 20px; border: 1px solid var(--border-color); }
        .form-section h6 { color: var(--text-primary); margin-bottom: 16px; font-weight: 600; }
        .required { color: var(--danger-color); }
    </style>""",

    "hr.html": """    <style>
        :root {
            --primary-color: #1e3a5f;
            --secondary-color: #7c3aed;
            --success-color: #10b981;
            --warning-color: #f59e0b;
            --danger-color: #ef4444;
            --bg-page: #F8FAFC;
            --bg-card: #ffffff;
            --border-color: #e2e8f0;
            --text-primary: #1e293b;
            --text-secondary: #64748b;
            --accent-color: #7c3aed;
            --accent-light: #faf5ff;
            --navy: #1e3a5f;
        }
        .navbar { background: var(--navy); padding: 0.75rem 0; border-bottom: 1px solid rgba(255,255,255,0.08); box-shadow: 0 1px 3px rgba(0,0,0,0.15); }
        .navbar-brand { font-weight: 700; font-size: 1.35rem; color: #fff !important; display: flex; align-items: center; gap: 8px; }
        .navbar-brand i { color: #38bdf8; font-size: 1.6rem; }
        .nav-group { display: flex; align-items: center; gap: 2px; }
        .nav-group-label { font-size: 0.65rem; font-weight: 700; text-transform: uppercase; letter-spacing: 1px; color: rgba(255,255,255,0.35); padding: 0 8px; margin-right: 4px; border-right: 1px solid rgba(255,255,255,0.15); }
        .nav-link { color: rgba(255,255,255,0.65) !important; font-weight: 500; font-size: 0.875rem; padding: 0.5rem 0.75rem !important; border-radius: 8px; transition: all 0.2s ease; display: flex; align-items: center; gap: 6px; position: relative; }
        .nav-link i { font-size: 1rem; opacity: 0.8; }
        .nav-link:hover, .nav-link.active { color: #fff !important; background: rgba(124,58,237,0.25); }
        .nav-link.active::before { content: ''; position: absolute; left: 0; top: 50%; transform: translateY(-50%); width: 3px; height: 60%; background: #38bdf8; border-radius: 0 2px 2px 0; }
        body { background-color: var(--bg-page); min-height: 100vh; padding-top: 76px; font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif; color: var(--text-primary); line-height: 1.6; }
        .main-container { background: var(--bg-card); border-radius: 16px; box-shadow: 0 1px 4px rgba(0,0,0,0.06), 0 4px 12px rgba(0,0,0,0.04); border: 1px solid var(--border-color); padding: 28px 32px; margin-top: 20px; }
        .page-header { background: linear-gradient(135deg, #1e3a5f 0%, #7c3aed 100%); color: white; padding: 24px 28px; border-radius: 12px; margin-bottom: 28px; box-shadow: 0 4px 12px rgba(124,58,237,0.3); }
        .page-header h1 { margin: 0 0 4px 0; font-size: 1.5rem; font-weight: 700; letter-spacing: -0.01em; }
        .page-header p { margin: 0; opacity: 0.75; font-size: 0.875rem; }
        .stats-card { background: var(--bg-card); border-radius: 12px; padding: 20px 16px; margin-bottom: 16px; box-shadow: 0 1px 3px rgba(0,0,0,0.06), 0 2px 8px rgba(0,0,0,0.04); border: 1px solid var(--border-color); text-align: center; transition: transform 0.2s ease, box-shadow 0.2s ease; }
        .stats-card:hover { transform: translateY(-2px); box-shadow: 0 4px 12px rgba(0,0,0,0.1); }
        .stats-card h6 { color: var(--text-secondary); margin-bottom: 10px; font-weight: 600; font-size: 0.7rem; text-transform: uppercase; letter-spacing: 0.5px; }
        .stats-card .number { font-size: 1.625rem; font-weight: 700; line-height: 1.2; }
        .stats-card.employee { border-top: 3px solid var(--secondary-color); }
        .stats-card.employee .number { color: var(--secondary-color); }
        .stats-card.active { border-top: 3px solid var(--success-color); }
        .stats-card.active .number { color: var(--success-color); }
        .stats-card.present { border-top: 3px solid #0891b2; }
        .stats-card.present .number { color: #0891b2; }
        .stats-card.absent { border-top: 3px solid var(--danger-color); }
        .stats-card.absent .number { color: var(--danger-color); }
        .stats-card.payroll { border-top: 3px solid var(--accent-color); }
        .stats-card.payroll .number { color: var(--accent-color); }
        .filter-section { background: var(--bg-page); padding: 20px 24px; border-radius: 12px; margin-bottom: 24px; border: 1px solid var(--border-color); border-left: 4px solid var(--accent-color); }
        .form-label { font-weight: 600; color: var(--text-secondary); font-size: 0.8rem; text-transform: uppercase; letter-spacing: 0.4px; margin-bottom: 6px; }
        .form-select, .form-control { border-radius: 10px; border: 1.5px solid var(--border-color); padding: 10px 14px; font-size: 0.875rem; background: #fff; color: var(--text-primary); transition: border-color 0.2s ease, box-shadow 0.2s ease; }
        .form-select:focus, .form-control:focus { border-color: var(--accent-color); box-shadow: 0 0 0 3px rgba(124,58,237,0.1); outline: none; }
        .btn { border-radius: 10px; padding: 9px 22px; font-weight: 600; font-size: 0.875rem; transition: all 0.2s ease; border: none; display: inline-flex; align-items: center; gap: 6px; }
        .btn-primary { background: var(--accent-color); color: white; }
        .btn-primary:hover { background: #6d28d9; transform: translateY(-1px); box-shadow: 0 4px 12px rgba(124,58,237,0.35); }
        .btn-success { background: var(--success-color); color: white; }
        .btn-success:hover { background: #059669; transform: translateY(-1px); box-shadow: 0 4px 12px rgba(16,185,129,0.35); }
        .btn-danger { background: var(--danger-color); color: white; }
        .btn-danger:hover { background: #dc2626; transform: translateY(-1px); box-shadow: 0 4px 12px rgba(239,68,68,0.35); }
        .btn-warning { background: var(--warning-color); color: white; }
        .btn-purple { background: var(--accent-color); color: white; }
        .table-responsive { border-radius: 12px; overflow: hidden; }
        .table { margin-bottom: 0; font-size: 0.875rem; }
        .table thead th { background: var(--bg-page); color: var(--text-secondary); font-weight: 600; font-size: 0.72rem; text-transform: uppercase; letter-spacing: 0.5px; border-bottom: 2px solid var(--border-color) !important; padding: 12px 16px; white-space: nowrap; }
        .table tbody tr { transition: background-color 0.15s ease; border-bottom: 1px solid var(--border-color); }
        .table tbody tr:last-child { border-bottom: none; }
        .table tbody tr:hover { background-color: #f8fafc; }
        .table tbody td { padding: 13px 16px; vertical-align: middle; color: var(--text-primary); }
        .badge { padding: 4px 12px; border-radius: 50px; font-weight: 600; font-size: 0.7rem; letter-spacing: 0.3px; text-transform: uppercase; }
        .badge-active { background: #d1fae5; color: #065f46; }
        .badge-inactive { background: #f1f5f9; color: #475569; }
        .badge-resigned { background: #fef3c7; color: #92400e; }
        .badge-terminated { background: #fee2e2; color: #991b1b; }
        .badge-present { background: #d1fae5; color: #065f46; }
        .badge-absent { background: #fee2e2; color: #991b1b; }
        .badge-late { background: #fef3c7; color: #92400e; }
        .badge-sick { background: #fed7aa; color: #9a3412; }
        .badge-leave { background: #dbeafe; color: #1e40af; }
        .badge-permit { background: #f3e8ff; color: #6b21a8; }
        .badge-draft { background: #f1f5f9; color: #475569; }
        .badge-calculated { background: #dbeafe; color: #1e40af; }
        .badge-approved { background: #d1fae5; color: #065f46; }
        .badge-paid { background: #d1fae5; color: #065f46; }
        .nav-tabs { border-bottom: 2px solid var(--border-color); margin-bottom: 24px; gap: 4px; }
        .nav-tabs .nav-link { border: none; color: var(--text-secondary); font-weight: 500; font-size: 0.875rem; padding: 10px 20px; border-radius: 8px 8px 0 0; background: transparent; margin-right: 0; border-bottom: 2px solid transparent; margin-bottom: -2px; transition: all 0.2s ease; }
        .nav-tabs .nav-link:hover { color: var(--accent-color); background: var(--accent-light); }
        .nav-tabs .nav-link.active { color: var(--accent-color); background: transparent; border-bottom-color: var(--accent-color); font-weight: 600; }
        .tab-content { display: none; }
        .tab-content.active { display: block; }
        .action-buttons .btn { padding: 5px 10px; font-size: 0.75rem; border-radius: 8px; }
        .modal-header { background: var(--accent-color); color: white; border-radius: 12px 12px 0 0; padding: 20px 24px; }
        .modal-header .btn-close { filter: invert(1); }
        .form-section { background: var(--bg-page); padding: 20px 24px; border-radius: 12px; margin-bottom: 20px; border: 1px solid var(--border-color); }
        .form-section h6 { color: var(--text-primary); margin-bottom: 16px; font-weight: 600; }
        .required { color: var(--danger-color); }
        .upload-zone { border: 2px dashed var(--border-color); border-radius: 12px; padding: 40px; text-align: center; background: var(--bg-page); transition: all 0.2s ease; }
        .upload-zone:hover { border-color: var(--accent-color); background: var(--accent-light); }
    </style>""",
}

def update_file(filepath, new_css):
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()
    start = content.find('<style>')
    end = content.find('</style>') + len('</style>')
    if start == -1 or end == -1:
        print(f'  SKIP: no <style> block found in {filepath}')
        return
    new_content = content[:start] + new_css + '\n' + content[end:]
    with open(filepath, 'w', encoding='utf-8') as f:
        f.write(new_content)
    print(f'  UPDATED: {os.path.basename(filepath)}')

base = 'c:/LFN/LFNHaulingApp/HaulingDemoApp/wwwroot'
for fname, css in CSS_TEMPLATES.items():
    update_file(f'{base}/{fname}', css)
print('Done batch 1!')
