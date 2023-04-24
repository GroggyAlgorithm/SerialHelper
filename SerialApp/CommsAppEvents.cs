using System.Text.RegularExpressions;
using System.IO;
using System.Collections;
using System.Collections.Generic;


public static class ToolStripExtensions 
{
    public static void Dispose(this ToolStripItemCollection tsmic)
    {
        foreach(ToolStripDropDownItem item in tsmic)
        {
            item.Dispose();
        }
    }

    public static void HideItems(this ToolStripMenuItem item)
    {

        item.DropDown.Items.Hide();

        for (var i = 0; i < item.DropDownItems.Count; i++)
        {
            item.DropDownItems[i].Visible = false;
        }

        
    }

    public static void Hide(this ToolStripItemCollection  item)
    {
        for (var i = 0; i < item.Count; i++)
        {
            item[i].Visible = false;
        }
    }


    public static void ShowItems(this ToolStripMenuItem item)
    {
        item.DropDown.Items.Show();

        for (var i = 0; i < item.DropDownItems.Count; i++)
        {
            item.DropDownItems[i].Visible = true;
        }
    }

    public static void Show(this ToolStripItemCollection  item)
    {
        for (var i = 0; i < item.Count; i++)
        {
            item[i].Visible = true;
        }
    }


    
    public static void AddRange(this ToolStripItemCollection tsmic, ICollection collection)
    {
        foreach(var item in collection)
        {   
            if(item.GetType() == typeof(char))
            {
                var asChar = (char)item;

                if (asChar < ' ')
                // if (char.IsControl(asChar))
                {
                    tsmic.Add(Regex.Unescape(asChar.ToString()));
                    // tsmic.Add((Regex.Escape(asChar.ToString())));

                }
                else
                {
                    tsmic.Add(item.ToString());
                }


            }
            else
            {
                tsmic.Add(item.ToString());
            }
        }
    }


    public static void AddDropdownRange(this ToolStripMenuItem tsmic, ICollection collection)
    {
        foreach(var item in collection)
        {
            if(item.GetType() == typeof(char))
            {
                var asChar = (char)item;

                // if(char.IsControl(asChar))
                if (asChar < ' ')
                {
                    tsmic.DropDownItems.Add(Regex.Unescape(asChar.ToString()));
                    // tsmic.DropDownItems.Add((Regex.Escape(asChar.ToString())));
                    // tsmic.DropDownItems.Add((Regex.Escape(asChar.ToString())));
                    // switch(asChar)
                    // {
                    //     case '\n':
                    //         tsmic.DropDownItems.Add("\\n");
                    //     break;
                        
                    //     case '\f':
                    //         tsmic.DropDownItems.Add("\\f");
                    //     break;
                        
                    //     case '\r':
                    //         tsmic.DropDownItems.Add("\\r");
                    //     break;

                    //     default:
                    //         tsmic.DropDownItems.Add(((byte)(asChar)).ToString());
                    //     break;
                    // };
                    // tsmic.DropDownItems.Add(char.GetNumericValue(asChar).ToString());
                }
                else
                {
                    tsmic.DropDownItems.Add(item.ToString());
                }
            }
            else
            {
                tsmic.DropDownItems.Add(item.ToString());
            }
        }
    }
}



/// <summary>
/// Partial class - File for class events and event handlers
/// </summary>
public partial class CommsApp : SerialLoggingAppForm
{
    ToolStripMenuItem previousTxModeItem = new ToolStripMenuItem();


    void ClearRxItemsClick()
    {
        for(var i = 0; i < rxData.Count(); i++)
        {
            var tb = rxData[i];
            tb.Text = "";
        }
    }


    void ClearTxItemsClick()
    {
        for(var i = 0; i < txData.Count(); i++)
        {
            var tb = txData[i];
            tb.Text = "";
        }
    }


    // void PossibleCharTextChange(ToolStripMenuItem tsmi, TextBox tb)
    // {
    //     var tbText = tb.Text;
    //     if(poss)
    //     foreach(var item in tsmi.DropDownItems)
    //     {
            
    //     }

    // }

