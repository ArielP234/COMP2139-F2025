document.addEventListener("DOMContentLoaded", function () {
    const searchInput = document.getElementById("event-search");
    const resultsDiv = document.getElementById("event-results");

    if (!searchInput || !resultsDiv) return;

    searchInput.addEventListener("input", function () {
        const term = searchInput.value;

        fetch(`/Events/Search?term=${encodeURIComponent(term)}`)
            .then(r => r.text())
            .then(html => {
                resultsDiv.innerHTML = html;
            });
    });
});