#include <Wire.h>
#include <SoftwareSerial.h>
#include <HMC5883L.h>
#include <math.h>

class MPU {
private:
  int adress;
  int gyroSen, accelSen;
  float gyroLSBVal, accelLSBVal;  
  
public:
  float gForceX, gForceY, gForceZ;
  float rotX, rotY, rotZ;
  
  bool isAsleep;
  MPU(int ad, int gSen, int aSen, float gLSB, float aLSB) {
    adress = ad;
    gyroSen = gSen; gyroLSBVal = gLSB;
    accelSen = aSen; accelLSBVal = aLSB;    
  }

  void begin() {
    Wire.beginTransmission(adress);
    Wire.write(0x6B); //Accessing the register 6B - Power Management
    Wire.write(0b00000000); //Setting SLEEP register to 0.
    Wire.endTransmission();
    isAsleep = false;
  
    Wire.beginTransmission(adress);
    Wire.write(0x1B); //Accessing the register 1B - Gyroscope Configuration
    Wire.write(gyroSen);
    Wire.endTransmission();
  
    Wire.beginTransmission(adress);
    Wire.write(0x1C); //Accessing the register 1C - Acccelerometer Configuration
    Wire.write(accelSen);
    Wire.endTransmission();
  }

  void updateValues() {
    Wire.beginTransmission(adress);
    Wire.write(0x3B); //Starting register for Accel Readings
    Wire.endTransmission();
    Wire.requestFrom(adress, 6); //Request Accel Registers (3B - 40)
    while (Wire.available() < 6);
    long ax = Wire.read() << 8 | Wire.read(); //Store first two bytes into x
    long ay = Wire.read() << 8 | Wire.read(); //Store middle two bytes into y
    long az = Wire.read() << 8 | Wire.read(); //Store last two bytes into z
  
    gForceX = ax / accelLSBVal;
    gForceY = ay / accelLSBVal;
    gForceZ = az / accelLSBVal;

    Wire.beginTransmission(adress);
    Wire.write(0x43); //Starting register for Gyro Readings
    Wire.endTransmission();
    Wire.requestFrom(adress, 6); //Request Gyro Registers (43 - 48)
    while (Wire.available() < 6);
    long gx = Wire.read() << 8 | Wire.read(); //Store first two bytes into x
    long gy = Wire.read() << 8 | Wire.read(); //Store middle two bytes into y
    long gz = Wire.read() << 8 | Wire.read(); //Store last two bytes into z
  
    rotX = gx / gyroLSBVal;
    rotY = gy / gyroLSBVal;
    rotZ = gz / gyroLSBVal;
  }  

  void Sleep() {
    Wire.beginTransmission(adress);
    Wire.write(0x6B); //Accessing the register 6B - Power Management
    Wire.write(0b00000000); //Setting SLEEP register to 0.
    Wire.endTransmission();
    isAsleep = false;
  }
  void Wake() {
    Wire.beginTransmission(adress);
    Wire.write(0x6B); //Accessing the register 6B - Power Management
    Wire.write(0b01000000); //Setting SLEEP register to 1.
    Wire.endTransmission();
    isAsleep = true;  
  }
};

const int MPUGyroSensitivity =  0b00010000;   const float MPUGyroLSB  = 32.8;

const int MPUAccelSensitivity = 0b00001000; const float MPUAccelLSB = 8192.0;
const int MPUAdress = 0x68;

MPU sensor(MPUAdress, MPUGyroSensitivity, MPUAccelSensitivity, MPUGyroLSB, MPUAccelLSB);

SoftwareSerial btConnection(11, 10);
const int buttonB = 3;
const int buttonA = 4;

const int potPin = A6;

HMC5883L compass;

float previousTime = 0;
float currentTime = 0;

void setup()
{
  pinMode(buttonA, INPUT);
  pinMode(buttonB, INPUT);  
  
  Serial.begin(9600); 
  btConnection.begin(9600); 
  Wire.begin(); 
  sensor.begin();
  compass.begin();
  //i have no idea what the lib is doing here and im too lazy to find out so im copyin the examples lmao
  compass.setRange(HMC5883L_RANGE_1_3GA);
  compass.setMeasurementMode(HMC5883L_CONTINOUS);
  compass.setDataRate(HMC5883L_DATARATE_30HZ);
  compass.setSamples(HMC5883L_SAMPLES_8);
  compass.setOffset(0, 0);
}

void loop()
{
  previousTime = currentTime;
  currentTime = millis();
  float elapsedTime = (currentTime - previousTime) / 1000;
  if (!sensor.isAsleep)
  {
    sensor.updateValues();
    float roll = atan2(sensor.gForceY, sensor.gForceZ);
    float pitch = atan2(sensor.gForceX, sqrt(sensor.gForceY*sensor.gForceY + sensor.gForceZ*sensor.gForceZ));
    float foreArmRotation = (float)(analogRead(potPin))/1023;

    Vector dir = compass.readNormalize();
    
    String sentData = String(sensor.rotX * elapsedTime) + "," +  String(sensor.rotY * elapsedTime) + "," +
    String(sensor.rotZ * elapsedTime) + "," +  String(roll) + "," +  String(pitch) + "," 
    + String(dir.XAxis) + "," + String(dir.YAxis) + "," + String(dir.ZAxis) + "," +
    String(foreArmRotation) + "," + String(digitalRead(buttonA)) + "," + String(digitalRead(buttonB)) + "|";
    
    for (int i = 0; i < sentData.length();i++)
    {
      btConnection.write((char)sentData[i]);
    }
    
    Serial.println(sentData);  
    
  }
}