    #warning unfinished function here
    void RxSetupIncreaseChars(object? obj, EventArgs e)
    {
        if(obj.GetType() == typeof(ToolStripMenuItem))
        {
            var asTsmi = (ToolStripMenuItem)(obj);

            asTsmi.DropDownItems.Clear();

            foreach(var item in possibleIncreaseCharacters)
            {
                // TextBox tb = new TextBox();
                // tb.AcceptsReturn = false;
                // tb.AcceptsTab = false;
                // tb.Text = (item.ToString());
                // tb.ReadOnly = true;
                
                asTsmi.DropDownItems.Add(item.ToString());
                // tb.MaxLength = 2;
                // tb.TextChanged += new EventHandler((_,_) => PossibleCharTextChange(asTsmi, tb) );
            }

        }
    }


    void OnEnableAll()
    {
        
        if(rxBufferSize <= 0)
        {
            rxBufferSize = 3;
            rxBufferTextBox.Text = "3";
            SetRxDataBoxes();
        }

        if(txBufferSize <= 0)
        {
            txBufferSize = 3;
            txBufferTextBox.Text = "3";
            SetTxDataBoxes();
        }

        rxEnabled = true;
        rxEnableBox.Checked = true;

        txEnabled = true;
        txEnableBox.Checked = true;
        
    }


    void OnEnableRepeats()
    {
        overwriteRxData = true;
        reapeatTxData = true;
        txRepeatBox.Checked = true;
        rxRepeatBox.Checked = true;
    }



    void ToggleRxIncreaseOnChar()
    {
        this.rxIncreaseOnChars = !this.rxIncreaseOnChars;
    }



    void TxRxClickNoAuto()
    {
        
        if(serialPort.IsOpen)
        {
            OnPortStatusChange();
        }
    }


    void ClosePortEvent()
    {
        rxEnabled = false;
        txEnabled = false;
        currentRxIndex = 0;
        currentTxIndex = 0;

        rxEnableBox.Checked = false;
        txEnableBox.Checked = false;
        OnPortStatusChange();
        
    }


    void ClosePortEvent(bool disableRx, bool disableTx)
    {
        if(disableRx)
        {
            rxEnabled = false;
            currentRxIndex = 0;
            rxEnableBox.Checked = false;

        }

        if(disableTx)
        {
            txEnabled = false;
            currentTxIndex = 0;
            txEnableBox.Checked = false;
        }
        OnPortStatusChange();
        
    }


    void VoidKeypressHandler(object? o, KeyPressEventArgs e)
    {
        e.Handled = true;
    }


    void StatusButtonClick()
    {
        if(serialPort.IsOpen)
        {
            ClosePortEvent();
        }
        else
        {
            OnPortStatusChange();
        }

        
    }


    void OnAutoConnectClicked()
    {
        this.autoConnect = !this.autoConnect;
        
        this.mainDisplay.Dispose();
        this.Controls.Clear();
        this.mainDisplay = new System.Windows.Forms.FlowLayoutPanel();
        this.mainDisplay.Size = this.ClientSize;
        this.mainDisplay.AutoSize = true;
        
        SerialOptionsPanelConfig();
        // if(this.infoOnly)
        // {
        //     SerialOptionsPanelConfig();
        // }
        // else
        // {
        //     SerialOptionsPanelConfig(this.showAdvancedParameters);
        // }




    }

    void OnReadOnlyClicked()
    {
        this.infoOnly = !this.infoOnly;

        this.mainDisplay.Dispose();
        this.Controls.Clear();
        this.mainDisplay = new System.Windows.Forms.FlowLayoutPanel();
        this.mainDisplay.Size = this.ClientSize;
        this.mainDisplay.AutoSize = true;
        
        SerialOptionsPanelConfig();
        // if(this.infoOnly)
        // {
        //     SerialOptionsPanelConfig();
        // }
        // else
        // {
        //     SerialOptionsPanelConfig(this.showAdvancedParameters);
        // }




    }

    void OnAdvancedParametersClicked()
    {
        showAdvancedParameters = !showAdvancedParameters;
        this.mainDisplay.Dispose();
        this.Controls.Clear();
        this.mainDisplay = new System.Windows.Forms.FlowLayoutPanel();
        this.mainDisplay.Size = this.ClientSize;
        this.mainDisplay.AutoSize = true;
        
        SerialOptionsPanelConfig();
    }





