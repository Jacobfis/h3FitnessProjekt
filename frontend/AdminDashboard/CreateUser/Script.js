
document.getElementById("registerForm").addEventListener("submit", function(event) {
    event.preventDefault(); // Forhindrer formularen i at sende normalt

    const username = document.getElementById("newName").value;
    const email = document.getElementById("newEmail").value;
    const password = document.getElementById("newPassword").value;

    // Headers og body til registrerings-anmodningen
    const registerHeaders = new Headers();
    registerHeaders.append("Content-Type", "application/json");

    const registerData = JSON.stringify({
        username: username,
        email: email,
        password: password
    });

    const requestOptions = {
        method: "POST",
        headers: registerHeaders,
        body: registerData,
        redirect: "follow"
    };

    // Send registrerings-anmodningen til backend
    fetch("https://h3-fitsmart2024.onrender.com/api/Users/register", requestOptions)
        .then(response => {
            if (!response.ok) {
                // Håndterer fejlstatuskoder (fx 400, 401, 500)
                return response.text().then(text => {
                    throw new Error(text || "Kunne ikke oprette bruger.");
                });
            }
            return response.json();
        })
        .then(data => {
            // Hvis registrering er succesfuld, vis succesmeddelelse
            document.getElementById("registerSuccess").style.display = "block";
            document.getElementById("registerError").style.display = "none";

            // Ryd input felter
            document.getElementById("newName").value = "";
            document.getElementById("newEmail").value = "";
            document.getElementById("newPassword").value = "";
        })
        .catch(error => {
            console.error("Error:", error);
            document.getElementById("registerError").style.display = "block";
            document.getElementById("registerError").textContent = error.message;
            document.getElementById("registerSuccess").style.display = "none";
        });
    });
    