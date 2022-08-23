
namespace FinalProjectClient
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dgvBoard = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.lblPlayer = new System.Windows.Forms.Label();
            this.lblServer = new System.Windows.Forms.Label();
            this.pbrobot = new System.Windows.Forms.PictureBox();
            this.btnSurrender = new System.Windows.Forms.Button();
            this.pbQuiter = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBoard)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbrobot)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbQuiter)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvBoard
            // 
            this.dgvBoard.AllowUserToAddRows = false;
            this.dgvBoard.AllowUserToDeleteRows = false;
            this.dgvBoard.BackgroundColor = System.Drawing.Color.Firebrick;
            this.dgvBoard.ColumnHeadersHeight = 29;
            this.dgvBoard.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvBoard.GridColor = System.Drawing.Color.Firebrick;
            this.dgvBoard.Location = new System.Drawing.Point(239, 32);
            this.dgvBoard.Margin = new System.Windows.Forms.Padding(2);
            this.dgvBoard.Name = "dgvBoard";
            this.dgvBoard.ReadOnly = true;
            this.dgvBoard.RowHeadersWidth = 62;
            this.dgvBoard.RowTemplate.Height = 28;
            this.dgvBoard.Size = new System.Drawing.Size(500, 500);
            this.dgvBoard.TabIndex = 0;
            this.dgvBoard.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvBoard_CellContentClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label1.Location = new System.Drawing.Point(55, 188);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "the turn is:";
            // 
            // lblPlayer
            // 
            this.lblPlayer.AutoSize = true;
            this.lblPlayer.BackColor = System.Drawing.Color.DarkGray;
            this.lblPlayer.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.lblPlayer.Location = new System.Drawing.Point(40, 215);
            this.lblPlayer.Name = "lblPlayer";
            this.lblPlayer.Size = new System.Drawing.Size(61, 20);
            this.lblPlayer.TabIndex = 3;
            this.lblPlayer.Text = "WHITE";
            // 
            // lblServer
            // 
            this.lblServer.AutoSize = true;
            this.lblServer.BackColor = System.Drawing.Color.DarkGray;
            this.lblServer.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.lblServer.Location = new System.Drawing.Point(120, 215);
            this.lblServer.Name = "lblServer";
            this.lblServer.Size = new System.Drawing.Size(61, 20);
            this.lblServer.TabIndex = 4;
            this.lblServer.Text = "BLACK";
            // 
            // pbrobot
            // 
            this.pbrobot.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pbrobot.InitialImage = null;
            this.pbrobot.Location = new System.Drawing.Point(68, 110);
            this.pbrobot.Margin = new System.Windows.Forms.Padding(2);
            this.pbrobot.Name = "pbrobot";
            this.pbrobot.Size = new System.Drawing.Size(104, 73);
            this.pbrobot.TabIndex = 5;
            this.pbrobot.TabStop = false;
            this.pbrobot.Visible = false;
            // 
            // btnSurrender
            // 
            this.btnSurrender.BackColor = System.Drawing.Color.LightGreen;
            this.btnSurrender.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSurrender.Location = new System.Drawing.Point(50, 422);
            this.btnSurrender.Name = "btnSurrender";
            this.btnSurrender.Size = new System.Drawing.Size(122, 43);
            this.btnSurrender.TabIndex = 6;
            this.btnSurrender.Text = "SURRENDER";
            this.btnSurrender.UseVisualStyleBackColor = false;
            this.btnSurrender.Click += new System.EventHandler(this.btnSurrender_Click);
            // 
            // pbQuiter
            // 
            this.pbQuiter.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pbQuiter.Image = global::FinalProjectClient.Properties.Resources.ko_gameover;
            this.pbQuiter.Location = new System.Drawing.Point(283, 143);
            this.pbQuiter.Name = "pbQuiter";
            this.pbQuiter.Size = new System.Drawing.Size(392, 277);
            this.pbQuiter.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbQuiter.TabIndex = 7;
            this.pbQuiter.TabStop = false;
            this.pbQuiter.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::FinalProjectClient.Properties.Resources.checkersBOARD2_NEW;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(914, 558);
            this.Controls.Add(this.pbQuiter);
            this.Controls.Add(this.btnSurrender);
            this.Controls.Add(this.pbrobot);
            this.Controls.Add(this.lblServer);
            this.Controls.Add(this.lblPlayer);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dgvBoard);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBoard)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbrobot)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbQuiter)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvBoard;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblPlayer;
        private System.Windows.Forms.Label lblServer;
        private System.Windows.Forms.PictureBox pbrobot;
        private System.Windows.Forms.Button btnSurrender;
        private System.Windows.Forms.PictureBox pbQuiter;
    }
}

