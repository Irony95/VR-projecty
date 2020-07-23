# VR-projecty

This repo contains the code for the an arduino VR controller, and the unity project that works with the controller.
The code is like super badly written, and still has a lot of bugs that i dont have time to fix. So feel free to change, update and copy if u want to, just make sure to fix and tidy things up :).

The arduino uses a magnetometer, gyroscope, acelerometer for the upper arm rotation measurement, a potentiometer for the forearm rotation measurement, and some buttons for the inputs. The data are all sent to the unity code.

The arduino sends a string of information to the bluetooth in an android phone, which reads it and calculates values for the rotation.

for the shoulder rotation, by using corrosponding angles in parallel lines with the gyroscope, accelerometer and magnetometer data, you can determine the rotation of your shoulder no matter what length down you place the IMUs. the potentiometer reads the angle of your forearm to your upper arm.
