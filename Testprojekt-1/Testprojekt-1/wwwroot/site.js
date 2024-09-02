function downloadFile(href, fileName) {
    var link = document.createElement('a');
    link.href = href;
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

window.initializeDragAndDrop = function (canvasContainer) {
    const items = document.querySelectorAll('[draggable=true]');

    items.forEach(item => {
        item.addEventListener('mousedown', (e) => {
            let isDragging = false;
            item.classList.add('dragging');
            item.style.position = 'absolute';  // Ensure it's absolutely positioned
            item.style.zIndex = '1000';        // Bring to front
            item.style.opacity = '0.9';        // Ensure visibility

            const offsetX = e.clientX - item.getBoundingClientRect().left;
            const offsetY = e.clientY - item.getBoundingClientRect().top;

            function onMouseMove(e) {
                if (!isDragging) {
                    isDragging = true;
                }

                // Calculate the new position relative to the canvas
                const canvasRect = canvasContainer.getBoundingClientRect();
                let newX = e.clientX - offsetX;
                let newY = e.clientY - offsetY;

                // Constrain within the canvas
                newX = Math.max(0, Math.min(newX, canvasRect.width - item.offsetWidth));
                newY = Math.max(0, Math.min(newY, canvasRect.height - item.offsetHeight));

                item.style.left = `${newX}px`;
                item.style.top = `${newY}px`;

                // Log the positions for debugging
                console.log(`Dragging to: X: ${newX}, Y: ${newY}`);
            }

            function onMouseUp() {
                isDragging = false;
                item.classList.remove('dragging');
                item.style.zIndex = '';        // Reset z-index
                item.style.opacity = '';       // Reset opacity
                document.removeEventListener('mousemove', onMouseMove);
                document.removeEventListener('mouseup', onMouseUp);
            }

            document.addEventListener('mousemove', onMouseMove);
            document.addEventListener('mouseup', onMouseUp);
        });
    });
};

document.addEventListener('DOMContentLoaded', function () {
    const canvasContainer = document.querySelector('.canvas-container');
    if (canvasContainer) {
        initializeDragAndDrop(canvasContainer);
    }
});
