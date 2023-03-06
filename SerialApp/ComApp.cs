using System.IO.Ports;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Partial class - Main class file
/// </summary>
public partial class CommsApp : SerialLoggingAppForm
{

    Dictionary<ToolStripMenuItem,Label> setupControlsToLabel = new Dictionary<ToolStripMenuItem, Label>();
    bool infoOnly = false;
    bool autoConnect = false;
    bool showAdvancedParameters = false;
    List<ToolStripMenuItem> settingsItems = new List<ToolStripMenuItem>();
    MenuStrip? menuStrip = new MenuStrip();
    string baud = "9600";
    string port = "";
    string parity = "None";
    string dataBit = "8";
    string stopBit = "1";
    string handshake = "None";
    string readTimeout = "500";
    string writeTimeout = "500";
    bool rxEnabled = false;
    bool txEnabled = false;
    int currentRxIndex = 0;
    int currentTxIndex = 0;
    bool overwriteRxData = false;
    bool reapeatTxData = false;
    bool rxIncreaseOnChars = false;
    TextBox rxBufferTextBox = new TextBox();
    int rxBufferSize = 0;
    TextBox txBufferTextBox = new TextBox();
    int txBufferSize = 0;
    int currentTxDelay = 1000;
    bool loggingEnabled = false;
    TextBox[] rxData = new TextBox[0];
    TextBox currentRxData = new TextBox();
    TextBox[] txData = new TextBox[0];
    TextBox delayMsTb = new TextBox();
    Panel rxBoxPanel = new Panel();
    Panel txBoxPanel = new Panel();
    Button statusButton = new Button();
    System.Windows.Forms.Timer txTimer = new System.Windows.Forms.Timer();
    SerialDataReceivedEventHandler ComReceiveHandler;

    List<string> possibleIncreaseCharacters = new List<string>();


    ComboBox comComboBox = new ComboBox();
    ComboBox parityComboBox = new ComboBox();
    ComboBox dataBitsComboBox = new ComboBox();
    ComboBox stopBitsComboBox = new ComboBox();
    ComboBox handshakeComboBox = new ComboBox();
    ComboBox baudComboBox = new ComboBox();
    ComboBox readTimeoutComboBox = new ComboBox();
    ComboBox writeTimeoutComboBox = new ComboBox();

    CheckBox rxEnableBox = new CheckBox();
    CheckBox rxRepeatBox = new CheckBox();
    CheckBox txEnableBox = new CheckBox();
    CheckBox txRepeatBox = new CheckBox();



    public enum TransmissionModes
    {
        TxString,
        TxByte,
        TxAscii,
        Other
    };


    public static readonly Dictionary<TransmissionModes, string> txModeToString = new Dictionary<TransmissionModes, string>()
    {
        [TransmissionModes.TxString] = "String",
        [TransmissionModes.TxByte] =   "Convert to Bytes",
        [TransmissionModes.TxAscii] =  "Ascii Char"

    };


    TransmissionModes currentTxMode = TransmissionModes.TxString;



    /// <summary>
    /// Inherited Constructor, already calls ComponentConfig and more
    /// </summary>
    public CommsApp() : base ()
    {
        
    }



    /// <summary>
    /// Class constructor
    /// </summary>
    /// <param name="panelAsReadOnly"></param>
    /// <param name="showAdvancedData"></param>
    public CommsApp(bool panelAsReadOnly, bool showAdvancedData)
    {
        this.mainDisplay = new System.Windows.Forms.FlowLayoutPanel();
        this.mainDisplay.Size = this.ClientSize;
        this.mainDisplay.AutoSize = true;
        this.ClientSize = new System.Drawing.Size(725, 348);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
        this.Text = "Serial COMs";
        logger = new SQLiteCommLogger();
        serialPort = new PortChat();
        
        this.infoOnly = panelAsReadOnly;
        this.showAdvancedParameters = showAdvancedData;

        if(panelAsReadOnly)
        {
            ComponentConfig();
        }
        else
        {
            ComponentConfig(showAdvancedData);
        }
        
        

    
    }



    /// <summary>
    /// Inherited Class constructor, calls component config from inheritance
    /// </summary>
    /// <param name="inputDisabled"></param>
    public CommsApp(bool inputDisabled) : base()
    {
        if(inputDisabled)
        {
            this.infoOnly = true;
            

            this.Text = "Serial COMs: Logs";
            
            for (var i = 0; i < settingsItems.Count; i++)
            {
                settingsItems[i].Enabled = false;
            }
            
            
            menuStrip.Enabled = false;

            var rxDbData = logger.RxData;
            var txDbData = logger.TxData;

            rxBufferTextBox.Text = (rxDbData.Count+1).ToString();
            rxBufferTextBox.ReadOnly = true;

            SetRxDataBoxes();

            txBufferTextBox.Text = (txDbData.Count+1).ToString();
            txBufferTextBox.ReadOnly = true;
            
            SetTxDataBoxes();

            foreach(var Control in this.Controls)
            {
                var type = Control.GetType();

                if(type == typeof(CheckBox))
                {
                    ((CheckBox)Control).Enabled = false;
                }
                else if(type == typeof(TextBox))
                {
                    ((TextBox)Control).ReadOnly = true;
                    ((TextBox)Control).Cursor = Cursors.Arrow;
                }
                else if(type == typeof(ComboBox))
                {
                    ((ComboBox)Control).Enabled = false;
                }
                else if(type == typeof(Panel))
                {
                    var panel = ((Panel)Control);
                    foreach(var pControl in panel.Controls)
                    {
                        type = pControl.GetType();
                        if(type == typeof(CheckBox))
                        {
                            ((CheckBox)pControl).Enabled = false;
                        }
                        else if(type == typeof(TextBox))
                        {
                            ((TextBox)pControl).ReadOnly = true;
                            ((TextBox)pControl).BackColor = Color.Gray;
                            ((TextBox)pControl).Cursor = Cursors.Arrow;
                        }
                        else if(type == typeof(ComboBox))
                        {
                            ((ComboBox)pControl).Enabled = false;
                        }
                    }
                    
                }
            }

            

            for (var i = 0; i < rxDbData.Count; i++)
            {
                if(i < rxData.Length)
                {
                    rxData[i].Text = rxDbData[i];
                    rxData[i].ReadOnly = true;
                }
                else
                {
                    break;
                }
            }

            for (var i = 0; i < txDbData.Count; i++)
            {
                if(i < txData.Length)
                {
                    txData[i].Text = txDbData[i];
                    txData[i].ReadOnly = true;
                }
                else
                {
                    break;
                }
            }
                

        }


        
    }



    /// <summary>
    /// Sets up gui components
    /// </summary>
    protected override void ComponentConfig()
    {

        settingsItems = new List<ToolStripMenuItem>();
        setupControlsToLabel = new Dictionary<ToolStripMenuItem, Label>();
        
        this.mainDisplay= new System.Windows.Forms.FlowLayoutPanel();
        this.mainDisplay.Size = this.ClientSize;
        this.mainDisplay.AutoSize = true;

        ComReceiveHandler = new SerialDataReceivedEventHandler((_, _) => ReadFromComm());

        possibleIncreaseCharacters = new List<string>();
        possibleIncreaseCharacters.Add("\\n");
        possibleIncreaseCharacters.Add("\\f");
        possibleIncreaseCharacters.Add("\\r");
        possibleIncreaseCharacters.Add(";");


        if(this.infoOnly)
        {
            SerialOptionsPanelConfig();
        }
        else
        {
            SerialOptionsPanelConfig(this.showAdvancedParameters);
        }
        
        serialPort = new PortChat();
        

        this.MinimizeBox = true;
        this.MaximizeBox = false;
        this.ControlBox = true;
        this.CenterToScreen();
        
        this.Text = string.Format("Port: {0}, Baud: {1}, Parity: {2}, Stop Bit: {3}, Data Bits: {4}, Handshake {5} - Closed",port, baud,parity,stopBit,dataBit,handshake);
        

        SetPortSetting("COM", port);
        SetPortSetting("Baud", baud);
        SetPortSetting("Parity", parity);
        SetPortSetting("Stop Bits", stopBit);
        SetPortSetting("Handshake", handshake);
        SetPortSetting("Data Bits", dataBit);
        SetPortSetting("Read Timeout", readTimeout);
        SetPortSetting("Write Timeout", writeTimeout);
    }



