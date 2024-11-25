#include <Wire.h>
#include <Adafruit_GFX.h>
#include <Adafruit_SSD1306.h>
#include <Arduino_LSM9DS1.h>
#include <WiFiNINA.h>
#include <ArduinoHttpClient.h>

// Wi-Fi credentials
const char* ssid = "Familien.Fischer";     // Indsæt dit netværksnavn
const char* password = "Norregade29"; // Indsæt din adgangskode

// API endpoint
const char* serverName = "h3-fitsmart2024.onrender.com/api/ActivityLog";
const int serverPort = 443; // HTTPS

WiFiSSLClient wifiClient; // Til HTTPS
HttpClient client(wifiClient, serverName, serverPort);

// Fitness tracker variabler
int stepCount = 0;
float strideLength = 0.78; // Meter pr. skridt
float caloriesPerStep = 0.04; // Kalorier pr. skridt
bool stepDetected = false;
float threshold = 1.2; // Accelerometer tærskelværdi

void setup() {
  Serial.begin(9600);
  connectToWiFi();

  if (!IMU.begin()) {
    Serial.println("Accelerometer kunne ikke starte.");
    while (1);
  }
  Serial.println("Accelerometer startet.");
}

void loop() {
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

    // Beregn distance og kalorier
    float distance = stepCount * strideLength;
    float calories = stepCount * caloriesPerStep;

    // Send data til API
    sendToAPI(stepCount, distance, calories);

    // Print til Serial Monitor
    Serial.print("Steps: ");
    Serial.print(stepCount);
    Serial.print(", Distance: ");
    Serial.print(distance);
    Serial.print(" m, Calories: ");
    Serial.println(calories);
  }

  delay(1000); // 1 sekund forsinkelse
}

void connectToWiFi() {
  Serial.print("Forbinder til Wi-Fi");
  while (WiFi.begin(ssid, password) != WL_CONNECTED) {
    Serial.print(".");
    delay(1000);
  }
  Serial.println("\nForbundet til Wi-Fi!");
}

//Forstår dette :D HELLS YES W
void sendToAPI(int steps, float distance, float calories) {
  if (WiFi.status() == WL_CONNECTED) {
    String jsonPayload = "{\"id\":\"123\","
                         "\"date\":\"" + String("2024-11-25T00:00:00Z") + "\","
                         "\"steps\":" + String(steps) + ","
                         "\"distance\":" + String(distance) + ","
                         "\"calories\":" + String(calories) + ","
                         "\"duration\":\"00:10:00\","
                         "\"type\":\"Walking\"}";

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
    Serial.println("Wi-Fi frakoblet. Forsøger at forbinde...");
    connectToWiFi();
  }
}
