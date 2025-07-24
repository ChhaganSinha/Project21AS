// Enhanced Sidebar JavaScript Functions

window.toggleSideManu = () => {
    const sidebar = document.querySelector('.sidebar');
    const home = document.querySelector('.home');
    const themePage = document.querySelector('.theme-page');

    if (sidebar) {
        sidebar.classList.toggle('close');

        // Update main content area width
        if (home) {
            if (sidebar.classList.contains('close')) {
                home.style.left = '78px';
                home.style.width = 'calc(100% - 78px)';
            } else {
                home.style.left = '250px';
                home.style.width = 'calc(100% - 250px)';
            }
        }

        if (themePage) {
            if (sidebar.classList.contains('close')) {
                themePage.style.marginLeft = '78px';
                themePage.style.width = 'calc(100% - 78px)';
            } else {
                themePage.style.marginLeft = '250px';
                themePage.style.width = 'calc(100% - 250px)';
            }
        }

        // Close all submenus when sidebar is closed
        if (sidebar.classList.contains('close')) {
            document.querySelectorAll('.sub-menu').forEach(m => m.classList.remove('show'));
            document.querySelectorAll('.submenu').forEach(s => s.classList.remove('active'));
            document.querySelectorAll('.submenu .arrow').forEach(a => a.classList.remove('rotated'));
        }
    }
};

window.toggleSubMenu = (id) => {
    const item = document.getElementById(id);
    if (!item) return;
    const subMenu = item.querySelector('.sub-menu');
    const arrow = item.querySelector('.arrow');

    if (subMenu) {
        subMenu.classList.toggle('show');
    }
    item.classList.toggle('active');

    if (arrow) {
        arrow.classList.toggle('rotated');
    }
};

window.toggleLightDarkMode = () => {
    const body = document.body;
    const switchElement = document.querySelector('.toggle-switch .switch');
    const toggleSwitch = document.querySelector('.toggle-switch');
    const moonIcon = document.querySelector('.sun-moon .moon');
    const sunIcon = document.querySelector('.sun-moon .sun');

    body.classList.toggle('dark');

    if (toggleSwitch) {
        toggleSwitch.classList.toggle('active');
    }

    // Toggle icon visibility
    if (body.classList.contains('dark')) {
        if (moonIcon) moonIcon.style.opacity = '0';
        if (sunIcon) sunIcon.style.opacity = '1';
    } else {
        if (moonIcon) moonIcon.style.opacity = '1';
        if (sunIcon) sunIcon.style.opacity = '0';
    }
};

// Initialize sidebar state on page load
document.addEventListener('DOMContentLoaded', function () {
    const sidebar = document.querySelector('.sidebar');
    const body = document.body;

    // Set initial sidebar state
    if (sidebar && sidebar.classList.contains('close')) {
        const home = document.querySelector('.home');
        const themePage = document.querySelector('.theme-page');

        if (home) {
            home.style.left = '78px';
            home.style.width = 'calc(100% - 78px)';
        }

        if (themePage) {
            themePage.style.marginLeft = '78px';
            themePage.style.width = 'calc(100% - 78px)';
        }
    }

    // Set initial dark mode state
    const moonIcon = document.querySelector('.sun-moon .moon');
    const sunIcon = document.querySelector('.sun-moon .sun');

    if (body.classList.contains('dark')) {
        if (moonIcon) moonIcon.style.opacity = '0';
        if (sunIcon) sunIcon.style.opacity = '1';
    } else {
        if (moonIcon) moonIcon.style.opacity = '1';
        if (sunIcon) sunIcon.style.opacity = '0';
    }
});

// Handle responsive behavior
window.addEventListener('resize', function () {
    const sidebar = document.querySelector('.sidebar');

    if (window.innerWidth <= 768) {
        if (sidebar) {
            sidebar.classList.add('close');
        }

        document.querySelectorAll('.sub-menu').forEach(m => m.classList.remove('show'));
        document.querySelectorAll('.submenu').forEach(s => s.classList.remove('active'));
        document.querySelectorAll('.submenu .arrow').forEach(a => a.classList.remove('rotated'));
    }
});

// Add click outside to close submenu on mobile
document.addEventListener('click', function (event) {
    const sidebar = document.querySelector('.sidebar');

    if (window.innerWidth <= 768 && sidebar && !sidebar.contains(event.target)) {
        document.querySelectorAll('.sub-menu').forEach(m => m.classList.remove('show'));
        document.querySelectorAll('.submenu').forEach(s => s.classList.remove('active'));
        document.querySelectorAll('.submenu .arrow').forEach(a => a.classList.remove('rotated'));
    }
});

//// Add hover effects for closed sidebar
//document.addEventListener('mouseenter', function (event) {
//    const target = event.target;
//    if (target && typeof target.closest === 'function' && target.closest('.sidebar.close .submenu')) {
//        const submenu = target.closest('.submenu');
//        const subMenu = submenu.querySelector('.sub-menu');

//        if (subMenu && window.innerWidth > 768) {
//            subMenu.style.display = 'block';
//            subMenu.style.opacity = '1';
//        }
//    }
//}, true);

//document.addEventListener('mouseleave', function (event) {
//    const target = event.target;
//    if (target && typeof target.closest === 'function' && target.closest('.sidebar.close .submenu')) {
//        const submenu = target.closest('.submenu');
//        const subMenu = submenu.querySelector('.sub-menu');

//        if (subMenu && window.innerWidth > 768) {
//            setTimeout(() => {
//                if (!submenu.matches(':hover')) {
//                    subMenu.style.display = 'none';
//                    subMenu.style.opacity = '0';
//                }
//            }, 100);
//        }
//    }
//}, true);
