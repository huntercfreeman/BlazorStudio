window.plainTextEditor = {
    intersectionObserver: 0,
    elementByIdIsIntersecting: new Map(),
    getActiveRowId: function (plainTextEditorGuid) {
        return `pte_active-row_${plainTextEditorGuid}`;
    },
    getPlainTextEditorId: function (plainTextEditorGuid) {
        return `pte_plain-text-editor-display_${plainTextEditorGuid}`;
    },
    initOnKeyDownProvider: function (onKeyDownProviderDisplayReference) {
        document.addEventListener('keydown', (e) => {
            if (e.key === "Tab") {
                e.preventDefault();
            }
            if (e.key === "a" && e.ctrlKey) {
                e.preventDefault();
            }

            let dto = {
                "key": e.key,
                "code": e.code,
                "ctrlWasPressed": e.ctrlKey,
                "shiftWasPressed": e.shiftKey,
                "altWasPressed": e.altKey
            };

            onKeyDownProviderDisplayReference.invokeMethodAsync('FireOnKeyDownEvent', dto);
        });
    },
    clearInputElement: function (inputElementReference) {
        inputElementReference.value = "";
    },
    scrollIntoViewIfOutOfViewport: function (elementId) {
        const value = this.elementByIdIsIntersecting.get(elementId);

        if (value.intersectionRatio >= 1) {
            return;
        }

        let element = document.getElementById(elementId);
        let plainTextEditorDisplay = document.getElementById(this.getPlainTextEditorId(value.plainTextEditorGuid));

        plainTextEditorDisplay.scrollTop = element.offsetTop - 5;
    },
    initializeIntersectionObserver: function () {
        let options = {
            rootMargin: '0px',
            threshold: [
                0, .25, .50, .75, 1
            ]
        }

        this.intersectionObserver = new IntersectionObserver((entries) => this.handleThresholdChange(entries, this.elementByIdIsIntersecting), options);
    },
    subscribeScrollIntoView: function (elementId, plainTextEditorGuid) {
        this.elementByIdIsIntersecting.set(elementId, {
            intersectionRatio: 0,
            plainTextEditorGuid: plainTextEditorGuid
        });

        let element = document.getElementById(elementId);
        this.intersectionObserver.observe(element);
    },
    disposeScrollIntoView: function (elementId) {
        let element = document.getElementById(elementId);
        this.intersectionObserver.unobserve(element);
    },
    handleThresholdChange: function (entries, elementByIdIsIntersecting) {
        for (let i = 0; i < entries.length; i++) {
            let currentEntry = entries[i];

            let previousValue = elementByIdIsIntersecting.get(currentEntry.target.id);

            elementByIdIsIntersecting.set(currentEntry.target.id, {
                intersectionRatio: currentEntry.intersectionRatio,
                plainTextEditorGuid: previousValue.plainTextEditorGuid
            });
        }
    },
    getScrollTop: function (plainTextEditorId) {
        let element = document.getElementById(plainTextEditorId);

        return element.scrollTop;
    }
};
