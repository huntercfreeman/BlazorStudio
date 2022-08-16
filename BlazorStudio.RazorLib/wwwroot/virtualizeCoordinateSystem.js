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
    initializeVirtualizeIntersectionObserver: function (virtualizeCoordinateSystemIdentifier,
                                                        virtualizeCoordinateSystemDotNetReference,
                                                        virtualizeBoundaryIds) {

        if (!virtualizeBoundaryIds || virtualizeBoundaryIds.length === 0) {
            return;
        }
        
        // Need reference to parent element to know the scroll top and scroll events
        let blazorVirtualizeBoundary = document.getElementById(virtualizeBoundaryIds[0]);

        let parentElement = blazorVirtualizeBoundary.parentElement;

        parentElement.addEventListener('scroll', (event) => {
            let intersectionObserverWrap = this.intersectionObserverMap.get(virtualizeCoordinateSystemIdentifier);

            for (let i = 0; i < virtualizeBoundaryIds.length; i++) {
                let currentVirtualizeBoundaryId = virtualizeBoundaryIds[i];

                let previousIntersectionThreshold = intersectionObserverWrap.intersectionRatioMap
                    .get(currentVirtualizeBoundaryId);

                if (previousIntersectionThreshold >= 0) {
                    // In view
                    let parentElement = blazorVirtualizeBoundary.parentElement;

                    let scrollDimensions = {
                        ScrollLeft: parentElement.scrollLeft,
                        ScrollTop: parentElement.scrollTop
                    };

                    virtualizeCoordinateSystemDotNetReference.invokeMethodAsync("OnIntersectionObserverThresholdChanged", scrollDimensions);
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
                    virtualizeCoordinateSystemDotNetReference,
                    blazorVirtualizeBoundary,
                    virtualizeCoordinateSystemIdentifier), 
            options);
        
        let intersectionRatioMap = new Map();
        
        this.intersectionObserverMap.set(virtualizeCoordinateSystemIdentifier, {
            intersectionObserver: intersectionObserver,
            intersectionRatioMap: intersectionRatioMap
        });
        
        for (let i = 0; i < virtualizeBoundaryIds.length; i++) {
            let currentId = virtualizeBoundaryIds[i];
            
            if (currentId) {
                let element = document.getElementById(currentId);

                intersectionRatioMap.set(currentId, -1);
                
                intersectionObserver.observe(element);
            }
        }
    },
    disposeVirtualizeIntersectionObserver: function (virtualizeCoordinateSystemIdentifier,
                                                        virtualizeBoundaryIds) {
        let intersectionObserverWrap = this.intersectionObserverMap.get(virtualizeCoordinateSystemIdentifier);

        this.intersectionObserverMap.delete(virtualizeCoordinateSystemIdentifier);

        intersectionObserverWrap.intersectionObserver.disconnect();
    },
    handleThresholdChange: function (entries, virtualizeCoordinateSystemDotNetReference, blazorVirtualizeBoundary, virtualizeCoordinateSystemIdentifier) {
        let intersectionObserverWrap = this.intersectionObserverMap.get(virtualizeCoordinateSystemIdentifier);

        for (let i = 0; i < entries.length; i++) {
            let currentEntry = entries[i];

            intersectionObserverWrap.intersectionRatioMap
                .set(currentEntry.target.id, currentEntry.intersectionRatio);
        }
        
        for (let i = 0; i < entries.length; i++) {
            let currentEntry = entries[i];

            if (currentEntry.intersectionRatio >= 0) {
                // Scrolling into view

                let parentElement = blazorVirtualizeBoundary.parentElement;

                let scrollDimensions = {
                    ScrollLeft: parentElement.scrollLeft,
                    ScrollTop: parentElement.scrollTop
                };
                
                virtualizeCoordinateSystemDotNetReference.invokeMethodAsync("OnIntersectionObserverThresholdChanged", scrollDimensions);
                return;
            }
        }
    },
};