    /// <summary>
    /// Sets up gui components
    /// </summary>
    /// <param name="showAdvancedData"></param>
    protected void ComponentConfig(bool showAdvancedData)
    {

        settingsItems = new List<ToolStripMenuItem>();
        setupControlsToLabel = new Dictionary<ToolStripMenuItem, Label>();
        
        this.mainDisplay= new System.Windows.Forms.FlowLayoutPanel();
        this.mainDisplay.Size = this.ClientSize;
        this.mainDisplay.AutoSize = true;

        ComReceiveHandler = new SerialDataReceivedEventHandler((_, _) => ReadFromComm());

        possibleIncreaseCharacters = new List<string>();
        possibleIncreaseCharacters.Add("\\n");
        possibleIncreaseCharacters.Add("\\f");
        possibleIncreaseCharacters.Add("\\r");
        possibleIncreaseCharacters.Add(";");


        this.showAdvancedParameters = showAdvancedData;
        SerialOptionsPanelConfig(showAdvancedData);

        serialPort = new PortChat();
        

        this.MinimizeBox = true;
        this.MaximizeBox = false;
        this.ControlBox = true;
        this.CenterToScreen();
        
        this.Text = string.Format("Port: {0}, Baud: {1}, Parity: {2}, Stop Bit: {3}, Data Bits: {4}, Handshake {5} - Closed",port, baud,parity,stopBit,dataBit,handshake);
        

        SetPortSetting("COM", port);
        SetPortSetting("Baud", baud);
        SetPortSetting("Parity", parity);
        SetPortSetting("Stop Bits", stopBit);
        SetPortSetting("Handshake", handshake);
        SetPortSetting("Data Bits", dataBit);
        SetPortSetting("Read Timeout", readTimeout);
        SetPortSetting("Write Timeout", writeTimeout);
    }


   
    /// <summary>
    /// Sets up the status info section of gui
    /// </summary>
    void SetStatusInfo()
    {
        if (serialPort.IsOpen)
        {
            statusButton.BackColor = Color.SpringGreen;
            statusButton.Text = "Connected";
            this.Text = string.Format("Serial COMs: Port: {0}, Baud: {1}, Parity: {2}, Stop Bit: {3}, Data Bits: {4}, Handshake {5} - {6}", port, baud, parity, stopBit, dataBit, handshake, "Open");
        }
        else
        {
            statusButton.BackColor = Color.DarkRed;
            statusButton.Text = "Closed";
            this.Text = string.Format("Serial COMs: Port: {0}, Baud: {1}, Parity: {2}, Stop Bit: {3}, Data Bits: {4}, Handshake {5} - {6}", port, baud, parity, stopBit, dataBit, handshake, "Closed");
        }
    }

    

    /// <summary>
    /// Creates the full status section
    /// </summary>
    /// <param name="comInfoPanel"></param>
    void CreateStatusSection(ref Panel comInfoPanel)
    {
        var statusLabel = new Label();
        statusLabel.Text = "Status:";
        statusLabel.Margin = new Padding(5,5,0,5);
        statusLabel.AutoSize = true;
        statusLabel.Location = new Point(10,255);
        statusLabel.BorderStyle = BorderStyle.None;
        statusLabel.ForeColor = Color.Black;
        statusLabel.Font = new Font("Arial", 10, FontStyle.Regular);
        comInfoPanel.Controls.Add(statusLabel);

        statusButton = new Button();
        statusButton.Text = "Closed";
        statusButton.Width = comComboBox.Width;
        statusButton.Location = new Point(comInfoPanel.Width - statusButton.Width - 3, statusLabel.Location.Y-2);
        
        statusButton.BackColor = Color.DarkRed;
        statusButton.Click += new EventHandler((_, _) => StatusButtonClick());
        statusButton.Click += new EventHandler((_, _) => SetStatusInfo());
        comInfoPanel.Controls.Add(statusButton);
    }



