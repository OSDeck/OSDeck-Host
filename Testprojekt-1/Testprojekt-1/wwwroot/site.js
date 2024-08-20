function downloadFile(href, fileName) {
    var link = document.createElement('a');
    link.href = href;
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

window.initializeDragAndDrop = function (canvasContainer) {
    console.log("Initializing drag and drop");

    const canvas = canvasContainer.querySelector('.canvas');

    document.querySelectorAll('[draggable=true]').forEach(item => {
        item.addEventListener('dragstart', (e) => {
            console.log("Drag start:", e.target.getAttribute('data-item-type'));
            e.dataTransfer.setData('text/plain', e.target.getAttribute('data-item-type'));
        });
    });

    canvas.addEventListener('dragover', (e) => {
        e.preventDefault();
        console.log("Drag over canvas");
    });

    canvas.addEventListener('drop', (e) => {
        e.preventDefault();
        console.log("Drop event on canvas");

        const itemType = e.dataTransfer.getData('text/plain');
        console.log("Dropped item type:", itemType);

        if (!itemType) return;

        let newItem;

        switch (itemType) {
            case 'circle':
                newItem = document.createElement('div');
                newItem.className = 'shape-circle';
                break;
            case 'square':
                newItem = document.createElement('div');
                newItem.className = 'shape-square';
                break;
            case 'vertical-slider':
                newItem = document.createElement('div');
                newItem.className = 'slider-placeholder vertical-slider-placeholder';
                break;
            case 'horizontal-slider':
                newItem = document.createElement('div');
                newItem.className = 'slider-placeholder horizontal-slider-placeholder';
                break;
        }

        if (newItem) {
            const rect = canvas.getBoundingClientRect();
            newItem.style.position = 'absolute';
            newItem.style.left = `${e.clientX - rect.left}px`;
            newItem.style.top = `${e.clientY - rect.top}px`;
            canvas.appendChild(newItem);
            console.log("New item added to canvas:", newItem);
        }
    });
};
