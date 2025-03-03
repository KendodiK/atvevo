using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Atvevo.db;

namespace Atvevo
{
    public partial class MainWindow : Form
    {
        private ComboBox supplierDropdown;

        public MainWindow()
        {
            InitializeComponent();
            var databaseConnection = new DatabaseConnection(true);
            supplierDropdown = new ComboBox{
                Parent = this,
                Width = 100,
                Height = 20, 
                Top = 10,
                Left = 20,
                DropDownStyle = ComboBoxStyle.DropDownList,
            };

            string[] suppliers = {"egy", "kettő", "három", "négy"};
            foreach (var supplier in suppliers)
            {
                supplierDropdown.Items.Add(supplier);
            }

            FormClosing += (sender, args) => { databaseConnection.DatabaseDisconnect(); };
        }
    }
}