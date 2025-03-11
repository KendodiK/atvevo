using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Atvevo.db;

namespace Atvevo {
    enum ArrivalTimeRange { Day, Week, Month, All }
    public partial class SupplyArrivalsList : Form {
        private DatabaseConnection _databaseConnection;
        
        private FlowLayoutPanel _list = new FlowLayoutPanel();
        private Panel _rightMenu = new Panel();
        private Button _selectDay = new Button();
        private Button _selectWeek = new Button();
        private Button _selectMonth = new Button();
        private Button _selectAll = new Button();
        public SupplyArrivalsList(DatabaseConnection databaseConnection) {
            _databaseConnection = databaseConnection;
            InitializeComponent();
            MinimumSize = new Size(700, 400);
            BuildMenu();
            BuildList(_databaseConnection.SupplyArrivalsTable.GetByArrivalTime());
            Resize += ResizeForm;
        }
        private void BuildList(SupplyArrival[] listItems) {
            _list.Dock = DockStyle.Fill;
            _list.BackColor = Color.Crimson;
            _list.FlowDirection = FlowDirection.TopDown;
            _list.AutoScroll = true;

            if (listItems.Length > 0) {
                var firstDate = listItems.Select(x => x.ArrivalTime).ToArray()[0];
                var year = firstDate.Year;
                var month = firstDate.Month;
                var day = firstDate.Day;
                _list.Controls.Add(ListItemNextDate(firstDate));
                for (int i = 0; i < listItems.Length; i++) {
                    _list.Controls.Add(ListItem(listItems[i], _list.Width, i));
                    if (listItems[i].ArrivalTime.Year != year || listItems[i].ArrivalTime.Month != month || listItems[i].ArrivalTime.Day != day) {
                        year = listItems[i].ArrivalTime.Year;
                        month = listItems[i].ArrivalTime.Month;
                        day = listItems[i].ArrivalTime.Day;
                        _list.Controls.Add(ListItemNextDate(listItems[i].ArrivalTime));
                    }
                }
            }
        }
        private Panel ListItem(SupplyArrival item, int width, int index) {
            FlowLayoutPanel panel = new FlowLayoutPanel();
            panel.Size = new Size(width, 100);
            panel.FlowDirection = FlowDirection.LeftToRight;
            if (index % 2 == 0) {
                panel.BackColor = Color.White;
            }
            else {
                panel.BackColor = Color.DimGray;
            }
            Label supplier = new Label() {
                Text = _databaseConnection.SuppliersTable.Read()[item.SupplierId].Name,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(width / 5, 100),
                Location = new Point(0, 0),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.Black
            };
            panel.Controls.Add(supplier);
            Label product = new Label() {
                Text = _databaseConnection.ProductsTable.Read()[item.ProductId].Name,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(width / 5, 100),
                Location = new Point(0, 0),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.Black
            };
            panel.Controls.Add(product);
            Label arrivalTime = new Label {
                Text = item.ArrivalTime.ToString("yyyy. M. dddd, HH:mm"),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(width / 5, 100),
                Location = new Point(0, 0),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.Black
            };
            panel.Controls.Add(arrivalTime);
            Label quantity = new Label {
                Text = item.Quantity.ToString() + " kg",
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(width / 5, 100),
                Location = new Point(0, 0),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.Black
            };
            return panel;
        }
        private Panel ListItemNextDate(DateTime date) {
            return new Panel{BackColor = Color.Green};
        }
        private void BuildMenu() {
            _rightMenu.Dock = DockStyle.Right;
            _rightMenu.Width = 100;
            _rightMenu.Height = Height;
            _rightMenu.BackColor = Color.Blue;

            _selectMonth.Tag = ArrivalTimeRange.Month;
            _selectMonth.Size = new Size(80, 40);
            _selectMonth.Location = new Point(_rightMenu.Width / 2 - 80 / 2, (int)(Height * 0.24));
            _selectMonth.Text = "Hónap";
            _selectMonth.Click += OnListFilterChanged;
            _rightMenu.Controls.Add(_selectMonth);

            _selectWeek.Tag = ArrivalTimeRange.Week;
            _selectWeek.Size = new Size(80, 40);
            _selectWeek.Location = new Point(_rightMenu.Width / 2 - 80 / 2, (int)(Height * 0.24) + 50);
            _selectWeek.Text = "Hét";
            _selectWeek.Click += OnListFilterChanged;
            _rightMenu.Controls.Add(_selectWeek);

            _selectDay.Tag = ArrivalTimeRange.Day;
            _selectDay.Size = new Size(80, 40);
            _selectDay.Location = new Point(_rightMenu.Width / 2 - 80 / 2, (int)(Height * 0.24) + 100);
            _selectDay.Text = "Nap";
            _selectDay.Click += OnListFilterChanged;
            _rightMenu.Controls.Add(_selectDay);

            _selectAll.Tag = ArrivalTimeRange.All;
            _selectAll.Size = new Size(80, 40);
            _selectAll.Location = new Point(_rightMenu.Width / 2 - 80 / 2, (int)(Height * 0.24) + 150);
            _selectAll.Text = "Összes";
            _selectAll.Click += OnListFilterChanged;
            _rightMenu.Controls.Add(_selectAll);

            Controls.Add(_rightMenu);
        }
        private void OnListFilterChanged(object sender, EventArgs e) {
            var btn = (Button)sender;
            Controls.Remove(_list);
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
        }
    }
}