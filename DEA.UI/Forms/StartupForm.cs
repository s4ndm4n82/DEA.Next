using DEA.Next.Data;
using DEA.UI.Forms;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.Versioning;

namespace DEA.UI;

[SupportedOSPlatform("windows")]
public partial class StartupForm : Form
{
    private readonly IServiceProvider _services;

    public StartupForm(IServiceProvider services)
    {
        InitializeComponent();
        _services = services;

        // Check the database on load
        Load += StartupForm_Load;

        // Set the Auto Size to true
        AutoSize = true;

        // Set the AutoSizeMode property to GrowAndShrink
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
    }

    /// <summary>
    /// Loads the forms on startup if the database exists
    /// </summary>
    private async void StartupForm_Load(object? sender, EventArgs e)
    {
        var context = _services.GetRequiredService<DataContext>();
        var databaseChecker = new HelperClasses.CheckDbConnection(context);
        var result = await databaseChecker.CheckDataBaseExistsAsync("DeaDataBase");

        if (!result)
        {
            Application.Exit();
        }
        else
        {
            LoadForms(_services);
        }
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
        var editCustomersForm = new EditCustomersList(services.GetRequiredService<DataContext>())
        {
            TopLevel = false,
            FormBorderStyle = FormBorderStyle.None,
            Dock = DockStyle.Fill
        };
        EditCustomers?.Controls.Add(editCustomersForm);
        editCustomersForm.Show();

        // Load the Remove Customers form
        var removeCustomersForm = new RemoveCustomers(services.GetRequiredService<DataContext>())
        {
            TopLevel = false,
            FormBorderStyle = FormBorderStyle.None,
            Dock = DockStyle.Fill
        };
        RemoveCustomers?.Controls.Add(removeCustomersForm);
        removeCustomersForm.Show();

        // Load the About form
        var abtForm = new AboutForm()
        {
            TopLevel = false,
            FormBorderStyle = FormBorderStyle.None,
            Dock = DockStyle.Fill
        };
        AboutForm?.Controls.Add(abtForm);
        abtForm.Show();
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
        AboutForm = new TabPage();
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
        mainTabControler.Controls.Add(AboutForm);
        mainTabControler.Location = new Point(12, 2);
        mainTabControler.Name = "mainTabControler";
        mainTabControler.SelectedIndex = 0;
        mainTabControler.Size = new Size(1436, 813);
        mainTabControler.TabIndex = 0;
        // 
        // AddClients
        // 
        AddClients.Location = new Point(4, 24);
        AddClients.Name = "AddClients";
        AddClients.Padding = new Padding(3);
        AddClients.Size = new Size(1428, 785);
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
        EditCustomers.Size = new Size(1428, 785);
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
        RemoveCustomers.Size = new Size(1428, 785);
        RemoveCustomers.TabIndex = 2;
        RemoveCustomers.Text = "Remove Customers";
        RemoveCustomers.UseVisualStyleBackColor = true;
        // 
        // AboutForm
        // 
        AboutForm.Location = new Point(4, 24);
        AboutForm.Name = "AboutForm";
        AboutForm.Size = new Size(1428, 785);
        AboutForm.TabIndex = 3;
        AboutForm.Text = "About";
        AboutForm.UseVisualStyleBackColor = true;
        // 
        // StartupForm
        // 
        AccessibleDescription = "Main window for the application.";
        AccessibleName = "DEA.UI";
        BackColor = SystemColors.Control;
        ClientSize = new Size(1460, 827);
        Controls.Add(mainTabControler);
        FormBorderStyle = FormBorderStyle.FixedSingle;
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