    void OnPortStatusChange()
    {
        if(txTimer.Enabled)
        {
            txTimer.Stop();
        }
        txTimer = new System.Windows.Forms.Timer();
        currentTxIndex = 0;

        

        if (rxEnabled || txEnabled)
        {

            if (rxEnabled == false)
            {
                try
                {
                    serialPort._serialPort.DataReceived -= ComReceiveHandler;
                }
                catch
                {

                }
            }

            if (serialPort.TryOpenPort(port, baud, parity, stopBit, handshake, readTimeout, writeTimeout, dataBit))
            {

            }
           

        }
        else
        {
            try
            {
                serialPort._serialPort.DataReceived -= ComReceiveHandler;
            }
            catch
            {

            }
            serialPort.TryClosePort();

            
            

        }

        SetStatusInfo();


        //Leaving this down here to avoid events triggering while setting status texts
        if(serialPort.IsOpen)
        {
            statusButton.BackColor = Color.SpringGreen;
            statusButton.Text = "Connected";

            if(rxEnabled)
            {
                serialPort._serialPort.DataReceived += ComReceiveHandler;
            }

            if(txEnabled)
            {
                SetTxDelayWithText();
                txTimer.Interval = currentTxDelay;
                txTimer.Tick += new EventHandler((_,_) => WriteToComm());
                txTimer.Start();
            }
            
        }
        else
        {
            for (var i = 0; i < txData.Length; i++)
            {
                txData[i].BackColor = TextBox.DefaultBackColor;
            }

            statusButton.BackColor = Color.DarkRed;
            statusButton.Text = "Closed";
        }

        
    }

    void SetRxDataBoxes()
    {
        if(int.TryParse(rxBufferTextBox.Text,out var buffSize))
        {
            rxBufferSize = buffSize;
        }
        else
        {
            rxBufferSize = 0;
        }
        List<string> previousData = new List<string>();
        for(var i = 0; i < rxData.Count(); i++)
        {
            var tb = rxData[i];
            previousData.Add(tb.Text);
            tb.Dispose();
        }
        rxData = new TextBox[(rxBufferSize >= 0) ? rxBufferSize : 0];

        
        for(int i = 0; i < rxData.Count(); i++)
        {
            var tb = new TextBox();
            if(i > 0)
            {
                
                tb.Location = new Point(10,rxData[i-1].Bottom+1);
            }
            else
            {
                tb.Location = new Point(10,10);
            }
            
            tb.Width = rxBoxPanel.Width - 35;
            tb.ReadOnly = true;
            if(i < previousData.Count)
            {
                tb.Text = previousData[i];
            }
            rxBoxPanel.Controls.Add(tb);
            rxData[i] = tb;
        }
        
    }



    void SetTxDataBoxes()
    {
        if(int.TryParse(txBufferTextBox.Text,out var buffSize))
        {
            txBufferSize = buffSize;
        }
        else
        {
            txBufferSize = 0;
        }

        List<string> previousData = new List<string>();
        for(var i = 0; i < txData.Count(); i++)
        {
            var tb = txData[i];
            previousData.Add(tb.Text);
            tb.Dispose();
        }
        txData = new TextBox[(txBufferSize >= 0) ? txBufferSize : 0];

        for(int i = 0; i < txData.Count(); i++)
        {
            var tb = new TextBox();
            
            if(i > 0)
            {
                
                tb.Location = new Point(10,txData[i-1].Bottom+1);
            }
            else
            {
                tb.Location = new Point(10,10);
            }
            tb.Width = txBoxPanel.Width - 35;
            if(i < previousData.Count)
            {
                tb.Text = previousData[i];
            }
            txBoxPanel.Controls.Add(tb);
            txData[i] = tb;
        }
    }
    


    


    void RxSetup(object? sender, System.EventArgs e)
    {
        if(sender == null)
        {
            return;
        }
        else if(sender.GetType() == typeof(CheckBox))
        {
            var cb = (CheckBox)sender;

            currentRxIndex = 0;

           

            if(cb.Checked || rxBufferSize <= 0)
            {
                cb.Checked = false;
            }
            else
            {
                cb.Checked = true;
            }

            rxEnabled = cb.Checked;

            
            

            
        }

        
    }



    void TxSetup(object? sender, System.EventArgs e)
    {
        if(sender == null)
        {
            return;
        }
        else if(sender.GetType() == typeof(CheckBox))
        {
            var cb = (CheckBox)sender;
            
            currentTxIndex = 0;

            

            if(cb.Checked || txBufferSize <= 0)
            {
                cb.Checked = false;
            }
            else
            {
                cb.Checked = true;
            }

            txEnabled = cb.Checked;

            

            
        }
    
        
    
    }

    void SetTxDelayWithText()
    {
        if(int.TryParse(delayMsTb.Text,out var newDelay))
        {
            if(newDelay < 1)
            {
                newDelay = 1;
                delayMsTb.Text = "1";
            }
            
            currentTxDelay = newDelay;
        }
        else
        {
            currentTxDelay = 100;
            delayMsTb.Text = "100";
        }
    }
    