    /// <summary>
    /// Creates the data sections of the gui
    /// </summary>
    /// <param name="rxBufferSelectionPanel"></param>
    /// <param name="txBufferSelectionPanel"></param>
    void CreateAllDataSections(ref Panel rxBufferSelectionPanel, ref Panel txBufferSelectionPanel)
    {
        rxBufferSize = 0;
        txBufferSize = 0;
        
        rxEnableBox = new CheckBox();
        rxEnableBox.Location = new Point(15,45);
        rxEnableBox.Text = "\u2713-Rx Enabled";
        rxEnableBox.AutoSize = true;
        rxEnableBox.AutoCheck = false;
        rxEnableBox.Appearance = Appearance.Normal;
        rxEnableBox.Click += RxSetup;

        var rxEnableAllMenuItem = new CheckBox();
        rxEnableAllMenuItem.Text = "\u2713-Enable Rx and Tx";
        rxEnableAllMenuItem.AutoSize = true;
        rxEnableAllMenuItem.AutoCheck = false;
        rxEnableAllMenuItem.Appearance = Appearance.Button;
        rxEnableAllMenuItem.BackColor = txBufferSelectionPanel.BackColor;
        rxEnableAllMenuItem.Location = new Point(rxEnableBox.Right - rxEnableAllMenuItem.Width/2,rxEnableBox.Bottom+5);
        rxEnableAllMenuItem.Click += new EventHandler((_,_) => OnEnableAll());

        
        
        if(autoConnect)
        {
            rxEnableBox.Click += new EventHandler((_,_)=>OnPortStatusChange());
            rxEnableAllMenuItem.Click += new EventHandler((_,_) => OnPortStatusChange());

            
        }
        else
        {
            
            rxEnableBox.Click += new EventHandler((_, _) => TxRxClickNoAuto() );
        }

        CreateBufferSizeSettings("Rx Buffer Size",out var rxBufferLabel, out rxBufferTextBox, new Point(10,15), new Padding(5,5,0,5));
        rxBufferTextBox.Text = "0";
        
        rxBufferTextBox.TextChanged += new EventHandler((_,_)=>SetRxDataBoxes());

        rxRepeatBox = new CheckBox();
        rxRepeatBox.Location = new Point(rxEnableBox.Right,45);
        rxRepeatBox.Text = "\u21BB-Rx Overwrite";
        rxRepeatBox.AutoSize = true;
        rxRepeatBox.AutoCheck = true;
        rxRepeatBox.Appearance = Appearance.Normal;
        rxRepeatBox.CheckStateChanged += OnRxOverwriteClicked;

        rxBufferSelectionPanel.Controls.Add(rxEnableBox);
        rxBufferSelectionPanel.Controls.Add(rxBufferLabel);
        rxBufferSelectionPanel.Controls.Add(rxBufferTextBox);
        rxBufferSelectionPanel.Controls.Add(rxRepeatBox);
        rxBufferSelectionPanel.Controls.Add(rxEnableAllMenuItem);

        CreateBufferSizeSettings("Tx Buffer Size",out var txBufferLabel, out txBufferTextBox, new Point(10,15), new Padding(5,5,0,5));
        txBufferTextBox.Text = "0";
        txBufferTextBox.TextChanged += new EventHandler((_,_)=>SetTxDataBoxes());

        var delayMsLabel= new Label();
        delayMsLabel.Text = "Tx Delay(ms): ";
        delayMsLabel.Margin = new Padding(5,5,0,5);
        delayMsLabel.AutoSize = true;
        delayMsLabel.Location = new Point(10,50);
        delayMsLabel.BorderStyle = BorderStyle.None;
        delayMsLabel.ForeColor = Color.Black;
        delayMsLabel.Font = new Font("Arial", 10, FontStyle.Regular);

        delayMsTb = new TextBox();
        delayMsTb.AcceptsReturn = true;
        delayMsTb.AcceptsTab = false;
        delayMsTb.Text = this.currentTxDelay.ToString();
        delayMsTb.MaxLength = 5;
        delayMsTb.Width = 75;
        delayMsTb.Location = new Point(120, 47);
        delayMsTb.KeyPress += IntTextBoxKeyPress;
        delayMsTb.TextChanged += new EventHandler((_,_)=> SetTxDelay());

        if(autoConnect)
        {
            delayMsTb.TextChanged += new EventHandler((_, _) => OnPortStatusChange());
        }
        delayMsTb.LostFocus += new EventHandler((_,_)=> SetTxDelayWithText());
        
        
        
        
        txEnableBox = new CheckBox();
        txEnableBox.Location = new Point(15,delayMsLabel.Bottom+5);
        txEnableBox.Text = "\u2713-Tx Enabled";
        txEnableBox.AutoSize = true;
        txEnableBox.AutoCheck = false;
        txEnableBox.Appearance = Appearance.Normal;
        txEnableBox.Click += TxSetup;

        var txEnableAllMenuItem = new CheckBox();
        txEnableAllMenuItem.Text = "\u2713-Enable Rx and Tx";
        txEnableAllMenuItem.AutoSize = true;
        txEnableAllMenuItem.AutoCheck = false;
        txEnableAllMenuItem.Appearance = Appearance.Button;
        txEnableAllMenuItem.BackColor = rxBufferSelectionPanel.BackColor;
        txEnableAllMenuItem.Location = new Point(txEnableBox.Right - txEnableAllMenuItem.Width/2,txEnableBox.Bottom+5);
        txEnableAllMenuItem.Click += new EventHandler((_,_) => OnEnableAll());
            

        if(autoConnect)
        {
            txEnableBox.Click += new EventHandler((_,_)=>OnPortStatusChange());
            txEnableAllMenuItem.Click += new EventHandler((_,_) => OnPortStatusChange());
        }
        else
        {
            
            txEnableBox.Click += new EventHandler((_, _) => TxRxClickNoAuto() );
        }
        txRepeatBox = new CheckBox();
        txRepeatBox.Location = new Point(txEnableBox.Right,delayMsLabel.Bottom+5);
        txRepeatBox.Text = "\u21BB-Tx Repeat";
        txRepeatBox.AutoSize = true;
        txRepeatBox.AutoCheck = true;
        txRepeatBox.Appearance = Appearance.Normal;
        txRepeatBox.Click += OnTxRepeatClicked;
        if(autoConnect)
        {
            txRepeatBox.Click += new EventHandler((_,_)=>OnPortStatusChange());
        }

        txBufferSelectionPanel.Controls.Add(delayMsLabel);
        txBufferSelectionPanel.Controls.Add(delayMsTb);
        txBufferSelectionPanel.Controls.Add(txEnableBox);
        txBufferSelectionPanel.Controls.Add(txBufferLabel);
        txBufferSelectionPanel.Controls.Add(txBufferTextBox);
        txBufferSelectionPanel.Controls.Add(txRepeatBox);
        txBufferSelectionPanel.Controls.Add(txEnableAllMenuItem);
    }



