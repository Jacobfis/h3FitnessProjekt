#include <Arduino_MKRIoTCarrier.h>
#include <WiFiNINA.h>
#include <ArduinoHttpClient.h>
#include <Arduino_LSM6DS3.h>

// Wi-Fi
const char* ssid = "WifiNam";
const char* password = "WifiPassword";

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
unsigned long programEndTime = 0;
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
        carrier.display.setCursor(10, 20);
        carrier.display.print("Program Started");
        programStartTime = millis();
        stepCount = 0;
        programRunning = true;
    }
}

void endProgram() {
    if (programRunning) {
        Serial.println("Fitness program ended!");
        carrier.display.fillScreen(ST77XX_BLACK);
        carrier.display.setCursor(10, 20);
        carrier.display.print("Program Ended");
        programEndTime = millis();
        programRunning = false;


        unsigned long durationMs = programEndTime - programStartTime;
        unsigned long durationTicks = durationMs * 10000;


        float distance = stepCount * strideLength;
        float calories = stepCount * caloriesPerStep;

        sendToAPI(stepCount, distance, calories, durationTicks);
    }
}

void trackSteps() {
    float x, y, z;
    if (IMU.accelerationAvailable()) {
        IMU.readAcceleration(x, y, z);

        if (z > threshold && !stepDetected) {
            stepCount++;
            stepDetected = true;
        }
        if (z < threshold) {
            stepDetected = false;
        }
    }

    carrier.display.fillScreen(ST77XX_BLACK);
    carrier.display.setCursor(10, 10);
    carrier.display.print("Steps: ");
    carrier.display.print(stepCount);
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
