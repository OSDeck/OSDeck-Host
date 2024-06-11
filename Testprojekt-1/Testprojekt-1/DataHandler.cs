using Comahandler;
using SkiaSharp;

namespace Testprojekt_1
{
    internal class DataHandler
    {
        private ComHandler _comHandler;

        public DataHandler(ComHandler comHandler)
        {
            _comHandler = comHandler;
        }

        public void SendDeleteData(int globalId)
        {
            var data = new
            {
                globalId = globalId,
                delete = true
            };
            _comHandler.SendData(data);
        }

        public void SendObjectData(int globalId, int objType, float x, float y, string text = null, float sizeX = 0, float sizeY = 0, string color = null, string secondaryColor = null, string imagePath = null)
        {
            var data = new
            {
                globalId = globalId,
                objType = objType,
                Position = new
                {
                    x = x,
                    y = y
                },
                Properties = new
                {
                    Text = text,
                    SizeX = sizeX,
                    SizeY = sizeY,
                    Color = color,
                    SecondaryColor = secondaryColor,
                    Image = imagePath != null ? ImageToUint32Array(imagePath) : null
                }
            };
            _comHandler.SendData(data);
        }

        private uint[] ImageToUint32Array(string imagePath)
        {
            try
            {
                using (SKBitmap bitmap = SKBitmap.Decode(imagePath))
                {
                    int width = bitmap.Width;
                    int height = bitmap.Height;
                    uint[] pixelArray = new uint[width * height];

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            SKColor pixelColor = bitmap.GetPixel(x, y);
                            uint pixelValue = (uint)(pixelColor.Alpha << 24 | pixelColor.Red << 16 | pixelColor.Green << 8 | pixelColor.Blue);
                            pixelArray[y * width + x] = pixelValue;
                        }
                    }

                    return pixelArray;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error converting image to uint32_t array: " + ex.Message);
                return null;
            }
        }
    }
}

