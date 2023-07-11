import React, { useRef, useEffect, useState } from "react";
import Camera from 'react-html5-camera-photo';
import 'react-html5-camera-photo/build/css/index.css';

interface BoundingBox {
  left: number;
  top: number;
  width: number;
  height: number;
}

interface ObjectData {
  probability: number;
  tagId: string;
  tagName: string;
  boundingBox: BoundingBox;
}

interface APIResponse {
  id: string;
  project: string;
  iteration: string;
  created: string;
  predictions: ObjectData[];
}

interface CoinPrediction {
  name: string;
  value: number;
  weight: number;
  probability: number;
  boundingBox: BoundingBox;
}

interface CalculationResult {
  totalAmount: number;
  totalWeight: number;
  coinPredictions: CoinPrediction[];
}


const IMAGE_TYPES = {
  PNG: 'png',
  JPG: 'jpg',
};

function dataURItoBlob(dataURI: string): Blob {
  let byteString = atob(dataURI.split(',')[1]);
  let mimeString = dataURI.split(',')[0].split(':')[1].split(';')[0];

  let ab = new ArrayBuffer(byteString.length);
  let ia = new Uint8Array(ab);
  for (let i = 0; i < byteString.length; i++) {
    ia[i] = byteString.charCodeAt(i);
  }
  let blob = new Blob([ab], { type: mimeString });
  return blob;
}

function App() {
  const [dataUri, setDataUri] = useState<string>('');
  const [isVisible, setIsVisible] = useState(true);
  
  const canvasRef = useRef<HTMLCanvasElement>(null);

  function onTakePhotoAnimationDone(): void {
    console.log("onTakePhotoAnimationDone");
    if (canvasRef.current) {
      const image = document.getElementById('photo') as HTMLImageElement;

      const canvas = canvasRef.current;
      const context = canvas.getContext("2d");
      if (context) {
        canvas.width = image.width;
        canvas.height = image.height;
        // canvas.height = image.height;
        console.log("Canvas: "+canvas.width + "x" + canvas.height);
        console.log("Image: "+image.width + "x" + image.height);
        context.clearRect(0, 0, canvas.width, canvas.height);
        context.drawImage(image, 0, 0, canvas.width, canvas.height);
      };
      setIsVisible(!isVisible);
    };
  }

  function handleTakePhoto(dataUri: string): void {
    const blob = dataURItoBlob(dataUri);
    setDataUri(dataUri);
    sendImage(blob);    
  }
  
  // const captureImage = () => {
  //   console.log("captureImage");
  //   if (videoRef.current && canvasRef.current) {
  //     const canvas = canvasRef.current;
  //     const context = canvas.getContext("2d");

  //     canvasRef.current.style.display = "block";
  //     videoRef.current.style.display = "none";

  //     canvas.width = videoRef.current.videoWidth;
  //     canvas.height = videoRef.current.videoHeight;
  //     if (context) {
  //       context.drawImage(
  //         videoRef.current,
  //         0,
  //         0,
  //         videoRef.current.videoWidth,
  //         videoRef.current.videoHeight
  //       );
  //       console.log("Canvas: "+canvas.width + "x" + canvas.height);
  //       canvas.toBlob(sendImage, "image/jpeg", 1);
  //     }
  //   }
  // };

  const handleVideoTouch = () => {
    handleCanvasClickAndTouch();
  };

  const handleCanvasTouch = () => {
    handleCanvasClickAndTouch();
  };

  const handleCanvasClickAndTouch = () => {
    if (canvasRef.current) {
      // if (canvasRef.current.style.display === "none") {
      //   canvasRef.current.style.display = "block";
      // } else {
      //   canvasRef.current.style.display = "none";
      // }
    }
  };

  const sendImage = async (blob: Blob | null) => {
    if (blob) {
      try {
        const url = process.env.COIN_COUNTER_URL || "https://localhost:7118/API";
        const responseStr = await fetch(url,
          {
            method: "POST",
            headers: {
              "Content-Type": "application/octet-stream",
            },
            body: blob,
          }
        );

        console.log(responseStr);

        const apiResponse: CalculationResult = await responseStr.json();

        console.log(apiResponse);

        if (canvasRef.current) {
          const canvas = canvasRef.current;
          const context = canvas.getContext("2d");
          if (context) {
            apiResponse.coinPredictions.forEach((obj) => {
              const { left, top, width, height } = obj.boundingBox;
              const scaledLeft = left * canvas.width;
              const scaledTop = top * canvas.height;
              const scaledWidth = width * canvas.width;
              const scaledHeight = height * canvas.height;

              context.beginPath();
              context.lineWidth = 5;
              context.strokeStyle = "red";
              context.rect(scaledLeft, scaledTop, scaledWidth, scaledHeight);
              context.stroke();

              context.fillStyle = "red";
              context.fillRect(scaledLeft, scaledTop, 80, 35);

              context.fillStyle = "white";
              context.font = "32px Helvetica Neue";
              context.fillText(obj.name,
                scaledLeft + 10,
                scaledTop + 25
              );

            });
          }
        }
      } catch (error) {
        console.error("Error sending image to Custom Vision API:", error);
      }
    }
  };

  return (
    <div className="App">
      <div>
        <img id={'photo'} src={dataUri} style={{ display: 'none' }}/>
      </div>
      <div 
        style={{ position: "absolute", left: '50%' , transform: 'scale(0.5) translate(-100%, -50%)', }}
      >
        <canvas 
          ref={canvasRef}  
          onTouchStart={handleCanvasTouch}
        ></canvas>
      </div>
      {isVisible && <div id="divCamera">
      <Camera
        onTakePhoto =  { handleTakePhoto } 
        onTakePhotoAnimationDone = {onTakePhotoAnimationDone}
        idealFacingMode={'environment'}
        sizeFactor={1}
        imageCompression={0}
        idealResolution={{ width: 1600, height: 900 }}
      />
      </div>}
    </div>
  );
  }


export default App;
