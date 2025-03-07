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
            private Product fruit { get; set;}
            public CheckBox fruitCheckBox { get; set;}
            private Label fruitNameLabel { get; set;}
            public NumericUpDown quantity { get; set;}
            
            //public NumericUpDown Quantity { get => quantity.Value; set => quantity = value; }

            public FruitSelectElement(int fomLeft, int fomTop, Product fruit, Panel mainWindow)
            {
                this.fomLeft = fomLeft;
                this.fomTop = fomTop;
                this.fruit = fruit;

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

                this.fruitNameLabel = new Label
                {
                    Parent = mainWindow,
                    Width = 100,
                    Height = 30,
                    Top = fomTop,
                    Left = fomLeft + 80,
                    Text = $"kg {fruit.Name}",
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
        private static TextBox inName;
        private static TextBox inCity;
        private static TextBox inZipCode;
        private static TextBox inPhone;
        private static TextBox inStreet;
        private static TextBox inHouseNumber;
        private static Panel supplierPanel;
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
            _supplierDropdown = new ComboBox { Parent = mainWin, Width = 130, Height = 20, Top = 10, Left = 20, DropDownStyle = ComboBoxStyle.DropDownList, };
            selectElementsPanel = new Panel { Parent = mainWin, Width = 250, Height = 300, Top = 50, Left = 20, AutoScroll = true};
            int supplierTop = 30;
            supplierPanel = new Panel { Parent = mainWin, Width = 490, Height = 300, Top = 10, Left = 310, };
            TextBox name = new TextBox { Parent = supplierPanel, Width = 35, Height = 20, Left = 10, Text = "Név:"};
            TextBox city = new TextBox { Parent = supplierPanel, Width = 50, Height = 20, Top = supplierTop, Left = 10, Text = "Város:"};
            TextBox streath = new TextBox { Parent = supplierPanel, Width = 80, Height = 20, Top = supplierTop * (supplierPanel.Controls.Count - 1), Left = 10, Text = "Utca, hsz.:"};
            TextBox phoneNum = new TextBox { Parent = supplierPanel, Width = 60, Height = 20, Top = supplierTop * (supplierPanel.Controls.Count - 1), Left = 10, Text = "Tel. sz." };
            var suppliers = databaseConnection.SuppliersTable.Read();
            foreach (var supplier in suppliers)
            {
                _supplierDropdown.Items.Add(supplier.Name);
            }
            _supplierDropdown.SelectedIndexChanged += 
                (sender, args) => SupplierDropdown_SelectedIndexChanged(
                    sender, args, databaseConnection, suppliers[_supplierDropdown.SelectedIndex]);
        }
        
        private static void SupplierDropdown_SelectedIndexChanged(object sender, EventArgs e, DatabaseConnection databaseConnection, Supplier supplier)
        {
            selectElements.Clear();
            selectElementsPanel.Controls.Clear();
            ComboBox suppliers = (ComboBox)sender;
            Product[] fruits = databaseConnection.ProductsTable.GetBySupplier(supplier);
            for (int i = 0; i < fruits.Length; i++)
            {
                FruitSelectElement fruitSelectElement = new FruitSelectElement(
                    10, 60 + i * 30 , fruits[i], selectElementsPanel);
                selectElements.Add(fruitSelectElement);
            }
        }
    }
}