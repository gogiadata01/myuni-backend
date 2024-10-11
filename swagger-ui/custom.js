// Custom Swagger UI login logic

window.onload = function () {
    const loginHtml = `
        <div id="login-form" style="margin: 20px;">
            <h4>Login to get JWT Token</h4>
            <input type="text" id="email" placeholder="Email" style="margin-right: 10px;"/>
            <input type="password" id="password" placeholder="Password" style="margin-right: 10px;"/>
            <button onclick="login()" style="margin-right: 10px;">Login</button>
            <p id="login-status" style="color: red;"></p>
        </div>
    `;

    // Append the login form to Swagger UI
    const div = document.createElement('div');
    div.innerHTML = loginHtml;
    document.body.insertBefore(div, document.querySelector('.swagger-container'));

    // Function to handle login
    window.login = function() {
        const email = document.getElementById('email').value;
        const password = document.getElementById('password').value;

        fetch('/api/User/signin', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                email: email,
                password: password
            })
        })
        .then(response => response.json())
        .then(data => {
            if (data.token) {
                // Set the JWT token for all subsequent requests
                const authToken = `Bearer ${data.token}`;
                window.localStorage.setItem('authToken', authToken); // Save token for later use

                // Update Swagger Authorize header with the token
                const apiKeyAuth = new SwaggerClient.ApiKeyAuthorization('Authorization', authToken, 'header');
                window.swaggerUi.api.clientAuthorizations.add('Bearer', apiKeyAuth);
                document.getElementById('login-status').innerText = "Login successful!";
            } else {
                document.getElementById('login-status').innerText = "Invalid email or password.";
            }
        })
        .catch(err => {
            console.error("Login failed", err);
            document.getElementById('login-status').innerText = "Login failed.";
        });
    };
};
