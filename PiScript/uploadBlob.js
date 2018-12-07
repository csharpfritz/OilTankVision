require("dotenv").config();
const storage = require("azure-storage");
const Webcam = require("node-webcam");

const { AZURE_STORAGE_ACCOUNT_NAME, AZURE_STORAGE_ACCESS_KEY, AZURE_STORAGE_CONTAINER_NAME } = process.env;
const blobService = storage.createBlobService(AZURE_STORAGE_ACCOUNT_NAME, AZURE_STORAGE_ACCESS_KEY);

var opts = {
    width: 1280,
    height: 720,
    quality: 100,
    saveShots: true,
    output: "jpeg",
    // use cam.list to find out the name or your specific camera
    device: "HD Pro Webcam C920 #2",
    callbackReturn: "location",
    verbose: false
};

const cam = Webcam.create(opts);

// uncomment this to debug and see the cameras connected to your computer
/*j
cam.list((list) => {
  console.log(list);
});
*/

const blobUploadCallback = (error, result, response) => {
  if (error) return console.error(error);
  console.log("successfully uploaded gauge photo to Blob Storage", result);
};

const captureCallback = (error, location) => {
  // @csharpfritz would like his photos to be labeled by date and time taken
  const filename = new Date().toISOString().replace(/\D+/g, "").substr(0,12);
  blobService.createBlockBlobFromLocalFile(AZURE_STORAGE_CONTAINER_NAME, `${filename}.jpg`, location, blobUploadCallback);
};

cam.capture("gauge", captureCallback);


