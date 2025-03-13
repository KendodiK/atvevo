using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
                saveButton.BackColor = Color.Indigo;
                products[0] = fruit.Id;
                products[1] = (int)quantity.Value;
            }
        }
        
        private static ComboBox _supplierDropdown;
        private static ComboBox _fruitDropdown;
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

        private DatabaseConnection _databaseConnection;
        private Supplier _activeSupplier;
        public MainWindow()
        {
            InitializeComponent();
            Text = "Átvétel";
            _databaseConnection = new DatabaseConnection(true);
            Font = new System.Drawing.Font("Microsoft Sans Serif", 13);
            Width = 800;
            Height = 550;
            BackColor = Color.Lavender;
            BuildForm(this);
            FormClosing += (sender, args) => { _databaseConnection.DatabaseDisconnect(); };
        }

        private void BuildForm(Form mainWin)
        {
            _supplierDropdown = new ComboBox { Parent = mainWin, Width = 130, Height = 20, Top = 10, Left = 20, DropDownStyle = ComboBoxStyle.DropDownList, };
            selectElementsPanel = new Panel { Parent = mainWin, Width = 390, Height = 300, Top = 210, Left = 20, AutoScroll = true};
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

            addFruitPanel = new Panel { Parent = mainWin, Width = 380, Height = 160, Top = 40, Left = 20, Visible = false};
            Label fruitName = new Label
                { Parent = addFruitPanel, Width = 50, Height = 25, Left = 30, Text = "Fajta",};
            Label fruitCategory = new Label
                { Parent = addFruitPanel, Width = 90, Left = 120, Height = 25, Text = "Gyömölcs típus",};
            Label price = new Label 
                { Parent = addFruitPanel, Width = 70, Left = 210, Height = 25, Text = "Kilóár",};
            inFruitCategory = new TextBox { Parent = addFruitPanel, Width = 120, Top = 30, Tag = ""};
            inFruitName = new TextBox { Parent = addFruitPanel, Width = 70, Left = 130, Top = 30, };
            inFruitPrice = new TextBox { Parent = addFruitPanel, Width = 50, Left = 210, Top = 30, };
            
            _fruitDropdown = new ComboBox
                { Parent = addFruitPanel, Width = 270, Height = 20, Top = 70, DropDownStyle = ComboBoxStyle.DropDownList,};
            var fruits = _databaseConnection.ProductsTable.Read();
            int i = 0;
            _fruitDropdown.Items.Add("Már rögzített fajta kiválasztása");
            _fruitDropdown.SelectedIndex = 0;
            foreach (Product fruitElement in fruits)
            {
                _fruitDropdown.Items.Add(fruitElement.Name + $" ({fruitElement.Category})");
                i++;
            }
            _fruitDropdown.SelectedIndexChanged += (sender, args) => FruitDropdown_SelectedIndexChanged(sender, args, fruits);
            addFruitButton = new Button 
                { Parent = addFruitPanel, Width = 200, Height = 30, Left = 20, Top = 110, Text = "Gyümölcs hozzáadása",
                    FlatStyle = FlatStyle.Flat,  BackColor = Color.Indigo, ForeColor = Color.Lavender,};
                addFruitButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 11);
                addFruitButton.Click += (sender, args) => AddFruitButton_Click(sender, args);

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

            changeSupplierButton = new Button
                { Parent = mainWin, Width = 100, Height = 60, Top = supplierTop * 5 + 10, Left = 530, 
                    Text = "Beszállító \nmódosítása", BackColor = Color.FromArgb(214, 157, 255), ForeColor = Color.Lavender,
                    FlatStyle = FlatStyle.Flat, Enabled = false
                };
                changeSupplierButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 11);

            deleteSupplierButton = new Button
                { Parent = mainWin, Width = 100, Height = 60, Top = supplierTop * 5 + 10, Left = 640, 
                    Text = "Beszállító \ntörlése", BackColor = Color.FromArgb(214, 157, 255), 
                    FlatStyle = FlatStyle.Flat, ForeColor = Color.Lavender, Enabled = false
                };
                deleteSupplierButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 11);

            showListButton = new Button
                { Parent = mainWin, Width = 100, Height = 60, Top = supplierTop * 8, Left = 540, Text = "Lista megjelenítése",
                    FlatStyle = FlatStyle.Flat,  BackColor = Color.Indigo, ForeColor = Color.Lavender};
                showListButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 11);
                showListButton.Click += ((sender, args) => {
                    Task.Run(() => {
                        Application.Run(new SupplyArrivalsList(_databaseConnection));
                    });
                });

            saveButton = new Button
                { Parent = mainWin, Width = 150, Height = 30, Top = 10, Left = 160, Text = "Beszállítás mentése", FlatStyle = FlatStyle.Flat, 
                  BackColor  = Color.FromArgb(214, 157, 255), ForeColor = Color.Lavender, Enabled = false};
                saveButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 11);
                
            var suppliers = _databaseConnection.SuppliersTable.Read();
            string[] suppliersNames = new string[suppliers.Length];
            i = 0;
            _supplierDropdown.Items.Add("---------");
            _supplierDropdown.SelectedIndex = 0;
            foreach (Supplier supplier in suppliers)
            {
                _supplierDropdown.Items.Add(supplier.Name);
                suppliersNames[i] = supplier.Name;
                i++;
            }
            _supplierDropdown.SelectedIndexChanged += 
                (sender, args) => SupplierDropdown_SelectedIndexChanged(
                    sender, args, suppliers[_supplierDropdown.SelectedIndex > 0 ? _supplierDropdown.SelectedIndex - 1 : 0]);
            
            addSupplierButton = new Button
                { Parent = mainWin, Width = 100, Height = 60, Top = supplierTop * 5 + 10, Left = 420, 
                    Text = "Beszállító \nhozzáadása", BackColor = Color.Indigo, ForeColor = Color.Lavender, FlatStyle = FlatStyle.Flat, };
            addSupplierButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 11);
            addSupplierButton.Click += (sender, args) => addNewSupplier(sender, args, suppliersNames);
        }
        private void SupplierDropdown_SelectedIndexChanged(object sender, EventArgs e, Supplier supplier)
        {
            selectElements.Clear();
            selectElementsPanel.Controls.Clear();
            if (_supplierDropdown.Items[_supplierDropdown.SelectedIndex] != "---------")
            {
                _activeSupplier = supplier;
                addFruitPanel.Visible = true;
                addSupplierButton.Enabled = false;
                addSupplierButton.BackColor = Color.FromArgb(214, 157, 255);
                changeSupplierButton.Enabled = true;
                changeSupplierButton.BackColor = Color.Indigo;
                changeSupplierButton.Click += (senderBtn, args) => changeSupplier(senderBtn, args);
                deleteSupplierButton.Enabled = true;
                deleteSupplierButton.BackColor = Color.Indigo;
                deleteSupplierButton.Click += (senderBtn, args) => deleteSupplier(senderBtn, args);
                ComboBox suppliers = (ComboBox)sender;
                Product[] fruits = _databaseConnection.ProductsTable.GetBySupplier(supplier);
                for (int i = 0; i < fruits.Length; i++)
                {
                    FruitSelectElement fruitSelectElement = new FruitSelectElement(
                        10, i * 60, fruits[i], selectElementsPanel);
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
                        FlatStyle = FlatStyle.Flat,
                        BackColor = Color.Indigo,
                        ForeColor = Color.Lavender,
                    };
                    delButton.Click += (senderBtn, args) => DeleteFruitButton_Click(senderBtn, args);
                    writeSupplierData();
                    saveButton.Click += saveButton_Click;
                }
            }
            else
            {
                   clearSupplierData();
                   addFruitPanel.Visible = false;
                   addSupplierButton.Enabled = true;
                   addSupplierButton.BackColor = Color.Indigo;
                   changeSupplierButton.Enabled = false;
                   changeSupplierButton.BackColor = Color.FromArgb(214, 157, 255);
                   deleteSupplierButton.Enabled = false;
                   deleteSupplierButton.BackColor = Color.FromArgb(214, 157, 255);
                   saveButton.Enabled = false;
                   saveButton.BackColor = Color.FromArgb(214, 157, 255);
            }
        }

        private void writeSupplierData()
        {
            inName.Text = _activeSupplier.Name;
            inCity.Text = _activeSupplier.City;
            inZipCode.Text = _activeSupplier.ZipCode;
            inStreet.Text = _activeSupplier.Street;
            inCounty.Text = _activeSupplier.County;
            inHouseNumber.Text = _activeSupplier.HouseNumber.ToString();
            inPhone.Text = _activeSupplier.Phone;
        }

        private void clearSupplierData()
        {
            inName.Text = "";
            inCity.Text = "";
            inZipCode.Text = "";
            inStreet.Text = "";
            inHouseNumber.Text = "";
            inPhone.Text = "";
            inCounty.Text = "";
        }

        private void addNewSupplier(object sender, EventArgs e, string[] suppliersNames)
        {
            bool notEmty = false;
            foreach (string supplierName in suppliersNames)
            {
                if (supplierName == inName.Text)
                {
                    notEmty = true;
                }
            }

            if (notEmty)
            {
                clearSupplierData();
            }
            
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
                if (_databaseConnection.SuppliersTable.Insert(supplier))
                {
                    MessageBox.Show("Sikeresen hozzáadva!", "Sikeres", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _supplierDropdown.Items.Add(supplier.Name);   
                    _supplierDropdown.SelectedIndex = _supplierDropdown.Items.Count;
                }
                else
                {
                    UnknownErrorMsg();
                }
            }
        }

        private void changeSupplier(object sender, EventArgs e)
        {
            if (inName.Text == "" || inCity.Text == "" || inZipCode.Text == "" || inStreet.Text == "" ||
                inHouseNumber.Text == "" || inPhone.Text == "" || inCounty.Text == "")
            {
                AddError();
            }
            else
            {
                _activeSupplier.Name = inName.Text;
                _activeSupplier.City = inCity.Text;
                _activeSupplier.ZipCode = inZipCode.Text;
                _activeSupplier.Street = inStreet.Text;
                _activeSupplier.HouseNumber = Convert.ToByte(inHouseNumber.Text);
                _activeSupplier.Phone = inPhone.Text;

                if (_databaseConnection.SuppliersTable.Update(_activeSupplier))
                {
                    MessageBox.Show("Sikeresen módosítva!", "Sikeres", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    UnknownErrorMsg();
                } 
            }
        }

        private void deleteSupplier(object sender, EventArgs e)
        {
            if (_databaseConnection.SuppliersTable.Delete(_activeSupplier))
            {
                MessageBox.Show("Sikeresen törölve!", "Sikeres", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _supplierDropdown.Items.Remove(_activeSupplier.Name);
                _supplierDropdown.SelectedIndex = 0;
                selectElements.Clear();
                selectElementsPanel.Controls.Clear();
                addFruitPanel.Visible = false;
            }
            else
            {
                UnknownErrorMsg();
            } 
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            foreach (var selectElement in selectElements)
            {
                int[] products = selectElement.Products;
                if (products[1] != 0)
                {
                    if (_databaseConnection.SupplyArrivalsTable.Insert(
                            new SupplyArrival
                            {
                                ArrivalTime = DateTime.Now,
                                SupplierId = _activeSupplier.Id,
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

        private void UnknownErrorMsg()
        {
            MessageBox.Show("Nem várt hiba! Kérjük próbálja meg újra!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void AddError()
        {
            MessageBox.Show("Minden mezőt közelező kitölteni!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void FruitDropdown_SelectedIndexChanged(object sender, EventArgs e, Product[] fruits)
        {
            inFruitCategory.Text = fruits[_fruitDropdown.SelectedIndex - 1].Name;
            inFruitCategory.Tag = fruits[_fruitDropdown.SelectedIndex - 1].Id;
            inFruitName.Text = fruits[_fruitDropdown.SelectedIndex - 1].Category;
            inFruitPrice.Text = fruits[_fruitDropdown.SelectedIndex - 1].Price.ToString();
        }
        
        private void AddFruitButton_Click(object sender, EventArgs e)
        {
            if (inFruitCategory.Text == "" || inFruitName.Text == "" || inFruitPrice.Text == "")
            {
                AddError();
            }
            else
            {
                bool isNew = Convert.ToString(inFruitCategory.Tag) == "";
                Product fruit = new Product
                {
                    Name = inFruitCategory.Text,
                    Category = inFruitName.Text,
                    Price = Convert.ToDouble(inFruitPrice.Text),
                };
                int id;
                if (isNew)
                {
                    _databaseConnection.ProductsTable.Insert(fruit);
                    id = _databaseConnection.ProductsTable.TableEntriesCount();
                }
                else
                {
                    _databaseConnection.ProductsTable.Update(fruit);
                    id = Convert.ToInt32(inFruitCategory.Tag);
                }
                SupplierProductConnection connection = new SupplierProductConnection
                {
                    ProductId = id,
                    SupplierId = _activeSupplier.Id,
                };
                if (_databaseConnection.SupplierProductConnectionTable.Insert(connection))
                {
                    MessageBox.Show("Sikeresen mentve!", "Sikeres", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    SupplierDropdown_SelectedIndexChanged(null, null, _activeSupplier);
                    inFruitCategory.Text = "";
                    inFruitCategory.Tag = "";
                    inFruitName.Text = "";
                    inFruitPrice.Text = "";
                }
                else
                {
                    UnknownErrorMsg();
                }
            }
        }

        private void DeleteFruitButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Biztos törölni akarja?", "Figyelmeztetés", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                Button delButton = (Button)sender;
                int index = Convert.ToInt32(delButton.Tag);
                SupplierProductConnection connection = new SupplierProductConnection
                {
                    ProductId = _databaseConnection.ProductsTable.GetBySupplier(_activeSupplier)[index].Id,
                    SupplierId = _activeSupplier.Id,
                };
                if(_databaseConnection.SupplierProductConnectionTable.Delete(connection))
                {
                    MessageBox.Show("Sikeresen törölve!", "Sikeres", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    SupplierDropdown_SelectedIndexChanged(null, null, _activeSupplier);
                }
                else
                {
                    UnknownErrorMsg();
                }
            }
            else
            {
                return;
            }
        }
    }
}