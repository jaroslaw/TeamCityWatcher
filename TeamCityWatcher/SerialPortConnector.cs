﻿using System;
using System.IO.Ports;
using System.Threading;

namespace TeamCityWatcher
{
    class SerialPortConnector
    {
        private readonly string _serialPortName;

        private const byte Off = 0;
        private const byte OneOn = 128; //switch no 8 =2^7
        private const byte TwoOn = 64; //switch no 7 =2^6
        private const byte ThreeOn = 32; //switch no 6 =2^5

        public SerialPortConnector(string serialPortName)
        {
            _serialPortName = serialPortName;
        }

        public void SetLights(bool greenOn, bool yellowOn, bool redOn)
        {
            var lights = Off;
            if (greenOn) lights += OneOn;
            if (yellowOn) lights += TwoOn;
            if (redOn) lights += ThreeOn;
            SetSwitch(lights);
        }

        private void SetSwitch(byte @switch)
        {
            try
            {
                var serialPort = GetSerialPort();
                WriteByte(@switch, serialPort);
                Thread.Sleep(500);
                serialPort.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Serial port error on port: " + _serialPortName, e);
            }
        }

        private static void WriteByte(byte @switch, SerialPort serialPort)
        {
            var data = new byte[4];
            data[0] = 3; //set command 
            data[1] = 0; //broadcast to all cards
            data[2] = @switch; //set switches 
            data[3] = (byte) (data[0] ^ data[1] ^ data[2]); //parity 
            serialPort.Write(data, 0, 4);
        }

        private SerialPort GetSerialPort()
        {
            var serialPort = new SerialPort
                                 {
                                     PortName = _serialPortName,
                                     BaudRate = 19200,
                                     Parity = Parity.None,
                                     DataBits = 8,
                                     StopBits = StopBits.One,
                                     Handshake = Handshake.None
                                 };
            serialPort.Open();
            return serialPort;
        }

        public void TurnOff()
        {
            SetSwitch(Off);
        }
    }
} 