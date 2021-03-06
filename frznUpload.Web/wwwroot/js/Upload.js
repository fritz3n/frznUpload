﻿
var form = $('.box');
var input = $('.uploadFile');
var progress = $(".uploadProgress");

input.on("change", function (e) {
    droppedFiles = e.target.files;
    form.trigger('submit');
})

form.on('drag dragstart dragend dragover dragenter dragleave drop', function (e) {
    e.preventDefault();
    e.stopPropagation();
})
.on('dragover dragenter', function () {
    form.addClass('is-dragover');
})
.on('dragleave dragend drop', function () {
    form.removeClass('is-dragover');
})
.on('drop', function (e) {
    droppedFiles = e.originalEvent.dataTransfer.files;
    form.trigger('submit');
});

document.onpaste = function (event) {
    var items = (event.clipboardData || event.originalEvent.clipboardData).items;
    console.log(JSON.stringify(items)); // will give you the mime types
    for (index in items) {
        var item = items[index];
        if (item.kind === 'file') {
            var blob = item.getAsFile();
            let extension = item.type.split("/")[1];
            let name = "file " + getDate() + "." + extension;
            droppedFiles = [{ blob: blob, name: name }];
            form.trigger('submit');
        }
    }
}

/**
 * 
 * @param {ProgressEvent} e
 */
function onprogress(e) {
    progress.css("right", (1 - e.loaded / e.total) * 100 + "%");
}

form.on('submit', function (e) {
    if (form.hasClass('is-uploading')) return false;

    form.addClass('is-uploading').removeClass('is-error');
    e.preventDefault();

    var ajaxData = new FormData();

    if (droppedFiles) {
        $.each(droppedFiles, function (i, file) {
            if ('blob' in file)
                ajaxData.append("file", file.blob, file.name);
            else
                ajaxData.append("file", file);
        });
    }

    var xhr = new XMLHttpRequest();
    xhr.upload.onprogress = onprogress;
    xhr.onload = function (e) {
        if (xhr.status != 200) {
            form.addClass('is-error');
            return;
        }
        var res = xhr.response.split(";");
        form.addClass('is-success');
        identifier = res[0];
        $("#fileId").prop("value", res[0]);
        $("#uploadedSpan").addClass("show");
        $("#uploadedLink").prop("href", "/Account/Files/View/" + encodeURIComponent(res[0]));
        $("#uploadedLink").text(res[1]);
        $("#shareButton").prop("disabled", false);
    }
    xhr.onerror = function () {
        form.addClass('is-error');
    }
    xhr.open("POST", form.attr('action'));
    xhr.setRequestHeader("RequestVerificationToken", $('input:hidden[name="__RequestVerificationToken"]').val());
    xhr.responseType = "";
    xhr.send(ajaxData);
});
$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip();
});

$("#Share_Whitelisted").on("change", function () {
    $("#Share_Whitelist").prop("disabled", !this.checked);
});

$("#shareUrlCopy").popover({ trigger: "manual" });
$("#shareButton").popover({ trigger: "manual" });

$("#shareUrl").on("click", function () {
    let shareUrl = $("#shareUrl").get(0);
    shareUrl.select();
    shareUrl.setSelectionRange(0, 9999999);
});

$("#shareUrlCopy").on("click", function (e) {
    e.preventDefault();
    let shareUrl = $("#shareUrl").get(0);
    shareUrl.select();
    shareUrl.setSelectionRange(0, 9999999);
    document.execCommand("copy");
    $("#shareUrlCopy").popover("show");
    setTimeout(() => {
        $('#shareUrlCopy').popover('hide');
    }, 1000);
})
let identifier;
$("#shareForm").on("submit", function (e) {
    e.preventDefault();
    let formData = new FormData($("#shareForm").get(0));
    $("#shareButton").prop("disabled", true);

    fetch("", {
        body: formData,
        credentials: "same-origin",
        method: "POST",
        cache: 'no-cache'
    }).then(response => {
        if (response.ok)
            return response.text();
        throw "Response was " + response.status + " " + response.statusText;
    }).then(text => {
        let getUrl = document.location;
        $("#shareUrl").prop("value", getUrl.protocol + "//" + getUrl.host + text);
        $(".shareUrlContainer").addClass("show");
        $("#shareButton").prop("disabled", false);
    }).catch(error => {
        $("#shareButton").removeClass("btn-success");
        $("#shareButton").addClass("btn-danger");
        $("#shareButton").prop("disabled", true);
        $("#shareButton").get(0).dataset.content = "Error while sharing: " + error;
        $("#shareButton").popover("show");

        setTimeout(function () {
            $("#shareButton").removeClass("btn-danger");
            $("#shareButton").addClass("btn-success");
            $("#shareButton").prop("disabled", false);
            $("#shareButton").popover("hide");
        }, 5000);
    });
});
(() => { 
    let editingName = false;

    function setName() {
        let newName = $("#filenameInput").val();

        let data = new FormData();
        data.append("newName", newName);
        data.append("identifier", identifier);

        fetch($("#renameLink").attr("href"), {
            body: data,
            credentials: "same-origin",
            method: "POST",
            cache: 'no-cache',
            headers: {
                "RequestVerificationToken": $("#renameLink").data("csrf")
            }
        }).then(response => {
            if (response.ok)
                return response.text();
            throw "Response was " + response.status + " " + response.statusText;
        }).then(text => {
            let link = document.createElement("a");
            link.href = "/Account/Files/View/" + encodeURIComponent(identifier);
            link.innerText = text;
            link.id = "uploadedLink";
            link.target = "_blank";
            $(link).insertBefore("#filenameInput");
            $("#filenameInput").remove();
            $("#filenameButton").html('<i class="far fa-edit"></i>');

            editingName = false;
        }).catch(error => {
            window.alert("Could not change the name\n" + error);
        });
    }

    $("#filenameButton").on("click", function () {
        if (!editingName) {
            var input = document.createElement("input");
            input.value = $("#uploadedLink").text();
            input.id = "filenameInput";
            $(input).insertBefore("#uploadedLink");
            input.select();
            $("#uploadedLink").remove();
            $("#filenameButton").html('<i class="fas fa-check"></i>');
            editingName = true;

            $("#filenameInput").keypress(function (e) {
                var keycode = (event.keyCode ? event.keyCode : event.which);
                if (editingName && keycode == '13') {
                    setName();
                }
            });
        } else {
            setName();
        }
    });
})();