using System.Windows.Forms;
using Atvevo.db;

namespace Atvevo
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
            var databaseConnection = new DatabaseConnection(true);
            FormClosing += (sender, args) => { databaseConnection.DatabaseDisconnect(); };
        }
    }
}