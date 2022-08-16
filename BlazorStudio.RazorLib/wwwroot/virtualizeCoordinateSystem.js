window.virtualizeCoordinateSystem = {
    intersectionObserverMap: new Map(),
    getVirtualizeItemDimensions: function (virtualizeItemLocatorElementReference) {
        let renderedItem = virtualizeItemLocatorElementReference
            .previousElementSibling;

        let parentElement = virtualizeItemLocatorElementReference
            .parentElement;
        
        return {
            WidthOfItemInPixels: renderedItem.offsetWidth,
            HeightOfItemInPixels: renderedItem.offsetHeight,
            WidthOfScrollableContainerInPixels: parentElement.offsetWidth,
            HeightOfScrollableContainerInPixels: parentElement.offsetHeight
        };
    },
    getVirtualizeScrollDimensions: function (virtualizeItemLocatorElementReference) {
        let parentElement = virtualizeItemLocatorElementReference
            .parentElement;
        
        return {
            ScrollLeft: parentElement.scrollLeft,
            ScrollTop: parentElement.scrollTop
        };
    },
    subscribeToVirtualizeScrollEvent: function (virtualizeItemLocatorElementReference,
                                                virtualizeCoordinateSystemDotNetReference) {
        let parentElement = virtualizeItemLocatorElementReference
            .parentElement;

        parentElement.addEventListener('scroll', (event) => {
            virtualizeCoordinateSystemDotNetReference.invokeMethodAsync("OnParentElementScrollEvent", {
                    ScrollLeft: parentElement.scrollLeft,
                    ScrollTop: parentElement.scrollTop
                });
        }, true)
    },
    initializeVirtualizeIntersectionObserver: function (virtualizeCoordinateSystemIdentifier,
                                                        virtualizeCoordinateSystemDotNetReference,
                                                        virtualizeBoundaryIds) {

        let options = {
            rootMargin: '0px',
            threshold: [
                .1
            ]
        }

        let intersectionObserver = new IntersectionObserver(
            (entries) => 
                this.handleThresholdChange(entries, virtualizeCoordinateSystemDotNetReference), 
            options);
        
        this.intersectionObserverMap.set(virtualizeCoordinateSystemIdentifier, intersectionObserver);
        
        for (let i = 0; i < virtualizeBoundaryIds.length; i++) {
            let currentId = virtualizeBoundaryIds[i];
            
            if (currentId) {
                let element = document.getElementById(currentId);
                
                intersectionObserver.observe(element);
            }
        }
    },
    disposeVirtualizeIntersectionObserver: function (virtualizeCoordinateSystemIdentifier,
                                                        virtualizeBoundaryIds) {
        let intersectionObserver = this.intersectionObserverMap.get(virtualizeCoordinateSystemIdentifier);
        
        intersectionObserver.disconnect();
    },
    handleThresholdChange: function (entries, virtualizeCoordinateSystemDotNetReference) {
        for (let i = 0; i < entries.length; i++) {
            let currentEntry = entries[i];

            if (currentEntry.intersectionRatio >= .1) {
                // Scrolling into view (whereas < .1 would be scrolling out of view)

                virtualizeCoordinateSystemDotNetReference.invokeMethodAsync("OnIntersectionObserverThresholdChanged");
            }
        }
    },
};
