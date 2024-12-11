// API endpoint URL
const apiUrl = "https://h3-fitsmart2024.onrender.com/api/ActivityLog";

// Function to fetch and display data
async function loadActivityLogs() {
    try {
        const response = await fetch(apiUrl);
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const data = await response.json();

        // Get table body
        const tableBody = document.getElementById("activityLogTable").querySelector("tbody");

        // Clear existing rows
        tableBody.innerHTML = "";

        // Populate table with data
        data.forEach(log => {
            const row = document.createElement("tr");

            row.innerHTML = `
                <td>${log.id}</td>
                <td>${new Date(log.date).toLocaleDateString()}</td>
                <td>${log.steps}</td>
                <td>${log.distance.toFixed(2)}</td>
                <td>${log.calories.toFixed(2)}</td>
                <td>${formatDuration(log.duration)}</td>
                <td>${log.type}</td>
            `;

            tableBody.appendChild(row);
        });
    } catch (error) {
        console.error("Failed to fetch activity logs:", error);
    }
}

// Helper function to format duration (ISO 8601)
function formatDuration(duration) {
    const totalSeconds = duration.totalSeconds || (duration.hours * 3600 + duration.minutes * 60 + duration.seconds);
    const hours = Math.floor(totalSeconds / 3600);
    const minutes = Math.floor((totalSeconds % 3600) / 60);
    const seconds = totalSeconds % 60;

    return `${hours}h ${minutes}m ${seconds}s`;
}

// Load activity logs when the page loads
window.onload = loadActivityLogs;