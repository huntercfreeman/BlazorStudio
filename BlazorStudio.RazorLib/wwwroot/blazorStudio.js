window.blazorStudio = {
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
    },
    intersectionObserver: 0,
    dotNetObjectReferenceByVirtualizeCoordinateSystemElementId: new Map(),
    scrollIntoViewIfOutOfViewport: function (elementId) {
        const value = this.dotNetObjectReferenceByVirtualizeCoordinateSystemElementId.get(elementId);

        if (value.intersectionRatio >= 1) {
            return;
        }

        let element = document.getElementById(elementId);
        let plainTextEditorDisplay = document.getElementById(this.getPlainTextEditorId(value.plainTextEditorGuid));

        plainTextEditorDisplay.scrollTop = element.offsetTop - 5;
    },
    initializeVirtualizeCoordinateSystemIntersectionObserver: function () {
        let options = {
            rootMargin: '0px',
            threshold: [
                0
            ]
        }

        this.intersectionObserver = new IntersectionObserver((entries) =>
            this.handleThresholdChange(entries,
                this.dotNetObjectReferenceByVirtualizeCoordinateSystemElementId),
            options);
    },
    subscribeVirtualizeCoordinateSystemInsersectionObserver: function (dotNetObjectReference, elementIds) {
        for (let i = 0; i < elementIds.length; i++) {
            let elementId = elementIds[i];

            this.dotNetObjectReferenceByVirtualizeCoordinateSystemElementId.set(elementId, {
                    dotNetObjectReference: dotNetObjectReference,
                    intersectionRatio: 0
            });

            let element = document.getElementById(elementId);

            this.intersectionObserver.observe(element);
        };
    },
    disposeVirtualizeCoordinateSystemInsersectionObserver: function (elementIds) {
        for (let i = 0; i < elementIds.length; i++) {
            let elementId = elementIds[i];

            let element = document.getElementById(elementId);
            this.intersectionObserver.unobserve(element);
        };
    },
    handleThresholdChange: function (entries, dotNetObjectReferenceByVirtualizeCoordinateSystemElementId) {
        for (let i = 0; i < entries.length; i++) {
            let currentEntry = entries[i];

            let mapValue = dotNetObjectReferenceByVirtualizeCoordinateSystemElementId.get(currentEntry.target.id);

            dotNetObjectReferenceByVirtualizeCoordinateSystemElementId.set(currentEntry.target.id, {
                dotNetObjectReference: mapValue.dotNetObjectReference,
                intersectionRatio: currentEntry.intersectionRatio
            });
        }
    },
    getDimensions: function (elementId, dotNetObjectReference) {
        let elementReference = document.getElementById(elementId);

        dotNetObjectReference.invokeMethodAsync("FireRequestCallbackAction",
            "",
            elementReference.scrollLeft,
            elementReference.scrollTop,
            elementReference.scrollWidth,
            elementReference.scrollHeight,
            elementReference.offsetWidth,
            elementReference.offsetHeight);
    },
    checkIfInView: function(dotNetObjectReference, elementIds) {
        for (let i = 0; i < elementIds.length; i++) {
            let elementId = elementIds[i];

            let mapValue = this.dotNetObjectReferenceByVirtualizeCoordinateSystemElementId.get(elementId);

            if (mapValue.intersectionRatio > 0) {
                let boundaryElement = document.getElementById(elementId);

                if (boundaryElement.offsetWidth === 0 || boundaryElement.offsetHeight === 0) {
                    continue;
                }

                // Scrolling into view
                const splitId = elementId.split("_");

                const isolatedGuid = splitId[splitId.length - 1];

                var virtualizeCoordinateSystemElementId = `bstudio_virtualize-coordinate-system_${isolatedGuid}`;

                let elementVirtualizeCoordinateSystem = document.getElementById(virtualizeCoordinateSystemElementId);

                dotNetObjectReference.invokeMethodAsync("FireRequestCallbackAction",
                    elementId,
                    elementVirtualizeCoordinateSystem.scrollLeft,
                    elementVirtualizeCoordinateSystem.scrollTop,
                    elementVirtualizeCoordinateSystem.scrollWidth,
                    elementVirtualizeCoordinateSystem.scrollHeight,
                    elementVirtualizeCoordinateSystem.offsetWidth,
                    elementVirtualizeCoordinateSystem.offsetHeight);

                return;
            }
        };
    },
    setScrollPosition: function (scrollLeft, scrollTop, elementId) {
        let element = document.getElementById(elementId);

        element.scrollLeft = scrollLeft;
        element.scrollTop = scrollTop;

        return;
    },
};