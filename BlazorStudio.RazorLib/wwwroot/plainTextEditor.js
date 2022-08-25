window.plainTextEditor = {
    intersectionObserver: 0,
    elementByIdIsIntersecting: new Map(),
    getActiveRowId: function (plainTextEditorGuid) {
        return `pte_active-row_${plainTextEditorGuid}`;
    },
    getPlainTextEditorId: function (plainTextEditorGuid) {
        return `pte_plain-text-editor-display_${plainTextEditorGuid}`;
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
    },
    widthAndHeightTest: function (widthAndHeightTestId) {
        let row = document.getElementById(widthAndHeightTestId);

        let heightOfARow = row.offsetHeight;
        
        // TODO: I feel like this is not a good way to find the width of character.
        /*
            Here I am looking at a test invisible row and getting offsetWidth
            of the single row's single token.
            
            That single token as of this comment contains 768 characters in it
            as the span width seemingly is not the
            character width when rendering 1 character only
         */
        let widthOfTestToken = row.children[1].children[1].offsetWidth;

        let amountOfCharactersRendered = 768;
        
        let widthOfACharacter = widthOfTestToken / amountOfCharactersRendered
        
        /*
            -Plain text editor html element
                -Virtualization html element
                    -Row html element
                    
            therefore psuedo code:
                WidthOfEditor -> row.parent.parent.width
                HEightOfEditor -> row.parent.parent.height
         */
        
        let widthOfEditor = row.parentElement.parentElement.offsetWidth;
        let heightOfEditor = row.parentElement.parentElement.offsetHeight;
        
        return {
            HeightOfARow: heightOfARow,
            WidthOfACharacter: widthOfACharacter,
            WidthOfEditor: widthOfEditor,
            HeightOfEditor: heightOfEditor
        };
    },
    readClipboard: async function () {
        // First, ask the Permissions API if we have some kind of access to
        // the "clipboard-read" feature.

        try {
            return await navigator.permissions.query({ name: "clipboard-read" }).then(async (result) => {
                // If permission to read the clipboard is granted or if the user will
                // be prompted to allow it, we proceed.

                if (result.state === "granted" || result.state === "prompt") {
                    return await navigator.clipboard.readText().then((data) => {
                        return data;
                    });
                }
                else {
                    return "";
                }
            });
        }
        catch (e) {
            return "";
        }
    },
    setClipboard: function (value) {
        // Copies a string to the clipboard. Must be called from within an
        // event handler such as click. May return false if it failed, but
        // this is not always possible. Browser support for Chrome 43+,
        // Firefox 42+, Safari 10+, Edge and Internet Explorer 10+.
        // Internet Explorer: The clipboard feature may be disabled by
        // an administrator. By default a prompt is shown the first
        // time the clipboard is used (per session).
        if (window.clipboardData && window.clipboardData.setData) {
            // Internet Explorer-specific code path to prevent textarea being shown while dialog is visible.
            return window.clipboardData.setData("Text", text);

        }
        else if (document.queryCommandSupported && document.queryCommandSupported("copy")) {
            var textarea = document.createElement("textarea");
            textarea.textContent = value;
            textarea.style.position = "fixed";  // Prevent scrolling to bottom of page in Microsoft Edge.
            document.body.appendChild(textarea);
            textarea.select();
            try {
                return document.execCommand("copy");  // Security exception may be thrown by some browsers.
            }
            catch (ex) {
                console.warn("Copy to clipboard failed.", ex);
                return false;
            }
            finally {
                document.body.removeChild(textarea);
            }
        }
    }
};
