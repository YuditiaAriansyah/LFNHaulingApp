import re, os

BASE = r"c:\LFN\LFNHaulingApp\HaulingDemoApp\wwwroot"

ACCENT = {
    'datalist-fuel.html':        '#dc2626',
    'datalist- tyre.html':      '#d97706',
    'data-Upload-Fuel.html':     '#dc2626',
    'data-Upload-Tyre.html':     '#d97706',
    'rm.html':                  '#0f766e',
    'hr.html':                  '#7c3aed',
    'inventory.html':           '#4f46e5',
    'purchase-request.html':    '#ec4899',
    'purchase-order.html':      '#0f766e',
    'good-receipt.html':       '#dc2626',
    'good-issue.html':         '#7c3aed',
    'fleet.html':              '#2563eb',
    'vendor.html':             '#ea580c',
    'finance.html':            '#38bdf8',
}


def fix_file(filepath, accent):
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()

    fname = os.path.basename(filepath)
    if 'switchTab' not in content or 'classList.add' not in content:
        return False
    if 'ACTIVE_STYLE' in content:
        return False

    active_s = 'background:' + accent + '!important;color:#fff!important;border:1.5px solid ' + accent + '!important;border-bottom:none!important;'
    inactive_s = 'background:#f1f5f9!important;color:#1e293b!important;border:1.5px solid #e2e8f0!important;border-bottom:none!important;'

    style_block = (
        '\n        const ACTIVE_STYLE   = \'' + active_s + '\';\n'
        '        const INACTIVE_STYLE = \'' + inactive_s + '\';\n\n'
    )

    fixed = content

    # Replace .classList.add('active') with .style.cssText = ACTIVE_STYLE
    fixed = re.sub(r"(\w+TabBtn)\.classList\.add\('active'\)", r"\1.style.cssText = ACTIVE_STYLE;", fixed)
    fixed = re.sub(r'(\w+TabBtn)\.classList\.add\("active"\)', r"\1.style.cssText = ACTIVE_STYLE;", fixed)
    fixed = re.sub(r"btn\.classList\.add\('active'\)", "btn.style.cssText = ACTIVE_STYLE;", fixed)
    fixed = re.sub(r'btn\.classList\.add\("active"\)', "btn.style.cssText = ACTIVE_STYLE;", fixed)

    # Replace .classList.remove('active') with .style.cssText = INACTIVE_STYLE
    fixed = re.sub(r"(\w+TabBtn)\.classList\.remove\('active'\)", r"\1.style.cssText = INACTIVE_STYLE;", fixed)
    fixed = re.sub(r'(\w+TabBtn)\.classList\.remove\("active"\)', r"\1.style.cssText = INACTIVE_STYLE;", fixed)
    fixed = re.sub(r"btn\.classList\.remove\('active'\)", "btn.style.cssText = INACTIVE_STYLE;", fixed)
    fixed = re.sub(r'btn\.classList\.remove\("active"\)', "btn.style.cssText = INACTIVE_STYLE;", fixed)

    # Fix forEach(btn => btn.classList.remove('active'))
    for_each_pat = re.compile(r"\.forEach\(btn => btn\.classList\.remove\('active'\)\)")
    fixed = for_each_pat.sub(
        ".forEach(btn => { btn.classList.remove('active'); btn.style.cssText = INACTIVE_STYLE; })",
        fixed
    )

    # Insert style constants before switchTab function
    sw_pat = re.search(r'(function switchTab\([^{]*\{)', fixed)
    if not sw_pat:
        print('  No switchTab: ' + fname)
        return False

    fixed = fixed[:sw_pat.start()] + style_block + fixed[sw_pat.start():]

    if fixed != content:
        with open(filepath, 'w', encoding='utf-8') as f:
            f.write(fixed)
        print('  Fixed: ' + fname)
        return True
    return False


def main():
    print('Fixing switchTab() in all files...')
    for fname, accent in ACCENT.items():
        fpath = os.path.join(BASE, fname)
        if os.path.exists(fpath):
            fix_file(fpath, accent)
        else:
            print('  Missing: ' + fname)
    print('\nDone!')


if __name__ == '__main__':
    main()
