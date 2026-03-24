"""Fix finance.html nav-tabs-custom."""
import re

ACCENT = '#38bdf8'

INACTIVE = 'background:rgba(255,255,255,0.05)!important;color:rgba(255,255,255,0.5)!important;border:1.5px solid rgba(255,255,255,0.08)!important;border-bottom:none!important;'
ACTIVE = 'background:' + ACCENT + '!important;color:#fff!important;border:1.5px solid ' + ACCENT + '!important;border-bottom:none!important;'

TABS = [
    ('coa', 'Chart of Accounts', 'bi-list-ul', True),
    ('budget', 'Budget', 'bi-calculator', False),
    ('gl', 'General Ledger', 'bi-journal-text', False),
    ('cpt', 'Cost per Ton', 'bi-bar-chart', False),
]

with open(r'c:\LFN\LFNHaulingApp\HaulingDemoApp\wwwroot\finance.html', 'r', encoding='utf-8') as f:
    content = f.read()

# Find the broken nav-tabs-custom ul
m = re.search(r'(<ul[^>]*class="[^"]*nav-tabs-custom[^"]*"[^>]*>)(.*?)(</ul>)',
              content, re.DOTALL | re.IGNORECASE)
if not m:
    print("nav-tabs-custom not found")
else:
    indent = content[m.start():m.start()]
    # Get indentation from original
    line_start = content.rfind('\n', 0, m.start()) + 1
    indent = content[line_start:m.start()]

    items = []
    for tab_id, label, icon, is_active in TABS:
        style = ACTIVE if is_active else INACTIVE
        cls = 'nav-link active' if is_active else 'nav-link'
        item = (
            indent + '    <li class="nav-item" role="presentation">\n' +
            indent + '        <button class="' + cls + '" id="' + tab_id + '-tab" data-bs-toggle="tab" data-bs-target="#' + tab_id + '" type="button" role="tab" style="' + style + '">\n' +
            indent + '            <i class="bi ' + icon + ' me-2"></i>' + label + '\n' +
            indent + '        </button>\n' +
            indent + '    </li>'
        )
        items.append(item)

    new_inner = '\n'.join(items)
    new_nav = m.group(1) + '\n' + new_inner + '\n' + indent + '</ul>'

    content = content[:m.start()] + new_nav + content[m.end():]

    with open(r'c:\LFN\LFNHaulingApp\HaulingDemoApp\wwwroot\finance.html', 'w', encoding='utf-8') as f:
        f.write(content)
    print("Fixed finance.html")
