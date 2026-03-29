const fs = require('fs');
const path = require('path');

const wwwroot = path.join(__dirname, 'HaulingDemoApp', 'wwwroot');
const files = fs.readdirSync(wwwroot).filter(f => f.endsWith('.html'));

const NAVBAR_CSS = `
    /* NAVBAR */
    .navbar { background: var(--navy) !important; box-shadow: 0 2px 12px rgba(0,0,0,0.15); }

    /* NAVBAR NAV */
    .nav-item.dropdown { position: relative; }
    .nav-item.dropdown .nav-link {
        color: rgba(255, 255, 255, 0.6) !important;
        font-weight: 500;
        font-size: 0.82rem;
        padding: 0.45rem 0.7rem !important;
        border-radius: 8px;
        transition: all 0.2s ease;
        display: flex;
        align-items: center;
        gap: 5px;
        white-space: nowrap;
        text-decoration: none;
    }
    .nav-item.dropdown .nav-link:hover { color: rgba(255,255,255,0.9) !important; background: rgba(255,255,255,0.08); }
    .nav-item.dropdown .nav-link.dropdown-toggle::after {
        display: inline-block;
        margin-left: 4px;
        content: '';
        border: 0;
        border-top: 4px solid rgba(255,255,255,0.4);
        border-right: 4px solid transparent;
        border-bottom: 0;
        border-left: 4px solid transparent;
        vertical-align: middle;
    }
    .nav-item.dropdown:hover .nav-link.dropdown-toggle::after { border-top-color: rgba(255,255,255,0.7); }
    .dropdown-menu {
        background: rgba(15, 23, 42, 0.98) !important;
        border: 1px solid rgba(255, 255, 255, 0.1) !important;
        border-radius: 12px !important;
        padding: 6px !important;
        box-shadow: 0 8px 32px rgba(0,0,0,0.5) !important;
        min-width: 220px !important;
        backdrop-filter: blur(16px);
        -webkit-backdrop-filter: blur(16px);
        animation: dropdownFade 0.15s ease;
        margin-top: 4px !important;
    }
    @keyframes dropdownFade {
        from { opacity: 0; transform: translateY(-6px); }
        to { opacity: 1; transform: translateY(0); }
    }
    .dropdown-divider { border-color: rgba(255,255,255,0.08) !important; margin: 4px 0; }
    .dropdown-item {
        color: rgba(255, 255, 255, 0.7) !important;
        font-size: 0.82rem;
        font-weight: 500;
        padding: 8px 12px !important;
        border-radius: 8px !important;
        transition: all 0.15s ease;
        display: flex !important;
        align-items: center;
        gap: 8px;
        white-space: nowrap;
    }
    .dropdown-item:hover { color: #fff !important; background: rgba(59, 130, 246, 0.25) !important; }
    .dropdown-item.active-page { color: #fff !important; background: rgba(59, 130, 246, 0.4) !important; }
    .dropdown-item i { font-size: 0.9rem; width: 16px; text-align: center; opacity: 0.7; }
    .dropdown-item:hover i { opacity: 1; }
    .dropdown-item .bi { font-size: 0.9rem; }
`;

