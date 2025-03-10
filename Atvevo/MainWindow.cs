using System;
using System.Collections.Generic;
using System.Drawing;
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
            private Label fruitNameLabel { get; set;}
            private List<(int, int)> products { get; set;}
            public CheckBox fruitCheckBox;
            public NumericUpDown quantity;
            
            List<(int, int)> Products {
                get => products;
            }
            
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
                    Width = 300,
                    Height = 60,
                    Top = fomTop,
                    Left = fomLeft + 80,
                    Text = $"kg {fruit.Name} \n {fruit.Category}",
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
                this.quantity.ValueChanged += (sender, args) => addProduct(sender, args);
            }

            private void EnableQuantity(object sender, EventArgs e)
            {
                quantity.Enabled = ((CheckBox)sender).Checked;
                saveButton.Enabled = true;
                if (!((CheckBox)sender).Checked)
                {
                    quantity.Value = 0;
                }
            }

            private void addProduct(object sender, EventArgs e)
            {
                products.Add((fruit.Id, (int)quantity.Value));
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
        private static Button addSupplierButton;
        private static Button changeSupplierButton;
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

        private void BuildForm(Form mainWin, DatabaseConnection databaseConnection)
        {
            _supplierDropdown = new ComboBox { Parent = mainWin, Width = 130, Height = 20, Top = 10, Left = 20, DropDownStyle = ComboBoxStyle.DropDownList, };
            selectElementsPanel = new Panel { Parent = mainWin, Width = 380, Height = 300, Top = 50, Left = 20, };
            selectElementsPanel.VerticalScroll.Enabled = true;
            
            int supplierTop = 30;
            Label name = new Label 
                { Parent = mainWin, Width = 45, Height = 20, Left = 400, Top = 10, Text = "Név:",};
            Label city = new Label 
                { Parent = mainWin, Width = 130, Height = 20, Top = supplierTop + 10, Left = 400, Text = "Város, ir. sz.:",};
            Label streeth = new Label 
                { Parent = mainWin, Width = 100, Height = 20, Top = supplierTop * 2 + 10, Left = 400, Text = "Utca, hsz.:",};
            Label phoneNum = new Label 
                { Parent = mainWin, Width = 80, Height = 20, Top = supplierTop * 3 + 10, Left = 400, Text = "Tel. sz.",};

            int leftCounted = name.Left + name.Width + 10;
            inName = new TextBox 
                { Parent = mainWin, Width = 200, Height = 20, Top = 10, Left = leftCounted };
            leftCounted = city.Left + city.Width + 10;
            inCity = new TextBox 
                { Parent = mainWin, Width = 110, Height = 20, Top = supplierTop + 10, Left = leftCounted };
            leftCounted += inCity.Width + 10;
            inZipCode = new TextBox 
                { Parent = mainWin, Width = 50, Height = 20, Top = supplierTop + 10, Left = leftCounted };
            leftCounted = streeth.Left + streeth.Width + 10;
            inStreet = new TextBox 
                { Parent = mainWin, Width = 150, Height = 20, Top = supplierTop * 2 + 10, Left = leftCounted };
            leftCounted += inStreet.Width + 10;
            inHouseNumber = new TextBox 
                { Parent = mainWin, Width = 30, Height = 20, Top = supplierTop * 2 + 10, Left = leftCounted };
            leftCounted = phoneNum.Left + phoneNum.Width + 10;
            inPhone = new TextBox 
                { Parent = mainWin, Width = 200, Height = 20, Top = supplierTop * 3 + 10, Left = leftCounted };

            addSupplierButton = new Button
                { Parent = mainWin, Width = 100, Height = 60, Top = supplierTop * 4 + 10, Left = 400, Text = "Beszállító \nhozzáadása",};
                addSupplierButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 11);
                addSupplierButton.Click += (sender, args) => addNewSupplier(sender, args, databaseConnection);

            changeSupplierButton = new Button
                { Parent = mainWin, Width = 100, Height = 60, Top = supplierTop * 4 + 10, Left = 510, 
                    Text = "Beszállító \nmódosítása", Enabled = false
                };
                changeSupplierButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 11);

            deleteSupplierButton = new Button
                { Parent = mainWin, Width = 100, Height = 60, Top = supplierTop * 4 + 10, Left = 620, 
                    Text = "Beszállító \ntörlése", Enabled = false
                };
                deleteSupplierButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 11);

            showListButton = new Button
                { Parent = mainWin, Width = 100, Height = 60, Top = supplierTop * 7, Left = 510, Text = "Lista megjelenítése", };
                showListButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 11);
                showListButton.Click += (sender, args) => { };

            saveButton = new Button
                { Parent = mainWin, Width = 150, Height = 30, Top = 360, Left = 400, Text = "Beszállítás mentése", Enabled = false};
                saveButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 11);
                    
                    
            var suppliers = databaseConnection.SuppliersTable.Read();
            foreach (var supplier in suppliers)
            {
                _supplierDropdown.Items.Add(supplier.Name);
            }
            _supplierDropdown.SelectedIndexChanged += 
                (sender, args) => SupplierDropdown_SelectedIndexChanged(
                    sender, args, databaseConnection, suppliers[_supplierDropdown.SelectedIndex]);
        }
        
        private void SupplierDropdown_SelectedIndexChanged(object sender, EventArgs e, DatabaseConnection databaseConnection, Supplier supplier)
        {
            selectElements.Clear();
            selectElementsPanel.Controls.Clear();
            addSupplierButton.Enabled = false;
            changeSupplierButton.Enabled = true;
            changeSupplierButton.Click += (senderBtn, args) => changeSupplier(senderBtn, args, databaseConnection, supplier);
            deleteSupplierButton.Enabled = true;
            deleteSupplierButton.Click += (senderBtn, args) => deleteSupplier(senderBtn, args, databaseConnection, supplier);
            ComboBox suppliers = (ComboBox)sender;
            Product[] fruits = databaseConnection.ProductsTable.GetBySupplier(supplier);
            for (int i = 0; i < fruits.Length; i++)
            {
                FruitSelectElement fruitSelectElement = new FruitSelectElement(
                    10, 60 + i * 60 , fruits[i], selectElementsPanel);
                selectElements.Add(fruitSelectElement);
            }
            writeSupplierData(supplier);
        }

        private void writeSupplierData(Supplier supplier)
        {
            inName.Text = supplier.Name;
            inCity.Text = supplier.City;
            inZipCode.Text = supplier.ZipCode;
            inStreet.Text = supplier.Street;
            inHouseNumber.Text = supplier.HouseNumber.ToString();
            inPhone.Text = supplier.Phone;
        }

        private void addNewSupplier(object sender, EventArgs e, DatabaseConnection databaseConnection)
        {
            if (inName.Text == "" || inCity.Text == "" || inZipCode.Text == "" || inStreet.Text == "" ||
                inHouseNumber.Text == "" || inPhone.Text == "")
            {
                MessageBox.Show("Minden mezőt közelező kitölteni!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                Supplier supplier = new Supplier
                {
                    Name = inName.Text,
                    City = inCity.Text,
                    ZipCode = inZipCode.Text,
                    Street = inStreet.Text,
                    HouseNumber = Convert.ToByte(inHouseNumber.Text),
                    Phone = inPhone.Text,
                };
                if (databaseConnection.SuppliersTable.Insert(supplier))
                {
                    MessageBox.Show("Sikeresen hozzáadva!", "Sikeres", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _supplierDropdown.Items.Add(supplier.Name);   
                    _supplierDropdown.SelectedIndex = _supplierDropdown.Items.Count - 1;
                }
                else
                {
                    ErrorMsg();
                }
            }
        }

        private void changeSupplier(object sender, EventArgs e, DatabaseConnection databaseConnection, 
            Supplier supplier)
        {
            if (inName.Text == "" || inCity.Text == "" || inZipCode.Text == "" || inStreet.Text == "" ||
                inHouseNumber.Text == "" || inPhone.Text == "")
            {
                MessageBox.Show("Minden mezőt közelező kitölteni!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                supplier.Name = inName.Text;
                supplier.City = inCity.Text;
                supplier.ZipCode = inZipCode.Text;
                supplier.Street = inStreet.Text;
                supplier.HouseNumber = Convert.ToByte(inHouseNumber.Text);
                supplier.Phone = inPhone.Text;

                if (databaseConnection.SuppliersTable.Update(supplier))
                {
                    MessageBox.Show("Sikeresen módosítva!", "Sikeres", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    ErrorMsg();
                } 
            }
        }

        private void deleteSupplier(object sender, EventArgs e, DatabaseConnection databaseConnection, Supplier supplier)
        {
            if (databaseConnection.SuppliersTable.Delete(supplier))
            {
                MessageBox.Show("Sikeresen törölve!", "Sikeres", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                ErrorMsg();
            } 
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            
        }

        private void ErrorMsg()
        {
            MessageBox.Show("Nem várt hiba! Kérjük próbálja meg újra!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}