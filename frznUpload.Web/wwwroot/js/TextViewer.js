let container = document.getElementById("textContainer");
let loader = document.getElementById("textOverlay");

let url = container.dataset.source;
let mime = container.dataset.mime;

var codeMirror = CodeMirror(container, {
    readOnly: true,
    mode: mime,
    lineNumbers: true,
    styleActiveLine: true,
    matchBrackets: true
});

fetch(url).then(response => {
    if (response.ok) {
        response.text().then(text => {
            codeMirror.setValue(text);
            loader.classList.add("hide");
        })
    }
});