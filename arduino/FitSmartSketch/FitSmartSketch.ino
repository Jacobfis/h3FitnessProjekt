#include <Wire.h>
#include <Adafruit_GFX.h>
#include <Adafruit_SSD1306.h>
#include <Arduino_LSM9DS1.h>
#include <Arduino_LSM6DS3.h>

// Display settings
#define SCREEN_WIDTH 128
#define SCREEN_HEIGHT 32
Adafruit_SSD1306 display(SCREEN_WIDTH, SCREEN_HEIGHT, &Wire, -1);

// Fitness tracker variables
int stepCount = 0;
float strideLength = 0.78;  // Length per step 
float caloriesPerStep = 0.04;  // calories burnt per step 
bool stepDetected = false;
float threshold = 1.2;  // Step detection 

void setup() {
  Serial.begin(9600);

  if (!display.begin(SSD1306_SWITCHCAPVCC, 0x3C)) {
    Serial.println("SSD1306 allocation failed");
    while (1);
  }
  display.display();
  delay(2000);
  display.clearDisplay();

  // Accelerometer begin o_o
  if (!IMU.begin()) {
    Serial.println("Failed to start accelerometer!");
    while (1);
  }
  Serial.println("Accelerometer started :scream:!");
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

    // Display
    display.clearDisplay();
    display.setTextSize(1);
    display.setTextColor(SSD1306_WHITE);
    display.setCursor(0, 0);
    display.print("Steps: ");
    display.println(stepCount);
    display.print("Distance: ");
    display.print(distance);
    display.println(" m");
    display.print("Calories: ");
    display.print(calories);
    display.println(" kcal");
    display.display();

    // output to Serial Monitor
    Serial.print("Steps: ");
    Serial.print(stepCount);
    Serial.print(", Distance: ");a
    Serial.print(distance);
    Serial.print(" m, Calories: ");
    Serial.println(calories);
  }

  delay(100);
}
