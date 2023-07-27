import { useRef, useState } from "react";
import Camera from 'react-html5-camera-photo';
import 'react-html5-camera-photo/build/css/index.css';

interface BoundingBox {
  left: number;
  top: number;
  width: number;
  height: number;
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
  const [isCameraVisible, setIsCameraVisible] = useState(true);
  const [isProcessing, setisProcessing] = useState(false);
  const [apiResponse, setApiResponse] = useState<CalculationResult | null>(null);
  const [apiError, setApiError] = useState(false);

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
        context.clearRect(0, 0, canvas.width, canvas.height);
        context.drawImage(image, 0, 0, canvas.width, canvas.height);
      };
    };
  }

  function handleTakePhoto(dataUri: string): void {
    console.log("handleTakePhoto");
    setisProcessing(true);
    setIsCameraVisible(false);
    const blob = dataURItoBlob(dataUri);
    setDataUri(dataUri);
    sendImage(blob);
  }

  const handleCanvasTouch = () => {
    handleCanvasClickAndTouch();
  };

  const handleCanvasClickAndTouch = () => {
    console.log("handleCanvasClickAndTouch");
    if (canvasRef.current) {
      setIsCameraVisible(true);
      setApiResponse(null);
    }
  };

  const sendImage = async (blob: Blob | null) => {
    if (blob) {
      try {
        const url = process.env.REACT_APP_COIN_COUNTER_URL || window.location.href + "backend/api";
        console.log("URL:" + url);
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

        const apiResponseJson: CalculationResult = await responseStr.json();
        console.log(apiResponseJson);
        setApiResponse(apiResponseJson);
        setisProcessing(false)

        if (canvasRef.current) {
          const canvas = canvasRef.current;
          const context = canvas.getContext("2d");
          if (context) {
            apiResponseJson.coinPredictions.forEach((obj) => {
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
              const boxHeight = 35;
              const boxWidth = 80;
              context.fillRect(scaledLeft, scaledTop - boxHeight, boxWidth, boxHeight);

              context.fillStyle = "white";
              context.font = "32px Helvetica Neue";
              const textMargin = 10;
              context.fillText(obj.name,
                scaledLeft + textMargin,
                scaledTop - textMargin
              );

            });
          }
        }
      } catch (error) {
        console.error("Error processing image: ", error);
        setApiResponse(null);
        setApiError(true);
        setisProcessing(false);
      }
    }
  };

  return (
    <div className="App">
      <div>
      <div>

        <div>
          <img id={'photo'} src={dataUri} alt='' style={{ display: 'none' }} />
        </div>
        <div
          style={{ position: "absolute", left: '50%', transform: 'scale(0.5) translate(-100%, -50%)', }}
        >

          {!isCameraVisible && (
          <canvas
            ref={canvasRef}
          >
          </canvas>
          )}
        </div>

          <div>
            {isProcessing && (
              <div className="d-flex gap-2 justify-content-center">
                <button className="btn btn-primary" type="button">
                  <span className="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
                  Procesando...
                </button>
              </div>
            )}

            {apiResponse && (

            <div>
              <div className="d-flex gap-2 justify-content-center py-5">
                <div className="row w-50">
                  <div className="col">
                    <h1>Total: $ {apiResponse.totalAmount}</h1>
                  </div>
                  <div className="col">
                    <h1>Peso: {apiResponse.totalWeight}</h1>
                  </div>
                </div>
              </div>
              <div className="d-flex justify-content-center">
                <button className="btn btn-primary d-inline-flex align-items-center" type="button" onClick={handleCanvasTouch} >Reiniciar</button>
              </div>
            </div>
            )}

            {apiError && (
              <div>
                <div className="alert alert-danger alert-dismissible fade show" role="alert">
                  Ups, se produjo un error!
                  <button type="button" className="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
                </div>
                <button className="btn btn-primary rounded-pill px-3" type="button" onClick={handleCanvasTouch} >Reiniciar</button>
              </div>
            )}
          </div>
        </div>


        {isCameraVisible && <div id="divCamera">
          <Camera
            onTakePhoto={handleTakePhoto}
            onTakePhotoAnimationDone={onTakePhotoAnimationDone}
            idealFacingMode={'environment'}
            sizeFactor={1}
            imageCompression={0}
            idealResolution={{ width: 1600, height: 900 }}
          />
        </div>}
      </div>
    </div>
  );
}


export default App;
