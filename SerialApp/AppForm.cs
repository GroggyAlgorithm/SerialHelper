
/// <summary>
/// <see langword="abstract"/> Abstract class for Forms to inherit from
/// </summary>
public abstract class AppForm : Form
{
    protected FlowLayoutPanel mainDisplay;

    public AppForm(FlowLayoutPanel mainDisplay)
    {
        ComponentConfig();
        this.mainDisplay = mainDisplay;
    }

    public AppForm()
    {
        ComponentConfig();
        this.mainDisplay = new System.Windows.Forms.FlowLayoutPanel();
        this.mainDisplay.Size = this.ClientSize;
        this.mainDisplay.AutoSize = true;
    }

    protected abstract void ComponentConfig();

    protected virtual TextBox CreateTextBox(int locationX, int locationY, string text, int width, int height, Color color)
    {
        TextBox newTextBox = new TextBox();
        newTextBox.Location = new Point(locationX,locationY);
        newTextBox.Text = text;
        newTextBox.Size = new Size(width,height);
        newTextBox.TextAlign = HorizontalAlignment.Center;
        newTextBox.BackColor = color;

        return newTextBox;
    }


    protected virtual Button CreateNewButton(string text, int locationX, int locationY, int marginX, int marginY, bool autoSize = true)
    {
        var newButton = new System.Windows.Forms.Button();
        
        newButton.Text = text;
        newButton.Location = new Point(locationX,locationY);
        newButton.AutoSize = true;
        newButton.Margin = new Padding(marginX,marginY,0,0);
        
        return newButton;
    }

    protected virtual Label CreateNewLabel(string text, int locationX, int locationY, int marginX, int marginY, Color foreColor, BorderStyle bStyle = BorderStyle.FixedSingle)
    {
        var newLabel = new System.Windows.Forms.Label();
        newLabel.Location = new Point(locationX,locationY);
        newLabel.Margin = new Padding(marginX,marginY,0,0);
        newLabel.BorderStyle = bStyle;
        newLabel.Text = text;
        newLabel.Font = new Font("Times New Roman", 12);
        newLabel.ForeColor = foreColor;
        return newLabel;
    }

    protected virtual Label CreateNewLabel(string text, int locationX, int locationY, int marginX, int marginY, Color foreColor, Color backColor, BorderStyle bStyle = BorderStyle.FixedSingle)
    {
        var newLabel = new System.Windows.Forms.Label();
        newLabel.Location = new Point(locationX,locationY);
        newLabel.Margin = new Padding(marginX,marginY,0,0);
        newLabel.BorderStyle = bStyle;
        newLabel.Text = text;
        newLabel.Font = new Font("Times New Roman", 12);
        newLabel.ForeColor = foreColor;
        newLabel.BackColor = backColor;
        return newLabel;
    }

    protected virtual Panel CreatePanel(int locationX, int locationY, int width, int height, Color color)
    {
        Panel newPanel = new Panel();
        newPanel.Location = new Point(locationX,locationY);
        newPanel.Size = new Size(width,height);
        newPanel.BackColor = color;

        return newPanel;
    }


    protected virtual VScrollBar CreateNewScrollBar(int locationX, int locationY, int width, int height, int minimumValues, int maximumValues, 
        Color color, int largeChange = 1)
    {
        var vScrollBar = new VScrollBar();
        vScrollBar.Minimum = minimumValues;
        vScrollBar.Maximum = maximumValues;
        vScrollBar.Width = width;
        vScrollBar.Height = height;
        vScrollBar.Location = new Point(locationX, locationY);
        vScrollBar.LargeChange = largeChange;
        vScrollBar.BackColor = color;
        return vScrollBar;
    }
}


