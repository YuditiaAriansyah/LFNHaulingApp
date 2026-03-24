"""
Restore all broken nav-tabs HTML across all files.
"""
import re, os

BASE = r"c:\LFN\LFNHaulingApp\HaulingDemoApp\wwwroot"

TABS = {
    'rm.html': [
        ('wo', 'Work Orders', 'bi-clipboard-check'),
        ('pm', 'PM Schedule', 'bi-calendar-check'),
        ('cm', 'Corrective Maint.', 'bi-tools'),
    ],
    'hr.html': [
        ('employee', 'Employee', 'bi-person-badge'),
        ('attendance', 'Attendance', 'bi-clock'),
        ('payroll', 'Payroll', 'bi-cash'),
    ],
    'purchase-request.html': [
        ('list', 'List PR', 'bi-list-ul'),
        ('create', 'Create PR', 'bi-plus-circle'),
        ('upload', 'Upload Data', 'bi-cloud-arrow-up'),
    ],
    'purchase-order.html': [
        ('list', 'List PO', 'bi-list-ul'),
        ('create', 'Create PO', 'bi-plus-circle'),
    ],
    'good-receipt.html': [
        ('list', 'List GR', 'bi-list-ul'),
        ('create', 'Create GR', 'bi-plus-circle'),
    ],
    'good-issue.html': [
        ('list', 'List GI', 'bi-list-ul'),
        ('create', 'Create GI', 'bi-plus-circle'),
    ],
    'fleet.html': [
        ('list', 'Fleet List', 'bi-list-ul'),
        ('create', 'Add Vehicle', 'bi-plus-circle'),
        ('upload', 'Upload Data', 'bi-cloud-arrow-up'),
    ],
    'vendor.html': [
        ('list', 'Vendor List', 'bi-list-ul'),
        ('create', 'Add Vendor', 'bi-plus-circle'),
    ],
    'datalist- tyre.html': [
        ('po', 'Upload PO Tyre', 'bi-cart'),
        ('issue', 'Tyre Issue', 'bi-exclamation-triangle'),
    ],
}

ACCENT = {
    'rm.html': '#0f766e',
    'hr.html': '#7c3aed',
    'purchase-request.html': '#ec4899',
    'purchase-order.html': '#0f766e',
    'good-receipt.html': '#dc2626',
    'good-issue.html': '#7c3aed',
    'fleet.html': '#2563eb',
    'vendor.html': '#ea580c',
    'datalist- tyre.html': '#d97706',
}


def get_line_indent(content, pos):
    """Get leading whitespace of the line containing pos."""
    line_start = content.rfind('\n', 0, pos) + 1
    return content[line_start:pos]


def fix_tabs_file(filepath, tabs, accent, use_onclick=True, use_bs_toggle=False):
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()

    fname = os.path.basename(filepath)
    m = re.search(r'<ul([^>]*)class="([^"]*nav-tabs[^"]*)"([^>]*)>(.*?)</ul>',
                  content, re.DOTALL | re.IGNORECASE)
    if not m:
        print(f"  No nav-tabs found: {fname}")
        return

    ul_attrs_before = m.group(1)
    ul_class = m.group(2)
    ul_attrs_after = m.group(3)
    indent = get_line_indent(content, m.start())
    base_indent = indent

    new_li_items = []
    for i, (tab_id, label, icon) in enumerate(tabs):
        is_active = (i == 0)
        if is_active:
            style = 'background:' + accent + '!important;color:#fff!important;border:1.5px solid ' + accent + '!important;border-bottom:none!important;'
            cls_suffix = ' active'
        else:
            style = 'background:#f1f5f9!important;color:#1e293b!important;border:1.5px solid #e2e8f0!important;border-bottom:none!important;'
            cls_suffix = ''

        if use_onclick:
            extra = 'onclick="switchTab(\'' + tab_id + '\')"'
        elif use_bs_toggle:
            extra = 'data-bs-toggle="tab" data-bs-target="#' + tab_id + '"'
        else:
            extra = ''

        li = (
            base_indent + '    <li class="nav-item" role="presentation">\n' +
            base_indent + '        <button class="nav-link' + cls_suffix + '" id="' + tab_id + 'Tab" ' +
            extra + ' type="button" role="tab" style="' + style + '">\n' +
            base_indent + '            <i class="bi ' + icon + ' me-2"></i>' + label + '\n' +
            base_indent + '        </button>\n' +
            base_indent + '    </li>'
        )
        new_li_items.append(li)

    new_inner = '\n'.join(new_li_items)
    new_nav = '<ul' + ul_attrs_before + 'class="' + ul_class + '"' + ul_attrs_after + '>\n' + new_inner + '\n' + base_indent + '</ul>'

    content = content[:m.start()] + new_nav + content[m.end():]

    with open(filepath, 'w', encoding='utf-8') as f:
        f.write(content)
    print(f"  Fixed: {fname} ({len(tabs)} tabs)")


def main():
    print("Restoring all broken nav-tabs...")

    for fname, tabs in TABS.items():
        fpath = os.path.join(BASE, fname)
        if os.path.exists(fpath):
            use_onclick = fname != 'datalist- tyre.html'
            use_bs = fname == 'datalist- tyre.html'
            fix_tabs_file(fpath, tabs, ACCENT.get(fname, '#dc2626'),
                         use_onclick=use_onclick, use_bs_toggle=use_bs)
        else:
            print(f"  Missing: {fname}")

    print("\nAll nav-tabs restored!")


if __name__ == '__main__':
    main()
