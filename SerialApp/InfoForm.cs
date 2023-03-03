

/// <summary>
/// Form for displaying app info
/// </summary>
public class InfoForm : AppForm
{
    public InfoForm(FlowLayoutPanel parentForm) : base(parentForm)
    {

    }

    protected override void ComponentConfig()
    {
        this.ClientSize = new System.Drawing.Size(800, 950);
        this.Text = "Information/Help Guide";
        var newLabel = CreateNewLabel("Example Label",0,0,0,0, Color.Black);
        this.Controls.Add(newLabel);
    }
}