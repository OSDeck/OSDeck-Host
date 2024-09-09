using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.IO.Ports;
using Newtonsoft.Json;

namespace Testprojekt_1.Helpers
{
    internal class ComHandler
    {
        private SerialPort _serialPort;

        public ComHandler(string portName, int baudRate = 115200)
        {
            _serialPort = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);
            _serialPort.Open();
        }

        public void SendData<T>(T data)
        {
            if (_serialPort.IsOpen)
            {
                string json = JsonConvert.SerializeObject(data);
                _serialPort.WriteLine(json + "\n"); // Sende JSON plus neue Zeile, um das Ende der Übertragung zu kennzeichnen
            }
            else
            {
                Console.WriteLine("Serial port is not open.");
            }
        }

        public string ReceiveData()
        {
            if (_serialPort.IsOpen && _serialPort.BytesToRead > 0)
            {
                string receivedJson = _serialPort.ReadLine();
                return receivedJson;
            }
            return null;
        }

        public void Close()
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
            }
        }
    }
}
