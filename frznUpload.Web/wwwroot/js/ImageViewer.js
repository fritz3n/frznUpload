
/**
 * 
 * @param {HTMLImageElement} image
 */
function resize() {
    let image = this;
    var maxWidth = image.parentElement.clientWidth; // Max width for the image
    var maxHeight = window.innerHeight * 0.8;    // Max height for the image
    var ratio = 0;  // Used for aspect ratio
    var width = image.width;    // Current image width
    var height = image.height;  // Current image height
    https://speed.af.de/results/?id=1dtd7cf

    // Check if the current width is larger than the max
    if (width > maxWidth) {
        ratio = maxWidth / width;   // get ratio for scaling image
        height = height * ratio;    // Reset height to match scaled image
        width = width * ratio;    // Reset width to match scaled image
    } else if (height > maxHeight) {
        ratio = maxHeight / height; // get ratio for scaling image
        width = width * ratio;    // Reset width to match scaled image
        height = height * ratio;    // Reset height to match scaled image
    } else if (height / maxHeight < width / maxWidth) {
        ratio = maxWidth / width;   // get ratio for scaling image
        height = height * ratio;    // Reset height to match scaled image
        width = width * ratio;    // Reset width to match scaled image
    } else {
        ratio = maxHeight / height; // get ratio for scaling image
        width = width * ratio;    // Reset width to match scaled image
        height = height * ratio;    // Reset height to match scaled image
    }

    image.style.width = width + "px";
    image.style.height = height + "px";
}

let image = document.getElementById('image');

image.onload = resize;
window.onresize = resize.bind(image);

if (image.complete)
    resize.bind(image)();

// View an image
const viewer = new Viewer(document.getElementById('image'),
    {
        zoomRatio: 0.5,
        transition: false
    });
