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

        // Close submenu when sidebar is closed
        if (sidebar.classList.contains('close')) {
            const subMenu = document.querySelector('.sub-menu');
            const submenu = document.querySelector('.submenu');
            const arrow = document.querySelector('.submenu .arrow');
            if (subMenu) {
                subMenu.classList.remove('show');
            }
            if (submenu) {
                submenu.classList.remove('active');
            }
            if (arrow) {
                arrow.classList.remove('rotated');
            }
        }
    }
};

window.toggleDriverMenu = () => {
    const submenu = document.querySelector('.submenu');
    const subMenu = document.querySelector('.sub-menu');
    const arrow = document.querySelector('.submenu .arrow');

    if (submenu && subMenu) {
        submenu.classList.toggle('active');
        subMenu.classList.toggle('show');

        if (arrow) {
            arrow.classList.toggle('rotated');
        }
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
    const subMenu = document.querySelector('.sub-menu');
    const submenu = document.querySelector('.submenu');

    if (window.innerWidth <= 768) {
        // Mobile behavior
        if (sidebar) {
            sidebar.classList.add('close');
        }

        // Reset submenu for mobile
        if (subMenu && submenu) {
            subMenu.classList.remove('show');
            submenu.classList.remove('show-submenu');
        }
    }
});

// Add click outside to close submenu on mobile
document.addEventListener('click', function (event) {
    const sidebar = document.querySelector('.sidebar');
    const submenu = document.querySelector('.submenu');
    const subMenu = document.querySelector('.sub-menu');

    if (window.innerWidth <= 768 && sidebar && !sidebar.contains(event.target)) {
        if (subMenu && submenu) {
            subMenu.classList.remove('show');
            submenu.classList.remove('active');
            const arrow = document.querySelector('.submenu .arrow');
            if (arrow) {
                arrow.classList.remove('rotated');
            }
        }
    }
});

// Add hover effects for closed sidebar
document.addEventListener('mouseenter', function (event) {
    const target = event.target;
    if (target && typeof target.closest === 'function' && target.closest('.sidebar.close .submenu')) {
        const submenu = target.closest('.submenu');
        const subMenu = submenu.querySelector('.sub-menu');

        if (subMenu && window.innerWidth > 768) {
            subMenu.style.display = 'block';
            subMenu.style.opacity = '1';
        }
    }
}, true);

document.addEventListener('mouseleave', function (event) {
    const target = event.target;
    if (target && typeof target.closest === 'function' && target.closest('.sidebar.close .submenu')) {
        const submenu = target.closest('.submenu');
        const subMenu = submenu.querySelector('.sub-menu');

        if (subMenu && window.innerWidth > 768) {
            setTimeout(() => {
                if (!submenu.matches(':hover')) {
                    subMenu.style.display = 'none';
                    subMenu.style.opacity = '0';
                }
            }, 100);
        }
    }
}, true);
