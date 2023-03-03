using System.Drawing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using System.Linq;

public class PortChat
{
    public System.IO.Ports.SerialPort _serialPort{get; private set;}
    public string currentComPort{get;private set;}
    public int currentBaud{get;private set;}
    public Parity currentParity{get; private set;}
    public int currentDataBits{get;private set;}
    public StopBits currentStopBits{get; private set;}
    public int currentReadTimeout{get;private set;}
    public int currentWriteTimeout{get;private set;}
    public Handshake currentHandshake{get; private set;}

    public static string[] AvailablePorts => System.IO.Ports.SerialPort.GetPortNames();

    public int AvailableBytes => _serialPort.BytesToRead;

    public bool IsOpen => _serialPort.IsOpen;

    public enum DATA_BITS
    {
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8
    };
    

    public static readonly Dictionary<string, DATA_BITS> DataBitOptions = new Dictionary<string, DATA_BITS>()
    {
        ["4"] = DATA_BITS.Four,
        ["5"] = DATA_BITS.Five,
        ["6"] = DATA_BITS.Six,
        ["7"] = DATA_BITS.Seven,
        ["8"] = DATA_BITS.Eight

    };


    public static readonly Dictionary<string, Parity> ParityOptions = new Dictionary<string, Parity>()
    {
        ["None"] = Parity.None,
        ["Even"] = Parity.Even,
        ["Odd"] = Parity.Odd,
        ["Mark"] = Parity.Mark,
        ["Space"] = Parity.Space
    };


    public static readonly Dictionary<string, StopBits> StopBitOptions = new Dictionary<string, StopBits>()
    {
        ["1"] = StopBits.One,
        ["1.5"] = StopBits.OnePointFive,
        ["2"] = StopBits.Two
    };



    public static readonly Dictionary<string, Handshake> HandShakeOptions = new Dictionary<string, Handshake>()
    {
        ["None"] = Handshake.None,
        ["XOnXOff"] = Handshake.XOnXOff,
        ["Send (Rq)"] = Handshake.RequestToSend,
        ["XOnXOff (Rq)"] = Handshake.RequestToSendXOnXOff
    };



    public static readonly string[] StandardTransmissionDelayOptions = new string[]
    {
        "-1","100","500","1000","5000"
    };



    public static readonly string[] StandardPossibleBauds = new string[]
    {
        "75","110","134","150","300","600","1200","1800","2400","4800","7200","9600","14400","19200","38400","57600","115200","128000"
    };
    


    public void SetToUtf8()
    {
        _serialPort.Encoding = System.Text.Encoding.UTF8;
    }

    public void SetToAscii()
    {
        _serialPort.Encoding = System.Text.Encoding.ASCII;
    }

    
    public PortChat()
    {
        currentComPort = "COM1";
        currentBaud = 9600;
        currentParity = Parity.None;
        currentReadTimeout = 500;
        currentWriteTimeout = 500;
        currentDataBits = 8;
        currentHandshake = Handshake.None;

        if(_serialPort != null)
        {
            if(_serialPort.IsOpen)
            {
                _serialPort.Close();
            }
        }
        _serialPort = new SerialPort();
        _serialPort.ReadTimeout = currentReadTimeout;
        _serialPort.WriteTimeout = currentWriteTimeout;

    }
    
    
    
    public PortChat(int desiredPort, int baudrate=1200, int readTimeout = 500, int writeTimeout = 500, Parity parity = Parity.None, DATA_BITS dataBits = DATA_BITS.Eight, StopBits stopBit = StopBits.One,
    Handshake handshake = Handshake.None)
    {
        currentComPort = "";
        SetDesiredPort(desiredPort);
        SetCustomBaud(baudrate);
        SetReadTimeout(readTimeout);
        SetWriteTimeout(writeTimeout);

        currentParity = parity;
        currentDataBits = (int)dataBits;
        currentStopBits = StopBits.One;
        currentHandshake = Handshake.None;

        if(_serialPort != null)
        {
            if(_serialPort.IsOpen)
            {
                _serialPort.Close();
            }
        }
        
        _serialPort = new SerialPort(currentComPort,currentBaud,currentParity,currentDataBits,currentStopBits);
        _serialPort.ReadTimeout = currentReadTimeout;
        _serialPort.WriteTimeout = currentWriteTimeout;


    }



