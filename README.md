# OilTankVision

An IoT and Azure project that uses a Raspberry Pi to convert an analog oil-tank meter into a digital one. This is accomplished with the following data flow:

1.  On a scheduled basis, the Raspberry Pi waskes up and takes a picture of the analog oil-tank meter.
1.  The picture is uploaded to Azure Blob storage
1.  A serverless function is triggered by the new picture.  This function sends the picture to Azure Cognitive Services for analysis
1.  The numbers detected in the photo and their location identified by Cognitive Services are used to determine a 'true reading'
1.  The true reading is saved in Azure Table storage for use in reporting or other applications