    /// <summary>
    /// Creates the settings section
    /// </summary>
    /// <param name="comInfoPanel"></param>
    /// <param name="comMenuItem"></param>
    /// <param name="advancedMenuItem"></param>
    /// <param name="showAdvancedData"></param>
    void CreateAllSettingsSections(ref Panel comInfoPanel, ref ToolStripMenuItem comMenuItem, ref ToolStripMenuItem advancedMenuItem, bool showAdvancedData)
    {
        var serialPortSelection = CreatePortSetting("COM",out var clabel,new Point(10,15),new Padding(5,5,0,5),port,out comComboBox, PortChat.AvailablePorts,comInfoPanel);
        var baudSelection = CreatePortSetting("Baud",out var blabel,new Point(10,45),new Padding(5,5,0,5),baud,out baudComboBox,PossibleBauds,comInfoPanel);
        var dataBitsSelection = CreatePortSetting("Data Bits",out var dblabel,new Point(10,75),new Padding(5,5,0,5),dataBit,out dataBitsComboBox,PortChat.DataBitOptions.Keys,comInfoPanel);
        var paritySelection = CreatePortSetting("Parity",out var plabel,new Point(10,105),new Padding(5,5,0,5),parity,out parityComboBox,ParityOptions.Keys,comInfoPanel);
        var stopBitsSelection = CreatePortSetting("Stop Bits",out var sblabel,new Point(10,135),new Padding(5,5,0,5),stopBit,out stopBitsComboBox,PortChat.StopBitOptions.Keys,comInfoPanel);
        var handshakeSelection = CreatePortSetting("Handshake",out var hslabel,new Point(10,165),new Padding(5,5,0,5),handshake,out handshakeComboBox,HandShakeOptions.Keys,comInfoPanel);
        var readTimeoutSelection = CreatePortSetting("Read Timeout",out var rtolabel,new Point(10,195),new Padding(5,5,0,5),readTimeout,out readTimeoutComboBox,TransmissionDelayOptions,comInfoPanel);
        var writeTimeoutSelection = CreatePortSetting("Write Timeout",out var wtolabel,new Point(10,225),new Padding(5,5,0,5),writeTimeout,out writeTimeoutComboBox,TransmissionDelayOptions,comInfoPanel);

        if(showAdvancedData == false)
        {
            readTimeoutComboBox.Hide();
            rtolabel.Hide();
            writeTimeoutComboBox.Hide();
            wtolabel.Hide();
            handshakeComboBox.Hide();
            hslabel.Hide();
        }
        

        CreateStatusSection(ref comInfoPanel);

        for(var i = 0; i < readTimeoutSelection.DropDownItems.Count; i++)
        {
            if(readTimeoutSelection.DropDownItems[i].Text == "-1")
            {
                readTimeoutSelection.DropDownItems[i].Text = "None";
                break;
            }
        }
        for(var i = 0; i < writeTimeoutSelection.DropDownItems.Count; i++)
        {
            if(writeTimeoutSelection.DropDownItems[i].Text == "-1")
            {
                writeTimeoutSelection.DropDownItems[i].Text = "None";
                break;
            }
        }

        for(var i = 0; i < readTimeoutComboBox.Items.Count; i++)
        {
            if((string)readTimeoutComboBox.Items[i] == "-1")
            {
                readTimeoutComboBox.Items[i] = "None";
                break;
            }
        }
        for(var i = 0; i < writeTimeoutComboBox.Items.Count; i++)
        {
            if((string)writeTimeoutComboBox.Items[i] == "-1")
            {
                writeTimeoutComboBox.Items[i] = "None";
                break;
            }
        }

        comMenuItem.DropDownItems.Add(serialPortSelection);
        comMenuItem.DropDownItems.Add(baudSelection);
        comMenuItem.DropDownItems.Add(paritySelection);
        comMenuItem.DropDownItems.Add(dataBitsSelection);
        comMenuItem.DropDownItems.Add(stopBitsSelection);

        advancedMenuItem.DropDownItems.Add(readTimeoutSelection);
        advancedMenuItem.DropDownItems.Add(writeTimeoutSelection);
        advancedMenuItem.DropDownItems.Add(handshakeSelection);
        

        


        settingsItems.Add(serialPortSelection);
        settingsItems.Add(baudSelection);
        settingsItems.Add(paritySelection);
        settingsItems.Add(dataBitsSelection);
        settingsItems.Add(stopBitsSelection);
        settingsItems.Add(handshakeSelection);
        settingsItems.Add(readTimeoutSelection);
        settingsItems.Add(writeTimeoutSelection);

        
    }


    
    /// <summary>
    /// Creates the settings section
    /// </summary>
    /// <param name="comInfoPanel"></param>
    /// <param name="comMenuItem"></param>
    /// <param name="advancedMenuItem"></param>
    void CreateAllSettingsSections(ref Panel comInfoPanel, ref ToolStripMenuItem comMenuItem, ref ToolStripMenuItem advancedMenuItem)
    {
        infoOnly = true;
        
        var serialPortSelection = CreatePortSetting("COM",out var clabel,new Point(10,15),new Padding(5,5,0,5),port,out comComboBox, PortChat.AvailablePorts,comInfoPanel);
        var baudSelection = CreatePortSetting("Baud",out var blabel,new Point(10,45),new Padding(5,5,0,5),baud,out baudComboBox,PossibleBauds,comInfoPanel);
        var dataBitsSelection = CreatePortSetting("Data Bits",out var dblabel,new Point(10,75),new Padding(5,5,0,5),dataBit,out dataBitsComboBox,PortChat.DataBitOptions.Keys,comInfoPanel);
        var paritySelection = CreatePortSetting("Parity",out var plabel,new Point(10,105),new Padding(5,5,0,5),parity,out parityComboBox,ParityOptions.Keys,comInfoPanel);
        var stopBitsSelection = CreatePortSetting("Stop Bits",out var sblabel,new Point(10,135),new Padding(5,5,0,5),stopBit,out stopBitsComboBox,PortChat.StopBitOptions.Keys,comInfoPanel);
        var handshakeSelection = CreatePortSetting("Handshake",out var hslabel,new Point(10,165),new Padding(5,5,0,5),handshake,out handshakeComboBox,HandShakeOptions.Keys,comInfoPanel);
        var readTimeoutSelection = CreatePortSetting("Read Timeout",out var rtolabel,new Point(10,195),new Padding(5,5,0,5),readTimeout,out readTimeoutComboBox,TransmissionDelayOptions,comInfoPanel);
        var writeTimeoutSelection = CreatePortSetting("Write Timeout",out var wtolabel,new Point(10,225),new Padding(5,5,0,5),writeTimeout,out writeTimeoutComboBox,TransmissionDelayOptions,comInfoPanel);

        blabel.Text += baud;
        dblabel.Text += dataBit;
        plabel.Text += parity;
        sblabel.Text += stopBit;
        hslabel.Text += handshake;
        rtolabel.Text += readTimeout;
        wtolabel.Text += writeTimeout;

        comComboBox.Hide();
        baudComboBox.Hide();
        dataBitsComboBox.Hide();
        parityComboBox.Hide();
        stopBitsComboBox.Hide();
        handshakeComboBox.Hide();
        readTimeoutComboBox.Hide();
        writeTimeoutComboBox.Hide();
        
        if(this.showAdvancedParameters == false)
        {
            hslabel.Hide();
            rtolabel.Hide();
            wtolabel.Hide();
        }
        

        comComboBox.Enabled = false;
        baudComboBox.Enabled = false;
        dataBitsComboBox.Enabled = false;
        parityComboBox.Enabled = false;
        stopBitsComboBox.Enabled = false;
        handshakeComboBox.Enabled = false;
        hslabel.Enabled = false;
        readTimeoutComboBox.Enabled = false;
        rtolabel.Enabled = false;
        writeTimeoutComboBox.Enabled = false;
        wtolabel.Enabled = false;
        

        CreateStatusSection(ref comInfoPanel);

        for(var i = 0; i < readTimeoutSelection.DropDownItems.Count; i++)
        {
            if(readTimeoutSelection.DropDownItems[i].Text == "-1")
            {
                readTimeoutSelection.DropDownItems[i].Text = "None";
                break;
            }
        }
        for(var i = 0; i < writeTimeoutSelection.DropDownItems.Count; i++)
        {
            if(writeTimeoutSelection.DropDownItems[i].Text == "-1")
            {
                writeTimeoutSelection.DropDownItems[i].Text = "None";
                break;
            }
        }

        for(var i = 0; i < readTimeoutComboBox.Items.Count; i++)
        {
            if((string)readTimeoutComboBox.Items[i] == "-1")
            {
                readTimeoutComboBox.Items[i] = "None";
                break;
            }
        }
        for(var i = 0; i < writeTimeoutComboBox.Items.Count; i++)
        {
            if((string)writeTimeoutComboBox.Items[i] == "-1")
            {
                writeTimeoutComboBox.Items[i] = "None";
                break;
            }
        }

        comMenuItem.DropDownItems.Add(serialPortSelection);
        comMenuItem.DropDownItems.Add(baudSelection);
        comMenuItem.DropDownItems.Add(paritySelection);
        comMenuItem.DropDownItems.Add(dataBitsSelection);
        comMenuItem.DropDownItems.Add(stopBitsSelection);

        advancedMenuItem.DropDownItems.Add(readTimeoutSelection);
        advancedMenuItem.DropDownItems.Add(writeTimeoutSelection);
        advancedMenuItem.DropDownItems.Add(handshakeSelection);
        

        


        settingsItems.Add(serialPortSelection);
        settingsItems.Add(baudSelection);
        settingsItems.Add(paritySelection);
        settingsItems.Add(dataBitsSelection);
        settingsItems.Add(stopBitsSelection);
        settingsItems.Add(handshakeSelection);
        settingsItems.Add(readTimeoutSelection);
        settingsItems.Add(writeTimeoutSelection);

        
    }

    
    