    public PortChat(string desiredPort, int baudrate=1200, int readTimeout = 500, int writeTimeout = 500, Parity parity = Parity.None, DATA_BITS dataBits = DATA_BITS.Eight, StopBits stopBit = StopBits.One,
    Handshake handshake = Handshake.None)
    {
        currentComPort = "";
        SetDesiredPort(desiredPort);
        SetCustomBaud(baudrate);
        SetReadTimeout(readTimeout);
        SetWriteTimeout(writeTimeout);

        currentParity = parity;
        currentDataBits = (int)dataBits;
        currentStopBits = StopBits.One;
        currentHandshake = Handshake.None;

        if(_serialPort != null)
        {
            if(_serialPort.IsOpen)
            {
                _serialPort.Close();
            }
        }
        
        _serialPort = new SerialPort(currentComPort,currentBaud,currentParity,currentDataBits,currentStopBits);
        _serialPort.ReadTimeout = currentReadTimeout;
        _serialPort.WriteTimeout = currentWriteTimeout;

    }


    public bool SetDataBits(string dataBits)
    {
        bool status = false;

        if(DataBitOptions.TryGetValue(dataBits,out var dbOp))
        {
            currentDataBits = (int)dbOp;
            status = true;
        }

        return status;
    }


    public bool SetDataBits(DATA_BITS dataBits)
    {
        currentDataBits = (int)dataBits;
        return true;
    }


    public bool SetCustomBaud(string baudrate)
    {
        bool status = false;

        if(int.TryParse(baudrate,out var intBaud))
        {
            if(intBaud > 0 && intBaud <= 128000)
            {
                currentBaud = intBaud;
                status = true;
            }
        }
        return status;
    }



    public bool SetStandardBaud(string baudrate)
    {
        bool status = false;

        if(StandardPossibleBauds.Contains(baudrate) == true)
        {
            if(int.TryParse(baudrate,out var intBaud))
            {
                currentBaud = intBaud;
                status = true;
            }
        }
        return status;
    }



    public bool SetCustomBaud(int baudrate)
    {
        bool status = false;
        if(baudrate > 0 && baudrate <= 128000)
        {
            currentBaud = baudrate;
            status = true;
        }
        return status;
    }



    public bool SetHandshake(Handshake handshake)
    {
        this.currentHandshake = handshake;
        return true;
    }



    public bool SetHandshake(string handshake)
    {
        bool status = false;

        if(HandShakeOptions.TryGetValue(handshake,out var hsOption))
        {
            status = true;
            this.currentHandshake = hsOption;
        }

        return status;
    }


    public bool SetStopBit(StopBits stopBit)
    {
        this.currentStopBits = stopBit;
        return true;
    }



    public bool SetStopBit(string stopBit)
    {
        bool status = false;

        if(StopBitOptions.TryGetValue(stopBit,out var sbOption))
        {
            status = true;
            this.currentStopBits = sbOption;
        }

        return status;
    }



    public bool SetParity(Parity parity)
    {
        this.currentParity = parity;
        return true;
    }



    public bool SetParity(string parity)
    {
        bool status = false;

        if(ParityOptions.TryGetValue(parity,out var parOption))
        {
            status = true;
            this.currentParity = parOption;
        }

        return status;
    }



    public bool SetReadTimeout(int timeout)
    {
        bool status = false;

        if(timeout > 0 && timeout < 10000)
        {
            this.currentReadTimeout = timeout;
            status = true;
        }

        return status;
    }



    public bool SetReadTimeout(string timeout)
    {
        bool status = false;
        if(int.TryParse(timeout,out var to))
        {
            if(StandardTransmissionDelayOptions.Contains(timeout))
            {
                this.currentReadTimeout = to;
                status = true;
            }
        }
        else if(timeout.ToLower() == "forever" || timeout.ToLower() == "none")
        {
            this.currentReadTimeout = -1;
            status = true;
        }
        return status;
    }



    public bool SetWriteTimeout(int timeout)
    {
        bool status = false;

        if(timeout > 0 && timeout < 10000)
        {
            this.currentWriteTimeout = timeout;
            status = true;
        }

        return status;
    }



    public bool SetWriteTimeout(string timeout)
    {
        bool status = false;
        if(int.TryParse(timeout,out var to))
        {
            if(StandardTransmissionDelayOptions.Contains(timeout))
            {
                this.currentWriteTimeout = to;
                status = true;
            }
        }
        else if(timeout.ToLower() == "forever" || timeout.ToLower() == "none")
        {
            this.currentWriteTimeout = -1;
            status = true;
        }
        return status;
    }



