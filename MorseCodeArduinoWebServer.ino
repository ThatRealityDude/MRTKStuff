/*
https://morsecode.world/international/translator.html
*/


#include <WiFiS3.h>
#include "ArduinoGraphics.h"
#include "Arduino_LED_Matrix.h"

ArduinoLEDMatrix matrix;

const int LED_PIN = 13;  // Arduino pin connected to LED's pin

const char ssid[] = "SynergyFlow";          // change your network SSID (name)
const char pass[] = "LetItFlow"; // change your network password (use for WPA, or use as key for WEP)

int status = WL_IDLE_STATUS;

WiFiServer server(80);

String morse[] = {".-", "-...", "-.-.", "-..", ".", "..-.", "--.", "....", "..", ".---", "-.-", ".-..", "--", "-.", "---", ".--.", "--.-", ".-.", "...", "-", "..-", "...-", ".--", "-..-", "-.--", "--..", ".----", "..---", "...--", "....-", ".....", "-....", "--...", "---..", "----.", "-----", "/"};
String characters[] = {"A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z","1","2","3","4","5","6","7","8","9","0"," "};


void setup() {
  Serial.begin(9600);
  matrixSetup();
  pinMode(LED_PIN, OUTPUT);  // set arduino pin to output mode
  Serial.println((WiFi.firmwareVersion() < WIFI_FIRMWARE_LATEST_VERSION) ? "Please upgrade the firmware" : "Firmware Up To Date!");
  //Ternary Operators are pretty rad
  while (status != WL_CONNECTED) {
    matrixPrint("Connecting: " + ((String)ssid), 5);
    Serial.print("Attempting to connect to SSID: ");
    Serial.println(ssid);
    status = WiFi.begin(ssid, pass);
    delay(1000); //Wait 10 seconds for the connection
  }
  Serial.print("Connected to SSID: ");
  Serial.println(ssid);
  matrixPrint("Connected: " + ((String)ssid), 5);
  server.begin();
  String IP = (String)WiFi.localIP()[0] + "." + (String)WiFi.localIP()[1] + "." + (String)WiFi.localIP()[2] + "." + (String)WiFi.localIP()[3];
  Serial.print("IP: ");
  Serial.println(IP);
  matrixPrint(IP, 5);
  matrixIPPrint(IP);
  /*
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
  matrix.renderBitmap(frame, 8, 12);
  */
}

void loop() {
    // listen for incoming clients
  WiFiClient client = server.available();
  if (client) {
    Serial.println("Someones Attempting to Connect");
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
      // expected header is one of the following:
      // - GET led1/on
      // - GET led1/off
      
      //test
      //String keyWords[] = {"thumb", "index", "middle", "ring", "little"};
      //for(int i = 0; i < 5; i++) {
        //HTTP_req.substring()
        //int keywordIndex = HTTP_req.indexOf(keyWords[i]);
        //int startIndex = (HTTP_req.indexOf("=", keywordIndex))+1;
        //int endIndex = HTTP_req.indexOf("?", keywordIndex);
        //Serial.println("End Index: " + endIndex);
        //motorValues[i] = HTTP_req.substring(startIndex, endIndex).toInt();
        //Serial.println(keyWords[i] + ": " + motorValues[i]);
        
        //Motor Code Goes Here

        //servos[i].write(motorValues[i]);
        //Serial.println("Motor Code Ran");


        
        //Serial.println("HTTP REQUEST: " + HTTP_req);
        //Serial.println(keyWords[i] + ": StartIndex:" + startIndex + " EndIndex: " + endIndex + " Value: " + HTTP_req.substring(startIndex, endIndex));
      //}
      //Serial.println("HTTP Request: " + HTTP_req);
      //Serial.print("\033[0H\033[0J");
      /*
      if (HTTP_req.indexOf("led1/on") > -1) {  // check the path
        digitalWrite(LED_PIN, HIGH);           // turn on LED
        Serial.println("Turned LED on");
      } else if (HTTP_req.indexOf("led1/off") > -1) {  // check the path
        digitalWrite(LED_PIN, LOW);                    // turn off LED
        Serial.println("Turned LED off");
      } else {
        Serial.println("No command");
      }
      */
    } 
    /*
    else if (HTTP_req.indexOf("POST") == 0) {
      Serial.println("POST RECIEVED!");
    }
    
    */
    
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
    client.println("<h1>Enter your message:</h1>");
    client.println("<form method=\"post\">");
    client.println("<input type=\"text\" name=\"message\">");
    client.println("<input type=\"submit\" value=\"Convert to Morse\">");
    client.println("</form>");

     if (client.available()) {
    String message = "";
    String line = client.readStringUntil('\r');
    // find the beginning of the message data
    if (line.startsWith("message=")) {
      // extract the message
      message = line.substring(8);
      // convert the message to Morse code
      String morse = textToMorse(message);
      Serial.println(morse);
      // send the Morse code back to the client
      client.print("<p><strong>Morse Code:</strong> ");
      client.print(morse);
      client.println("</p>");
    }
  }

    client.println("</head>");
    client.println("</html>");
    client.flush();

    // give the web browser time to receive the data
    delay(10);

    // close the connection:
    client.stop();
  }
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

String textToMorse(String txt) {
  Serial.println("Text: " + txt + "|END");
  String output = "";
  Serial.println(("Characters Length: " + ((String)sizeof(characters)) + "| Morse Length: " + ((String)sizeof(morse))));
  Serial.println(("Measured Down Values: " + (sizeof(characters)/sizeof(characters[0]))));
  Serial.println(("Measured Down Values: " + (sizeof(characters)/sizeof(characters[0]))));
  for(int i = 0; i < txt.length(); i++) {
    int count = 0;
    int index = -1;
    for(int j = 0; j < (sizeof(characters)/sizeof(characters[0])); j++) {
      String character = txt.substring(i, i+1);
      if(character.equals("+")) {
        character = " ";
      }
      character.toUpperCase();
      if(characters[j].equals(character)) {
        index = j;
        break;
      }
      count++;
    }
    if(index == -1) {
      Serial.println("INVALID OOPSIE DETECTED");
      matrixPrint("OOPSIE POOPSIE", 100);
      return "INVALID SOMETHING DETECTED";
    }
    output += morse[index] + " ";
  }
  return output;
}
