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
        private SerialPort _serialPort = null;
        private Timer _timer;
        private readonly int _interval = 30000;
        private readonly int _baudrate = 9600;
        private bool _isReading = false;

        public ComHandler()
        {
            StartScanning();
        }

        private void StartScanning()
        {
	        _timer = new Timer(async _ => FindDevice(), null, (long)0, _interval);
        }

        private void StartReading()
        {
			_timer = new Timer(async _ => ReceiveData(), null, (long)0, 100);
        }

        private void StopTimer()
        {
	        _timer.Dispose();
        }

		private async void FindDevice()
		{
			await Task.Run(() =>
			{
				//Test each port for a handshake response
				foreach (string portName in SerialPort.GetPortNames())
				{
					try
					{
						using SerialPort port = new SerialPort(portName, _baudrate);
						if (!port.IsOpen)
						{
							port.Open();
						}
						port.WriteLine("PC_HANDSHAKE");
						Thread.Sleep(1000);

						Console.WriteLine(portName);

						if (port.BytesToRead > 0)
						{
							string response = port.ReadLine().Trim();
							if (response == "ARDUINO_HANDSHAKE")
							{
								_serialPort = port;
								StopTimer();
								StartReading();
								break;
							}
							else
							{
								port.Close();
							}
						}
						else
						{
							port.Close();
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex);
					}
				}
			});
        }

        public bool SendData<T>(T data)
        {
            if (_serialPort.IsOpen)
            {
	            try
	            {
		            string json = JsonConvert.SerializeObject(data);
		            _serialPort.WriteLine(json +
		                                  "\n"); // Sende JSON plus neue Zeile, um das Ende der Übertragung zu kennzeichnen
		            return true;
	            }
	            catch (Exception ex)
	            {
		            Console.WriteLine(ex);
                    _serialPort.Close();
                    _serialPort = null;
                    StartScanning();
                    return false;
	            }
            }
            else
            {
                StartScanning();
	            return false;
            }
        }

        public async void ReceiveData()
        {
	        await Task.Run(() =>
	        {
		        if (_isReading) return;

		        _isReading = true;

		        if (!_serialPort.IsOpen)
		        {
					StopTimer();
				    StartScanning();
			        _isReading = false;
			        return;
		        }

				if (_serialPort.BytesToRead > 0)
			    {
		            try
		            {
						string receivedJson = _serialPort.ReadLine();
						// DO something with data
					}
				    catch (Exception ex)
			        {
						_serialPort.Close();
						_serialPort = null;
						StopTimer();
						StartScanning();
						_isReading = false;
						Console.WriteLine(ex);
		            }
	            }
				
	            _isReading = false;
	        });
		}
    }
}
