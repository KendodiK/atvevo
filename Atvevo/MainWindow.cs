using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Atvevo.db;

namespace Atvevo
{
    public partial class MainWindow : Form
    {
        class FruitSelectElement
        {
            private int fomLeft { get; set;}
            private int fomTop { get; set;}
            private int fruitId { get; set;}
            public CheckBox fruitCheckBox { get; set;}
            private Label fruitName { get; set;}
            public NumericUpDown quantity { get; set;}
            
            //public NumericUpDown Quantity { get => quantity.Value; set => quantity = value; }

            public FruitSelectElement(int fomLeft, int fomTop, int fruitId, Panel mainWindow)
            {
                this.fomLeft = fomLeft;
                this.fomTop = fomTop;
                this.fruitId = fruitId;

                this.fruitCheckBox = new CheckBox
                {
                    Parent = mainWindow,
                    Width = 30,
                    Height = 30,
                    Top = fomTop,
                    Left = fomLeft,
                    Checked = false,
                };
                this.fruitCheckBox.CheckedChanged += (sender, args) => { EnableQuantity(sender, args); };

                this.fruitName = new Label
                {
                    Parent = mainWindow,
                    Width = 100,
                    Height = 30,
                    Top = fomTop,
                    Left = fomLeft + 80,
                    Text = $"kg Fruit name",
                };

                this.quantity = new NumericUpDown
                {
                    Parent = mainWindow,
                    Width = 50,
                    Height = 30,
                    Top = fomTop,
                    Left = fomLeft + 30,
                    Value = 0,
                    Enabled = false,
                };
            }

            private void EnableQuantity(object sender, EventArgs e)
            {
                quantity.Enabled = ((CheckBox)sender).Checked;
                if (!((CheckBox)sender).Checked)
                {
                    quantity.Value = 0;
                }
            }
        }
        
        private static ComboBox _supplierDropdown;
        private static Panel selectElementsPanel;
        private static TextBox name;
        private static TextBox city;
        private static TextBox zipCode;
        private static TextBox phone;
        private static TextBox street;
        private static TextBox houseNumber;
        private static Button addSupplierButton;
        private static Button deleteSupplierButton;
        private static Button showListButton;
        private static Button saveButton;
        private static List<FruitSelectElement> selectElements = new List<FruitSelectElement>();

        public MainWindow()
        {
            InitializeComponent();
            Text = "Átvétel";
            var databaseConnection = new DatabaseConnection(true);
            Font = new System.Drawing.Font("Microsoft Sans Serif", 13);
            Width = 800;
            Height = 450;
            BuildForm(this, databaseConnection);
            FormClosing += (sender, args) => { databaseConnection.DatabaseDisconnect(); };
        }

        static void BuildForm(Form mainWin, DatabaseConnection databaseConnection)
        {
            _supplierDropdown = new ComboBox
            {
                Parent = mainWin,
                Width = 130,
                Height = 20,
                Top = 10,
                Left = 20,
                DropDownStyle = ComboBoxStyle.DropDownList,
            };
            selectElementsPanel = new Panel
            {
                Parent = mainWin,
                Width = 700,
                Height = 300,
                Top = 40,
                Left = 20,
            };
            var suppliers = databaseConnection.SuppliersTable.Read();
            foreach (var supplier in suppliers)
            {
                _supplierDropdown.Items.Add(supplier.Name);
            }
            _supplierDropdown.SelectedIndexChanged += 
                (sender, args) => SupplierDropdown_SelectedIndexChanged(
                    sender, args, mainWin, databaseConnection);
        }
        
        private static void SupplierDropdown_SelectedIndexChanged(object sender, EventArgs e, Form senderForm, DatabaseConnection databaseConnection)
        {
            selectElements.Clear();
            selectElementsPanel.Controls.Clear();
            ComboBox suppliers = (ComboBox)sender;
            int supplierId = suppliers.SelectedIndex + 1;
            //adatbázisból lekérni id alapján a lehetséges gyümölcsök id-jét
            int[] fruitIds = {1, 2, 3, 4};
            for (int i = 0; i < fruitIds.Length; i++)
            {
                FruitSelectElement fruitSelectElement = new FruitSelectElement(
                    10, 60 + i * 30 , fruitIds[i], selectElementsPanel);
                selectElements.Add(fruitSelectElement);
            }
        }
    }
}