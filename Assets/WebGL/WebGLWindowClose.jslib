mergeInto(LibraryManager.library, {
    QuitWebGLInstance: function () {
        if (typeof unityInstance !== 'undefined') {
            unityInstance.Quit().then(function() {
                // Optional: Redirect the user or hide the canvas area
                window.location.href = "about:blank";
                window.close();
            });
        }
    }
});
