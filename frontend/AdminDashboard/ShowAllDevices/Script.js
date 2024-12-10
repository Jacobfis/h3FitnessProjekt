// Hent token fra localStorage
let token = localStorage.getItem("authToken");

// Function to extract userId from JWT token
function getUserIdFromToken(token) {
    try {
        const payload = JSON.parse(atob(token.split('.')[1])); // Decodes the token payload
        console.log(payload);
        return payload.sub; // Adjust this if `userId` is named differently in the token
    } catch (error) {
        console.error("Failed to parse token payload:", error);
        return null;
    }
}

// Check if token has expired
function isTokenExpired(token) {
    const expirationTime = getTokenExpiration(token); // Replace with actual expiration time logic
    return Date.now() >= expirationTime;
}

// Function to update login/logout buttons
function updateAuthButtons() {
    const authButton = document.getElementById("authButton");
    const signupButton = document.getElementById("signupButton");

    token = localStorage.getItem("authToken");

    if (token) {
        authButton.textContent = "Logud";
        signupButton.style.display = "none";

        authButton.onclick = () => {
            localStorage.removeItem("authToken");
            updateAuthButtons();
            alert("Du er nu logget ud");
            window.location.href = "index.html";
        };
    } else {
        authButton.textContent = "Login";
        signupButton.style.display = "inline-block";

        authButton.onclick = () => alert("Åbn login-formular");
        signupButton.onclick = () => alert("Åbn opret-formular");
    }
}

// Function to add device to UserSitSmart
function addDeviceToUser(DeviceId) {
    const userId = getUserIdFromToken(token);
    
    if (!userId) {
        alert("Unable to retrieve user ID. Please log in again.");
        localStorage.removeItem("authToken");
        updateAuthButtons();
        window.location.href = "login.html";
        return;
    }

    const userDeviceData = JSON.stringify({
        userId: userId,
        DeviceId: DeviceId
    });

    fetch("https://h3-fitsmart2024.onrender.com/api/UserDevice", {
        method: "POST",
        headers: {
            "Authorization": `Bearer ${token}`,
            "Content-Type": "application/json",
            "Accept": "application/json"
        },
        body: userDeviceData
    })
    .then(response => {
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        alert("Device successfully linked to user!");
    })
    .catch(error => {
        console.error("Error linking device to user:", error);
        alert(`Failed to link device to user: ${error.message}`);
    });
}

function isTokenExpired(token) {
    const expirationTime = getTokenExpiration(token);
    return Date.now() >= expirationTime;
}
function getTokenExpiration(token) {
    const payloadBase64 = token.split(".")[1];
    const payload = JSON.parse(atob(payloadBase64));
    return payload.exp * 1000; // Returns as milliseconds
}
// Function to fetch and display devices
function fetchDevices() {
    if (!token || isTokenExpired(token)) {
        localStorage.removeItem("authToken");
        alert("Session expired. Please log in again.");
        updateAuthButtons();
        window.location.href = "login.html";
        return;
    }

    const headers = new Headers();
    headers.append("Accept", "application/json");
    headers.append("Authorization", `Bearer ${token}`);

    fetch("https://h3-fitsmart2024.onrender.com/api/Devices", { method: "GET", headers })
        .then(response => {
            if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
            return response.json();
        })
        .then(devices => {
            const deviceList = document.getElementById("deviceList");
            deviceList.innerHTML = "";

            devices.forEach(device => {
                const deviceItem = document.createElement("div");
                deviceItem.classList.add("device-item");

                deviceItem.innerHTML = `
                    <h3>Device ID: ${device.id}</h3>
                    <button class="link-device-button" onclick="addDeviceToUser('${device.id}')">Link Device</button>
                    <button class="delete-device-button" onclick="deleteDevice('${device.id}')">Delete</button>
                `;

                deviceList.appendChild(deviceItem);
            });
        })
        .catch(error => {
            console.error("Error:", error);
            document.getElementById("deviceList").textContent = `Could not fetch devices: ${error.message}`;
        });
}

// Initialize UI and fetch devices on load
document.addEventListener("DOMContentLoaded", function() {
    updateAuthButtons();
    if (token && !isTokenExpired(token)) {
        fetchDevices();
    }
});
