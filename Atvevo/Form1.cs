using System;
using System.Windows.Forms;
using Atvevo.db;

namespace Atvevo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            var databaseConnection = new DatabaseConnection(true);
        }
    }
}