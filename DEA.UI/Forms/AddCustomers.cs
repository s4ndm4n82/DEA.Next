using DEA.Next.Data;

namespace DEA.UI
{
    public partial class AddCustomers : Form
    {
        private readonly DataContext _conttext;

        public AddCustomers(DataContext context)
        {
            InitializeComponent();
            _conttext = context;
        }
    }
}
