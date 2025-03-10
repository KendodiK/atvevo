using System;
using System.Drawing;
using System.Windows.Forms;
using Atvevo.db;

namespace Atvevo {
    enum ArrivalTimerange { Day, Week, Month, All }
    public partial class SupplyArrivalsList : Form {
        private Panel _rightMenu = new Panel();
        private ArrivalTimerange _selectedArrivalTimerange = ArrivalTimerange.All;
        private Button _selectDay = new Button();
        private Button _selectWeek = new Button();
        private Button _selectMonth = new Button();
        private Button _selectAll = new Button();
        
        public SupplyArrivalsList(Supplier suppliers) {
            InitializeComponent();
            BuildForm();
            SizeChanged += ResizeForm;
        }
        private void BuildForm() {
            _rightMenu.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _rightMenu.Width = 100;
            _rightMenu.Height = Height;
            _rightMenu.BackColor = Color.Blue;

            _selectMonth.Size = new Size(80, 40);
            _selectMonth.Location = new Point(_rightMenu.Width / 2 - 80 / 2, 40 * 1 + 20 * 1 + 100);
            _selectMonth.Text = "Hónap";
            _selectMonth.Click += (sender, args) => { _selectedArrivalTimerange = ArrivalTimerange.Month; };
            _rightMenu.Controls.Add(_selectMonth);

            _selectWeek.Size = new Size(80, 40);
            _selectWeek.Location = new Point(_rightMenu.Width / 2 - 80 / 2, 40 * 2 + 20 * 2 + 100);
            _selectWeek.Text = "Hét";
            _selectWeek.Click += (sender, args) => { _selectedArrivalTimerange = ArrivalTimerange.Week; };
            _rightMenu.Controls.Add(_selectWeek);
            
            _selectDay.Size = new Size(80, 40);
            _selectDay.Location = new Point(_rightMenu.Width / 2 - 80 / 2, 40 * 3 + 20 * 3 + 100);
            _selectDay.Text = "Nap";
            _selectDay.Click += (sender, args) => { _selectedArrivalTimerange = ArrivalTimerange.Day; };
            _rightMenu.Controls.Add(_selectDay);
            
            _selectAll.Size = new Size(80, 40);
            _selectAll.Location = new Point(_rightMenu.Width / 2 - 80 / 2, 40 * 5 + 20 * 5 + 100);
            _selectAll.Text = "Összes";
            _selectAll.Click += (sender, args) => { _selectedArrivalTimerange = ArrivalTimerange.All; };
            _rightMenu.Controls.Add(_selectAll);

            Controls.Add(_rightMenu);
        }
        private void ResizeForm(object sender, EventArgs e) {
            _selectMonth.Location = new Point(_rightMenu.Width / 2 - 80 / 2, 40 * 1 + 20 * 1 + 100);
            _selectWeek.Location = new Point(_rightMenu.Width / 2 - 80 / 2, 40 * 2 + 20 * 2 + 100);
            _selectDay.Location = new Point(_rightMenu.Width / 2 - 80 / 2, 40 * 3 + 20 * 3 + 100);
            _selectAll.Location = new Point(_rightMenu.Width / 2 - 80 / 2, 40 * 5 + 20 * 5 + 100);
        }
    }
}