    void SetTxDelay()
    {
        if(int.TryParse(delayMsTb.Text,out var newDelay))
        {
            if(newDelay < 1)
            {
                newDelay = 1;
            }
            
            currentTxDelay = newDelay;
            
            
        }
        else
        {
            currentTxDelay = 100;
        }
    }



    void OnRxOverwriteClicked(object? sender, EventArgs e)
    {
        if(sender == null)
        {
            return;
        }
        else if(sender.GetType() == typeof(CheckBox))
        {
            var cb = (CheckBox)sender;
            overwriteRxData = cb.Checked;
        }
    }



    void OnTxRepeatClicked(object? sender, EventArgs e)
    {
        if(sender == null)
        {
            return;
        }
        else if(sender.GetType() == typeof(CheckBox))
        {
            var cb = (CheckBox)sender;
            reapeatTxData = cb.Checked;
        }
    }


    
    void IntTextBoxKeyPress(object? sender, KeyPressEventArgs e)
    {
        if(sender == null)
        {
            return;
        }
        else if(sender.GetType() == typeof(TextBox))
        {
            var tb =  (TextBox)sender;

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            
            
        }
        
    }

    void OnTxModeClick(object? o, EventArgs e)
    {
        if(o.GetType() == typeof(ToolStripMenuItem))
        {

            var clickedItem = ((ToolStripMenuItem)o);


            previousTxModeItem.BackColor = Color.White;


            previousTxModeItem = clickedItem;

            

            foreach(var key in txModeToString)
            {
                
                if(clickedItem.Text == key.Value)
                {
                    this.currentTxMode = key.Key;
                    ((ToolStripMenuItem)o).BackColor = Color.LightSeaGreen;
                    break;
                }
                
            }
        }
        
    }


    void OnLoggingChanged(object? sender, EventArgs e)
    {
        if(sender == null)
        {
            return;
        }
        else
        {
            var ObjectType = sender?.GetType();

            if (ObjectType == typeof(ToolStripMenuItem))
            {
                loggingEnabled = ((ToolStripMenuItem)sender).Text.Contains("Enable") ? true : false;

                if(loggingEnabled)
                {
                    ((ToolStripMenuItem)sender).Text = "Disable Logging";
                }
                else
                {
                    ((ToolStripMenuItem)sender).Text = "Enable Logging";
                }
            }
        }
    }

    


    void OnLoggingDelete()
    {
        logger.TryDeleteLogs();
        
    }

    
    /// <summary>
    /// Sets appropriate settings for serial ports based on the ID(text) for the form item
    /// </summary>
    /// <param name="id"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    bool SetPortSetting(string id, string value)
    {
        bool status = false;

        switch(id)
        {
            case "Baud":
                if(serialPort.SetStandardBaud(value))
                {
                    baud = value;
                    baudComboBox.Text = value;
                    status = true;
                }
            break;
            
            case "COM":
                
                if(serialPort.SetDesiredPort(value))
                {
                    port = value;
                    comComboBox.Text = value;
                    status = true;
                }
            break;

            case "Parity":
                if(serialPort.SetParity(value))
                {
                    parity = value;
                    parityComboBox.Text = value;
                    status = true;
                }
            break;
            
            case "Stop Bits":
                if(serialPort.SetStopBit(value))
                {
                    stopBit = value;
                    stopBitsComboBox.Text = value;
                    status = true;
                }
            break;
            
            case "Handshake":
                if(serialPort.SetHandshake(value))
                {
                    handshake = value;
                    handshakeComboBox.Text = value;
                    status = true;
                }
            break;

            case "Data Bits":
                if(serialPort.SetDataBits(value))
                {
                    dataBit = value;
                    dataBitsComboBox.Text = value;
                    status = true;
                }
            break;
            
            case "Read Timeout":
                if(serialPort.SetReadTimeout(value))
                {
                    readTimeout = value;
                    readTimeoutComboBox.Text = value;
                    status = true;
                }
            break;

            case "Write Timeout":
                if(serialPort.SetWriteTimeout(value))
                {
                    writeTimeout = value;
                    writeTimeoutComboBox.Text = value;
                    status = true;
                }
            break;
            
            default:

                if(serialPort.SetDesiredPort(value))
                {
                    port = value;
                    comComboBox.Text = value;
                    status = true;
                }

            break;
        };

        if(status == true)
        {
            ToolStripMenuItem? item = settingsItems.FindLast(x => (x.Text == id));

            

            foreach(ToolStripMenuItem dmi in item.DropDownItems)
            {
                if(dmi.DropDownItems.Count > 0)
                {
                    foreach(ToolStripMenuItem tsmi in dmi.DropDownItems)
                    {
                        if(tsmi.Text == value)
                        {
                            tsmi.BackColor = Color.LightSeaGreen;
                        }
                        else
                        {
                            tsmi.BackColor = Color.White;
                        }
                    }
                }
                else
                {
                    if(dmi.Text == value)
                    {
                        dmi.BackColor = Color.LightSeaGreen;
                    }
                    else
                    {
                        dmi.BackColor = Color.White;
                    }
                }
            }

            if(infoOnly)
            {
                if(setupControlsToLabel.TryGetValue(item, out var txt))
                {
                    txt.Text = id + ": " + value;
                }
            }
            
        }
        

        return status;
    }
    


