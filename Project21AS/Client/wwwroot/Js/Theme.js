
function toggleSideManu() {
    document.querySelector('body').querySelector('nav').classList.toggle("close");
}

function toggleLightDarkMode() {
    const body = document.querySelector('body'),
        sidebar = body.querySelector('nav'),
        toggle = body.querySelector(".toggle"),
        searchBtn = body.querySelector(".search-box"),
        modeSwitch = body.querySelector(".toggle-switch"),
        modeText = body.querySelector(".mode-text");

    body.classList.toggle("dark");

    if (body.classList.contains("dark")) {
        modeText.innerText = "Light mode";
    } else {
        modeText.innerText = "Dark mode";

    }
}

function toggleDriverMenu() {
    document.getElementById('driverSubMenu').classList.toggle('show');
}

