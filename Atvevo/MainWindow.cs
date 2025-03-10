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
            private int[] products { get; set;}
            public CheckBox fruitCheckBox;
            public NumericUpDown quantity;
            
            public int[] Products {
                get => products;
            }
            
            //public NumericUpDown Quantity { get => quantity.Value; set => quantity = value; }

            public FruitSelectElement(int fomLeft, int fomTop, Product fruit, Panel mainWindow)
            {
                this.fomLeft = fomLeft;
                this.fomTop = fomTop;
                this.fruit = fruit;
                this.products = new int[2];

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
                    Width = 250,
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
                if (!((CheckBox)sender).Checked)
                {
                    quantity.Value = 0;
                }
            }

            private void addProduct(object sender, EventArgs e)
            {
                saveButton.Enabled = true;
                products[0] = fruit.Id;
                products[1] = (int)quantity.Value;
            }
        }
        
        private static ComboBox _supplierDropdown;
        private static Panel selectElementsPanel;
        private static TextBox inName;
        private static TextBox inCity;
        private static TextBox inZipCode;
        private static TextBox inCounty;
        private static TextBox inPhone;
        private static TextBox inStreet;
        private static TextBox inHouseNumber;
        private static Panel addFruitPanel;
        private static TextBox inFruitName;
        private static TextBox inFruitCategory;
        private static TextBox inFruitPrice;
        private static Button addSupplierButton;
        private static Button changeSupplierButton;
        private static Button deleteSupplierButton;
        private static Button showListButton;
        private static Button saveButton;
        private static Button addFruitButton;
        private static List<FruitSelectElement> selectElements = new List<FruitSelectElement>();

        public MainWindow()
        {
            InitializeComponent();
            Text = "Átvétel";
            var databaseConnection = new DatabaseConnection(true);
            Font = new System.Drawing.Font("Microsoft Sans Serif", 13);
            Width = 800;
            Height = 500;
            BuildForm(this, databaseConnection);
            FormClosing += (sender, args) => { databaseConnection.DatabaseDisconnect(); };
        }

        private void BuildForm(Form mainWin, DatabaseConnection databaseConnection)
        {
            _supplierDropdown = new ComboBox { Parent = mainWin, Width = 130, Height = 20, Top = 10, Left = 20, DropDownStyle = ComboBoxStyle.DropDownList, };
            selectElementsPanel = new Panel { Parent = mainWin, Width = 390, Height = 300, Top = 160, Left = 20, AutoScroll = true};
            selectElementsPanel.VerticalScroll.Enabled = true;
            
            int supplierTop = 30;
            Label name = new Label 
                { Parent = mainWin, Width = 45, Height = 20, Left = 420, Top = 10, Text = "Név:",};
            Label city = new Label 
                { Parent = mainWin, Width = 130, Height = 20, Top = supplierTop + 10, Left = 420, Text = "Város, ir. sz.:",};
            Label county = new Label 
                {Parent = mainWin, Width = 100, Top = supplierTop * 2 + 10, Left = 420, Text = "Megye:",};
            Label streeth = new Label 
                { Parent = mainWin, Width = 100, Height = 20, Top = supplierTop * 3 + 10, Left = 420, Text = "Utca, hsz.:",};
            Label phoneNum = new Label 
                { Parent = mainWin, Width = 80, Height = 20, Top = supplierTop * 4 + 10, Left = 420, Text = "Tel. sz.",};

            addFruitPanel = new Panel { Parent = mainWin, Width = 380, Height = 110, Top = 40, Left = 20, Visible = false};
            Label fruitName = new Label
                { Parent = addFruitPanel, Width = 50, Height = 25, Left = 30, Text = "Fajta",};
            Label fruitCategory = new Label
                { Parent = addFruitPanel, Width = 90, Left = 120, Height = 25, Text = "Gyömölcs típus",};
            Label price = new Label 
                { Parent = addFruitPanel, Width = 70, Left = 210, Height = 25, Text = "Kilóár",};
            inFruitCategory = new TextBox { Parent = addFruitPanel, Width = 120, Top = 30, };
            inFruitName = new TextBox { Parent = addFruitPanel, Width = 70, Left = 130, Top = 30, };
            inFruitPrice = new TextBox { Parent = addFruitPanel, Width = 50, Left = 210, Top = 30, };
            addFruitButton = new Button 
                { Parent = addFruitPanel, Width = 200, Height = 30, Left = 20, Top = 60, Text = "Gyümölcs hozzáadása"};
                addFruitButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 11);
                addFruitButton.Click += (sender, args) => AddFruitButton_Click(sender, args, databaseConnection);


                int leftCounted = name.Left + name.Width + 10;
            inName = new TextBox 
                { Parent = mainWin, Width = 200, Height = 20, Top = 10, Left = leftCounted };
            leftCounted = city.Left + city.Width + 10;
            inCity = new TextBox 
                { Parent = mainWin, Width = 110, Height = 20, Top = supplierTop + 10, Left = leftCounted };
            leftCounted += inCity.Width + 10;
            inZipCode = new TextBox 
                { Parent = mainWin, Width = 50, Height = 20, Top = supplierTop + 10, Left = leftCounted };
            leftCounted = county.Left + county.Width + 10;
            inCounty = new TextBox 
                { Parent = mainWin, Width = 110, Top = supplierTop * 2 + 10, Left = leftCounted };
            leftCounted = streeth.Left + streeth.Width + 10;
            inStreet = new TextBox 
                { Parent = mainWin, Width = 150, Height = 20, Top = supplierTop * 3 + 10, Left = leftCounted };
            leftCounted += inStreet.Width + 10;
            inHouseNumber = new TextBox 
                { Parent = mainWin, Width = 30, Height = 20, Top = supplierTop * 3 + 10, Left = leftCounted };
            leftCounted = phoneNum.Left + phoneNum.Width + 10;
            inPhone = new TextBox 
                { Parent = mainWin, Width = 200, Height = 20, Top = supplierTop * 4 + 10, Left = leftCounted };

            addSupplierButton = new Button
                { Parent = mainWin, Width = 100, Height = 60, Top = supplierTop * 5 + 10, Left = 420, Text = "Beszállító \nhozzáadása",};
                addSupplierButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 11);
                addSupplierButton.Click += (sender, args) => addNewSupplier(sender, args, databaseConnection);

            changeSupplierButton = new Button
                { Parent = mainWin, Width = 100, Height = 60, Top = supplierTop * 5 + 10, Left = 530, 
                    Text = "Beszállító \nmódosítása", Enabled = false
                };
                changeSupplierButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 11);

            deleteSupplierButton = new Button
                { Parent = mainWin, Width = 100, Height = 60, Top = supplierTop * 5 + 10, Left = 640, 
                    Text = "Beszállító \ntörlése", Enabled = false
                };
                deleteSupplierButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 11);

            showListButton = new Button
                { Parent = mainWin, Width = 100, Height = 60, Top = supplierTop * 8, Left = 540, Text = "Lista megjelenítése", };
                showListButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 11);
                showListButton.Click += (sender, args) => { };

            saveButton = new Button
                { Parent = mainWin, Width = 150, Height = 30, Top = 360, Left = 430, Text = "Beszállítás mentése", Enabled = false};
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
            addFruitPanel.Visible = true;
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
                    10,  i * 60 , fruits[i], selectElementsPanel);
                selectElements.Add(fruitSelectElement);
                Button delButton = new Button
                {
                    Parent = selectElementsPanel,
                    Width = 30,
                    Height = 30,
                    Top = i * 60,
                    Left = 340,
                    Tag = i,
                    Text = "X",
                };
                delButton.Click += (senderBtn, args) => DeleteFruitButton_Click(senderBtn, args, databaseConnection, supplier);
            }
            writeSupplierData(supplier);
            saveButton.Click += (senderBtn, args) => saveButton_Click(senderBtn, args, databaseConnection);
        }

        private void writeSupplierData(Supplier supplier)
        {
            inName.Text = supplier.Name;
            inCity.Text = supplier.City;
            inZipCode.Text = supplier.ZipCode;
            inStreet.Text = supplier.Street;
            inCounty.Text = supplier.County;
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
                AddError();
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

        private void saveButton_Click(object sender, EventArgs e, DatabaseConnection databaseConnection)
        {
            foreach (var selectElement in selectElements)
            {
                int[] products = selectElement.Products;
                if (products[1] != 0)
                {
                    if (databaseConnection.SupplyArrivalsTable.Insert(
                            new SupplyArrival
                            {
                                ArrivalTime = DateTime.Now,
                                SupplierId = _supplierDropdown.SelectedIndex,
                                ProductId = products[0],
                                Quantity = products[1]

                            })
                        )
                    {
                        MessageBox.Show("Sikeresen mentve!", "Sikeres", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        selectElement.quantity.Value = 0;
                        selectElement.fruitCheckBox.Checked = false;
                    }
                }
            }
        }

        private void ErrorMsg()
        {
            MessageBox.Show("Nem várt hiba! Kérjük próbálja meg újra!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void AddError()
        {
            MessageBox.Show("Minden mezőt közelező kitölteni!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void AddFruitButton_Click(object sender, EventArgs e, DatabaseConnection databaseConnection)
        {
            if (inFruitCategory.Text == "" || inFruitName.Text == "" || inFruitPrice.Text == "")
            {
                AddError();
            }
            else
            {
                Product fruit = new Product
                {
                    Category = inFruitCategory.Text,
                    Name = inFruitName.Text,
                    Price =  Convert.ToDouble(inFruitPrice.Text),
                };
                databaseConnection.ProductsTable.Insert(fruit);
                SupplierProductConnection connection = new SupplierProductConnection
                {
                    ProductId = databaseConnection.ProductsTable.TableEntriesCount(),
                    SupplierId = _supplierDropdown.SelectedIndex + 1,
                };
                if (databaseConnection.SupplierProductConnectionTable.Insert(connection))
                {
                    MessageBox.Show("Sikeresen mentve!", "Sikeres", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    ErrorMsg();
                }
            }
        }

        private void DeleteFruitButton_Click(object sender, EventArgs e, DatabaseConnection databaseConnection, Supplier supplier)
        {
            if (MessageBox.Show("Biztos törölni akarja?", "Figyelmeztetés", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                Button delButton = (Button)sender;
                int index = Convert.ToInt32(delButton.Tag);
                SupplierProductConnection connection = new SupplierProductConnection
                {
                    ProductId = databaseConnection.ProductsTable.GetBySupplier(supplier)[index].Id,
                    SupplierId = supplier.Id,
                };
                if(databaseConnection.SupplierProductConnectionTable.Delete(connection))
                {
                    MessageBox.Show("Sikeresen törölve!", "Sikeres", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    SupplierDropdown_SelectedIndexChanged(null, null, databaseConnection, supplier);
                }
                else
                {
                    ErrorMsg();
                }
            }
            else
            {
                return;
            }
        }
    }
}