﻿using Newtonsoft.Json;
using System.IO.Ports;
namespace Testprojekt_1.Helpers
{
    internal class ComHandler
    {
        public bool IsConnected { get; private set; } = false;
        private SerialPort _serialPort = null;
        private Timer _timer;
        private readonly int _interval = 30000;
        private readonly int _baudrate = 9600;
        private bool _isReading = false;

        public event Action ConnectionStatusChanged;

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

                        if (port.BytesToRead > 0)
                        {
                            string response = port.ReadLine().Trim();
                            if (response == "ARDUINO_HANDSHAKE")
                            {
                                _serialPort = port;
                                IsConnected = true;
                                ConnectionStatusChanged?.Invoke();
                                StopTimer();
                                StartReading();
                                break;
                            }
                            else
                            {
                                port.Close();
                                IsConnected = false;
                                ConnectionStatusChanged?.Invoke();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        IsConnected = false;
                        ConnectionStatusChanged?.Invoke();
                        Console.WriteLine(ex);
                    }
                }
            });
        }

        public bool SendData<T>(T data)
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                try
                {
                    string json = JsonConvert.SerializeObject(data);
                    _serialPort.WriteLine(json + "\n");
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    _serialPort.Close();
                    _serialPort = null;
                    StartScanning();
                    IsConnected = false;
                    ConnectionStatusChanged?.Invoke();
                    return false;
                }
            }
            else
            {
                StartScanning();
                IsConnected = false;
                ConnectionStatusChanged?.Invoke();
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
                    IsConnected = false;
                    ConnectionStatusChanged?.Invoke();
                    return;
                }

                if (_serialPort.BytesToRead > 0)
                {
                    try
                    {
                        string receivedJson = _serialPort.ReadLine();
                    }
                    catch (Exception ex)
                    {
                        _serialPort.Close();
                        _serialPort = null;
                        StopTimer();
                        StartScanning();
                        _isReading = false;
                        IsConnected = false;
                        ConnectionStatusChanged?.Invoke();
                        Console.WriteLine(ex);
                    }
                }

                _isReading = false;
            });
        }
    }
}