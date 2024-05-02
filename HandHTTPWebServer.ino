/*
 * Created by ArduinoGetStarted.com
 *
 * This example code is in the public domain
 *
 * Tutorial page: https://arduinogetstarted.com/tutorials/arduino-uno-r4-wifi-controls-led-via-web
 */

#include <WiFiS3.h>
#include <Servo.h>
#include "ArduinoGraphics.h"
#include "Arduino_LED_Matrix.h"

ArduinoLEDMatrix matrix;

bool matrixRunning = false;
const int LED_PIN = 13;  // Arduino pin connected to LED's pin

const char ssid[] = "SynergyFlow";          // change your network SSID (name)
const char pass[] = "LetItFlow"; // change your network password (use for WPA, or use as key for WEP)

int status = WL_IDLE_STATUS;

int motorValues[5];

int motorPins[] = {3,5,6,9,10,11};
Servo servos[5];
WiFiServer server(80);

void setup() {
  //Initialize serial and wait for port to open:
  matrixSetup();
  Serial.begin(9600);
  pinMode(LED_PIN, OUTPUT);  // set arduino pin to output mode

  String fv = WiFi.firmwareVersion();
  if (fv < WIFI_FIRMWARE_LATEST_VERSION) {
    Serial.println("Please upgrade the firmware");
  } else {
    Serial.println("Firmware Up To Date!");
  }

  // attempt to connect to WiFi network
  while (status != WL_CONNECTED) {
    matrixPrint("Connecting: " + ((String)ssid), 30);
    Serial.print("Attempting to connect to SSID: ");
    Serial.println(ssid);
    // Connect to WPA/WPA2 network. Change this line if using open or WEP network:
    status = WiFi.begin(ssid, pass);

    // wait 10 seconds for connection:
    delay(10000);
  }
  while(matrixRunning) {
    ;
  }
  matrixPrint("Connected: " + ((String)ssid), 35);
  server.begin();
  // you're connected now, so print out the status:
  //printWifiStatus();
  Serial.println("ABOUT TO ATTEMPT WORDS");
  while(matrixRunning) {
    ;
  }
  String IP = (String)WiFi.localIP()[0] + "." + (String)WiFi.localIP()[1] + "." + (String)WiFi.localIP()[2] + "." + (String)WiFi.localIP()[3];
  Serial.print("IP: ");
  Serial.println(IP);
  matrixPrint(IP, 5);
  matrixIPPrint(IP);
  for(int i = 0; i < 5; i++) {
    servos[i].attach(motorPins[i]);  // attaches the servo on pin X to the servo object
    servos[i].write(0);
  }
}

void loop() {
  // listen for incoming clients
  WiFiClient client = server.available();
  if (client) {
    // read the first line of HTTP request header
  String HTTP_req = "";
    while (client.connected()) {
      if (client.available()) {
        //Serial.println("New HTTP Request");
        HTTP_req = client.readStringUntil('\n');  // read the first line of HTTP request
        //Serial.print("<< ");
        //Serial.println(HTTP_req);  // print HTTP request to Serial Monitor
        break;
      }
    }

    // read the remaining lines of HTTP request header
    while (client.connected()) {
      if (client.available()) {
        String HTTP_header = client.readStringUntil('\n');  // read the header line of HTTP request

        if (HTTP_header.equals("\r"))  // the end of HTTP request
          break;

        //Serial.print("<< ");
        //Serial.println(HTTP_header);  // print HTTP request to Serial Monitor
      }
    }

    if (HTTP_req.indexOf("GET") == 0) {  // check if request method is GET
      String keyWords[] = {"thumb", "index", "middle", "ring", "little"};
      for(int i = 0; i < 5; i++) {
        int keywordIndex = HTTP_req.indexOf(keyWords[i]);
        int startIndex = (HTTP_req.indexOf("=", keywordIndex))+1;
        int endIndex = HTTP_req.indexOf("?", keywordIndex);
        //Serial.println("End Index: " + endIndex);
        motorValues[i] = HTTP_req.substring(startIndex, endIndex).toInt();
        Serial.println(keyWords[i] + ": " + motorValues[i]);
        
        //Motor Code Goes Here

        servos[i].write(motorValues[i]);
        Serial.println("Motor Code Ran");
      }
    
    // send the HTTP response
    // send the HTTP response header
    client.println("HTTP/1.1 200 OK");
    client.println("Content-Type: text/html");
    client.println("Connection: close");  // the connection will be closed after completion of the response
    client.println();                     // the separator between HTTP header and body
    // send the HTTP response body
    client.println("<!DOCTYPE HTML>");
    client.println("<html>");
    client.println("<head>");
    client.println("<link rel=\"icon\" href=\"data:,\">");
    client.println("</head>");
    client.println("<a href=\"/led1/on\">LED ON</a>");
    client.println("<br><br>");
    client.println("<a href=\"/led1/off\">LED OFF</a>");
    client.println("</html>");
    client.flush();

    // give the web browser time to receive the data
    delay(10);

    // close the connection:
    client.stop();
  }
}

void printWifiStatus() {
  // print your board's IP address:
  Serial.print("IP Address: ");
  Serial.println(WiFi.localIP());

  // print the received signal strength:
  Serial.print("signal strength (RSSI):");
  Serial.print(WiFi.RSSI());
  Serial.println(" dBm");
}

void matrixSetup() {
  matrix.begin();

    matrix.beginDraw();
    matrix.stroke(0xFFFFFFFF);
    // add some static text
    // will only show "UNO" (not enough space on the display)
    const char text[] = "UNO r4";
    matrix.textFont(Font_4x6);
    matrix.beginText(0, 1, 0xFFFFFF);
    matrix.println(text);
    matrix.endText();
  
    matrix.endDraw();

    delay(2000);
}

void matrixPrint(String message, int speed) {
  matrixRunning = true;
  matrix.beginDraw();

  matrix.stroke(0xFFFFFFFF);
  matrix.textScrollSpeed(speed <= 0 ? 50 : speed); //Normal is 50

  // add the text
  //const char text[] = "    TEST    ";
  matrix.textFont(Font_5x7);
  matrix.beginText(0, 1, 0xFFFFFF);
  matrix.println("    " + message + "    ");
  matrix.endText(SCROLL_LEFT);

  matrix.endDraw();
  matrixRunning = false;
}

void matrixIPPrint(String IP) {
  byte frame[8][12] = {
  { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
  { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
  { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
  { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
  { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
  { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
  { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
  { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
};
  String lastPart = "";
  int currentLight = 0;
  lastPart = IP + ".";
  for(int i = 0; i < 4; i++) {
    currentLight = i * (24);
    //10.0.0.152
    String ipPart = lastPart.substring(0, lastPart.indexOf("."));
    lastPart = lastPart.substring(lastPart.indexOf(".")+1);
    Serial.println("IPPart: " + ipPart);
    for(int j = 0; j < ipPart.length(); j++) {
      for(int k = 0; k < (ipPart.substring(j, j+1)).toInt(); k++) {
        //Serial.println(((int)(currentLight/8)) + ":" + ((int)(currentLight % 12)));
        frame[(int)(currentLight/12)][(int)(currentLight % 12)] = 1;
        currentLight++;
      }
      if((ipPart.substring(j, j+1)).toInt() == 0) {
        currentLight++;
      }
      currentLight++;
    }
  }
  matrix.renderBitmap(frame, 8, 12);
}
