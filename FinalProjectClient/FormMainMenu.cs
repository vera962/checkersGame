using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    public partial class FormMainMenu : Form
    {
        public FormMainMenu()
        {
            InitializeComponent();
        }

        private void btnStartGame_Click(object sender, EventArgs e)
        {
            LoginForm lf = new LoginForm();
            this.Hide();
            lf.ShowDialog();

            this.Show();
            //this.Close();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            FormGameReplay rf = new FormGameReplay();

            this.Hide();
            rf.ShowDialog();

            this.Show();
            //this.Close();

        }

        private void FormMainMenu_Load(object sender, EventArgs e)
        {
        /*    var jsonSerializerSettings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            };
            List<SOLDIER> sol = new List<SOLDIER>()
            {
                SOLDIER.BLACK,
                SOLDIER.BLACK_KING
            };
            List<int> solInt = sol.Select(a => ((int)a)).ToList();
            var json = JsonConvert.SerializeObject(sol, jsonSerializerSettings);

            var movies = JsonConvert.DeserializeObject<List<int>>(json);
            

            List<SOLDIER> solNotInt = movies.Select(a => ((SOLDIER)a)).ToList();
            var x = 5;*/

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            HowToPlayDamka lf = new HowToPlayDamka();
            this.Hide();
            lf.ShowDialog();

            this.Show();
            //this.Close();

        }
    }
}