    /// <summary>
    /// Sets up the options panel
    /// </summary>
    /// <param name="showAdvancedData"></param>
    void SerialOptionsPanelConfig(bool showAdvancedData)
    {
        setupControlsToLabel = new Dictionary<ToolStripMenuItem, Label>();

        menuStrip = new MenuStrip();
        menuStrip.Parent = this;

        this.mainDisplay.BorderStyle = BorderStyle.FixedSingle;

        Panel comInfoPanel = new Panel();
        comInfoPanel.Size = new Size(223,300);
        comInfoPanel.BorderStyle = BorderStyle.FixedSingle;
        comInfoPanel.BackColor = Color.CadetBlue;
        comInfoPanel.Location = new Point(1,menuStrip.Bottom);
        comInfoPanel.Parent = this;
        
        
        Panel rxBufferSelectionPanel = new Panel();
        rxBufferSelectionPanel.Size = new Size(250,150);
        rxBufferSelectionPanel.BackColor = Color.Cornsilk;
        rxBufferSelectionPanel.Location = new Point(comInfoPanel.Right,menuStrip.Bottom);
        rxBufferSelectionPanel.Parent = this;
        rxBufferSelectionPanel.BorderStyle = BorderStyle.FixedSingle;

        rxBoxPanel = new Panel();
        rxBoxPanel.Size = new Size(250,150);
        rxBoxPanel.BackColor = Color.Honeydew;
        rxBoxPanel.Location = new Point(rxBufferSelectionPanel.Right,menuStrip.Bottom);
        rxBoxPanel.BorderStyle = BorderStyle.FixedSingle;
        rxBoxPanel.AutoScroll = true;
        
        rxBoxPanel.VerticalScroll.Enabled = true;
        rxBoxPanel.Parent = this;


        Panel txBufferSelectionPanel = new Panel();
        txBufferSelectionPanel.Size = new Size(250,150);
        txBufferSelectionPanel.BackColor = Color.AliceBlue;
        txBufferSelectionPanel.Location = new Point(comInfoPanel.Right,rxBufferSelectionPanel.Bottom);
        txBufferSelectionPanel.Parent = this;

        txBufferSelectionPanel.BorderStyle = BorderStyle.FixedSingle;

        txBoxPanel = new Panel();
        txBoxPanel.Size = new Size(250,150);
        txBoxPanel.BackColor = Color.Azure;
        txBoxPanel.Location = new Point(txBufferSelectionPanel.Right,rxBoxPanel.Bottom);
        txBoxPanel.AutoScroll = true;
        txBoxPanel.BorderStyle = BorderStyle.FixedSingle;
        txBoxPanel.VerticalScroll.Enabled = true;
        txBoxPanel.Parent = this;

        

        rxData = new TextBox[0];
        txData = new TextBox[0];

        var fileMenuItem = new ToolStripMenuItem("File");
        var closePortItem = new ToolStripMenuItem("Close Port");

        closePortItem.Click += new EventHandler((_,_)=>ClosePortEvent());
        fileMenuItem.DropDownItems.Add(closePortItem);

        var comMenuItem = new ToolStripMenuItem("COM");
        comMenuItem.DropDownOpened += ReloadPorts;

        var txOptionsMenuItem = new ToolStripMenuItem("TX Options");

        foreach(var key in txModeToString)
        {
            ToolStripMenuItem txMode = new ToolStripMenuItem();
            txMode.Text = key.Value;
            txMode.CheckOnClick = false;
            txMode.Click += OnTxModeClick;
            txMode.BackColor = Color.White;
            txOptionsMenuItem.DropDownItems.Add(txMode);
            
        }

        
        

        var loggingMenu = new ToolStripMenuItem("Logging");
        var viewLogsMenuItem = new ToolStripMenuItem("View Logs");
        var exportLogsMenuItem = new ToolStripMenuItem("Save Logs To Text File");
        var enableLoggingMenuItem = new ToolStripMenuItem("Enable Logging");
        var deleteLoggingMenuItem = new ToolStripMenuItem("Delete Logs");

        viewLogsMenuItem.Click += new EventHandler((_,_) => OnViewLogs());
        enableLoggingMenuItem.CheckOnClick = true;
        exportLogsMenuItem.Click += new EventHandler((_,_) => logger.ExportDbData());
        enableLoggingMenuItem.Click += OnLoggingChanged;
        deleteLoggingMenuItem.Click += new EventHandler((_,_) => OnLoggingDelete());

        loggingMenu.DropDownItems.Add(enableLoggingMenuItem);
        loggingMenu.DropDownItems.Add(viewLogsMenuItem);
        loggingMenu.DropDownItems.Add(exportLogsMenuItem);
        loggingMenu.DropDownItems.Add(deleteLoggingMenuItem);

        var settingsMenuItem = new ToolStripMenuItem("Settings");
        var exitMenuItem = new ToolStripMenuItem("Exit", null, (_, _) => OnClose());
        var advancedMenuItem = new ToolStripMenuItem("Advanced");
        
        exitMenuItem.ShortcutKeys = Keys.Control | Keys.X;

        var showAdvancedParametersMenuItem = new ToolStripMenuItem("Show Advanced");
        showAdvancedParametersMenuItem.CheckOnClick = true;
        showAdvancedParametersMenuItem.Checked = this.showAdvancedParameters;
        showAdvancedParametersMenuItem.Click += new EventHandler((_,_) => OnAdvancedParametersClicked());

        var panelInfoMenuItem = new ToolStripMenuItem("Port Panel Read Only");
        panelInfoMenuItem.CheckOnClick = true;
        panelInfoMenuItem.Checked = this.infoOnly;
        panelInfoMenuItem.Click += new EventHandler((_,_) => OnReadOnlyClicked());


        var autoConnectMenuItem = new ToolStripMenuItem("Connect On Parameters Set");
        autoConnectMenuItem.CheckOnClick = true;
        autoConnectMenuItem.Checked = this.autoConnect;
        autoConnectMenuItem.Click += new EventHandler((_,_) => OnAutoConnectClicked());
        

        var enableAllMenuItem = new ToolStripMenuItem("Enable Rx and Tx");
        enableAllMenuItem.Click += new EventHandler((_,_) => OnEnableAll());

        var enableRepeatAllMenuItem = new ToolStripMenuItem("Enable Rx Overwriting and Tx Repeating");
        enableRepeatAllMenuItem.Click += new EventHandler((_,_) => OnEnableRepeats());

        var rxIncreaseIndexSettingsMenuItem = new ToolStripMenuItem("Increase Rx Index On Set Characters Only");
        rxIncreaseIndexSettingsMenuItem.CheckOnClick = true;
        rxIncreaseIndexSettingsMenuItem.Checked = this.rxIncreaseOnChars;
        rxIncreaseIndexSettingsMenuItem.Click += new EventHandler((_,_) => ToggleRxIncreaseOnChar());
        rxIncreaseIndexSettingsMenuItem.Click += new EventHandler((_,_) => rxIncreaseIndexSettingsMenuItem.Checked = this.rxIncreaseOnChars);
        

        var openPortItem = new ToolStripMenuItem("Open Port");
        openPortItem.Click += new EventHandler((_,_)=>OnPortStatusChange());


        var mainSettingsMenuItem = new ToolStripMenuItem("Settings");
        mainSettingsMenuItem.DropDownItems.Add(autoConnectMenuItem);
        mainSettingsMenuItem.DropDownItems.Add(panelInfoMenuItem);
        mainSettingsMenuItem.DropDownItems.Add(showAdvancedParametersMenuItem);
        mainSettingsMenuItem.DropDownItems.Add(rxIncreaseIndexSettingsMenuItem);
        
        var mainAdvancedSettingsMenuItem = new ToolStripMenuItem("Advanced Settings");

        var rxIncreaseIndexCharListMenuItem = new ToolStripMenuItem("Rx Index Possible Increase Values");
        // rxIncreaseIndexCharListMenuItem.DropDownItems.AddRange(possibleIncreaseCharacters);
        rxIncreaseIndexCharListMenuItem.AddDropdownRange(possibleIncreaseCharacters);
        // rxIncreaseIndexCharListMenuItem.DropDownOpened += RxSetupIncreaseChars;


        mainAdvancedSettingsMenuItem.DropDownItems.Add(rxIncreaseIndexCharListMenuItem);
        mainSettingsMenuItem.DropDownItems.Add(mainAdvancedSettingsMenuItem);

        var clearRxItem = new ToolStripMenuItem("Clear Rx");
        clearRxItem.Click += new EventHandler((_,_)=>ClearRxItemsClick());
        
        var clearTxItem = new ToolStripMenuItem("Clear Tx");
        clearTxItem.Click += new EventHandler((_,_)=>ClearTxItemsClick());

        // fileMenuItem.DropDownItems.Add(panelInfoMenuItem);
        // fileMenuItem.DropDownItems.Add(showAdvancedParametersMenuItem);
        // fileMenuItem.DropDownItems.Add(autoConnectMenuItem);
        fileMenuItem.DropDownItems.Add(enableAllMenuItem);
        fileMenuItem.DropDownItems.Add(enableRepeatAllMenuItem);
        fileMenuItem.DropDownItems.Add(clearRxItem);
        fileMenuItem.DropDownItems.Add(clearTxItem);
        // fileMenuItem.DropDownItems.Add(rxIncreaseIndexSettingsMenuItem);
        fileMenuItem.DropDownItems.Add(openPortItem);
        fileMenuItem.DropDownItems.Add(closePortItem);
        fileMenuItem.DropDownItems.Add(mainSettingsMenuItem);
        fileMenuItem.DropDownItems.Add(exitMenuItem);

        CreateAllSettingsSections(ref comInfoPanel, ref comMenuItem, ref advancedMenuItem,showAdvancedData);
        CreateAllDataSections(ref rxBufferSelectionPanel, ref txBufferSelectionPanel);

        comMenuItem.DropDownItems.Add(advancedMenuItem);
        

        menuStrip.Items.Add(fileMenuItem);
        menuStrip.Items.Add(comMenuItem);
        menuStrip.Items.Add(loggingMenu);
        menuStrip.Items.Add(txOptionsMenuItem);

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        timer.Interval = 1000; // Update time every 1 second
        timer.Tick += new EventHandler(TimerTick);
        timer.Tick += new EventHandler((_,_) => SetStatusInfo());
        timer.Start();

        this.clock = CreateTextBox(1, comInfoPanel.Bottom,"",150,150,Color.LightBlue);
        this.clock.TextAlign = HorizontalAlignment.Center;
        this.clock.ReadOnly = true;
        this.clock.Cursor = Cursors.Arrow;
        this.Controls.Add(this.clock);

        

        var spacer = CreateTextBox(this.clock.Right,comInfoPanel.Bottom,"",comInfoPanel.Width + rxBoxPanel.Width + rxBufferSelectionPanel.Width - this.clock.Width,this.clock.Height,Color.LightBlue);
        spacer.ReadOnly = true;
        spacer.Cursor = Cursors.Arrow;
        this.Controls.Add(spacer);
    }

    
    
