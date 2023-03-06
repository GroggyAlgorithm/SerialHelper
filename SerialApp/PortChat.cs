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
    


    

    /// <summary>
    /// Class constructor
    /// </summary>
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
    
    
    
    /// <summary>
    /// Class constructor with passable serial port parameters
    /// </summary>
    /// <param name="desiredPort"></param>
    /// <param name="baudrate"></param>
    /// <param name="readTimeout"></param>
    /// <param name="writeTimeout"></param>
    /// <param name="parity"></param>
    /// <param name="dataBits"></param>
    /// <param name="stopBit"></param>
    /// <param name="handshake"></param>
    public PortChat(int desiredPort, int baudrate=1200, int readTimeout = 500, int writeTimeout = 500, Parity parity = Parity.None, DATA_BITS dataBits = DATA_BITS.Eight, StopBits stopBit = StopBits.One,
    Handshake handshake = Handshake.None)
    {
        currentComPort = "";
        //Set the desired port
        SetDesiredPort(desiredPort);

        //Set the desired baud rate
        SetCustomBaud(baudrate);

        //Set the desired timeouts
        SetReadTimeout(readTimeout);
        SetWriteTimeout(writeTimeout);

        //Set the parity
        SetParity(parity);

        //Set the data bits
        SetDataBits(dataBits);

        //Set the stop bits
        SetStopBit(stopBit);

        //Set the handshake
        SetHandshake(handshake);

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



    /// <summary>
    /// Class constructor with passable serial port parameters
    /// </summary>
    /// <param name="desiredPort"></param>
    /// <param name="baudrate"></param>
    /// <param name="readTimeout"></param>
    /// <param name="writeTimeout"></param>
    /// <param name="parity"></param>
    /// <param name="dataBits"></param>
    /// <param name="stopBit"></param>
    /// <param name="handshake"></param>
    public PortChat(string desiredPort, int baudrate=1200, int readTimeout = 500, int writeTimeout = 500, Parity parity = Parity.None, DATA_BITS dataBits = DATA_BITS.Eight, StopBits stopBit = StopBits.One,
    Handshake handshake = Handshake.None)
    {
        currentComPort = "";
        //Set the desired port
        SetDesiredPort(desiredPort);

        //Set the desired baud rate
        SetCustomBaud(baudrate);

        //Set the desired timeouts
        SetReadTimeout(readTimeout);
        SetWriteTimeout(writeTimeout);

        //Set the parity
        SetParity(parity);

        //Set the data bits
        SetDataBits(dataBits);

        //Set the stop bits
        SetStopBit(stopBit);

        //Set the handshake
        SetHandshake(handshake);

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



    /// <summary>
    /// Sets the serial port to the specified encoding
    /// </summary>
    public void SetToUtf8()
    {
        if(_serialPort != null) _serialPort.Encoding = System.Text.Encoding.UTF8;
    }



    /// <summary>
    /// Sets the serial port to the specified encoding
    /// </summary>
    public void SetToAscii()
    {
        if(_serialPort != null) _serialPort.Encoding = System.Text.Encoding.ASCII;
    }



    /// <summary>
    /// Sets the serial port to the specified encoding
    /// </summary>
    public void SetEncoding(System.Text.Encoding encoding)
    {
        if(_serialPort != null && encoding != null) _serialPort.Encoding = encoding;
    }



    /// <summary>
    /// Sets the serial ports buffer size to the specified value
    /// </summary>
    public void SetReadBufferSize(int bufferSize)
    {
        if(_serialPort != null && bufferSize > 0 && bufferSize < 100) _serialPort.ReadBufferSize = bufferSize;
    }



    /// <summary>
    /// Sets the serial ports buffer size to the specified value
    /// </summary>
    public void SetWriteBufferSize(int bufferSize)
    {
        if(_serialPort != null && bufferSize > 0 && bufferSize < 100) _serialPort.WriteBufferSize = bufferSize;
    }



    /// <summary>
    /// Set thes class variable to the one passed if requirements are met.
    /// Returns the status of if it was set good
    /// </summary>
    /// <param name="dataBits"></param>
    /// <returns></returns>
    public bool SetDataBits(string dataBits)
    {
        bool status = false;

        //If the standard values contains the passed value...
        if(DataBitOptions.TryGetValue(dataBits,out var dbOp))
        {
            //Set the status and variable
            currentDataBits = (int)dbOp;
            status = true;
        }

        return status;
    }



    /// <summary>
    /// Set thes class variable to the one passed if requirements are met.
    /// Returns the status of if it was set good
    /// </summary>
    /// <param name="dataBits"></param>
    /// <returns></returns>
    public bool SetDataBits(DATA_BITS dataBits)
    {
        //Set the status and variable
        currentDataBits = (int)dataBits;
        return true;
    }



    /// <summary>
    /// Set thes class variable to the one passed if requirements are met.
    /// Returns the status of if it was set good
    /// </summary>
    /// <param name="baudrate"></param>
    /// <returns></returns>
    public bool SetCustomBaud(string baudrate)
    {
        bool status = false;

        //If the passed value was parsed good...
        if(int.TryParse(baudrate,out var intBaud))
        {
            //If the value is within range...
            if(intBaud > 0 && intBaud <= 128000)
            {
                //Set the status and variable
                currentBaud = intBaud;
                status = true;
            }
        }
        return status;
    }



    /// <summary>
    /// Set thes class variable to the one passed if requirements are met.
    /// Returns the status of if it was set good
    /// </summary>
    /// <param name="baudrate"></param>
    /// <returns></returns>
    public bool SetStandardBaud(string baudrate)
    {
        bool status = false;

        //If the standard values contains the passed value...
        if(StandardPossibleBauds.Contains(baudrate) == true)
        {
            //If the passed value was parsed good...
            if(int.TryParse(baudrate,out var intBaud))
            {
                //Set the status and variable
                currentBaud = intBaud;
                status = true;
            }
        }
        return status;
    }



    /// <summary>
    /// Set thes class variable to the one passed if requirements are met.
    /// Returns the status of if it was set good
    /// </summary>
    /// <param name="baudrate"></param>
    /// <returns></returns>
    public bool SetCustomBaud(int baudrate)
    {
        bool status = false;

        //If the passed value is within range...
        if(baudrate > 0 && baudrate <= 128000)
        {
            //Set the status and variable
            currentBaud = baudrate;
            status = true;
        }
        return status;
    }



    /// <summary>
    /// Set thes class variable to the one passed if requirements are met.
    /// Returns the status of if it was set good
    /// </summary>
    /// <param name="handshake"></param>
    /// <returns></returns>
    public bool SetHandshake(Handshake handshake)
    {
        //Set the status and variable
        this.currentHandshake = handshake;
        return true;
    }



    /// <summary>
    /// Set thes class variable to the one passed if requirements are met.
    /// Returns the status of if it was set good
    /// </summary>
    /// <param name="handshake"></param>
    /// <returns></returns>
    public bool SetHandshake(string handshake)
    {
        bool status = false;

        //If the standard values contains the passed value...
        if(HandShakeOptions.TryGetValue(handshake,out var hsOption))
        {
            //Set the status and variable
            status = true;
            this.currentHandshake = hsOption;
        }

        return status;
    }



    /// <summary>
    /// Set thes class variable to the one passed if requirements are met.
    /// Returns the status of if it was set good
    /// </summary>
    /// <param name="stopBit"></param>
    /// <returns></returns>
    public bool SetStopBit(StopBits stopBit)
    {
        //Set the status and variable
        this.currentStopBits = stopBit;
        return true;
    }



    /// <summary>
    /// Set thes class variable to the one passed if requirements are met.
    /// Returns the status of if it was set good
    /// </summary>
    /// <param name="stopBit"></param>
    /// <returns></returns>
    public bool SetStopBit(string stopBit)
    {
        bool status = false;

        //If the standard values contains the passed value...
        if(StopBitOptions.TryGetValue(stopBit,out var sbOption))
        {
            //Set the status and variable
            status = true;
            this.currentStopBits = sbOption;
        }

        return status;
    }



    /// <summary>
    /// Set thes class variable to the one passed if requirements are met.
    /// Returns the status of if it was set good
    /// </summary>
    /// <param name="parity"></param>
    /// <returns></returns>
    public bool SetParity(Parity parity)
    {
        //Set the status and variable
        this.currentParity = parity;
        return true;
    }



    /// <summary>
    /// Set thes class variable to the one passed if requirements are met.
    /// Returns the status of if it was set good
    /// </summary>
    /// <param name="parity"></param>
    /// <returns></returns>
    public bool SetParity(string parity)
    {
        bool status = false;

        //If the standard values contains the passed value...
        if(ParityOptions.TryGetValue(parity,out var parOption))
        {
            //Set the status and variable
            status = true;
            this.currentParity = parOption;
        }

        return status;
    }



    /// <summary>
    /// Set thes class variable to the one passed if requirements are met.
    /// Returns the status of if it was set good
    /// </summary>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public bool SetReadTimeout(int timeout)
    {
        bool status = false;

        //If the passed value is within range...
        if(timeout > 0 && timeout < 10000)
        {
            //Set the status and variable
            this.currentReadTimeout = timeout;
            status = true;
        }

        return status;
    }



    /// <summary>
    /// Set thes class variable to the one passed if requirements are met.
    /// Returns the status of if it was set good
    /// </summary>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public bool SetReadTimeout(string timeout)
    {
        bool status = false;

        //If the passed value was parsed good...
        if(int.TryParse(timeout,out var to))
        {
            if(StandardTransmissionDelayOptions.Contains(timeout))
            {
                //Set the status and variable
                this.currentReadTimeout = to;
                status = true;
            }
        }
        //else if the timeout is forever or none...
        else if(timeout.ToLower() == "forever" || timeout.ToLower() == "none")
        {
            //Set the timeout to -1
            this.currentReadTimeout = -1;
            status = true;
        }
        return status;
    }



    /// <summary>
    /// Set thes class variable to the one passed if requirements are met.
    /// Returns the status of if it was set good
    /// </summary>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public bool SetWriteTimeout(int timeout)
    {
        bool status = false;

        //If the passed value is within range...
        if(timeout > 0 && timeout < 10000)
        {
            //Set the status and variable
            this.currentWriteTimeout = timeout;
            status = true;
        }

        return status;
    }



    /// <summary>
    /// Set thes class variable to the one passed if requirements are met.
    /// Returns the status of if it was set good
    /// </summary>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public bool SetWriteTimeout(string timeout)
    {
        bool status = false;

        //If the passed value was parsed good...
        if(int.TryParse(timeout,out var to))
        {
            //If the standard values contain the parsed value...
            if(StandardTransmissionDelayOptions.Contains(timeout))
            {
                //Set the status and variable
                this.currentWriteTimeout = to;
                status = true;
            }
        }
        //else if the timeout is forever or none...
        else if(timeout.ToLower() == "forever" || timeout.ToLower() == "none")
        {
            //Set the timeout to -1
            this.currentWriteTimeout = -1;
            status = true;
        }
        return status;
    }



    /// <summary>
    /// Set thes class variable to the one passed if requirements are met.
    /// Returns the status of if it was set good
    /// </summary>
    /// <param name="desiredPort"></param>
    /// <returns></returns>
    public bool SetDesiredPort(string desiredPort)
    {
        bool status  = false;
        
        //If the passed string is not null and is not empty and is not white space...
        if(!string.IsNullOrEmpty(desiredPort) && !string.IsNullOrWhiteSpace(desiredPort))
        {
            //If the available ports contains the passed port...
            if(AvailablePorts.Contains(desiredPort))
            {
                //Set the current port variable
                currentComPort = desiredPort;

                //Set the status to true
                status = true;
            }
        }

        return status;
    }



    /// <summary>
    /// Set thes class variable to the one passed if requirements are met.
    /// Returns the status of if it was set good
    /// </summary>
    /// <param name="desiredPort"></param>
    /// <returns></returns>
    public bool SetDesiredPort(int desiredPort)
    {
        bool status  = false;

        //If the available ports contains the passed port...
        if(AvailablePorts.Contains("COM"+desiredPort))
        {
            //Set the current port variable
            currentComPort = "COM"+desiredPort;

            //Set the status to true
            status = true;
        }
        

        return status;
    }



    /// <summary>
    /// Reloads the port, no try catch
    /// </summary>
    /// <param name="desiredPort"></param>
    /// <param name="baudrate"></param>
    /// <param name="readTimeout"></param>
    /// <param name="writeTimeout"></param>
    /// <param name="parity"></param>
    /// <param name="dataBits"></param>
    /// <param name="stopBit"></param>
    /// <param name="handshake"></param>
    public void ReloadPort(string desiredPort, int baudrate = 1200, int readTimeout = 500, int writeTimeout = 500, Parity parity = Parity.None, DATA_BITS dataBits = DATA_BITS.Eight, StopBits stopBit = StopBits.One,
    Handshake handshake = Handshake.None)
    {
        //Clear the current timeout
        currentComPort = "";

        //Set the desired port
        SetDesiredPort(desiredPort);

        //Set the desired baud rate
        SetCustomBaud(baudrate);

        //Set the desired timeouts
        SetReadTimeout(readTimeout);
        SetWriteTimeout(writeTimeout);

        //Set the parity
        SetParity(parity);

        //Set the data bits
        SetDataBits(dataBits);

        //Set the stop bits
        SetStopBit(stopBit);

        //Set the handshake
        SetHandshake(handshake);




        //If the serial port is not null...
        if (_serialPort != null)
        {
            //If the serial port is open...
            if (_serialPort.IsOpen == true)
            {
                //Try to close the port
                TryClosePort();
                // status = true;
            }
        }
        else
        {
            _serialPort = new SerialPort();
        }

        //Reinitialize the serial port
        //_serialPort = new SerialPort(currentComPort, currentBaud, currentParity, currentDataBits, currentStopBits);

        //Set any variables
        _serialPort.PortName = currentComPort;
        _serialPort.BaudRate = currentBaud;
        _serialPort.Parity = currentParity;
        _serialPort.DataBits = currentDataBits;
        _serialPort.StopBits = currentStopBits;
        _serialPort.ReadTimeout = currentReadTimeout;
        _serialPort.WriteTimeout = currentWriteTimeout;
        _serialPort.Handshake = currentHandshake;

    }



    /// <summary>
    /// Reloads the port, no try catch
    /// </summary>
    public void ReloadPort()
    {
        //If the serial port is not null...
        if(_serialPort != null)
        {
            //If the serial port is open...
            if(_serialPort.IsOpen == true)
            {
                //Try to close the port
                TryClosePort();
                // status = true;
            }
        }
        else
        {
            _serialPort = new SerialPort();
        }

        //Reinitialize the serial port
        //_serialPort = new SerialPort(currentComPort, currentBaud, currentParity, currentDataBits, currentStopBits);
        
        //Set any variables
        _serialPort.PortName = currentComPort;
        _serialPort.BaudRate = currentBaud;
        _serialPort.Parity = currentParity;
        _serialPort.DataBits = currentDataBits;
        _serialPort.StopBits = currentStopBits;
        _serialPort.ReadTimeout = currentReadTimeout;
        _serialPort.WriteTimeout = currentWriteTimeout;
        _serialPort.Handshake = currentHandshake;
    }



    /// <summary>
    /// Trys to reload and open the port, returns the status of if it opened or not
    /// </summary>
    /// <returns></returns>
    public bool TryReloadAndOpen()
    {
        bool status = false;

        //try to...
        try
        {
            //If the serial port is not null...
            if(_serialPort != null)
            {
                //If the serial port is open...
                if(_serialPort.IsOpen)
                {
                    //Try to close the port
                    status = !TryClosePort();
                    // _serialPort.Close();
                }
            }
            else
            {
                _serialPort = new SerialPort();
            }

            //If the Available ports contains the current com port string...
            if(AvailablePorts.Contains(currentComPort) == true)
            {
                //Reinitialize the serial port
                //_serialPort = new SerialPort(currentComPort, currentBaud, currentParity, currentDataBits, currentStopBits);
                
                //Set any variables
                _serialPort.PortName = currentComPort;
                _serialPort.BaudRate = currentBaud;
                _serialPort.Parity = currentParity;
                _serialPort.DataBits = currentDataBits;
                _serialPort.StopBits = currentStopBits;
                _serialPort.ReadTimeout = currentReadTimeout;
                _serialPort.WriteTimeout = currentWriteTimeout;
                _serialPort.Handshake = currentHandshake;

                //Open the port
                _serialPort.Open();

                //Set the status to the serial ports open status
                status = _serialPort.IsOpen;
                //status = true;
            }
            
        }
        //Catch any exceptions and...
        catch
        {
            //Make sure to set status to false
            status = false;
        }

        return status;
    }



    /// <summary>
    /// Trys to open the port, returns the status of if it opened or not
    /// </summary>
    /// <param name="desiredPort"></param>
    /// <param name="baudrate"></param>
    /// <param name="parity"></param>
    /// <param name="stopBit"></param>
    /// <returns></returns>
    public bool TryOpenPort(string desiredPort, string baudrate, string parity, string stopBit)
    {
        bool status = false;

        //If the serial port is not null...
        if(_serialPort != null)
        {
            //If the serial port is open...
            if(_serialPort.IsOpen == true)
            {
                //Try to close the port
                status = !TryClosePort();
                // status = true;
            }
        }
        else
        {
            _serialPort = new SerialPort();
        }

        //If still false at this Point...
        if(status == false)
        {
            //try to...
            try
            {
                
                //If setting the values passed worked...
                if(SetDesiredPort(desiredPort) && SetStandardBaud(baudrate) && SetStopBit(stopBit) && SetParity(parity))
                {
                    //Set any unadressed variables
                    currentDataBits = 8;
                    currentHandshake = Handshake.None;
                    currentReadTimeout = 500;
                    currentWriteTimeout = 500;

                    //Reinitialize the serial port
                    //_serialPort = new SerialPort(currentComPort, currentBaud, currentParity, currentDataBits, currentStopBits);
                    
                    //Set any variables
                    _serialPort.PortName = currentComPort;
                    _serialPort.BaudRate = currentBaud;
                    _serialPort.Parity = currentParity;
                    _serialPort.DataBits = currentDataBits;
                    _serialPort.StopBits = currentStopBits;
                    _serialPort.ReadTimeout = currentReadTimeout;
                    _serialPort.WriteTimeout = currentWriteTimeout;
                    _serialPort.Handshake = currentHandshake;

                    //Open the port
                    _serialPort.Open();

                    //Set the status to the serial ports open status
                    status = _serialPort.IsOpen;
                    //status = true;
                }
            }
            //Catch any exception and...
            catch
            {
                //Set the status to false
                status = false;
            }
            
        }
        return status;
    }



    /// <summary>
    /// Trys to open the port, returns the status of if it opened or not
    /// </summary>
    /// <param name="desiredPort"></param>
    /// <param name="baudrate"></param>
    /// <param name="parity"></param>
    /// <param name="stopBit"></param>
    /// <param name="handshake"></param>
    /// <param name="readTimeout"></param>
    /// <param name="writeTimeout"></param>
    /// <param name="dataBits"></param>
    /// <returns></returns>
    public bool TryOpenPort(string desiredPort, string baudrate, string parity, string stopBit, string handshake,
        string readTimeout, string writeTimeout, string dataBits)
    {
        bool status = false;

        //If the serial port is not null...
        if (_serialPort != null)
        {
            //If the serial port is open...
            if (_serialPort.IsOpen == true)
            {
                //Try to close the port
                status = !TryClosePort();
                // status = true;
            }
        }
        else
        {
            _serialPort = new SerialPort();
        }

        //If still false at this Point...
        if (status == false)
        {
            //try to...
            try
            {
                //If setting part 1 of the values passed worked...
                if (SetDesiredPort(desiredPort) && SetStandardBaud(baudrate) && SetStopBit(stopBit) && SetParity(parity))
                {
                    //If setting these values worked...
                    if (SetDataBits(dataBits) && SetHandshake(handshake) && SetReadTimeout(readTimeout) && SetWriteTimeout(writeTimeout))
                    {

                        //Reinitialize the serial port
                        //_serialPort = new SerialPort(currentComPort, currentBaud, currentParity, currentDataBits, currentStopBits);
                        
                        //Set any variables
                        _serialPort.PortName = currentComPort;
                        _serialPort.BaudRate = currentBaud;
                        _serialPort.Parity = currentParity;
                        _serialPort.DataBits = currentDataBits;
                        _serialPort.StopBits = currentStopBits;
                        _serialPort.ReadTimeout = currentReadTimeout;
                        _serialPort.WriteTimeout = currentWriteTimeout;
                        _serialPort.Handshake = currentHandshake;

                        //Open the port
                        _serialPort.Open();

                        //Set the status to the serial ports open status
                        status = _serialPort.IsOpen;
                        //status = true;
                    }

                }
            }
            //Catch any exceptions and...
            catch
            {
                //Make sure to set status to false
                status = false;
            }

        }
        return status;
    }



    /// <summary>
    /// Trys to open the port, returns the status of if it opened or not
    /// </summary>
    /// <param name="desiredPort"></param>
    /// <param name="baudrate"></param>
    /// <param name="parity"></param>
    /// <param name="stopBit"></param>
    /// <param name="handshake"></param>
    /// <returns></returns>
    public bool TryOpenPort(string desiredPort, string baudrate, string parity, string stopBit, string handshake)
    {
        bool status = false;

        //If the serial port is not null...
        if(_serialPort != null)
        {
            //If the serial port is open...
            if(_serialPort.IsOpen == true)
            {
                //Try to close the port
                status = !TryClosePort();
                // status = true;
            }
        }
        else
        {
            _serialPort = new SerialPort();
        }

        //If still false at this Point...
        if(status == false)
        {
            //try to...
            try
            {
                //If setting part 1 of the values passed worked...
                if(SetDesiredPort(desiredPort) && SetStandardBaud(baudrate) && SetStopBit(stopBit) && SetParity(parity))
                {
                    //If setting these values worked...
                    if(SetHandshake(handshake))
                    {
                        //Set any unadressed variables
                        currentReadTimeout = 500;
                        currentWriteTimeout = 500;

                        //Reinitialize the serial port
                        //_serialPort = new SerialPort(currentComPort, currentBaud, currentParity, currentDataBits, currentStopBits);
                        
                        //Set any variables
                        _serialPort.PortName = currentComPort;
                        _serialPort.BaudRate = currentBaud;
                        _serialPort.Parity = currentParity;
                        _serialPort.DataBits = currentDataBits;
                        _serialPort.StopBits = currentStopBits;
                        _serialPort.ReadTimeout = currentReadTimeout;
                        _serialPort.WriteTimeout = currentWriteTimeout;
                        _serialPort.Handshake = currentHandshake;

                        //Open the port
                        _serialPort.Open();

                        //Set the status to the serial ports open status
                        status = _serialPort.IsOpen;
                        //status = true;
                    }
                    
                }
            }
            //Catch any exceptions and...
            catch
            {
                //Make sure to set status to false
                status = false;
            }
            
        }
        return status;
    }



    /// <summary>
    /// Trys to open the port, returns the status of if it opened or not
    /// </summary>
    /// <returns></returns>
    public bool TryOpenPort()
    {
        bool status = false;

        //If the serial port is not null...
        if(_serialPort != null)
        {
            //If the serial port is open...
            if(_serialPort.IsOpen)
            {
                //Set the status to true
                status = true;
            }
            //else...
            else
            {
                //try to...
                try
                {
                    //Open the port and set the status 
                    _serialPort.Open();

                    //Set the status to the serial ports open status
                    status = _serialPort.IsOpen;
                }
                catch
                {
                    status = false;
                }
            }
        }

        return status;
    }


    /// <summary>
    /// Wait callback port close
    /// </summary>
    /// <param name="obj"></param>
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



    /// <summary>
    /// Trys to close the port, returns the status of if it closed or not
    /// </summary>
    /// <returns></returns>
    public bool TryClosePort()
    {
        bool status = false;

        //Schedule closing the port and set the status to the queue response
        status = ThreadPool.QueueUserWorkItem(new WaitCallback(ClosePort));

        //If queued...
        if(status == true)
        {
            //Set a variable for safety, avoid infinite loops
            short safetyCounts = short.MaxValue - 1;

            //do this...
            do
            {
                //Set the status to the opposite of the port being open
                status = !((_serialPort.IsOpen));

                //Subtract from the safety count
                safetyCounts--;

            } 
            //While the port is open and the safety counts are above 0
            while ((_serialPort.IsOpen) && (safetyCounts >= 0));

        }
        
        return status;
    }



    /// <summary>
    /// Writes the passed value to the port
    /// </summary>
    /// <param name="s"></param>
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



    /// <summary>
    /// Writes the passed value to the port
    /// </summary>
    /// <param name="c"></param>
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



    /// <summary>
    /// Writes the passed value to the port
    /// </summary>
    /// <param name="b"></param>
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



    /// <summary>
    /// Writes the passed value to the port
    /// </summary>
    /// <param name="bytes"></param>
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



    /// <summary>
    /// Writes the passed value to the port
    /// </summary>
    /// <param name="chars"></param>
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



    /// <summary>
    /// Writes the passed value to the port
    /// </summary>
    /// <param name="value"></param>
    public void WriteToPort(ICollection<char> value)
    {
        if(_serialPort != null)
        {
            if(_serialPort.IsOpen)
            {
                _serialPort.Write(value.ToArray(),0,value.Count);
            }
        }
    }



    /// <summary>
    /// Writes the passed value to the port
    /// </summary>
    /// <param name="value"></param>
    public void WriteToPort(ICollection<byte> value)
    {
        if(_serialPort != null)
        {
            if(_serialPort.IsOpen)
            {
                _serialPort.Write(value.ToArray(),0,value.Count);
            }
        }
    }



    /// <summary>
    /// Waits and reads a byte from the port
    /// </summary>
    /// <returns></returns>
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


    
    /// <summary>
    /// Reads any stromgs that exist on the port
    /// </summary>
    /// <returns></returns>
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


    
    /// <summary>
    /// Reads any stromgs that exist on the port
    /// </summary>
    /// <returns></returns>
    public string ReadExisting(int maxCount)
    {
        string returnValue = "";

        if(_serialPort != null)
        {
            if(_serialPort.IsOpen)
            {
                while(maxCount > 0 && AvailableBytes > 0)
                {
                    returnValue += _serialPort.ReadExisting();
                }
                
            }
        }

        return returnValue;
    }


    
}