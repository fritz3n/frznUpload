const OFFLINE_VERSION = 1;
const OFFLINE_CACHE_NAME = "offline";
const OFFLINE_URL = "offline.html";



/** @type IDBDatabase */
var db;

var formData = null;
var serializeForm = function (formData) {
    var obj = {};
    for (var key of formData.keys()) {
        obj[key] = formData.get(key);
    }
    return obj;
};

/**
 * 
 * @param {"readonly" | "readwrite"} type
 * @param {(trans: IDBTransaction) => void} query
 */
function executeQuery(type, query) {
    return new Promise(function (resolve, reject) {
        var transaction = db.transaction(["data"], type);
        transaction.oncomplete = event => resolve(event.target.result);
        transaction.onerror = event => reject(event);
        query(transaction);
    });
}


/**
 * @type {{self: ServiceWorker}}
 */
self.addEventListener("install", (event) => {
    event.waitUntil(
        (async () => {
            const offlineCache = await caches.open(OFFLINE_CACHE_NAME);
            await offlineCache.add(new Request(OFFLINE_URL, { cache: "reload" }));
        })()
    );
    // Force the waiting service worker to become the active service worker.
    self.skipWaiting();
});

self.addEventListener("activate", (event) => {
    event.waitUntil(
        (async () => {
            // Enable navigation preload if it's supported.
            // See https://developers.google.com/web/updates/2017/02/navigation-preload
            if ("navigationPreload" in self.registration) {
                await self.registration.navigationPreload.enable();
            }

            var request = indexedDB.open('formData', 1);
            request.onupgradeneeded = function (event) {
                // Save the IDBDatabase interface
                db = event.target.result;

                // Create an objectStore for this database
                var objectStore = db.createObjectStore("data", { autoIncrement: true });
            };
            request.onsuccess = function(event) {
                // Save the IDBDatabase interface
                db = event.target.result;
            };
        })()
    );

    // Tell the active service worker to take control of the page immediately.
    self.clients.claim();
});

self.addEventListener("message", async (evt) => {
    const client = evt.source;
    if (evt.data == "upload-request") {
        /*let datas = await executeQuery("readonly", trans => trans.objectStore("data").getAll());
        let data = datas[0];
        await executeQuery("readwrite", trans => trans.objectStore("data").clear());*/
        if(formData)
            client.postMessage({ name: "upload-data", data: serializeForm(formData) });
        formData = null;
    }
});

self.addEventListener("fetch", (event) => {
/** @type {{event: FetchEvent}} */

    if (event.request.method === "POST" && !event.request.url.endsWith("/pwa-upload"))
        return;
    if (true) {
        event.respondWith(
            (async () => {
                try {
                    //console.log(event.request.url + ": Serving request..");
                    if (event.request.url.endsWith("/pwa-upload") && event.request.method === "POST") {
                        formData = await event.request.formData();
                        //await executeQuery("readwrite", trans => trans.objectStore("data").add(formData));
                        return Response.redirect("/Upload?pwa=true", 302);
                    }

                    const preloadResponse = await event.preloadResponse;
                    if (preloadResponse) {
                        //console.log(event.request.url + ": PRELOAD");
                        return preloadResponse;
                    }

                    const networkResponse = await fetch(event.request);
                    //console.log(event.request.url + ": NETWORK");
                    return networkResponse;
                } catch (error) {

                    console.log("Fetch failed; returning offline page instead.", error);

                    const cache = await caches.open(OFFLINE_CACHE_NAME);
                    const cachedResponse = await cache.match(OFFLINE_URL);
                    return cachedResponse;
                }
            })()
        );
    }
});