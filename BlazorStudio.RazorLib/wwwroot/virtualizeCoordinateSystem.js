window.virtualizeCoordinateSystem = {
    intersectionObserverMap: new Map(),
    initializeVirtualizeIntersectionObserver: function (leftBoundary,
                                                        bottomBoundary,
                                                        topBoundary,
                                                        rightBoundary,
                                                        virtualizeCoordinateSystemDotNetObjectReference,
                                                        intersectionObserverMapKey) {

        let parentElement = leftBoundary.parentElement;

        parentElement.addEventListener('scroll', (event) => {
            let intersectionObserverWrap = this.intersectionObserverMap.get(intersectionObserverMapKey);

            let values = Array.from(intersectionObserverWrap.intersectionRatioMap.values());

            for (let i = 0; i < values.length; i++) {
                let currentValue = values[i];

                if (currentValue > 0) {
                    // In view
                    let parentElement = leftBoundary.parentElement;

                    virtualizeCoordinateSystemDotNetObjectReference.invokeMethodAsync(
                        "OnIntersectionObserverThresholdChanged",
                        parentElement.scrollLeft,
                        parentElement.scrollTop);

                    return;
                }
            }
        }, true)

        let options = {
            rootMargin: '0px',
            threshold: [
                0
            ]
        }

        let intersectionObserver = new IntersectionObserver(
            (entries) =>
                this.handleThresholdChange(entries,
                    virtualizeCoordinateSystemDotNetObjectReference,
                    intersectionObserverMapKey),
            options);

        let intersectionRatioMap = new Map();

        this.intersectionObserverMap.set(intersectionObserverMapKey, {
            intersectionObserver: intersectionObserver,
            intersectionRatioMap: intersectionRatioMap
        });

        intersectionRatioMap.set(leftBoundary.id, 0);
        intersectionObserver.observe(leftBoundary);

        intersectionRatioMap.set(bottomBoundary.id, 0);
        intersectionObserver.observe(bottomBoundary);

        intersectionRatioMap.set(topBoundary.id, 0);
        intersectionObserver.observe(topBoundary);

        intersectionRatioMap.set(rightBoundary.id, 0);
        intersectionObserver.observe(rightBoundary);
    },
    disposeVirtualizeIntersectionObserver: function (intersectionObserverMapKey) {
        let intersectionObserverWrap = this.intersectionObserverMap.get(intersectionObserverMapKey);

        this.intersectionObserverMap.delete(intersectionObserverMapKey);

        intersectionObserverWrap.intersectionObserver.disconnect();
    },
    handleThresholdChange: function (entries, virtualizeCoordinateSystemDotNetObjectReference, intersectionObserverMapKey) {
        let intersectionObserverWrap = this.intersectionObserverMap.get(intersectionObserverMapKey);

        for (let i = 0; i < entries.length; i++) {
            let currentEntry = entries[i];

            intersectionObserverWrap.intersectionRatioMap
                .set(currentEntry.target.id, currentEntry.intersectionRatio);
        }

        for (let i = 0; i < entries.length; i++) {
            let currentEntry = entries[i];

            if (currentEntry.intersectionRatio > 0) {
                // Scrolling into view

                let parentElement = currentEntry.target.parentElement;

                virtualizeCoordinateSystemDotNetObjectReference
                    .invokeMethodAsync(
                        "OnIntersectionObserverThresholdChanged",
                        parentElement.scrollLeft,
                        parentElement.scrollTop);
                return;
            }
        }
    },
};
