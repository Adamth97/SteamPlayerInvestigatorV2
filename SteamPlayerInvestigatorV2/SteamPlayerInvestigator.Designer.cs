namespace SteamPlayerInvestigatorV2
{
    partial class SteamPlayerInvestigator
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            ResultTxtbox = new TextBox();
            label4 = new Label();
            label5 = new Label();
            ApiKeyTxtbox = new TextBox();
            SteamIDTxtbox = new TextBox();
            AnalyseBtn = new Button();
            Savebtn = new Button();
            progressBar1 = new ProgressBar();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.White;
            label1.Font = new Font("Cambria", 32.25F, FontStyle.Regular, GraphicsUnit.Point);
            label1.Location = new Point(527, 9);
            label1.Name = "label1";
            label1.Size = new Size(261, 102);
            label1.TabIndex = 0;
            label1.Text = "Steam Player\r\n Investigator";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.White;
            label2.Font = new Font("Cambria", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label2.Location = new Point(541, 118);
            label2.Name = "label2";
            label2.Size = new Size(228, 45);
            label2.TabIndex = 1;
            label2.Text = "      Please enter your Steam APIKey \r\n aswell as the SteamID64 of the account\r\n you wish to analyse in the boxes below.";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.White;
            label3.Font = new Font("Cambria", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label3.Location = new Point(5, 6);
            label3.Name = "label3";
            label3.Size = new Size(102, 15);
            label3.TabIndex = 2;
            label3.Text = "Analysis Results:";
            // 
            // ResultTxtbox
            // 
            ResultTxtbox.BackColor = Color.White;
            ResultTxtbox.Font = new Font("Cambria", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            ResultTxtbox.ImeMode = ImeMode.On;
            ResultTxtbox.Location = new Point(6, 25);
            ResultTxtbox.Multiline = true;
            ResultTxtbox.Name = "ResultTxtbox";
            ResultTxtbox.ReadOnly = true;
            ResultTxtbox.Size = new Size(515, 403);
            ResultTxtbox.TabIndex = 3;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Cambria", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label4.Location = new Point(540, 203);
            label4.Name = "label4";
            label4.Size = new Size(47, 15);
            label4.TabIndex = 4;
            label4.Text = "ApiKey";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Cambria", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label5.Location = new Point(527, 245);
            label5.Name = "label5";
            label5.Size = new Size(67, 15);
            label5.TabIndex = 5;
            label5.Text = "SteamID64";
            // 
            // ApiKeyTxtbox
            // 
            ApiKeyTxtbox.BackColor = Color.White;
            ApiKeyTxtbox.Location = new Point(597, 199);
            ApiKeyTxtbox.Name = "ApiKeyTxtbox";
            ApiKeyTxtbox.PasswordChar = '*';
            ApiKeyTxtbox.PlaceholderText = "Please enter the ApiKey here.";
            ApiKeyTxtbox.Size = new Size(191, 23);
            ApiKeyTxtbox.TabIndex = 6;
            ApiKeyTxtbox.Text = "30FD39066A9E2628D7CDE47C35CAE10A";
            // 
            // SteamIDTxtbox
            // 
            SteamIDTxtbox.BackColor = Color.White;
            SteamIDTxtbox.Location = new Point(597, 242);
            SteamIDTxtbox.Name = "SteamIDTxtbox";
            SteamIDTxtbox.PlaceholderText = "Please enter the SteamID here.";
            SteamIDTxtbox.Size = new Size(191, 23);
            SteamIDTxtbox.TabIndex = 7;
            SteamIDTxtbox.Text = "76561198440969812";
            // 
            // AnalyseBtn
            // 
            AnalyseBtn.BackColor = Color.White;
            AnalyseBtn.Font = new Font("Cambria", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            AnalyseBtn.Location = new Point(555, 282);
            AnalyseBtn.Name = "AnalyseBtn";
            AnalyseBtn.Size = new Size(211, 41);
            AnalyseBtn.TabIndex = 8;
            AnalyseBtn.Text = "Analyse Account";
            AnalyseBtn.UseVisualStyleBackColor = false;
            AnalyseBtn.Click += AnalyseBtn_Click;
            // 
            // Savebtn
            // 
            Savebtn.BackColor = Color.White;
            Savebtn.Font = new Font("Cambria", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            Savebtn.Location = new Point(555, 387);
            Savebtn.Name = "Savebtn";
            Savebtn.Size = new Size(211, 41);
            Savebtn.TabIndex = 9;
            Savebtn.Text = "Save Results";
            Savebtn.UseVisualStyleBackColor = false;
            // 
            // progressBar1
            // 
            progressBar1.BackColor = Color.White;
            progressBar1.Location = new Point(556, 338);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(209, 34);
            progressBar1.TabIndex = 10;
            // 
            // SteamPlayerInvestigator
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.LightGray;
            ClientSize = new Size(800, 450);
            Controls.Add(progressBar1);
            Controls.Add(Savebtn);
            Controls.Add(AnalyseBtn);
            Controls.Add(SteamIDTxtbox);
            Controls.Add(ApiKeyTxtbox);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(ResultTxtbox);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "SteamPlayerInvestigator";
            Text = "SteamPlayerInvestigator";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private Label label3;
        private TextBox ResultTxtbox;
        private Label label4;
        private Label label5;
        private TextBox ApiKeyTxtbox;
        private TextBox SteamIDTxtbox;
        private Button AnalyseBtn;
        private Button Savebtn;
        private ProgressBar progressBar1;
    }
}