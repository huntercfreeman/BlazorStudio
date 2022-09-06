window.virtualizeCoordinateSystem = {
    intersectionObserverMap: new Map(),
    initializeVirtualizeIntersectionObserver: function (leftBoundary,
                                                        bottomBoundary,
                                                        topBoundary,
                                                        rightBoundary,
                                                        virtualizeCoordinateSystemDotNetObjectReference,
                                                        intersectionObserverMapKey) {

        let options = {
            rootMargin: '0px',
            threshold: [
                0
            ]
        }

        let intersectionObserver = new IntersectionObserver(
            (entries) =>
                this.handleThresholdChange(entries,
                    virtualizeCoordinateSystemDotNetObjectReference),
            options);

        intersectionObserver.observe(leftBoundary);
        intersectionObserver.observe(bottomBoundary);
        intersectionObserver.observe(topBoundary);
        intersectionObserver.observe(rightBoundary);

        this.intersectionObserverMap.set(intersectionObserverMapKey, intersectionObserver);
    },
    disposeVirtualizeIntersectionObserver: function (intersectionObserverMapKey) {
        let intersectionObserverWrap = this.intersectionObserverMap.get(intersectionObserverMapKey);

        this.intersectionObserverMap.delete(intersectionObserverMapKey);

        intersectionObserverWrap.intersectionObserver.disconnect();
    },
    handleThresholdChange: function (entries, virtualizeCoordinateSystemDotNetObjectReference) {
        for (let i = 0; i < entries.length; i++) {
            let currentEntry = entries[i];

            if (currentEntry.intersectionRatio >= 0) {
                // Scrolling into view

                let parentElement = currentEntry.target.parentElement;

                let scrollPosition = {
                    scrollLeft: parentElement.scrollLeft,
                    scrollTop: parentElement.scrollTop
                };

                virtualizeCoordinateSystemDotNetObjectReference
                    .invokeMethodAsync(
                        "OnIntersectionObserverThresholdChanged", 
                        scrollPosition);
                return;
            }
        }
    },
};
