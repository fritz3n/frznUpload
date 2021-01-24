
/**
 * 
 * @param {HTMLImageElement} image
 */
function resize() {
    let image = this;
    var maxWidth = image.parentElement.clientWidth; // Max width for the image
    var maxHeight = (window.innerHeight - image.offsetTop) * 0.8;    // Max height for the image
    var ratio = 0;  // Used for aspect ratio
    var width = image.width;    // Current image width
    var height = image.height;  // Current image height
    let widthRatio = image.naturalWidth / image.naturalHeight;
    let heightRatio = image.naturalHeight / image.naturalWidth;

    // Check if the current width is larger than the max
    if (width > maxWidth) {
        ratio = maxWidth / width;  
        width = width * ratio;  
        height = width * heightRatio;  
    }
    if (height > maxHeight) {
        ratio = maxHeight / height; 
        height = height * ratio; 
        width = height * widthRatio;   
    }

    if (ratio == 0 && height / maxHeight < width / maxWidth) {
        ratio = maxWidth / width;   
        width = width * ratio;    
        height = width * heightRatio;   
    } else if(ratio == 0) {
        ratio = maxHeight / height;
        height = height * ratio;   
        width = height * widthRatio;   
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