    /// <summary>
    /// Sets up the options panel
    /// </summary>
    void SerialOptionsPanelConfig()
    {
        setupControlsToLabel = new Dictionary<ToolStripMenuItem, Label>();
        infoOnly = true;

        menuStrip = new MenuStrip();
        menuStrip.Parent = this;

        this.mainDisplay.BorderStyle = BorderStyle.FixedSingle;

        Panel comInfoPanel = new Panel();
        comInfoPanel.Size = new Size(223,300);
        comInfoPanel.BorderStyle = BorderStyle.FixedSingle;
        comInfoPanel.BackColor = Color.CadetBlue;
        comInfoPanel.Location = new Point(1,menuStrip.Bottom);
        comInfoPanel.Parent = this;
        
        
        Panel rxBufferSelectionPanel = new Panel();
        rxBufferSelectionPanel.Size = new Size(250,150);
        rxBufferSelectionPanel.BackColor = Color.Cornsilk;
        rxBufferSelectionPanel.Location = new Point(comInfoPanel.Right,menuStrip.Bottom);
        rxBufferSelectionPanel.Parent = this;
        rxBufferSelectionPanel.BorderStyle = BorderStyle.FixedSingle;

        rxBoxPanel = new Panel();
        rxBoxPanel.Size = new Size(250,150);
        rxBoxPanel.BackColor = Color.Honeydew;
        rxBoxPanel.Location = new Point(rxBufferSelectionPanel.Right,menuStrip.Bottom);
        rxBoxPanel.BorderStyle = BorderStyle.FixedSingle;
        rxBoxPanel.AutoScroll = true;
        
        rxBoxPanel.VerticalScroll.Enabled = true;
        rxBoxPanel.Parent = this;


        Panel txBufferSelectionPanel = new Panel();
        txBufferSelectionPanel.Size = new Size(250,150);
        txBufferSelectionPanel.BackColor = Color.AliceBlue;
        txBufferSelectionPanel.Location = new Point(comInfoPanel.Right,rxBufferSelectionPanel.Bottom);
        txBufferSelectionPanel.Parent = this;

        txBufferSelectionPanel.BorderStyle = BorderStyle.FixedSingle;

        txBoxPanel = new Panel();
        txBoxPanel.Size = new Size(250,150);
        txBoxPanel.BackColor = Color.Azure;
        txBoxPanel.Location = new Point(txBufferSelectionPanel.Right,rxBoxPanel.Bottom);
        txBoxPanel.AutoScroll = true;
        txBoxPanel.BorderStyle = BorderStyle.FixedSingle;
        txBoxPanel.VerticalScroll.Enabled = true;
        txBoxPanel.Parent = this;

        

        rxData = new TextBox[0];
        txData = new TextBox[0];

        var fileMenuItem = new ToolStripMenuItem("File");
        var closePortItem = new ToolStripMenuItem("Close Port");
        closePortItem.Click += new EventHandler((_,_)=>ClosePortEvent());

        
        

        var comMenuItem = new ToolStripMenuItem("COM");
        comMenuItem.DropDownOpened += ReloadPorts;

        var txOptionsMenuItem = new ToolStripMenuItem("TX Options");

        foreach(var key in txModeToString)
        {
            ToolStripMenuItem txMode = new ToolStripMenuItem();
            txMode.Text = key.Value;
            txMode.CheckOnClick = false;
            txMode.Click += OnTxModeClick;
            txMode.BackColor = Color.White;
            txOptionsMenuItem.DropDownItems.Add(txMode);
        }

        

        var loggingMenu = new ToolStripMenuItem("Logging");
        var viewLogsMenuItem = new ToolStripMenuItem("View Logs");
        var exportLogsMenuItem = new ToolStripMenuItem("Save Logs To Text File");
        var enableLoggingMenuItem = new ToolStripMenuItem("Enable Logging");
        var deleteLoggingMenuItem = new ToolStripMenuItem("Delete Logs");

        viewLogsMenuItem.Click += new EventHandler((_,_) => OnViewLogs());
        enableLoggingMenuItem.CheckOnClick = true;
        exportLogsMenuItem.Click += new EventHandler((_,_) => logger.ExportDbData());
        enableLoggingMenuItem.Click += OnLoggingChanged;
        deleteLoggingMenuItem.Click += new EventHandler((_,_) => OnLoggingDelete());

        loggingMenu.DropDownItems.Add(enableLoggingMenuItem);
        loggingMenu.DropDownItems.Add(viewLogsMenuItem);
        loggingMenu.DropDownItems.Add(exportLogsMenuItem);
        loggingMenu.DropDownItems.Add(deleteLoggingMenuItem);

        var settingsMenuItem = new ToolStripMenuItem("Settings");
        var exitMenuItem = new ToolStripMenuItem("&Exit", null, (_, _) => OnClose());
        var advancedMenuItem = new ToolStripMenuItem("Advanced");
        
        exitMenuItem.ShortcutKeys = Keys.Control | Keys.X;

        var showAdvancedParametersMenuItem = new ToolStripMenuItem("Show Advanced");
        showAdvancedParametersMenuItem.CheckOnClick = true;
        showAdvancedParametersMenuItem.Checked = this.showAdvancedParameters;
        showAdvancedParametersMenuItem.Click += new EventHandler((_,_) => OnAdvancedParametersClicked());

        var panelInfoMenuItem = new ToolStripMenuItem("Port Panel Read Only");
        panelInfoMenuItem.CheckOnClick = true;
        panelInfoMenuItem.Checked = this.infoOnly;
        panelInfoMenuItem.Click += new EventHandler((_,_) => OnReadOnlyClicked());


        var autoConnectMenuItem = new ToolStripMenuItem("Autoconnect On Parameters Set");
        autoConnectMenuItem.CheckOnClick = true;
        autoConnectMenuItem.Checked = this.autoConnect;
        autoConnectMenuItem.Click += new EventHandler((_,_) => OnAutoConnectClicked());

        var enableAllMenuItem = new ToolStripMenuItem("Enable Rx and Tx");
        enableAllMenuItem.Click += new EventHandler((_,_) => OnEnableAll());

        var enableRepeatAllMenuItem = new ToolStripMenuItem("Enable Rx Overwriting and Tx Repeating");
        enableRepeatAllMenuItem.Click += new EventHandler((_,_) => OnEnableRepeats());
        

        var rxIncreaseIndexSettingsMenuItem = new ToolStripMenuItem("Increase Rx Index On Set Characters Only");
        rxIncreaseIndexSettingsMenuItem.CheckOnClick = true;
        rxIncreaseIndexSettingsMenuItem.Checked = this.rxIncreaseOnChars;
        rxIncreaseIndexSettingsMenuItem.Click += new EventHandler((_,_) => ToggleRxIncreaseOnChar());
        rxIncreaseIndexSettingsMenuItem.Click += new EventHandler((_,_) => rxIncreaseIndexSettingsMenuItem.Checked = this.rxIncreaseOnChars);
        
        var openPortItem = new ToolStripMenuItem("Open Port");
        openPortItem.Click += new EventHandler((_,_)=>OnPortStatusChange());

        var mainSettingsMenuItem = new ToolStripMenuItem("Settings");
        mainSettingsMenuItem.DropDownItems.Add(autoConnectMenuItem);
        mainSettingsMenuItem.DropDownItems.Add(panelInfoMenuItem);
        mainSettingsMenuItem.DropDownItems.Add(showAdvancedParametersMenuItem);
        mainSettingsMenuItem.DropDownItems.Add(rxIncreaseIndexSettingsMenuItem);

        var mainAdvancedSettingsMenuItem = new ToolStripMenuItem("Advanced Settings");

        var rxIncreaseIndexCharListMenuItem = new ToolStripMenuItem("Rx Index Possible Increase Values");
        // rxIncreaseIndexCharListMenuItem.DropDownItems.AddRange(possibleIncreaseCharacters);
        rxIncreaseIndexCharListMenuItem.AddDropdownRange(possibleIncreaseCharacters);
        // rxIncreaseIndexCharListMenuItem.DropDownOpened += RxSetupIncreaseChars;


        mainAdvancedSettingsMenuItem.DropDownItems.Add(rxIncreaseIndexCharListMenuItem);
        mainSettingsMenuItem.DropDownItems.Add(mainAdvancedSettingsMenuItem);
        
        var clearRxItem = new ToolStripMenuItem("Clear Rx");
        clearRxItem.Click += new EventHandler((_,_)=>ClearRxItemsClick());
        
        var clearTxItem = new ToolStripMenuItem("Clear Tx");
        clearTxItem.Click += new EventHandler((_,_)=>ClearTxItemsClick());

        // fileMenuItem.DropDownItems.Add(panelInfoMenuItem);
        // fileMenuItem.DropDownItems.Add(showAdvancedParametersMenuItem);
        // fileMenuItem.DropDownItems.Add(autoConnectMenuItem);
        fileMenuItem.DropDownItems.Add(enableAllMenuItem);
        fileMenuItem.DropDownItems.Add(enableRepeatAllMenuItem);
        fileMenuItem.DropDownItems.Add(clearRxItem);
        fileMenuItem.DropDownItems.Add(clearTxItem);
        // fileMenuItem.DropDownItems.Add(rxIncreaseIndexSettingsMenuItem);
        fileMenuItem.DropDownItems.Add(openPortItem);
        fileMenuItem.DropDownItems.Add(closePortItem);
        fileMenuItem.DropDownItems.Add(mainSettingsMenuItem);
        fileMenuItem.DropDownItems.Add(exitMenuItem);

        CreateAllSettingsSections(ref comInfoPanel, ref comMenuItem, ref advancedMenuItem);
        CreateAllDataSections(ref rxBufferSelectionPanel, ref txBufferSelectionPanel);

        comMenuItem.DropDownItems.Add(advancedMenuItem);
        

        menuStrip.Items.Add(fileMenuItem);
        menuStrip.Items.Add(comMenuItem);
        menuStrip.Items.Add(loggingMenu);
        menuStrip.Items.Add(txOptionsMenuItem);

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        timer.Interval = 1000; // Update time every 1 second
        timer.Tick += new EventHandler(TimerTick);
        timer.Tick += new EventHandler((_,_) => SetStatusInfo());
        timer.Start();

        this.clock = CreateTextBox(1, comInfoPanel.Bottom,"",150,150,Color.LightBlue);
        this.clock.TextAlign = HorizontalAlignment.Center;
        this.clock.ReadOnly = true;
        this.clock.Cursor = Cursors.Arrow;
        this.Controls.Add(this.clock);

        var spacer = CreateTextBox(this.clock.Right,comInfoPanel.Bottom,"",comInfoPanel.Width + rxBoxPanel.Width + rxBufferSelectionPanel.Width - this.clock.Width,this.clock.Height,Color.LightBlue);
        spacer.ReadOnly = true;
        spacer.Cursor = Cursors.Arrow;
        this.Controls.Add(spacer);

    }


    


