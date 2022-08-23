using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FinalProjectClient
{
    public partial class FormGameReplay : Form
    {
        public FormGameReplay()
        {
            InitializeComponent();
        }
        private BindingSource TblBindingSource = new BindingSource();

        private GAMEDBDataContext db = new GAMEDBDataContext();

        private void TblDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            var gameid = TblDataGridView.Rows[e.RowIndex].Cells[0].Value;

            this.Hide();
            Form1 f1 = new Form1((int)gameid);
            f1.ShowDialog();

            this.Close();
        }

        private void FormGameReplay_Load(object sender, EventArgs e)
        {
            TblBindingSource.DataSource = db.TblGames;

            var newGameId = db.TblGames.OrderByDescending(x => x.GameId).First().GameId + 1;


            TblDataGridView.DataSource = TblBindingSource;
            bindingNavigator.BindingSource = TblBindingSource;
        }

        private void TblDataGridView_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