    void OnMenuItemSetupSelected(object? sender, ToolStripItemClickedEventArgs e)
    {
        if(sender == null)
        {
            return;
        }
        else
        {


            var ObjectType = sender?.GetType();

            if (e.ClickedItem != null)
            {
                if (ObjectType == typeof(ToolStripMenuItem))
                {
                    
                    SetPortSetting(((ToolStripMenuItem)sender).Text, e.ClickedItem.Text);
                }

            }
        }
    }


    void ComBoxDrop(object? sender, EventArgs e)
    {
        comComboBox.Items.Clear();
        
        foreach(var p in PortChat.AvailablePorts)
        {
            if(comComboBox.Items.Contains(p) == false)
            {
                comComboBox.Items.Add(p);
            }
        }
    }

    void ReloadPorts(object? sender, EventArgs e)
    {
        foreach(var tsmi in settingsItems)
        {
            if(tsmi.Text == "COM")
            {
                

                for (var i = 0; i < tsmi.DropDownItems.Count; i++)
                {
                    tsmi.DropDownItems[i].Dispose();
                }

                foreach(var p in PortChat.AvailablePorts)
                {
                    bool containsCom = false;

                    foreach(ToolStripMenuItem item in tsmi.DropDownItems)
                    {
                        if(item.Text == p)
                        {
                            containsCom = true;
                            break;
                        }
                    }
                    if(containsCom == false)
                    {
                        var ddmi = new ToolStripMenuItem(p);
                        
                        if(port == p)
                        {
                            ddmi.BackColor = Color.LightSeaGreen;
                        }
                        else
                        {
                            ddmi.BackColor = Color.White;
                        }
                        
                        tsmi.DropDownItems.Add(ddmi);
                    }
                    

                }

                comComboBox.Text = port;
            }
        }
    }



    void OnClose()
    {
        serialPort.TryClosePort();
        Close();
    }






    void OnViewLogs()
    {
        
        CommsApp tempForm = new CommsApp(true,this.logger);
        this.Hide();
        this.ClosePortEvent();
        tempForm.Focus();
        tempForm.FormClosed += new FormClosedEventHandler((_, _) => this.Show());
        tempForm.Show();
    }



    void OnViewLogs(bool selectDb)
    {
        
        CommsApp tempForm = new CommsApp(true,this.logger,selectDb);
        this.Hide();
        this.ClosePortEvent();
        tempForm.Focus();
        tempForm.FormClosed += new FormClosedEventHandler((_, _) => this.Show());
        tempForm.Show();
    }


    void OnLoggerDropdown(object? obj, EventArgs e)
    {
        if(this.logger == null)
        {
            // MessageBox.Show("Please select log folder path", "IO_LOG.db Path not set",MessageBoxButtons.OK,MessageBoxIcon.Information);
            this.logger = new SQLiteCommLogger();
        }
    }


    void OnNewLogger(object? obj, EventArgs e)
    {
        this.logger.CreateNewDatabase();
        //MessageBox.Show("Please select log folder path", "IO_LOG.db",MessageBoxButtons.OK,MessageBoxIcon.Information);
        //this.logger = new SQLiteCommLogger(" ");


    }

    void OnSelectLogger()
    {
        this.logger.SelectDatabase();
        //MessageBox.Show("Please select log folder path", "IO_LOG.db",MessageBoxButtons.OK,MessageBoxIcon.Information);
        //this.logger = new SQLiteCommLogger(" ");


    }



}