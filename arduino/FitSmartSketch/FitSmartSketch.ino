#include <Arduino_MKRIoTCarrier.h>
#include <WiFiNINA.h>
#include <ArduinoHttpClient.h>
#include <Arduino_LSM6DS3.h>

// Wi-Fi
const char* ssid = "Familien.Fischer";
const char* password = "Norregade29";

// API endpoint
const char* serverName = "h3-fitsmart2024.onrender.com";
const int serverPort = 443;

MKRIoTCarrier carrier;

int stepCount = 0;
float strideLength = 0.78; 
float caloriesPerStep = 0.04; 
bool stepDetected = false;
float threshold = 1.2; 
unsigned long programStartTime = 0;
bool programRunning = false;

WiFiSSLClient wifiClient;
HttpClient client(wifiClient, serverName, serverPort);

void setup() {
    Serial.begin(9600);
    if (!carrier.begin()) {
        Serial.println("Failed to initialize MKRIoTCarrier!");
        while (1);
    }
    carrier.display.fillScreen(ST77XX_BLACK);
    carrier.display.setTextColor(ST77XX_WHITE);
    carrier.display.setTextSize(2);

    // Initialize IMU
    if (!IMU.begin()) {
        Serial.println("Failed to initialize IMU!");
        while (1);
    }
    Serial.println("IMU initialized!");

    connectToWiFi();
}

void loop() {
    // Check for touch inputs
    carrier.Buttons.update();

    if (carrier.Buttons.onTouchDown(TOUCH0)) { // Start button
        startProgram();
    }

    if (carrier.Buttons.onTouchDown(TOUCH1)) { // Stop button
        endProgram();
    }

    if (programRunning) {
        trackSteps();
    }

    delay(100);
}

void startProgram() {
    if (!programRunning) {
        Serial.println("Fitness program started!");
        carrier.display.fillScreen(ST77XX_BLACK);
        drawCenteredText("Program Started", 60);
        programStartTime = millis();
        stepCount = 0;
        programRunning = true;
    }
}

void endProgram() {
    if (programRunning) {
        Serial.println("Fitness program ended!");
        programRunning = false;

        unsigned long durationMs = millis() - programStartTime;
        float distance = stepCount * strideLength;
        float calories = stepCount * caloriesPerStep;

        // Beregn tid
        int seconds = (durationMs / 1000) % 60;
        int minutes = (durationMs / 60000);

        // Vis detaljer på skærmen
        carrier.display.fillScreen(ST77XX_BLACK);

        drawCenteredText("Program Ended", 40);
        String stepText = "Steps: " + String(stepCount);
        String distanceText = "Dist: " + String(distance, 2) + " m";
        String caloriesText = "Cals: " + String(calories, 2);
        String timeText = "Time: " + String(minutes) + "m " + String(seconds) + "s";

        drawCenteredText(stepText, 80);
        drawCenteredText(distanceText, 110);
        drawCenteredText(caloriesText, 140);
        drawCenteredText(timeText, 170);

        // Send data til API
        sendToAPI(stepCount, distance, calories, durationMs * 10); // Omregner til ticks
    }
}

void trackSteps() {
    float x, y, z;
    if (IMU.accelerationAvailable()) {
        IMU.readAcceleration(x, y, z);

        if (z > threshold && !stepDetected) {
            stepCount++;
            stepDetected = true;
        } else if (z < threshold) {
            stepDetected = false;
        }

        // Beregn tid
        unsigned long durationMs = millis() - programStartTime;
        int seconds = (durationMs / 1000) % 60;
        int minutes = (durationMs / 60000);

        // Beregn distance og kalorier
        float distance = stepCount * strideLength;
        float calories = stepCount * caloriesPerStep;

        // Opdater skærmen med realtidsdata
        carrier.display.fillScreen(ST77XX_BLACK);

        String stepText = "Steps: " + String(stepCount);
        String distanceText = "Dist: " + String(distance, 2) + " m";
        String caloriesText = "Cals: " + String(calories, 2);
        String timeText = "Time: " + String(minutes) + "m " + String(seconds) + "s";

        drawCenteredText(stepText, 40);
        drawCenteredText(distanceText, 70);
        drawCenteredText(caloriesText, 100);
        drawCenteredText(timeText, 130);
    }
}

void connectToWiFi() {
    Serial.print("Connecting to Wi-Fi");
    while (WiFi.begin(ssid, password) != WL_CONNECTED) {
        Serial.print(".");
        delay(1000);
    }
    Serial.println("\nConnected to Wi-Fi!");
}

void sendToAPI(int steps, float distance, float calories, unsigned long durationTicks) {
    if (WiFi.status() == WL_CONNECTED) {
        // Payload
        String jsonPayload = "{";
        jsonPayload += "\"id\":\"123\",";
        jsonPayload += "\"date\":\"" + getDateTime() + "\",";
        jsonPayload += "\"steps\":" + String(steps) + ",";
        jsonPayload += "\"distance\":" + String(distance) + ",";
        jsonPayload += "\"calories\":" + String(calories) + ",";
        jsonPayload += "\"duration\":{\"ticks\":" + String(durationTicks) + "},";
        jsonPayload += "\"type\":\"Walking\"";
        jsonPayload += "}";

        Serial.println("Sending payload:");
        Serial.println(jsonPayload);

        // POST
        client.beginRequest();
        client.post("/api/ActivityLog");
        client.sendHeader("Content-Type", "application/json");
        client.sendHeader("Content-Length", jsonPayload.length());
        client.beginBody();
        client.print(jsonPayload);
        client.endRequest();

        int statusCode = client.responseStatusCode();
        String response = client.responseBody();
        Serial.print("Status code: ");
        Serial.println(statusCode);
        Serial.print("Response: ");
        Serial.println(response);
    } else {
        Serial.println("Wi-Fi disconnected. Reconnecting...");
        connectToWiFi();
    }
}

String getDateTime() {
    // Test kode
    return "2024-11-27T12:00:00Z";
}

void drawCenteredText(String text, int y) {
    int16_t x1, y1;
    uint16_t w, h;
    carrier.display.getTextBounds(text, 0, y, &x1, &y1, &w, &h);
    int x = (carrier.display.width() - w) / 2;
    carrier.display.setCursor(x, y);
    carrier.display.print(text);
}