    public bool SetDesiredPort(string desiredPort)
    {
        bool status  = false;

        if(!string.IsNullOrEmpty(desiredPort) && !string.IsNullOrWhiteSpace(desiredPort))
        {
            if(AvailablePorts.Contains(desiredPort))
            {
                currentComPort = desiredPort;
                status = true;
            }
        }

        return status;
    }



    public bool SetDesiredPort(int desiredPort)
    {
        bool status  = false;

        if(AvailablePorts.Contains("COM"+desiredPort))
        {
            currentComPort = "COM"+desiredPort;
            status = true;
        }
        

        return status;
    }



    public void ReloadPort(string desiredPort, int baudrate=1200, int readTimeout = 500, int writeTimeout = 500, Parity parity = Parity.None, DATA_BITS dataBits = DATA_BITS.Eight, StopBits stopBit = StopBits.One,
    Handshake handshake = Handshake.None)
    {
        var portNames = AvailablePorts;
        currentComPort = "";

       

        if(!string.IsNullOrEmpty(desiredPort) && !string.IsNullOrWhiteSpace(desiredPort))
        {
            if(portNames.Contains(desiredPort))
            {
                currentComPort = desiredPort;
            }
            
        }
        
        if(string.IsNullOrEmpty(currentComPort))
        {
            throw new Exception("The port name must be real");
        }

        if(baudrate > 0)
        {
            currentBaud = baudrate;
        }
        else
        {
            throw new Exception("Baud rate must be more than 0");
        }
        currentParity = parity;
        currentDataBits = (int)dataBits;
        currentStopBits = StopBits.One;
        currentReadTimeout = readTimeout;
        currentWriteTimeout = writeTimeout;
        currentHandshake = Handshake.None;

        if(_serialPort != null)
        {
            if(_serialPort.IsOpen)
            {
                _serialPort.Close();
            }
        }
        _serialPort = new SerialPort(currentComPort,currentBaud,currentParity,currentDataBits,currentStopBits);
        _serialPort.ReadTimeout = currentReadTimeout;
        _serialPort.WriteTimeout = currentWriteTimeout;
    }



    public void ReloadPort()
    {
        if(_serialPort != null)
        {
            if(_serialPort.IsOpen == true)
            {
                _serialPort.Close();
            }
        }

        _serialPort = new SerialPort(currentComPort,currentBaud,currentParity,currentDataBits,currentStopBits);
        _serialPort.ReadTimeout = currentReadTimeout;
        _serialPort.WriteTimeout = currentWriteTimeout;
    }


    public bool TryReloadAndOpen()
    {
        bool status = false;
        try
        {
            if(_serialPort != null)
            {
                if(_serialPort.IsOpen)
                {
                    _serialPort.Close();
                }
            }

            if(AvailablePorts.Contains(currentComPort) == true)
            {
                _serialPort = new SerialPort(currentComPort,currentBaud,currentParity,currentDataBits,currentStopBits);
                _serialPort.ReadTimeout = currentReadTimeout;
                _serialPort.WriteTimeout = currentWriteTimeout;

                status = true;
            }
            
        }
        catch
        {
            status = false;
        }

        return status;
    }


    public bool TryOpenPort(string desiredPort, string baudrate, string parity, string stopBit)
    {
        bool status = false;

        if(_serialPort != null)
        {
            if(_serialPort.IsOpen == true)
            {
                status = true;
            }
        }

        //If still false at this Point...
        if(status == false)
        {
            try
            {

                currentDataBits = 8;
                currentHandshake = Handshake.None;
                currentReadTimeout = 500;
                currentWriteTimeout = 500;

                if(SetDesiredPort(desiredPort) && SetStandardBaud(baudrate) && SetStopBit(stopBit) && SetParity(parity))
                {
                    _serialPort = new SerialPort(currentComPort,currentBaud,currentParity,currentDataBits,currentStopBits);
                    _serialPort.ReadTimeout = currentReadTimeout;
                    _serialPort.WriteTimeout = currentWriteTimeout;
                    _serialPort.Handshake = currentHandshake;
                    _serialPort.Open();
                    status = true;
                }
            }
            catch
            {
                status = false;
            }
            
        }
        return status;
    }