    ToolStripMenuItem CreatePortSetting(string name, Point location, Padding margin, string connectedValue, ICollection<string> possibleValues, Panel panel)
    {
        var label= new Label();
        label.Text = name + ": " + connectedValue;
        label.Margin = margin;
        label.Width = 200;
        label.Location = location;
        label.BorderStyle = BorderStyle.None;
        label.ForeColor = Color.Black;
        label.Font = new Font("Arial", 10, FontStyle.Regular);

        var selection = new ToolStripMenuItem(name);

        if(setupControlsToLabel.TryAdd(selection,label))
        {
            panel.Controls.Add(label);
        }

        foreach(var val in possibleValues)
        {
            ToolStripMenuItem ddmi = new ToolStripMenuItem(val);
            if (autoConnect)
            {
                ddmi.Click += new EventHandler((_, _) => OnPortStatusChange());
            }
            selection.DropDownItems.Add(ddmi);
        }

        
        selection.DropDownItemClicked += OnMenuItemSetupSelected;
        return selection;
    }



    ToolStripMenuItem CreatePortSetting(string name, out Label label, Point location, Padding margin, string connectedValue, ICollection<string> possibleValues, Panel panel)
    {
        label= new Label();
        label.Text = name + ": ";// + connectedValue;
        label.Margin = margin;
        // label.Width = 200;
        label.AutoSize = true;
        label.Location = location;
        label.BorderStyle = BorderStyle.None;
        label.ForeColor = Color.Black;
        label.Font = new Font("Arial", 10, FontStyle.Regular);

        var selection = new ToolStripMenuItem(name);

        if(setupControlsToLabel.TryAdd(selection,label))
        {
            panel.Controls.Add(label);
        }

        foreach(var val in possibleValues)
        {
            ToolStripMenuItem ddmi = new ToolStripMenuItem(val);
            if (autoConnect)
            {
                ddmi.Click += new EventHandler((_, _) => OnPortStatusChange());
            }
            selection.DropDownItems.Add(ddmi);
        }

        
        selection.DropDownItemClicked += OnMenuItemSetupSelected;
        return selection;
    }


    ToolStripMenuItem CreatePortSetting(string name, out Label label, Point location, Padding margin, string connectedValue, out ComboBox comboBox, ICollection<string> possibleValues, Panel panel)
    {
        label= new Label();
        label.Text = name + ": ";
        label.Margin = margin;
        label.AutoSize = true;
        label.Location = location;
        label.BorderStyle = BorderStyle.None;
        label.ForeColor = Color.Black;
        label.Font = new Font("Arial", 10, FontStyle.Regular);

        var selection = new ToolStripMenuItem(name);

        if(setupControlsToLabel.TryAdd(selection,label))
        {
            panel.Controls.Add(label);
        }
        
        var newComboBox = new ComboBox();
        newComboBox.Text = connectedValue;
        newComboBox.KeyPress += VoidKeypressHandler;
        newComboBox.Width = (newComboBox.Width - 13);
        newComboBox.Location = new Point(panel.Width - newComboBox.Width - 3, label.Location.Y-2);
        newComboBox.Cursor = Cursors.Arrow;

        foreach(var val in possibleValues)
        {
            ToolStripMenuItem ddmi = new ToolStripMenuItem(val);
            if (autoConnect)
            {
                ddmi.Click += new EventHandler((_, _) => OnPortStatusChange());
            }
            selection.DropDownItems.Add(ddmi);
            newComboBox.Items.Add(val);
        }

        
        selection.DropDownItemClicked += OnMenuItemSetupSelected;

        newComboBox.SelectedValueChanged += new EventHandler((_, _) => SetPortSetting(name, newComboBox.Text) );
        if (autoConnect)
        {
            newComboBox.SelectedValueChanged += new EventHandler((_, _) => OnPortStatusChange());
        }
        comboBox = newComboBox;

        panel.Controls.Add(comboBox);

        return selection;
    }



