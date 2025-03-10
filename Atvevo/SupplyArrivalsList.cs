using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Atvevo.db;

namespace Atvevo {
    enum ArrivalTimeRange { Day, Week, Month, All }
    public partial class SupplyArrivalsList : Form {
        private DatabaseConnection _databaseConnection;
        
        private Panel _list = new Panel();
        private Panel _rightMenu = new Panel();
        private ArrivalTimeRange _selectedArrivalTimeRange = ArrivalTimeRange.All;
        private Button _selectDay = new Button();
        private Button _selectWeek = new Button();
        private Button _selectMonth = new Button();
        private Button _selectAll = new Button();
        
        public SupplyArrivalsList(Supplier suppliers, DatabaseConnection databaseConnection) {
            InitializeComponent();
            BuildMenu();
            SizeChanged += ResizeForm;
        }
        private void BuildList(SupplyArrival[] listItems) {
            
        }
        private void BuildMenu() {
            _rightMenu.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _rightMenu.Width = 100;
            _rightMenu.Height = Height;
            _rightMenu.BackColor = Color.Blue;

            _selectMonth.Tag = ArrivalTimeRange.Month;
            _selectMonth.Size = new Size(80, 40);
            _selectMonth.Location = new Point(_rightMenu.Width / 2 - 80 / 2, 40 * 1 + 20 * 1 + 100);
            _selectMonth.Text = "Hónap";
            _selectMonth.Click += (sender, args) => { _selectedArrivalTimeRange = ArrivalTimeRange.Month; };
            _rightMenu.Controls.Add(_selectMonth);

            _selectWeek.Tag = ArrivalTimeRange.Week;
            _selectWeek.Size = new Size(80, 40);
            _selectWeek.Location = new Point(_rightMenu.Width / 2 - 80 / 2, 40 * 2 + 20 * 2 + 100);
            _selectWeek.Text = "Hét";
            _selectWeek.Click += (sender, args) => { _selectedArrivalTimeRange = ArrivalTimeRange.Week; };
            _rightMenu.Controls.Add(_selectWeek);

            _selectDay.Tag = ArrivalTimeRange.Day;
            _selectDay.Size = new Size(80, 40);
            _selectDay.Location = new Point(_rightMenu.Width / 2 - 80 / 2, 40 * 3 + 20 * 3 + 100);
            _selectDay.Text = "Nap";
            _selectDay.Click += (sender, args) => { _selectedArrivalTimeRange = ArrivalTimeRange.Day; };
            _rightMenu.Controls.Add(_selectDay);

            _selectAll.Tag = ArrivalTimeRange.All;
            _selectAll.Size = new Size(80, 40);
            _selectAll.Location = new Point(_rightMenu.Width / 2 - 80 / 2, 40 * 5 + 20 * 5 + 100);
            _selectAll.Text = "Összes";
            _selectAll.Click += OnListFilterChanged;
            _rightMenu.Controls.Add(_selectAll);

            Controls.Add(_rightMenu);
        }
        private void OnListFilterChanged(object sender, EventArgs e) {
            var btn = (Button)sender;
            Controls.Remove(_list);
            var unixTimeRange = TimeRangeHelper((ArrivalTimeRange)btn.Tag).ToUnixTimestamp();
            BuildList(_databaseConnection.SupplyArrivalsTable.GetBySupplierAndArrivalTime(unixTimeRange));
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
            _selectMonth.Location = new Point(_rightMenu.Width / 2 - 80 / 2, 40 * 1 + 20 * 1 + 100);
            _selectWeek.Location = new Point(_rightMenu.Width / 2 - 80 / 2, 40 * 2 + 20 * 2 + 100);
            _selectDay.Location = new Point(_rightMenu.Width / 2 - 80 / 2, 40 * 3 + 20 * 3 + 100);
            _selectAll.Location = new Point(_rightMenu.Width / 2 - 80 / 2, 40 * 5 + 20 * 5 + 100);
        }
    }
}