function downloadFile(href, fileName) {
    var link = document.createElement('a');
    link.href = href;
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

window.getCanvas = function() {
    const canvas = document.getElementsByClassName("canvas")[0];
    if (canvas) {
        const rect = canvas.getBoundingClientRect();
        return [rect.x, rect.y, rect.width, rect.height].map(Math.round);
    }

    return [0, 0, 0, 0];
}

window.getSize = function (shape) {
    let width, height = 0
    if (shape === "shape-circle") {
        width = document.getElementById("circleSize").value;
        height = width;
    } else if (shape === "shape-square") {
        width = document.getElementById("width").value;
        height = document.getElementById("height").value;
    } else if (shape === "vertical-slider") {
        width = 8;
        height = document.getElementById("sliderSize").value;
    } else if (shape === "horizontal-slider") {
        width = document.getElementById("sliderSize").value
        height = 8;
    }

    return [width, height].map(Math.round);
}

window.getColor = function () {
    let fill, stroke = "";
    fill = document.getElementById("fill").value;
    stroke = document.getElementById("stroke").value;
    return [fill, stroke];
}

document.querySelectorAll('.draggableItem').forEach(item => {
    item.addEventListener('click', function () {
        // Remove any class that was applied from the previous click
        document.querySelectorAll('.draggableItem').forEach(i => {
            i.classList.remove(i.dataset.class);
        });

        // Add the class from the data-class attribute to the clicked element
        this.classList.add(this.dataset.class);
    });
});

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