function makeNavbar(section, currentFile) {
    function item(file, label, icon) {
        return '              <li><a class="dropdown-item' + (currentFile === file ? ' active-page' : '') + '" href="' + file + '"><i class="bi ' + icon + '"></i> ' + label + '</a></li>';
    }
    function divider() {
        return '              <li class="dropdown-divider"></li>';
    }
    function p1Link(file, label, icon) { return item(file, label, icon); }
    function p2Link(file, label, icon) { return item(file, label, icon); }
    function p3Link(file, label, icon) { return item(file, label, icon); }
    function p4Link(file, label, icon) { return item(file, label, icon); }
    function opLink(file, label, icon) { return item(file, label, icon); }
    function procLink(file, label, icon) { return item(file, label, icon); }
    function financeLink(file, label, icon) { return item(file, label, icon); }

    const homeActive = section === 'home' ? ' active' : '';
    const p1Active = section === 'p1' ? ' active' : '';
    const p2Active = section === 'p2' ? ' active' : '';
    const p3Active = section === 'p3' ? ' active' : '';
    const p4Active = section === 'p4' ? ' active' : '';
    const operasiActive = section === 'operasi' ? ' active' : '';
    const procActive = section === 'procurement' ? ' active' : '';
    const keuanganActive = section === 'keuangan' ? ' active' : '';

    return `            <div class="collapse navbar-collapse" id="navbarNav">
                <ul class="navbar-nav ms-auto">
          <li class="nav-item"><a class="nav-link${homeActive}" href="index.html"><i class="bi bi-house"></i> Home</a></li>

          <li class="nav-item dropdown">
            <a class="nav-link dropdown-toggle${p1Active}" href="#" data-bs-toggle="dropdown">P1 Master</a>
            <ul class="dropdown-menu">
              ${p1Link('master-site.html', 'Site', 'bi-geo-alt')}
              ${p1Link('master-cost-center.html', 'Cost Center', 'bi-diagram-3')}
              ${p1Link('master-user.html', 'User', 'bi-people')}
            </ul>
          </li>

          <li class="nav-item dropdown">
            <a class="nav-link dropdown-toggle${p2Active}" href="#" data-bs-toggle="dropdown">P2 Master</a>
            <ul class="dropdown-menu">
              ${p2Link('master-department.html', 'Department', 'bi-building')}
              ${p2Link('master-driver.html', 'Driver', 'bi-person-badge')}
            </ul>
          </li>

          <li class="nav-item dropdown">
            <a class="nav-link dropdown-toggle${p3Active}" href="#" data-bs-toggle="dropdown">P3 Master</a>
            <ul class="dropdown-menu">
              ${p3Link('master-vehicle.html', 'Vehicle', 'bi-truck')}
              ${p3Link('master-route.html', 'Route', 'bi-signpost-split')}
              ${p3Link('master-material.html', 'Material', 'bi-box-seam')}
              ${p3Link('master-uom.html', 'UOM', 'bi-rulers')}
              ${p3Link('master-coa.html', 'COA', 'bi-book')}
              ${p3Link('master-tax.html', 'Tax', 'bi-percent')}
            </ul>
          </li>

          <li class="nav-item dropdown">
            <a class="nav-link dropdown-toggle${p4Active}" href="#" data-bs-toggle="dropdown">P4 Master</a>
            <ul class="dropdown-menu">
              ${p4Link('master-workflow.html', 'Workflow', 'bi-diagram-2')}
            </ul>
          </li>

          <li class="nav-item dropdown">
            <a class="nav-link dropdown-toggle${operasiActive}" href="#" data-bs-toggle="dropdown">Operasi</a>
            <ul class="dropdown-menu">
              ${opLink('datalist-fuel.html', 'Fuel List', 'bi-fuel')}
              ${opLink('data-Upload-Fuel.html', 'Fuel Upload', 'bi-cloud-upload')}
              ${opLink('datalist-tyre.html', 'Tyre List', 'bi-circle')}
              ${opLink('data-Upload-Tyre.html', 'Tyre Upload', 'bi-cloud-upload')}
              ${divider()}
              ${opLink('rm.html', 'R&M', 'bi-wrench')}
              ${opLink('data-Upload-RM.html', 'R&M Upload', 'bi-cloud-upload')}
              ${opLink('fleet.html', 'Fleet', 'bi-truck')}
              ${divider()}
              ${opLink('inventory.html', 'Inventory', 'bi-box-seam')}
              ${opLink('haul-trip.html', 'Haul Trip', 'bi-arrow-left-right')}
              ${opLink('weighbridge.html', 'Weighbridge', 'bi-scale')}
              ${opLink('kpi-dashboard.html', 'KPI Dashboard', 'bi-graph-up')}
            </ul>
          </li>

          <li class="nav-item dropdown">
            <a class="nav-link dropdown-toggle${procActive}" href="#" data-bs-toggle="dropdown">Procurement</a>
            <ul class="dropdown-menu">
              ${procLink('purchase-request.html', 'Purchase Request', 'bi-file-earmark-text')}
              ${procLink('purchase-order.html', 'Purchase Order', 'bi-receipt')}
              ${procLink('good-receipt.html', 'Goods Receipt', 'bi-clipboard-check')}
              ${procLink('good-issue.html', 'Goods Issue', 'bi-box-arrow-up')}
              ${divider()}
              ${procLink('vendor.html', 'Vendor', 'bi-shop')}
            </ul>
          </li>

          <li class="nav-item dropdown">
            <a class="nav-link dropdown-toggle${keuanganActive}" href="#" data-bs-toggle="dropdown">Keuangan</a>
            <ul class="dropdown-menu">
              ${financeLink('finance.html', 'Finance', 'bi-cash-stack')}
              ${financeLink('hr.html', 'HR', 'bi-person-lines-fill')}
            </ul>
          </li>
                </ul>
            </div>`;
}

