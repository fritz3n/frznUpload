
const screenshotCanvas = document.getElementById("screenshotCanvas");
const screenshotVideo = document.getElementById("screenshotVideo");

const modal = $("#screenshotModal");

var screenshotPrepared = false;
var shotTaken = false;
var screenshotImage;
var uploadFunction = null;
var screenshotMedia;

if (!navigator.mediaDevices.getDisplayMedia)
    $("#screenshotButton").remove();

function prepareScreenshot(shouldTakeScreenshot = false) {
    if (screenshotPrepared)
        clearScreenshot();
    shotTaken = false;
    uploadFunction = null;
    mouseUp(null, true);
    let displayMediaOptions = {
        video: {
            cursor: "never"
        },
        audio: false
    };

    return navigator.mediaDevices.getDisplayMedia(displayMediaOptions).then(media => {
        screenshotMedia = media;
        modal.modal("show");
        screenshotPrepared = true;
        modal.addClass("shooting");
        $("#screenshotUploadButton").attr("disabled", true);
        
        if (shouldTakeScreenshot)
            screenshotVideo.onplaying = takeScreenshot;
        else
            screenshotVideo.onplaying = null;
        screenshotVideo.srcObject = media;
        screenshotVideo.controls = false;
        screenshotVideo.play();
    });
}

(() => { // recording context
    var recording = false;
    var recorder;
    var chunks;
    var disable = false;

    window.stopRecoring = function (disableIt = true) {
        if (!recording)
            return;

        disable = disableIt;
        recording = false;
        recorder.stop();
        $("#recordButton").text("Record Video");
    }

    $("#recordButton").on("click", async function () {
        if (!recording) {
            if (!screenshotPrepared)
                await prepareScreenshot(false);
            uploadFunction = null;
            screenshotVideo.controls = false;
            screenshotVideo.onplaying = null;
            if (!screenshotVideo.srcObject) {
                screenshotVideo.srcObject = screenshotMedia;
                screenshotVideo.src = null;
            }

            disable = false;
            recording = true;
            recorder = new MediaRecorder(screenshotMedia);
            chunks = [];
            recorder.ondataavailable = e => chunks.push(e.data);
            recorder.start();
            recorder.onstop = e => {
                if (disable)
                    return;

                const completeBlob = new Blob(chunks, { type: chunks[0].type });

                screenshotVideo.srcObject = null;
                screenshotVideo.src = URL.createObjectURL(completeBlob);

                screenshotVideo.controls = true;
                screenshotVideo.play();
                modal.addClass("playing");
                uploadFunction = function (submit) {
                    let name = "Video " + getDate() + ".mp4";
                    submit({ blob: completeBlob, name: name });
                }
                $("#screenshotUploadButton").attr("disabled", false);
            };

            $("#recordButton").text("Stop recording");
            modal.addClass("shooting");
            modal.removeClass("playing");
            $("#screenshotUploadButton").attr("disabled", true);
        } else {
            stopRecoring(false);
        }
    })
})();

function clearScreenshot(closeModal = false) {
    let tracks = screenshotMedia.getTracks();

    tracks.forEach(track => track.stop());
    screenshotVideo.srcObject = null;
    screenshotPrepared = false;
    stopRecoring();

    if (closeModal)
        modal.modal("hide");
}

function takeScreenshot() {
    stopRecoring();
    screenshotCanvas.width = screenshotVideo.videoWidth;
    screenshotCanvas.height = screenshotVideo.videoHeight;

    screenshotCanvas.getContext('2d').drawImage(screenshotVideo, 0, 0);
    prepareCanvas();

    modal.removeClass("shooting");
    modal.removeClass("blink");
    let element = modal.get(0);
    void element.offsetWidth;
    modal.addClass("blink");
    $("#screenshotUploadButton").attr("disabled", false);
    shotTaken = true;

    let dataUrl = screenshotCanvas.toDataURL("image/png", 0.9);
    screenshotImage = new Image;
    screenshotImage.src = dataUrl;

    uploadFunction = function (submit) {
        screenshotCanvas.toBlob(function (blob) {
            let name = "Screenshot " + getDate() + ".png";
            submit({ blob: blob, name: name });
        }, "image/png", 0.9);
    }

}

function uploadScreenshot() {
    if (!uploadFunction)
        return;

    var submitFunction = function (file) {
        droppedFiles = [file];
        form.trigger('submit');
        clearScreenshot(true);
    }

    uploadFunction(submitFunction);
}

function getDate() {
    let date = new Date(Date.now());
    var str = date.getDate() + "-";
    str += (date.getMonth() + 1) + "-";
    str += date.getFullYear() + " ";
    str += date.getHours() + ":";
    str += date.getMinutes();
    return str;
}

var ctx = screenshotCanvas.getContext('2d'),
    rect = {},
    drag = false,
    cutTimeout = -1;

function draw() {
    ctx.globalAlpha = 0.2;
    ctx.fillStyle = "#FFFFFF";
    ctx.fillRect(rect.startX, rect.startY, rect.w, rect.h);
    ctx.globalAlpha = 1;
    ctx.fillStyle = "#000000";
    ctx.strokeRect(rect.startX, rect.startY, rect.w, rect.h);
}

function mouseDown(e) {
    clearTimeout(cutTimeout);
    if (!shotTaken)
        return;
    let pos = getPos(e);
    rect.startX = pos.x;
    rect.startY = pos.y;
    drag = true;
}

function mouseUp(e = null, dontCut = false) {
    drag = false;
    if (!dontCut && rect.w > 50 && rect.h > 50) {
        cutTimeout = setTimeout(cut, 5);
    }
}

function cut() {
    screenshotCanvas.width = rect.w;
    screenshotCanvas.height = rect.h;
    ctx.drawImage(screenshotImage, -rect.startX, -rect.startY);
    let dataUrl = screenshotCanvas.toDataURL("image/png", 0.9);
    screenshotImage = new Image;
    screenshotImage.src = dataUrl;
    modal.removeClass("blink");
    let element = modal.get(0);
    void element.offsetWidth;
    modal.addClass("blink");
}

function mouseMove(e) {
    if (!shotTaken)
        return;
    if (drag) {
        let pos = getPos(e);
        rect.w = pos.x - rect.startX;
        rect.h = pos.y - rect.startY;
        ctx.drawImage(screenshotImage, 0, 0);
        draw();
    }
}

function getPos(e) {
    var rect = screenshotCanvas.getBoundingClientRect();
    return {
        x: (e.clientX - rect.left) * (screenshotCanvas.width / screenshotCanvas.clientWidth),
        y: (e.clientY - rect.top) * (screenshotCanvas.height / screenshotCanvas.clientHeight)
    }
}

function init() {
    screenshotCanvas.addEventListener('mousedown', mouseDown, false);
    screenshotCanvas.addEventListener('mouseup', mouseUp, false);
    screenshotCanvas.addEventListener('mousemove', mouseMove, false);
    screenshotCanvas.addEventListener('click',
        function () {
            if (!shotTaken)
                takeScreenshot();
    }, false);
}

function prepareCanvas() {
    let width = screenshotCanvas.width, height = screenshotCanvas.height;
    let avg = (width + height) / 2;
    ctx.lineWidth = avg / 500;
}

init();
$('[data-toggle="tooltip"]').tooltip()