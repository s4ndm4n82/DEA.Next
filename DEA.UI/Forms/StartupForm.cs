using DEA.Next.Data;
using Microsoft.Extensions.DependencyInjection;

namespace DEA.UI;

public partial class StartupForm : Form
{
    public StartupForm(IServiceProvider services)
    {
        InitializeComponent();
        LoadForms(services);

        // Set the Auto Size to true
        AutoSize = true;

        // Set the AutoSizeMode property to GrowAndShrink
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
    }

    ///
    /// <summary>
    /// Loads the client forms
    /// </summary>
    private void LoadForms(IServiceProvider services)
    {
        // Load the Add Clients form
        var addClienstForm = new AddCustomers(services.GetRequiredService<DataContext>())
        {
            TopLevel = false,
            FormBorderStyle = FormBorderStyle.None,
            Dock = DockStyle.Fill
        };
        AddClients?.Controls.Add(addClienstForm);
        addClienstForm.Show();

        // Load the Edit Customers form
        var editCustomersForm = new EditCustomers
        {
            TopLevel = false,
            FormBorderStyle = FormBorderStyle.None,
            Dock = DockStyle.Fill
        };
        EditCustomers?.Controls.Add(editCustomersForm);
        editCustomersForm.Show();

        // Load the Remove Customers form
        var removeCustomersForm = new RemoveCustomers
        {
            TopLevel = false,
            FormBorderStyle = FormBorderStyle.None,
            Dock = DockStyle.Fill
        };
        RemoveCustomers?.Controls.Add(removeCustomersForm);
        removeCustomersForm.Show();
    }


    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        mainTabControler = new TabControl();
        AddClients = new TabPage();
        EditCustomers = new TabPage();
        RemoveCustomers = new TabPage();
        mainTabControler.SuspendLayout();
        SuspendLayout();
        // 
        // mainTabControler
        // 
        mainTabControler.AccessibleDescription = "Tab controller to load the other forms.";
        mainTabControler.AccessibleName = "mainTabControler";
        mainTabControler.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        mainTabControler.Controls.Add(AddClients);
        mainTabControler.Controls.Add(EditCustomers);
        mainTabControler.Controls.Add(RemoveCustomers);
        mainTabControler.Location = new Point(12, 2);
        mainTabControler.Name = "mainTabControler";
        mainTabControler.SelectedIndex = 0;
        mainTabControler.Size = new Size(1940, 1047);
        mainTabControler.TabIndex = 0;
        // 
        // AddClients
        //
        AddClients.Location = new Point(4, 24);
        AddClients.Name = "AddClients";
        AddClients.Padding = new Padding(3);
        AddClients.Size = new Size(1932, 1019);
        AddClients.TabIndex = 0;
        AddClients.Text = "Add Clients";
        AddClients.UseVisualStyleBackColor = true;
        // 
        // EditCustomers
        // 
        EditCustomers.AutoScroll = true;
        EditCustomers.Location = new Point(4, 24);
        EditCustomers.Name = "EditCustomers";
        EditCustomers.Padding = new Padding(3);
        EditCustomers.Size = new Size(1932, 1019);
        EditCustomers.TabIndex = 1;
        EditCustomers.Text = "Edit Customers";
        EditCustomers.UseVisualStyleBackColor = true;
        // 
        // RemoveCustomers
        // 
        RemoveCustomers.AutoScroll = true;
        RemoveCustomers.Location = new Point(4, 24);
        RemoveCustomers.Name = "RemoveCustomers";
        RemoveCustomers.Padding = new Padding(3);
        RemoveCustomers.Size = new Size(1932, 1019);
        RemoveCustomers.TabIndex = 2;
        RemoveCustomers.Text = "Remove Customers";
        RemoveCustomers.UseVisualStyleBackColor = true;
        // 
        // StartupForm
        // 
        AccessibleDescription = "Main window for the application.";
        AccessibleName = "DEA.UI";
        BackColor = SystemColors.Control;
        ClientSize = new Size(1964, 1061);
        Controls.Add(mainTabControler);
        MaximizeBox = false;
        Name = "StartupForm";
        StartPosition = FormStartPosition.CenterScreen;
        mainTabControler.ResumeLayout(false);
        ResumeLayout(false);
    }

    private TabControl? mainTabControler;
    private TabPage? AddClients;
    private TabPage? EditCustomers;
    private TabPage? RemoveCustomers;
}