document.addEventListener("DOMContentLoaded", function () {
    loadCategoryChart();
    loadRevenueChart();
});

function loadCategoryChart() {
    fetch("/api/Analytics/TicketSalesByCategory")
        .then(r => r.json())
        .then(data => {
            const labels = data.map(d => d.category);
            const values = data.map(d => d.tickets);

            const ctx = document.getElementById("categoryChart");
            new Chart(ctx, {
                type: "bar",
                data: {
                    labels: labels,
                    datasets: [{
                        label: "Tickets Sold",
                        data: values
                    }]
                }
            });
        });
}

function loadRevenueChart() {
    fetch("/api/Analytics/RevenuePerMonth")
        .then(r => r.json())
        .then(data => {
            const labels = data.map(d => d.month);
            const values = data.map(d => d.revenue);

            const ctx = document.getElementById("revenueChart");
            new Chart(ctx, {
                type: "line",
                data: {
                    labels: labels,
                    datasets: [{
                        label: "Revenue",
                        data: values
                    }]
                }
            });
        });
}