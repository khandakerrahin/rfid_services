# Automated School Attendance System using UHF RFID Reader DL950S

The project aims to design and deploy an automated school attendance system using **UHF RFID Reader DL950S** where the RFID devices are deployed at the school premises and managed remotely using Raspberry Pi 3. For this project, I contributed to the development of the console applications from the provided SDK which involved configuring the devices, collecting and stream processing of the sensor readings, transferring the data to the inhouse datastore.

#
**Technologies used:**
- C#
- Java
- JSP
- MySQL
#

**OS Environments:**
- Windows 10 IoT Core edition
- Ubuntu 18.04.1 LTS

#
**Devices used**: 
- [UHF RFID Reader DL950S](https://www.rfid-in-china.com/poe-uhf-rfid-reader-dl950s-poe.html)
- [Raspberry Pi 3 Model B](https://www.raspberrypi.com/products/raspberry-pi-3-model-b/)

#

**System Architecture**: 
- [**ACCConsoleApplication**](https://github.com/khandakerrahin/rfid_services/tree/master/ACCConsoleApplication) runs on the remote Raspberry Pi3, controls the devices and dumps the sensors readings as files
- [**ACCZipUploader**](https://github.com/khandakerrahin/rfid_services/tree/master/ACCZipUploader) runs on the remote Raspberry Pi3, zips the files and uploads them to the server using SFTP
- [**ACCZipDataExtractor**](https://github.com/khandakerrahin/rfid_services/tree/master/ACCZipDataExtractor) runs on the server, unzips the files and inserts the reading into the MySQL databse.
- [**ACCLogUploader**](https://github.com/khandakerrahin/rfid_services/tree/master/ACCLogUploader) runs on the remote Raspberry Pi3, zips the logs and uploads them to the server using SFTP
- [**ACCLogExtractor**](https://github.com/khandakerrahin/rfid_services/tree/master/ACCLogExtractor) runs on the server, unzips the logs and inserts the reading into the MySQL databse.
