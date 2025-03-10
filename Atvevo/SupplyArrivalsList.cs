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
        }
        private void BuildForm() {
            _rightMenu.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _rightMenu.Width = 100;
            _rightMenu.Height = Height;
            _rightMenu.BackColor = Color.Blue;

            var timerangeSelectorBtnWidth = 80;
            var timerangeSelectorBtnHeight = 40;
            _selectMonth.Size = new Size(timerangeSelectorBtnWidth, timerangeSelectorBtnHeight);
            _selectMonth.Location = new Point(_rightMenu.Width / 2 - timerangeSelectorBtnWidth / 2, timerangeSelectorBtnHeight * 1 + 20 * 1 + 100);
            _selectMonth.Text = "Hónap";
            _selectMonth.Click += (sender, args) => { _selectedArrivalTimerange = ArrivalTimerange.Month; };
            
            _selectWeek.Size = new Size(timerangeSelectorBtnWidth, timerangeSelectorBtnHeight);
            _selectWeek.Location = new Point(_rightMenu.Width / 2 - timerangeSelectorBtnWidth / 2, timerangeSelectorBtnHeight * 2 + 20 * 2 + 100);
            _selectWeek.Text = "Hét";
            _selectWeek.Click += (sender, args) => { _selectedArrivalTimerange = ArrivalTimerange.Week; };
            
            _selectDay.Size = new Size(timerangeSelectorBtnWidth, timerangeSelectorBtnHeight);
            _selectDay.Location = new Point(_rightMenu.Width / 2 - timerangeSelectorBtnWidth / 2, timerangeSelectorBtnHeight * 3 + 20 * 3 + 100);
            _selectDay.Text = "Nap";
            _selectDay.Click += (sender, args) => { _selectedArrivalTimerange = ArrivalTimerange.Day; };
            
            _selectAll.Size = new Size(timerangeSelectorBtnWidth, timerangeSelectorBtnHeight);
            _selectAll.Location = new Point(_rightMenu.Width / 2 - timerangeSelectorBtnWidth / 2, timerangeSelectorBtnHeight * 4 + 20 * 4 + 100);
            _selectAll.Text = "Összes";
            _selectAll.Click += (sender, args) => { _selectedArrivalTimerange = ArrivalTimerange.All; };

            Controls.Add(_rightMenu);
        }
    }
}