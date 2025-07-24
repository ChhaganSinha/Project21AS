window.setup = (id, config, dotNetHelper) => {
    const ctx = document.getElementById(id);
    if (!ctx) return;

    const chart = new Chart(ctx, {
        type: config.Type,
        data: config.Data,
        options: config.Options
    });

    if (dotNetHelper) {
        ctx.onclick = function (evt) {
            const points = chart.getElementsAtEventForMode(evt, 'nearest', { intersect: true }, true);
            if (points.length > 0) {
                const firstPoint = points[0];
                const label = chart.data.labels[firstPoint.index];
                const datasetLabel = chart.data.datasets[firstPoint.datasetIndex].label;
                dotNetHelper.invokeMethodAsync('ChartClick', datasetLabel, label, id);
            }
        };
    }
};
