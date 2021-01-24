
$("tr").on("click", function () {
    if (this.dataset.path) {
        window.location = "/Account/Files/Index" + this.dataset.path;
    } else if (this.dataset.identifier) {
        window.location = "/Account/Files/View/" + this.dataset.identifier;
    }
});