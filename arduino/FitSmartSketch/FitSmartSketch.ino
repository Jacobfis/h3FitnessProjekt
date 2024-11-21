#include <Wire.h>
#include <Adafruit_GFX.h>
#include <Adafruit_SSD1306.h>
#include <Arduino_LSM9DS1.h>
#include <WiFiNINA.h>
#include <ArduinoHttpClient.h>
#include <Arduino_LSM6DS3.h>

// Wi-Fi credentials
const char* ssid = "Jacob - iPhone (2)";
const char* password = "kodener2";

// API endpoint
const char* serverName = "https://h3-fitsmart2024.onrender.com/api/ActivityLog";
const int serverPort = 443; // 443, 80

// Fitness tracker variables
int stepCount = 0;
float strideLength = 0.78;
float caloriesPerStep = 0.04;
bool stepDetected = false;
float threshold = 1.2; 

WiFiClient wifiClient; 
HttpClient client(wifiClient, serverName, serverPort);

void setup() {
  Serial.begin(9600);


  connectToWiFi();

  if (!IMU.begin()) {
    Serial.println("Failed to start accelerometer!");
    while (1);
  }
  Serial.println("Accelerometer started!");
}

void loop() {
  float x, y, z;
  if (IMU.accelerationAvailable()) {
    IMU.readAcceleration(x, y, z);

    // Step detection logic based on Z-axis peaks
    if (z > threshold && !stepDetected) {
      stepCount++;
      stepDetected = true;
    }
    if (z < threshold) {
      stepDetected = false;
    }

    // Calculate distance and calories
    float distance = stepCount * strideLength;
    float calories = stepCount * caloriesPerStep;

    // Send data to API
    sendToAPI(stepCount, distance, calories);

    // Serial Monitor output
    Serial.print("Steps: ");
    Serial.print(stepCount);
    Serial.print(", Distance: ");
    Serial.print(distance);
    Serial.print(" m, Calories: ");
    Serial.println(calories);
  }

  delay(100);
}

void connectToWiFi() {
  Serial.print("Connecting to Wi-Fi");
  while (WiFi.begin(ssid, password) != WL_CONNECTED) {
    Serial.print(".");
    delay(1000);
  }
  Serial.println("\nConnected to Wi-Fi!");
}

void sendToAPI(int steps, float distance, float calories) {
  if (WiFi.status() == WL_CONNECTED) {
    // Opret JSON payload
    String jsonPayload = "{\"steps\":" + String(steps) +
                         ",\"distance\":" + String(distance) +
                         ",\"calories\":" + String(calories) + "}";

    // Send HTTP POST request
    client.beginRequest();
    client.post("/data");
    client.sendHeader("Content-Type", "application/json");
    client.sendHeader("Content-Length", jsonPayload.length());
    client.beginBody();
    client.print(jsonPayload);
    client.endRequest();

    // Håndter respons
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
