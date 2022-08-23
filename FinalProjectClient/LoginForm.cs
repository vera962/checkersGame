using FinalProjectClient.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FinalProjectClient
{
    public partial class LoginForm : Form
    {


        public string UserName { get; set; }
        public string UserPassword { get;set; }
     
       
        public LoginForm()
        {
            InitializeComponent();
          
    }
       
        private HttpClient client = new HttpClient();
        private void LoginForm_Load(object sender, EventArgs e)
        {
            client.BaseAddress = new Uri("https://localhost:44332/");

           
        }

        async Task<Users> GetProductAsync(string path)
        {
            Users users = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                users = await response.Content.ReadAsAsync<Users>();
            }
            return users;
        }


      
        private void label1_Click(object sender, EventArgs e)
        {

        }

      

        private void LoginForm_Paint(object sender, PaintEventArgs e)
        {
            
           SolidBrush p = new SolidBrush(Color.FromArgb(180, 255, 255, 255));

           // Pen p = new Pen(Color.FromArgb(180, 255, 255, 255));
            e.Graphics.FillRectangle(p, 120, 70, 200,350);
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Link_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                VisitLink();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to open link that was clicked. Error Message: " + ex.Message );
            }
        }

        private void VisitLink()
        {
            // Change the color of the link text by setting LinkVisited
            // to true.
            Link.LinkVisited = true;
            //Call the Process.Start method to open the default browser
            //with a URL:
            System.Diagnostics.Process.Start("https://localhost:44332/UsersDb/Create");
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            UserName =NameTxtBox.Text;
            UserPassword = PasswordTxtBox.Text;
            Users User = await GetProductAsync("api/userInfo/" + UserPassword);
            if (User != null)
            {
                if (User.Name == UserName)
                {
                    this.Hide();
                    Form1 form1 = new Form1
                    {
                        UserId = User.Id,
                        UserName = User.Name
                    };
                    form1.ShowDialog();
                }
                else MessageBox.Show("User Or Password does not exist");
            }
            else MessageBox.Show("User Or Password does not exist");
           

        }
    }
}
