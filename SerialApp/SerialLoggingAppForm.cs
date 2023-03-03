using System.IO.Ports;





/// <summary>
/// Class for Forms that will log serial data to inherit from
/// </summary>
public abstract class SerialLoggingAppForm : AppForm
{
    protected SQLiteCommLogger logger;
    protected TextBox clock;
    protected PortChat serialPort;
    public Dictionary<string, Parity> ParityOptions => PortChat.ParityOptions;
    public Dictionary<string, StopBits> StopBitOptions => PortChat.StopBitOptions;
    public Dictionary<string, Handshake> HandShakeOptions => PortChat.HandShakeOptions;
    public string[] TransmissionDelayOptions => PortChat.StandardTransmissionDelayOptions;
    public string[] PossibleBauds => PortChat.StandardPossibleBauds;
    public Dictionary<string, PortChat.DATA_BITS> DataBitOptions => PortChat.DataBitOptions;

    

    
    protected void TimerTick(object? sender, EventArgs e)
    {
        this.clock.Text = DateTime.Now.ToString("MM-dd-yyyy hh:mm:ss"); // Display current time in textbox
    }

    public SerialLoggingAppForm() : base()
    {
        
        this.ClientSize = new System.Drawing.Size(725, 348);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
        this.Text = "Serial COMs";
        logger = new SQLiteCommLogger();

        

        serialPort = new PortChat();
    }


    protected abstract void WriteToComm();
    protected abstract void ReadFromComm();
}



