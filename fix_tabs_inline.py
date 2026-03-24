"""
Add inline styles to nav-tabs .nav-link elements.
Only processes <ul class="...nav-tabs..."> or <ul class="...nav-tabs-custom...">.
Avoids .navbar-nav. Uses BeautifulSoup for reliable HTML parsing.
"""
from bs4 import BeautifulSoup
import os

BASE = r"c:\LFN\LFNHaulingApp\HaulingDemoApp\wwwroot"

LIGHT_FILES = {
    'datalist-fuel.html': True,
    'datalist- tyre.html': True,
    'data-Upload-Fuel.html': True,
    'data-Upload-Tyre.html': True,
    'rm.html': True,
    'hr.html': True,
    'inventory.html': True,
    'purchase-request.html': True,
    'purchase-order.html': True,
    'good-receipt.html': True,
    'good-issue.html': True,
    'fleet.html': True,
    'vendor.html': True,
    'finance.html': False,
}

ACCENT_COLORS = {
    'datalist-fuel.html': '#dc2626',
    'datalist- tyre.html': '#d97706',
    'data-Upload-Fuel.html': '#dc2626',
    'data-Upload-Tyre.html': '#d97706',
    'rm.html': '#0f766e',
    'hr.html': '#7c3aed',
    'inventory.html': '#4f46e5',
    'purchase-request.html': '#ec4899',
    'purchase-order.html': '#0f766e',
    'good-receipt.html': '#dc2626',
    'good-issue.html': '#7c3aed',
    'fleet.html': '#2563eb',
    'vendor.html': '#ea580c',
    'finance.html': '#38bdf8',
}


def get_styles(is_light, accent):
    if is_light:
        inactive = 'background:#f1f5f9!important;color:#1e293b!important;border:1.5px solid #e2e8f0!important;border-bottom:none!important;'
        active = f'background:{accent}!important;color:#fff!important;border:1.5px solid {accent}!important;border-bottom:none!important;'
    else:
        inactive = 'background:rgba(255,255,255,0.05)!important;color:rgba(255,255,255,0.5)!important;border:1.5px solid rgba(255,255,255,0.08)!important;border-bottom:none!important;'
        active = f'background:{accent}!important;color:#fff!important;border:1.5px solid {accent}!important;border-bottom:none!important;'
    return inactive, active


def fix_file(filepath):
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()

    fname = os.path.basename(filepath)
    is_light = LIGHT_FILES.get(fname, True)
    accent = ACCENT_COLORS.get(fname, '#dc2626')
    inactive, active = get_styles(is_light, accent)

    soup = BeautifulSoup(content, 'html.parser')
    changed = False

    # Find all <ul> elements with class containing 'nav-tabs' (but not navbar-nav)
    for ul in soup.find_all('ul'):
        ul_classes = ul.get('class', [])
        ul_class_str = ' '.join(ul_classes) if ul_classes else ''

        # Skip if not a tab nav
        if 'nav-tabs' not in ul_class_str or 'navbar-nav' in ul_class_str:
            continue

        # Process all .nav-link elements inside this ul
        for link in ul.find_all(class_='nav-link'):
            is_active = 'active' in (link.get('class', []))
            link['style'] = active if is_active else inactive
            changed = True

    if changed:
        # Preserve original DOCTYPE and encoding
        with open(filepath, 'w', encoding='utf-8') as f:
            f.write(str(soup))
        print(f"  Fixed: {fname}")
    else:
        print(f"  No tab-nav found: {fname}")


def main():
    print("Adding inline styles to nav-tabs...")
    for fname in LIGHT_FILES:
        fpath = os.path.join(BASE, fname)
        if os.path.exists(fpath):
            fix_file(fpath)
        else:
            print(f"  Missing: {fname}")
    print("\nDone!")


if __name__ == '__main__':
    main()