    public bool TryOpenPort(string desiredPort, string baudrate, string parity, string stopBit, string handshake,
        string readTimeout, string writeTimeout, string dataBits)
    {
        bool status = false;

        if(_serialPort != null)
        {
            if(_serialPort.IsOpen == true)
            {
                status = true;
            }
        }

        //If still false at this Point...
        if(status == false)
        {
            try
            {
                if(SetDesiredPort(desiredPort) && SetStandardBaud(baudrate) && SetStopBit(stopBit) && SetParity(parity))
                {
                    if(SetDataBits(dataBits) && SetHandshake(handshake) && SetReadTimeout(readTimeout) && SetWriteTimeout(writeTimeout))
                    {
                        _serialPort = new SerialPort(currentComPort,currentBaud,currentParity,currentDataBits,currentStopBits);
                        _serialPort.ReadTimeout = currentReadTimeout;
                        _serialPort.WriteTimeout = currentWriteTimeout;
                        _serialPort.Handshake = currentHandshake;
                        _serialPort.Open();
                        status = true;
                    }
                    
                }
            }
            catch
            {
                status = false;
            }
            
        }
        return status;
    }



    
    public bool TryOpenPort(string desiredPort, string baudrate, string parity, string stopBit, string handshake)
    {
        bool status = false;

        if(_serialPort != null)
        {
            if(_serialPort.IsOpen == true)
            {
                status = true;
            }
        }

        //If still false at this Point...
        if(status == false)
        {
            try
            {
                if(SetDesiredPort(desiredPort) && SetStandardBaud(baudrate) && SetStopBit(stopBit) && SetParity(parity))
                {
                    if(SetHandshake(handshake))
                    {
                        currentReadTimeout = 500;
                        currentWriteTimeout = 500;
                        _serialPort = new SerialPort(currentComPort,currentBaud,currentParity,currentDataBits,currentStopBits);
                        _serialPort.ReadTimeout = currentReadTimeout;
                        _serialPort.WriteTimeout = currentWriteTimeout;
                        _serialPort.Handshake = currentHandshake;
                        _serialPort.Open();
                        status = true;
                    }
                    
                }
            }
            catch
            {
                status = false;
            }
            
        }
        return status;
    }



    public bool TryOpenPort()
    {
        bool status = false;

        if(_serialPort != null)
        {
            if(_serialPort.IsOpen)
            {
                status = true;
            }
            else
            {
                try
                {
                    _serialPort.Open();
                    status = true;
                }
                catch
                {
                    status = false;
                }
            }
        }

        return status;
    }



    protected void ClosePort(object? obj)
    {
        if(_serialPort.IsOpen)
        {
            try
            {
                _serialPort.Close();
            }
            catch
            {
            }
        }
    }



    public bool TryClosePort()
    {
        bool status = false;
        status = ThreadPool.QueueUserWorkItem(new WaitCallback(ClosePort));
        // if(_serialPort.IsOpen)
        // {
        //     try
        //     {
        //         _serialPort.Close();
        //         status = true;
        //     }
        //     catch
        //     {
        //         status = false;
        //     }
        // }
        // else
        // {
        //     status = false;
        // }
        

        return status;
    }


    public void WriteToPort(string s)
    {
        if(_serialPort != null)
        {
            if(_serialPort.IsOpen)
            {
                _serialPort.Write(s);
            }
        }
    }


    public void WriteToPort(char c)
    {
        if(_serialPort != null)
        {
            if(_serialPort.IsOpen)
            {
                _serialPort.Write(c.ToString());
            }
        }
    }


    public void WriteToPort(byte b)
    {
        if(_serialPort != null)
        {
            if(_serialPort.IsOpen)
            {
                var bArr = new Byte[1];
                bArr[1] = b;

                _serialPort.Write(bArr,0,1);
            }
        }
    }



    public void WriteToPort(byte[] bytes)
    {
        if(_serialPort != null)
        {
            if(_serialPort.IsOpen)
            {

                _serialPort.Write(bytes,0,bytes.Length);
            }
        }
    }



    public void WriteToPort(char[] chars)
    {
        if(_serialPort != null)
        {
            if(_serialPort.IsOpen)
            {

                _serialPort.Write(chars,0,chars.Length);
            }
        }
    }



    public int WaitRead()
    {
        int returnValue = 0;

        if(_serialPort != null)
        {
            if(_serialPort.IsOpen)
            {

                returnValue = _serialPort.ReadByte();
            }
        }


        return returnValue;
    }



    public string ReadExisting()
    {
        string returnValue = "";

        if(_serialPort != null)
        {
            if(_serialPort.IsOpen)
            {

                returnValue = _serialPort.ReadExisting();
            }
        }

        return returnValue;
    }


    
}