const pageMap = {
    'index.html':               { section: 'home',        label: 'Home' },
    'master-site.html':         { section: 'p1',          label: 'Site' },
    'master-cost-center.html':  { section: 'p1',          label: 'Cost Center' },
    'master-user.html':         { section: 'p1',          label: 'User' },
    'master-department.html':   { section: 'p2',          label: 'Department' },
    'master-driver.html':       { section: 'p2',          label: 'Driver' },
    'master-vehicle.html':     { section: 'p3',          label: 'Vehicle' },
    'master-route.html':        { section: 'p3',          label: 'Route' },
    'master-material.html':     { section: 'p3',          label: 'Material' },
    'master-uom.html':         { section: 'p3',          label: 'UOM' },
    'master-coa.html':         { section: 'p3',          label: 'COA' },
    'master-tax.html':         { section: 'p3',          label: 'Tax' },
    'master-workflow.html':    { section: 'p4',          label: 'Workflow' },
    'datalist-fuel.html':      { section: 'operasi',     label: 'Fuel List' },
    'data-Upload-Fuel.html':   { section: 'operasi',     label: 'Fuel Upload' },
    'rm.html':                 { section: 'operasi',     label: 'R&M' },
    'data-Upload-RM.html':     { section: 'operasi',     label: 'R&M Upload' },
    'fleet.html':              { section: 'operasi',     label: 'Fleet' },
    'datalist-tyre.html':     { section: 'operasi',     label: 'Tyre List' },
    'data-Upload-Tyre.html':  { section: 'operasi',     label: 'Tyre Upload' },
    'inventory.html':         { section: 'operasi',     label: 'Inventory' },
    'haul-trip.html':         { section: 'operasi',     label: 'Haul Trip' },
    'weighbridge.html':       { section: 'operasi',     label: 'Weighbridge' },
    'kpi-dashboard.html':     { section: 'operasi',     label: 'KPI Dashboard' },
    'purchase-request.html':  { section: 'procurement', label: 'Purchase Request' },
    'purchase-order.html':     { section: 'procurement', label: 'Purchase Order' },
    'good-receipt.html':      { section: 'procurement', label: 'Goods Receipt' },
    'good-issue.html':        { section: 'procurement', label: 'Goods Issue' },
    'vendor.html':            { section: 'procurement', label: 'Vendor' },
    'finance.html':           { section: 'keuangan',    label: 'Finance' },
    'hr.html':                { section: 'keuangan',    label: 'HR' },
};

let updated = 0, skipped = 0;

for (const file of files) {
    if (file === 'index.html') { skipped++; continue; }

    const filePath = path.join(wwwroot, file);
    let content = fs.readFileSync(filePath, 'utf8');

    const pageInfo = pageMap[file];
    if (!pageInfo) { console.log('SKIP (no map):', file); skipped++; continue; }

    const section = pageInfo.section;
    const newNavbar = makeNavbar(section, file);

    // Pattern 1: <div class="collapse navbar-collapse"> ... </div> (with or without id)
    const divPattern = /<div class="collapse navbar-collapse"[^>]*>[\s\S]*?<\/div>\s*/;
    if (divPattern.test(content)) {
        content = content.replace(divPattern, newNavbar + '\n');
    } else {
        console.log('WARN: no navbar found in', file);
    }

    // Fix malformed /*</style> CSS comment.
    // Original has:  /*</style>   (opens comment, has literal text </style>, never closes)
    // Fix: replace with: </style><style>  (closes first style block, opens a new one for navbar CSS)
    content = content.replace(/\/\*<\/style>/, '</style><style>');

    // Remove old nav-group CSS
    content = content.replace(/\.nav-group[^}]*\{[^}]*\}/g, '');
    content = content.replace(/\.nav-group-label[^}]*\{[^}]*\}/g, '');

    // Inject navbar CSS — always wraps in <style>...</style> before </head>
    if (content.includes('.nav-item.dropdown')) {
        // Dropdown CSS already present; ensure navbar background is injected too
        if (!content.includes('.navbar { background')) {
            content = content.replace('</head>', '\n    .navbar { background: var(--navy) !important; box-shadow: 0 2px 12px rgba(0,0,0,0.15); }\n    </style>\n</head>');
        } else {
            // Close the second <style> block before </head>
            content = content.replace('</head>', '\n    </style>\n</head>');
        }
    } else {
        content = content.replace('</head>', NAVBAR_CSS + '\n    </style>\n</head>');
    }

    // Ensure Bootstrap JS is present
    if (!content.includes('bootstrap.bundle')) {
        content = content.replace('</body>', '<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>\n</body>');
    }

    // Add navbar-dark class to <nav> element for Bootstrap 5 dark theme support
    content = content.replace(
        '<nav class="navbar navbar-expand-lg sticky-top">',
        '<nav class="navbar navbar-expand-lg sticky-top navbar-dark">'
    );

    fs.writeFileSync(filePath, content, 'utf8');
    console.log('UPDATED:', file, '(section:', section + ')');
    updated++;
}

console.log('\nDone! Updated:', updated, '| Skipped:', skipped);
