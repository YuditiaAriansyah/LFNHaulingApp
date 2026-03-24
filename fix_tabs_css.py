"""
Fix nav-tabs visibility across all HTML files.
The old Bootstrap nav-tabs styles make text invisible on the light card background.
This adds proper dark-theme-compatible tab styles.
"""
import re
import os

BASE = r"c:\LFN\LFNHaulingApp\HaulingDemoApp\wwwroot"

# Strong nav-tabs override CSS (adds to existing style block)
NAV_TABS_CSS = """
        /* Tabs - override Bootstrap default dark style */
        .nav-tabs { border-bottom: 2px solid var(--border-color); gap: 4px; padding: 0; margin-bottom: 0; }
        .nav-tabs .nav-item { margin-bottom: 0; }
        .nav-tabs .nav-link {
            color: var(--text-secondary) !important;
            background: #f1f5f9;
            border: 1.5px solid var(--border-color);
            border-bottom: none;
            padding: 10px 20px;
            border-radius: 10px 10px 0 0;
            font-weight: 600;
            font-size: 0.85rem;
            transition: all 0.2s;
            margin-right: 4px;
        }
        .nav-tabs .nav-link i { font-size: 0.9rem; margin-right: 4px; opacity: 0.7; }
        .nav-tabs .nav-link:hover {
            color: var(--accent) !important;
            background: var(--accent-light);
            border-color: var(--accent);
        }
        .nav-tabs .nav-link.active {
            color: #fff !important;
            background: var(--accent);
            border-color: var(--accent);
            box-shadow: 0 -2px 8px rgba(0,0,0,0.1);
        }
        .nav-tabs .nav-link.active i { opacity: 1; }
        .nav-pills .nav-link { border-radius: 10px; }
        .nav-pills .nav-link.active { background: var(--accent); color: #fff !important; }
"""

# Nav-tabs CSS for data-Upload files (no card tabs, just simple pills)
NAV_PILLS_CSS = """
        /* Tabs / Pills - readable style */
        .nav-tabs { border-bottom: 2px solid var(--border-color); gap: 4px; padding: 0; }
        .nav-tabs .nav-item { margin-bottom: 0; }
        .nav-tabs .nav-link {
            color: var(--text-secondary) !important;
            background: #f1f5f9;
            border: 1.5px solid var(--border-color);
            border-bottom: none;
            padding: 10px 20px;
            border-radius: 10px 10px 0 0;
            font-weight: 600;
            font-size: 0.85rem;
            transition: all 0.2s;
            margin-right: 4px;
        }
        .nav-tabs .nav-link i { font-size: 0.9rem; margin-right: 4px; opacity: 0.7; }
        .nav-tabs .nav-link:hover {
            color: var(--accent) !important;
            background: var(--accent-light);
        }
        .nav-tabs .nav-link.active {
            color: #fff !important;
            background: var(--accent);
            border-color: var(--accent);
        }
        .nav-tabs .nav-link.active i { opacity: 1; }
        .nav-pills .nav-link { border-radius: 10px; font-weight: 500; }
        .nav-pills .nav-link.active { background: var(--accent); color: #fff !important; }
"""

# Nav-tabs CSS for datalist- tyre.html (amber accent - replace old gradient)
NAV_TABS_AMBER_CSS = """
        /* Tabs - override Bootstrap default dark style */
        .nav-tabs { border-bottom: 2px solid var(--border-color); gap: 4px; padding: 0; margin-bottom: 0; }
        .nav-tabs .nav-item { margin-bottom: 0; }
        .nav-tabs .nav-link {
            color: var(--text-secondary) !important;
            background: #f1f5f9;
            border: 1.5px solid var(--border-color);
            border-bottom: none;
            padding: 10px 20px;
            border-radius: 10px 10px 0 0;
            font-weight: 600;
            font-size: 0.85rem;
            transition: all 0.2s;
            margin-right: 4px;
        }
        .nav-tabs .nav-link i { font-size: 0.9rem; margin-right: 4px; opacity: 0.7; }
        .nav-tabs .nav-link:hover {
            color: var(--accent) !important;
            background: var(--accent-light);
            border-color: var(--accent);
        }
        .nav-tabs .nav-link.active {
            color: #fff !important;
            background: var(--accent);
            border-color: var(--accent);
            box-shadow: 0 -2px 8px rgba(0,0,0,0.1);
        }
        .nav-tabs .nav-link.active i { opacity: 1; }
        .nav-pills .nav-link { border-radius: 10px; }
        .nav-pills .nav-link.active { background: var(--accent); color: #fff !important; }
"""


def add_nav_tabs_css(filepath, new_css):
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()

    # Remove the old nav-tabs block if it exists (lines 97-101 range in datalist-fuel)
    old_pattern = re.compile(
        r'\s*/* Tabs.*?\.nav-tabs \.nav-link\.active \{[^}]*\}\s*',
        re.DOTALL
    )
    content = old_pattern.sub('', content)

    # Remove old gradient badges from datalist- tyre (the badge-po / badge-problem)
    # Add the new nav-tabs CSS before </style>
    content = content.replace('    </style>', new_css + '\n    </style>')

    with open(filepath, 'w', encoding='utf-8') as f:
        f.write(content)
    print(f"  Fixed tabs in: {os.path.basename(filepath)}")


def main():
    print("Fixing nav-tabs visibility in all HTML files...")

    # datalist-fuel.html - red accent
    add_nav_tabs_css(
        os.path.join(BASE, 'datalist-fuel.html'),
        NAV_TABS_CSS
    )

    # datalist- tyre.html - amber accent (has old gradient CSS to replace)
    add_nav_tabs_css(
        os.path.join(BASE, 'datalist- tyre.html'),
        NAV_TABS_AMBER_CSS
    )

    # data-Upload-Fuel.html - red accent
    add_nav_tabs_css(
        os.path.join(BASE, 'data-Upload-Fuel.html'),
        NAV_PILLS_CSS
    )

    # data-Upload-Tyre.html - amber accent
    add_nav_tabs_css(
        os.path.join(BASE, 'data-Upload-Tyre.html'),
        NAV_PILLS_CSS
    )

    # Also check other files for Bootstrap nav-tabs overrides
    other_files = [
        ('rm.html', 'teal'),
        ('hr.html', 'purple'),
        ('inventory.html', 'indigo'),
        ('vendor.html', 'orange'),
        ('purchase-request.html', 'pink'),
        ('purchase-order.html', 'teal'),
        ('good-receipt.html', 'red'),
        ('good-issue.html', 'purple'),
        ('fleet.html', 'blue'),
        ('finance.html', 'blue'),
        ('master-site.html', 'purple'),
        ('master-cost-center.html', 'teal'),
        ('master-user.html', 'red'),
    ]
    for fname, color in other_files:
        fpath = os.path.join(BASE, fname)
        if os.path.exists(fpath):
            with open(fpath, 'r', encoding='utf-8') as f:
                content = f.read()
            # Only add if not already have proper nav-tabs
            if 'background: #f1f5f9' not in content and '.nav-tabs .nav-link' in content:
                # It's already using our new CSS - skip
                print(f"  Already fixed: {fname}")
            elif '.nav-tabs' in content:
                # Has old nav-tabs, add the override
                add_nav_tabs_css(fpath, NAV_TABS_CSS)
            else:
                print(f"  No nav-tabs in: {fname}")

    print("\nAll nav-tabs fixed!")


if __name__ == '__main__':
    main()
