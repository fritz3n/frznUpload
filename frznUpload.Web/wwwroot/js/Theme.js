
let cookie = document.cookie
    .split('; ')
    .find(row => row.startsWith('theme='));

if (!cookie) {
    let darkMode = window.matchMedia('(prefers-color-scheme: dark)').matches;
    if (darkMode)
        enableDarkMode();
    else
        disableDarkMode();
}

function enableDarkMode() {
    document.cookie = "theme=dark; path=/;";

    document.children[0].dataset.theme = "dark";

    newTheme = "/lib/bootstrap-dark/dist/bootstrap-night.min.css";
    document.getElementById("css").href = newTheme;
}

function disableDarkMode() {
    document.cookie = "theme=light; path=/;";
    document.children[0].dataset.theme = "light";
    newTheme = "/lib/bootstrap/css/bootstrap.min.css";
    document.getElementById("css").href = newTheme;
}

function toggleDarkMode() {
    let cookie = document.cookie
        .split('; ')
        .find(row => row.startsWith('theme='));


    if (cookie && cookie.endsWith("=dark")) {
        disableDarkMode();
    } else {
        enableDarkMode();
    }
}