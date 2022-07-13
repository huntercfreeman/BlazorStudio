window.blazorStudioComponents = {
    addToLocalStorage(key, value) {
         localStorage[key] = value;
    },
    readLocalStorage(key) {
         return localStorage[key];
    },
    getViewportDimensions: function() {
        // Use the specified window or the current window if no argument
        let w = window;

        // This works for all browsers except IE8 and before
        if (w.innerWidth != null) return { widthInPixels: w.innerWidth, heightInPixels: w.innerHeight };

        // For IE (or any browser) in Standards mode
        var d = w.document;
        if (document.compatMode == "CSS1Compat")
            return {
                widthInPixels: d.documentElement.clientWidth,
                heightInPixels: d.documentElement.clientHeight
            };

        // For browsers in Quirks mode
        return { widthInPixels: d.body.clientWidth, heightInPixels: d.body.clientHeight };
    }
};