    void CreateBufferSizeSettings(string name, out Label label, out TextBox tb, Point location, Padding margin)
    {
        label= new Label();
        label.Text = name + ": ";
        label.Margin = margin;
        label.AutoSize = true;
        label.Location = location;
        label.BorderStyle = BorderStyle.None;
        label.ForeColor = Color.Black;
        label.Font = new Font("Arial", 10, FontStyle.Regular);

        tb = new TextBox();
        tb.AcceptsReturn = false;
        tb.AcceptsTab = false;
        tb.MaxLength = 3;
        tb.Width = 75;
        tb.BackColor = TextBox.DefaultBackColor;
        tb.Location = new Point(location.X+110, location.Y-2);
        tb.KeyPress += IntTextBoxKeyPress;

        tb.Text = "0";
        if(autoConnect)
        {
            tb.TextChanged += new EventHandler((_,_)=>OnPortStatusChange());
        }
        
    }


    
    protected override void WriteToComm()
    {
        if(serialPort.IsOpen == true)
        {
            if(txData.Length > 0)
            {
                string txtOut = "";

                if(currentTxIndex < txData.Length)
                {
                    
                    if (currentTxMode == TransmissionModes.TxByte)
                    {
                        foreach (var c in txData[currentTxIndex].Text)
                        {
                            txtOut += Convert.ToByte(c).ToString();
                        }

                    }
                    else if(currentTxMode == TransmissionModes.TxAscii)
                    {
                        if(txData[currentTxIndex].Text.Length > 1)
                        {
                            txtOut += txData[currentTxIndex].Text[0];
                            txData[currentTxIndex].Text = txtOut;
                        }
                        
                        else
                        {
                            txtOut = txData[currentTxIndex].Text;
                        }
                    }
                    else
                    {
                        txtOut = txData[currentTxIndex].Text;
                    }
                    
                    
                    txData[currentTxIndex].BackColor = Color.SeaGreen;

                    if(currentTxIndex == 0)
                    {
                        txData.Last().BackColor = TextBox.DefaultBackColor;
                    }
                    else
                    {
                        txData[currentTxIndex-1].BackColor = TextBox.DefaultBackColor;
                    }

                    serialPort.WriteToPort(txtOut);

                    if(loggingEnabled)
                    {
                        logger.WriteToTxData(txtOut);
                    }

                    currentTxIndex++;
                }
                else if(reapeatTxData)
                {
                    if (currentTxMode == TransmissionModes.TxByte)
                    {
                        foreach (var c in txData[0].Text)
                        {
                            txtOut += Convert.ToByte(c).ToString();
                        }

                    }
                    else if(currentTxMode == TransmissionModes.TxAscii)
                    {
                        if(txData[0].Text.Length > 1)
                        {
                            txtOut += txData[0].Text[0];
                            txData[0].Text = txtOut;
                        }
                        else
                        {
                            txtOut = txData[0].Text;
                        }
                    }
                    else
                    {
                        txtOut = txData[0].Text;
                    }
                    


                    txData[0].BackColor = Color.SeaGreen;

                    txData.Last().BackColor = TextBox.DefaultBackColor;

                    serialPort.WriteToPort(txtOut);

                    if(loggingEnabled)
                    {
                        logger.WriteToTxData(txtOut);
                    }
                    currentTxIndex = 1;
                }
                
            }
        }
    }


    

    
    protected override void ReadFromComm()
    {
        if(serialPort.IsOpen == true)
        {
            if(serialPort.AvailableBytes > 0 && rxData.Length > 0)
            {
                var currentData = serialPort.ReadExisting(100);

                if (rxIncreaseOnChars == true)
                {
                    if (currentRxIndex >= rxBufferSize)
                    {
                        if (overwriteRxData)
                        {
                            currentRxIndex = 0;
                        }
                        else
                        {
                            rxEnabled = false;
                            serialPort.TryClosePort();

                            if (txEnabled)
                            {
                                serialPort.TryOpenPort(port, baud, parity, stopBit, handshake, readTimeout, writeTimeout, dataBit);
                            }
                            else
                            {
                                ClosePortEvent();
                            }

                            return;
                        }
                    }
                    string strOut = "";
                    
                    foreach (var c in currentData)
                    {
                        string checkVal = c.ToString();
                        if(c < ' ')
                        // if(char.IsControl(c))
                        {
                            checkVal = Regex.Unescape(c.ToString());
                        }
                        if (possibleIncreaseCharacters.Contains(checkVal) == true)
                        {

                            rxData[currentRxIndex].Invoke((MethodInvoker)(() => rxData[currentRxIndex].Text = strOut));

                            if (loggingEnabled)
                            {
                                logger.WriteToRxData(rxData[currentRxIndex].Text);
                            }

                            currentRxIndex++;
                            strOut = "";
                            
                            if (currentRxIndex >= rxBufferSize)
                            {
                                if (overwriteRxData)
                                {
                                    currentRxIndex = 0;
                                }
                                else
                                {
                                    rxEnabled = false;
                                    serialPort.TryClosePort();

                                    if (txEnabled)
                                    {
                                        serialPort.TryOpenPort(port, baud, parity, stopBit, handshake, readTimeout, writeTimeout, dataBit);
                                    }
                                    else
                                    {
                                        ClosePortEvent();
                                    }

                                    return;
                                }
                            }
                        }
                        else
                        {
                            strOut += c.ToString();
                        }

                    }

                    // currentRxIndex++;
                    
                    if(strOut != "")
                    {
                        if (currentRxIndex >= rxBufferSize)
                        {
                            rxBufferSize += 1;
                            rxBufferTextBox.Invoke((MethodInvoker)(() => rxBufferTextBox.Text = rxBufferSize.ToString())); 
                            rxData.Last().Invoke((MethodInvoker)(SetRxDataBoxes));

                            if (overwriteRxData)
                            {
                                currentRxIndex = 0;
                            }
                            else
                            {
                                rxEnabled = false;
                                serialPort.TryClosePort();

                                if (txEnabled)
                                {
                                    serialPort.TryOpenPort(port, baud, parity, stopBit, handshake, readTimeout, writeTimeout, dataBit);
                                }
                                else
                                {
                                    ClosePortEvent();
                                }
                            }
                        }


                        
                        rxData[currentRxIndex].Invoke((MethodInvoker)(() => rxData[currentRxIndex].Text += strOut));

                        if (loggingEnabled)
                        {
                            logger.WriteToRxData(rxData[currentRxIndex].Text);
                        }
                        
                    }

                }
                else
                {

                    if(string.IsNullOrEmpty(currentData))
                    {
                        return;
                    }


                    if (currentRxIndex >= rxBufferSize)
                    {

                        if (overwriteRxData)
                        {


                            rxData[0].Invoke((MethodInvoker)(() => rxData[0].Text = currentData));
                            if (loggingEnabled)
                            {
                                logger.WriteToRxData(rxData[0].Text);
                            }

                            currentRxIndex = 1;
                        }
                        else
                        {
                            rxEnabled = false;
                            serialPort.TryClosePort();

                            if (txEnabled)
                            {
                                serialPort.TryOpenPort(port, baud, parity, stopBit, handshake, readTimeout, writeTimeout, dataBit);
                            }


                        }
                    }
                    else
                    {
                        rxData[currentRxIndex].Invoke((MethodInvoker)(() => rxData[currentRxIndex].Text = currentData));
                        if (loggingEnabled)
                        {
                            logger.WriteToRxData(rxData[currentRxIndex].Text);
                        }
                        currentRxIndex += 1;
                    }
                }


            }
        }
    }







    



}