namespace YemekhaneBakiyeKart
{
    partial class NewUser
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
            this.grupKisiKayit = new System.Windows.Forms.GroupBox();
            this.txtPass_yeni = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtUser_yeni = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.cmbCafeList = new System.Windows.Forms.ComboBox();
            this.btn_Kaydet_yeni = new System.Windows.Forms.Button();
            this.txtSoyad_yeni = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtAd_yeni = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtTckimlik_yeni = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.lblCaps = new System.Windows.Forms.Label();
            this.grupKisiKayit.SuspendLayout();
            this.SuspendLayout();
            // 
            // grupKisiKayit
            // 
            this.grupKisiKayit.Controls.Add(this.lblCaps);
            this.grupKisiKayit.Controls.Add(this.txtPass_yeni);
            this.grupKisiKayit.Controls.Add(this.label1);
            this.grupKisiKayit.Controls.Add(this.txtUser_yeni);
            this.grupKisiKayit.Controls.Add(this.label2);
            this.grupKisiKayit.Controls.Add(this.label25);
            this.grupKisiKayit.Controls.Add(this.cmbCafeList);
            this.grupKisiKayit.Controls.Add(this.btn_Kaydet_yeni);
            this.grupKisiKayit.Controls.Add(this.txtSoyad_yeni);
            this.grupKisiKayit.Controls.Add(this.label7);
            this.grupKisiKayit.Controls.Add(this.txtAd_yeni);
            this.grupKisiKayit.Controls.Add(this.label4);
            this.grupKisiKayit.Controls.Add(this.txtTckimlik_yeni);
            this.grupKisiKayit.Controls.Add(this.label6);
            this.grupKisiKayit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.grupKisiKayit.ForeColor = System.Drawing.Color.Black;
            this.grupKisiKayit.Location = new System.Drawing.Point(12, 12);
            this.grupKisiKayit.Name = "grupKisiKayit";
            this.grupKisiKayit.Size = new System.Drawing.Size(410, 305);
            this.grupKisiKayit.TabIndex = 31;
            this.grupKisiKayit.TabStop = false;
            this.grupKisiKayit.Text = "Kullanıcı Bilgileri";
            // 
            // txtPass_yeni
            // 
            this.txtPass_yeni.Location = new System.Drawing.Point(138, 185);
            this.txtPass_yeni.MaxLength = 30;
            this.txtPass_yeni.Name = "txtPass_yeni";
            this.txtPass_yeni.Size = new System.Drawing.Size(261, 22);
            this.txtPass_yeni.TabIndex = 5;
            this.txtPass_yeni.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPass_yeni_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 188);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 16);
            this.label1.TabIndex = 175;
            this.label1.Text = "Parola (Geçici) :";
            // 
            // txtUser_yeni
            // 
            this.txtUser_yeni.Location = new System.Drawing.Point(138, 149);
            this.txtUser_yeni.MaxLength = 30;
            this.txtUser_yeni.Name = "txtUser_yeni";
            this.txtUser_yeni.Size = new System.Drawing.Size(261, 22);
            this.txtUser_yeni.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 152);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 16);
            this.label2.TabIndex = 174;
            this.label2.Text = "Kullanıcı Adı :";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.label25.Location = new System.Drawing.Point(9, 222);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(98, 16);
            this.label25.TabIndex = 173;
            this.label25.Text = "Yemekhane :";
            // 
            // cmbCafeList
            // 
            this.cmbCafeList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCafeList.FormattingEnabled = true;
            this.cmbCafeList.Location = new System.Drawing.Point(138, 218);
            this.cmbCafeList.Name = "cmbCafeList";
            this.cmbCafeList.Size = new System.Drawing.Size(261, 24);
            this.cmbCafeList.TabIndex = 6;
            // 
            // btn_Kaydet_yeni
            // 
            this.btn_Kaydet_yeni.BackColor = System.Drawing.Color.AliceBlue;
            this.btn_Kaydet_yeni.FlatAppearance.BorderColor = System.Drawing.Color.DodgerBlue;
            this.btn_Kaydet_yeni.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lavender;
            this.btn_Kaydet_yeni.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Kaydet_yeni.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.btn_Kaydet_yeni.ForeColor = System.Drawing.Color.DodgerBlue;
            this.btn_Kaydet_yeni.Location = new System.Drawing.Point(257, 259);
            this.btn_Kaydet_yeni.Name = "btn_Kaydet_yeni";
            this.btn_Kaydet_yeni.Size = new System.Drawing.Size(142, 31);
            this.btn_Kaydet_yeni.TabIndex = 7;
            this.btn_Kaydet_yeni.Text = "Kaydet";
            this.btn_Kaydet_yeni.UseVisualStyleBackColor = false;
            this.btn_Kaydet_yeni.Click += new System.EventHandler(this.btn_Kaydet_yeni_Click);
            // 
            // txtSoyad_yeni
            // 
            this.txtSoyad_yeni.Location = new System.Drawing.Point(138, 112);
            this.txtSoyad_yeni.MaxLength = 30;
            this.txtSoyad_yeni.Name = "txtSoyad_yeni";
            this.txtSoyad_yeni.Size = new System.Drawing.Size(261, 22);
            this.txtSoyad_yeni.TabIndex = 3;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 115);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(61, 16);
            this.label7.TabIndex = 20;
            this.label7.Text = "Soyad :";
            // 
            // txtAd_yeni
            // 
            this.txtAd_yeni.Location = new System.Drawing.Point(138, 76);
            this.txtAd_yeni.MaxLength = 30;
            this.txtAd_yeni.Name = "txtAd_yeni";
            this.txtAd_yeni.Size = new System.Drawing.Size(261, 22);
            this.txtAd_yeni.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 79);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 16);
            this.label4.TabIndex = 14;
            this.label4.Text = "Ad :";
            // 
            // txtTckimlik_yeni
            // 
            this.txtTckimlik_yeni.Location = new System.Drawing.Point(138, 39);
            this.txtTckimlik_yeni.MaxLength = 13;
            this.txtTckimlik_yeni.Name = "txtTckimlik_yeni";
            this.txtTckimlik_yeni.Size = new System.Drawing.Size(261, 22);
            this.txtTckimlik_yeni.TabIndex = 1;
            this.txtTckimlik_yeni.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 44);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(89, 16);
            this.label6.TabIndex = 12;
            this.label6.Text = "T.C. Kimlik :";
            // 
            // lblCaps
            // 
            this.lblCaps.AutoSize = true;
            this.lblCaps.ForeColor = System.Drawing.Color.Firebrick;
            this.lblCaps.Location = new System.Drawing.Point(131, 266);
            this.lblCaps.Name = "lblCaps";
            this.lblCaps.Size = new System.Drawing.Size(120, 16);
            this.lblCaps.TabIndex = 176;
            this.lblCaps.Text = "CAPS Tuşu Açık";
            this.lblCaps.Visible = false;
            // 
            // NewUser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(433, 327);
            this.Controls.Add(this.grupKisiKayit);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(449, 366);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(449, 366);
            this.Name = "NewUser";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Yeni kullanıcı ekle";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.NewUser_Load);
            this.grupKisiKayit.ResumeLayout(false);
            this.grupKisiKayit.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grupKisiKayit;
        private System.Windows.Forms.Button btn_Kaydet_yeni;
        private System.Windows.Forms.TextBox txtSoyad_yeni;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtAd_yeni;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtTckimlik_yeni;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtPass_yeni;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtUser_yeni;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.ComboBox cmbCafeList;
        private System.Windows.Forms.Label lblCaps;

    }
}