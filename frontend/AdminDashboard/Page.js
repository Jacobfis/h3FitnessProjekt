// Funktion til at opdatere knapperne baseret på loginstatus
function updateAuthButtons() {
    const authButton = document.getElementById("authButton");
    const signupButton = document.getElementById("signupButton");

    // Tjek om token findes i localStorage
    const token = localStorage.getItem("authToken");

    if (token) {
        // Hvis token findes (logget ind)
        authButton.textContent = "Logud";
        signupButton.style.display = "none"; // Skjuler "Opret"-knappen

        // Ændrer knapfunktion til logud
        authButton.onclick = () => {
            localStorage.removeItem("authToken"); // Fjern token ved logud
            updateAuthButtons(); // Opdater knapper efter logud
        };
    } else {
        // Hvis token ikke findes (ikke logget ind)
        authButton.textContent = "Login";
        signupButton.style.display = "inline-block"; // Viser "Opret"-knappen igen

    }
}

// Kald funktionen når siden indlæses
document.addEventListener("DOMContentLoaded", updateAuthButtons);
