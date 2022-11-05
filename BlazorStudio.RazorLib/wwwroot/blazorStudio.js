window.blazorStudio = {
    getContextMenuFixedPositionRelativeToElement: function (elementId) {
        let element = document.getElementById(elementId);

        let bounds = element.getBoundingClientRect();

        return {
            OccurredDueToMouseEvent: false,
            LeftPositionInPixels: bounds.left,
            TopPositionInPixels: bounds.top + bounds.height
        }
    },
}

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