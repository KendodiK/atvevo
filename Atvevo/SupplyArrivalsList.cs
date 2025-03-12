using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Atvevo.db;

namespace Atvevo {
    enum ArrivalTimeRange { Day, Week, Month, All }
    public partial class SupplyArrivalsList : Form {
        private readonly DatabaseConnection _databaseConnection;

        private readonly Label _noContent = new Label();
        private readonly FlowLayoutPanel _list = new FlowLayoutPanel();
        private readonly Panel _rightMenu = new Panel();
        private readonly Button _selectDay = new Button();
        private readonly Button _selectWeek = new Button();
        private readonly Button _selectMonth = new Button();
        private readonly Button _selectAll = new Button();
        public SupplyArrivalsList(DatabaseConnection databaseConnection) {
            _databaseConnection = databaseConnection;
            InitializeComponent();
            MinimumSize = new Size(600, 400);
            BuildMenu();
            BuildList(_databaseConnection.SupplyArrivalsTable.GetByArrivalTime());
            Resize += ResizeForm;
        }
        private void BuildList(SupplyArrival[] listItems) {
            _list.Size = new Size(Width - _rightMenu.Width - 20, Height);
            _list.FlowDirection = FlowDirection.TopDown;
            _list.VerticalScroll.Enabled = true;

            if (listItems.Length > 0) {
                var firstDate = listItems.Select(x => x.ArrivalTime).ToArray()[0];
                var year = firstDate.Year;
                var month = firstDate.Month;
                var day = firstDate.Day;
                _list.Controls.Add(ListItemNextDate(firstDate));
                for (int i = 0; i < listItems.Length; i++) {
                    _list.Controls.Add(ListItem(listItems[i], i));
                    if (listItems[i].ArrivalTime.Year != year || listItems[i].ArrivalTime.Month != month || listItems[i].ArrivalTime.Day != day) {
                        year = listItems[i].ArrivalTime.Year;
                        month = listItems[i].ArrivalTime.Month;
                        day = listItems[i].ArrivalTime.Day;
                        _list.Controls.Add(ListItemNextDate(listItems[i].ArrivalTime));
                    }
                }
                Controls.Add(_list);
            }
            else {
                _noContent.Size = new Size(Width - _rightMenu.Width - 20, Height);
                _noContent.TextAlign = ContentAlignment.MiddleCenter;
                _noContent.Text = "NEM VOLT BEVÉTELEZÉS AZ ADATT IDŐINTERVALLUMBAN";
                _noContent.Font = new Font(FontFamily.GenericSansSerif, 15);
                Controls.Add(_noContent);
            }
        }
        private Panel ListItem(SupplyArrival item, int index) {
            TableLayoutPanel panel = new TableLayoutPanel();
            panel.Dock = DockStyle.Fill;
            panel.ColumnCount = 4;
            panel.RowCount = 1;
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12));
            panel.AutoSize = true;
            panel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panel.Font = new Font(FontFamily.GenericSansSerif, 13);
            if (index % 2 == 0) {
                panel.BackColor = Color.White;
            }
            else {
                panel.BackColor = Color.Lavender;
            }
            Label supplier = new Label() {
                Text = _databaseConnection.SuppliersTable.Read()[item.SupplierId].Name,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 0),
                Height = 50,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.Black
            };
            panel.Controls.Add(supplier,0, 0);
            Label product = new Label() {
                Text = _databaseConnection.ProductsTable.Read()[item.ProductId].Name,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 0),
                Height = 50,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.Black
            };
            panel.Controls.Add(product, 1, 0);
            Label arrivalTime = new Label {
                Text = item.ArrivalTime.ToString("HH:mm:ss"),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 0),
                Height = 50,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.Black
            };
            panel.Controls.Add(arrivalTime, 2, 0);
            Label quantity = new Label {
                Text = item.Quantity + " kg",
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 0),
                Height = 50,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.Black
            };
            panel.Controls.Add(quantity, 3, 0);
            _list.SizeChanged += (object sender, EventArgs e) => {
                panel.Width = _list.Width;
            };
            return panel;
        }
        private Panel ListItemNextDate(DateTime date) {
            Panel panel = new Panel {
                Size = new Size(_list.Width, 30),
                Location = new Point(0, 0),
                ForeColor = Color.Black,
                BackColor = Color.Plum
            };
            Label dateLabel = new Label {
                Text = date.ToString("yyyy. m. d dddd"),
                Font = new Font(FontFamily.GenericSansSerif, 13),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 0),
                Height = 30,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.Black
            };
            panel.Controls.Add(dateLabel);
            _list.SizeChanged += (object sender, EventArgs e) => {
                panel.Width = _list.Width;
            };
            return panel;
        }
        private void BuildMenu() {
            _rightMenu.Dock = DockStyle.Right;
            _rightMenu.Width = 100;
            _rightMenu.Height = Height;
            _rightMenu.BackColor = Color.Lavender;
            _rightMenu.Font = new Font(FontFamily.GenericSansSerif, 12);

            _selectMonth.Tag = ArrivalTimeRange.Month;
            _selectMonth.Size = new Size(80, 40);
            _selectMonth.Location = new Point(_rightMenu.Width / 2 - 80 / 2, (int)(Height * 0.24));
            _selectMonth.Text = "Hónap";
            _selectMonth.FlatStyle = FlatStyle.Flat;
            _selectMonth.BackColor = Color.Indigo;
            _selectMonth.ForeColor = Color.Lavender;
            _selectMonth.Click += OnListFilterChanged;
            _rightMenu.Controls.Add(_selectMonth);

            _selectWeek.Tag = ArrivalTimeRange.Week;
            _selectWeek.Size = new Size(80, 40);
            _selectWeek.Location = new Point(_rightMenu.Width / 2 - 80 / 2, (int)(Height * 0.24) + 50);
            _selectWeek.Text = "Hét";
            _selectWeek.FlatStyle = FlatStyle.Flat;
            _selectWeek.BackColor = Color.Indigo;
            _selectWeek.ForeColor = Color.Lavender;
            _selectWeek.Click += OnListFilterChanged;
            _rightMenu.Controls.Add(_selectWeek);

            _selectDay.Tag = ArrivalTimeRange.Day;
            _selectDay.Size = new Size(80, 40);
            _selectDay.Location = new Point(_rightMenu.Width / 2 - 80 / 2, (int)(Height * 0.24) + 100);
            _selectDay.Text = "Nap";
            _selectDay.FlatStyle = FlatStyle.Flat;
            _selectDay.BackColor = Color.Indigo;
            _selectDay.ForeColor = Color.Lavender;
            _selectDay.Click += OnListFilterChanged;
            _rightMenu.Controls.Add(_selectDay);

            _selectAll.Tag = ArrivalTimeRange.All;
            _selectAll.Size = new Size(80, 40);
            _selectAll.Location = new Point(_rightMenu.Width / 2 - 80 / 2, (int)(Height * 0.24) + 150);
            _selectAll.Text = "Összes";
            _selectAll.FlatStyle = FlatStyle.Flat;
            _selectAll.BackColor = Color.Indigo;
            _selectAll.ForeColor = Color.Lavender;
            _selectAll.Click += OnListFilterChanged;
            _rightMenu.Controls.Add(_selectAll);

            Controls.Add(_rightMenu);
        }
        private void OnListFilterChanged(object sender, EventArgs e) {
            var btn = (Button)sender;
            _list.Controls.Clear();
            Controls.Remove(_list);
            Controls.Remove(_noContent);
            var unixTimeRange = TimeRangeHelper((ArrivalTimeRange)btn.Tag).ToUnixTimestamp();
            BuildList(_databaseConnection.SupplyArrivalsTable.GetByArrivalTime(unixTimeRange));
        }
        private DateTime TimeRangeHelper(ArrivalTimeRange timeRange) {
            var current = DateTime.Now;
            switch (timeRange) {
                case ArrivalTimeRange.Day:
                    return current.Subtract(TimeSpan.FromDays(1));
                case ArrivalTimeRange.Week:
                    return current.Subtract(TimeSpan.FromDays(7));
                case ArrivalTimeRange.Month:
                    return current.Subtract(TimeSpan.FromDays(30));
                case ArrivalTimeRange.All:
                    return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);
                default:
                    throw new ArgumentOutOfRangeException(nameof(timeRange), timeRange, null);
            }
        }
        private void ResizeForm(object sender, EventArgs e) {
            _rightMenu.Width = 100;
            _selectMonth.Location = new Point(_rightMenu.Width / 2 - 80 / 2, (int)(Height * 0.24));
            _selectWeek.Location = new Point(_rightMenu.Width / 2 - 80 / 2, (int)(Height * 0.24) + 50);
            _selectDay.Location = new Point(_rightMenu.Width / 2 - 80 / 2, (int)(Height * 0.24) + 100);
            _selectAll.Location = new Point(_rightMenu.Width / 2 - 80 / 2, (int)(Height * 0.24) + 150);
            
            _list.Width = Width - _rightMenu.Width - 20;
        }
    }
}