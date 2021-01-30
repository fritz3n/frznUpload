
(() => {

	if (document.location.search.includes("pwa=true")) {
		if (!navigator.serviceWorker.controller)
			return;
		navigator.serviceWorker.ready
			.then(registration => {
				navigator.serviceWorker.onmessage = function (e) {
					if (!e.data.name === "upload-data")
						return;

					droppedFiles = [e.data.data.file];
					form.trigger('submit');
				}
				registration.active.postMessage("upload-request");

			});
	}

})();