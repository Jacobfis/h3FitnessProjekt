// Hent token fra localStorage
let token = localStorage.getItem("authToken");

// Funktion til at opdatere knapperne baseret på loginstatus
function updateAuthButtons() {
    const authButton = document.getElementById("authButton");
    const signupButton = document.getElementById("signupButton");

    // Tjek om token findes i localStorage
    token = localStorage.getItem("authToken");

    if (token && !isTokenExpired(token)) {
        // Hvis token findes og ikke er udløbet (logget ind)
        authButton.textContent = "Logud";
        signupButton.style.display = "none"; // Skjuler "Opret"-knappen

        // Ændrer knapfunktion til logud
        authButton.onclick = () => {
            localStorage.removeItem("authToken"); // Fjern token ved logud
            updateAuthButtons(); // Opdater knapper efter logud
            alert("Du er nu logget ud");
            window.location.href = "index.html"; // Omdiriger til index.html efter logud
        };
    } else {
        // Hvis token ikke findes eller er udløbet (ikke logget ind)
        authButton.textContent = "Login";
        signupButton.style.display = "inline-block"; // Viser "Opret"-knappen igen
        localStorage.removeItem("authToken"); // Fjern udløbet token
    }
}

// Funktion til at kontrollere om token er udløbet
function isTokenExpired(token) {
    const expirationTime = getTokenExpiration(token);
    return Date.now() >= expirationTime;
}

// Funktion til at få tokenens udløbstidspunkt fra JWT (hvis det er i JWT format)
function getTokenExpiration(token) {
    const payloadBase64 = token.split(".")[1];
    const payload = JSON.parse(atob(payloadBase64));
    return payload.exp * 1000; // Returner som millisekunder
}

// Funktion til at hente og vise brugerdata
function fetchUserData() {
    if (!token || isTokenExpired(token)) {
        localStorage.removeItem("authToken"); // Fjern token hvis udløbet
        alert("Session expired. Please log in again.");
        updateAuthButtons();
        window.location.href = "login.html"; // Viderestil til login-siden hvis token er udløbet
        return;
    }

    const myHeaders = new Headers();
    myHeaders.append("Accept", "application/json");
    myHeaders.append("Authorization", `Bearer ${token}`);

    fetch("https://h3-fitsmart2024.onrender.com/api/Users", { method: "GET", headers: myHeaders })
        .then(response => {
            if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
            return response.json();
        })
        .then(data => {
            const userList = document.getElementById("userList");
            userList.innerHTML = ""; // Ryd tidligere indhold

            data.forEach(user => {
                const userCard = document.createElement("div");
                userCard.classList.add("user-card");
                
                // Startopbygning af HTML-strukturen for hvert brugerkort
                userCard.innerHTML = `
                    <h3><span id="name-${user.id}">${user.name}</span></h3>
                    <p><strong>Email:</strong> <span id="email-${user.id}">${user.email}</span></p>
                    <button id="editButton-${user.id}" onclick="toggleEditUser('${user.id}')">Edit</button>
                    <button class="delete-button" onclick="deleteUser('${user.id}')">Delete</button>
                `;

                userList.appendChild(userCard);
            });
        })
        .catch(error => {
            console.error("Error:", error);
            document.getElementById("userList").textContent = `Could not fetch data: ${error.message}`;
        });
}

// Funktion til at slette bruger
function deleteUser(userId) {
    if (confirm("Er du sikker på, at du vil slette denne bruger?")) {
        if (!token || isTokenExpired(token)) {
            localStorage.removeItem("authToken");
            alert("Session expired. Please log in again.");
            updateAuthButtons();
            window.location.href = "login.html";
            return;
        }

        fetch(`https://h3-fitsmart2024.onrender.com/api/Users/${userId}`, {
            method: "DELETE",
            headers: {
                "Authorization": `Bearer ${token}`,
                "Accept": "application/json"
            }
        })
        .then(response => {
            if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
            fetchUserData(); // Opdater brugerlisten efter sletning
        })
        .catch(error => {
            console.error("Error:", error);
            alert(`Kunne ikke slette brugeren: ${error.message}`);
        });
    }
}

// Funktion til at skifte mellem visningstilstand og redigeringstilstand
function toggleEditUser(userId) {
    const nameElement = document.getElementById(`name-${userId}`);
    const emailElement = document.getElementById(`email-${userId}`);
    const editButton = document.getElementById(`editButton-${userId}`);

    if (editButton.textContent === "Edit") {
        const currentName = nameElement.textContent;
        const currentEmail = emailElement.textContent;

        nameElement.innerHTML = `<input type="text" id="editName-${userId}" value="${currentName}">`;
        emailElement.innerHTML = `<input type="text" id="editEmail-${userId}" value="${currentEmail}">`;
        editButton.textContent = "Save";
    } else {
        const updatedName = document.getElementById(`editName-${userId}`).value;
        const updatedEmail = document.getElementById(`editEmail-${userId}`).value;

        editUser(userId, updatedName, updatedEmail)
            .then(() => {
                nameElement.textContent = updatedName;
                emailElement.textContent = updatedEmail;
                editButton.textContent = "Edit";
            })
            .catch(error => {
                console.error("Error:", error);
                alert(`Kunne ikke opdatere brugeren: ${error.message}`);
            });
    }
}

// Funktion til at redigere brugerdata
function editUser(userId, name, email) {
    const updatedData = JSON.stringify({
        id: userId,
        name: name,
        email: email
    });

    return fetch(`https://h3-fitsmart2024.onrender.com/api/Users/${userId}`, {
        method: "PUT",
        headers: {
            "Authorization": `Bearer ${token}`,
            "Content-Type": "application/json"
        },
        body: updatedData
    })
    .then(response => {
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
    });
}

// Hent brugerdata og opdater knapper ved indlæsning af siden
document.addEventListener("DOMContentLoaded", function() {
    updateAuthButtons();
    if (token && !isTokenExpired(token)) fetchUserData();
});
