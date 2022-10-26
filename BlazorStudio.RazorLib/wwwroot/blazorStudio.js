window.blazorStudio = {
    addToLocalStorage(key, value) {
        localStorage[key] = value;
    },
    readLocalStorage(key) {
        return localStorage[key];
    },
    getViewportDimensions: function () {
        // Use the specified window or the current window if no argument
        let w = window;

        // This works for all browsers except IE8 and before
        if (w.innerWidth != null) return {widthInPixels: w.innerWidth, heightInPixels: w.innerHeight};

        // For IE (or any browser) in Standards mode
        var d = w.document;
        if (document.compatMode == "CSS1Compat")
            return {
                widthInPixels: d.documentElement.clientWidth,
                heightInPixels: d.documentElement.clientHeight
            };

        // For browsers in Quirks mode
        return {widthInPixels: d.body.clientWidth, heightInPixels: d.body.clientHeight};
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
        }

    },
    disposeVirtualizeCoordinateSystemInsersectionObserver: function (elementIds) {
        for (let i = 0; i < elementIds.length; i++) {
            let elementId = elementIds[i];

            let element = document.getElementById(elementId);
            this.intersectionObserver.unobserve(element);
        }

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
    checkIfInView: function (dotNetObjectReference, elementIds) {
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
        }

    },
    setScrollPosition: function (scrollLeft, scrollTop, elementId) {
        let element = document.getElementById(elementId);

        element.scrollLeft = scrollLeft;
        element.scrollTop = scrollTop;


    },
    initKeymap: function (keymapDotNetObjectReference) {
        document.addEventListener('keydown', (e) => {
            let keydownDto = {
                "key": e.key,
                "code": e.code,
                "ctrlWasPressed": e.ctrlKey,
                "shiftWasPressed": e.shiftKey,
                "altWasPressed": e.altKey
            };

            keymapDotNetObjectReference.invokeMethodAsync('DispatchHandleKeymapEvent', keydownDto);
        });
    },
    getRelativePosition: function (elementId, clientX, clientY) {
        let bounds = document
            .getElementById(elementId)
            .getBoundingClientRect();

        let x = clientX - bounds.left;
        let y = clientY - bounds.top;

        return {
            relativeX: x,
            relativeY: y
        };
    },
    measureFontSizeByElementId: function (elementId, amountOfCharactersRendered) {
        let row = document.getElementById(elementId);

        let heightOfARow = row.offsetHeight;

        /*
            Here I am looking at a test invisible row and getting offsetWidth
            of the single row's single token.
            
            That single token as of this comment contains 768 characters in it
            as the span width seemingly is not the
            character width when rendering 1 character only
         */
        let widthOfGivenRow = row.offsetWidth;

        let widthOfACharacter = widthOfGivenRow / amountOfCharactersRendered

        return {
            RowHeight: heightOfARow,
            CharacterWidth: widthOfACharacter
        };
    },
    measureDimensionsByElementId: function (elementId) {
        let element = document.getElementById(elementId);

        return {
            Width: element.offsetWidth,
            Height: element.offsetHeight
        };
    }
};

Blazor.registerCustomEventType('customkeydown', {
    browserEventName: 'keydown',
    createEventArgs: e => {
        if (e.code !== "Tab") {
            e.preventDefault();
        }

        return {
            "key": e.key,
            "code": e.code,
            "ctrlWasPressed": e.ctrlKey,
            "shiftWasPressed": e.shiftKey,
            "altWasPressed": e.altKey
        };
    }
});

Blazor.registerCustomEventType('customclick', {
    browserEventName: 'click',
    createEventArgs: e => {
        return {
            clientX: e.clientX,
            clientY: e.clientY,
            offsetX: e.offsetX,
            offsetY: e.offsetY,
            targetElementId: e.target.id,
            currentTargetId: e.currentTarget.id,
        };
    }
});