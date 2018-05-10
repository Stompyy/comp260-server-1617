namespace Winform_Client
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
            this.buttonSend = new System.Windows.Forms.Button();
            this.textBox_Input = new System.Windows.Forms.TextBox();
            this.textBox_Output = new System.Windows.Forms.TextBox();
            this.textBox_ClientName = new System.Windows.Forms.TextBox();
            this.listBox_ClientList = new System.Windows.Forms.ListBox();
            this.North = new System.Windows.Forms.Button();
            this.South = new System.Windows.Forms.Button();
            this.East = new System.Windows.Forms.Button();
            this.West = new System.Windows.Forms.Button();
            this.LookAround = new System.Windows.Forms.Button();
            this.Inventory = new System.Windows.Forms.Button();
            this.helpButton = new System.Windows.Forms.Button();
            this.textBox_chatMessage = new System.Windows.Forms.TextBox();
            this.sendChatButton = new System.Windows.Forms.Button();
            this.showStatsButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonSend
            // 
            this.buttonSend.Location = new System.Drawing.Point(640, 491);
            this.buttonSend.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(112, 35);
            this.buttonSend.TabIndex = 0;
            this.buttonSend.Text = "Send";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // textBox_Input
            // 
            this.textBox_Input.Location = new System.Drawing.Point(48, 491);
            this.textBox_Input.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox_Input.Name = "textBox_Input";
            this.textBox_Input.Size = new System.Drawing.Size(574, 26);
            this.textBox_Input.TabIndex = 2;
            // 
            // textBox_Output
            // 
            this.textBox_Output.Location = new System.Drawing.Point(48, 80);
            this.textBox_Output.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox_Output.Multiline = true;
            this.textBox_Output.Name = "textBox_Output";
            this.textBox_Output.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_Output.Size = new System.Drawing.Size(574, 359);
            this.textBox_Output.TabIndex = 3;
            // 
            // textBox_ClientName
            // 
            this.textBox_ClientName.Location = new System.Drawing.Point(62, 25);
            this.textBox_ClientName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox_ClientName.Name = "textBox_ClientName";
            this.textBox_ClientName.ReadOnly = true;
            this.textBox_ClientName.Size = new System.Drawing.Size(148, 26);
            this.textBox_ClientName.TabIndex = 4;
            // 
            // listBox_ClientList
            // 
            this.listBox_ClientList.FormattingEnabled = true;
            this.listBox_ClientList.ItemHeight = 20;
            this.listBox_ClientList.Location = new System.Drawing.Point(780, 80);
            this.listBox_ClientList.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.listBox_ClientList.Name = "listBox_ClientList";
            this.listBox_ClientList.Size = new System.Drawing.Size(256, 244);
            this.listBox_ClientList.TabIndex = 5;
            // 
            // North
            // 
            this.North.Location = new System.Drawing.Point(1175, 287);
            this.North.Name = "North";
            this.North.Size = new System.Drawing.Size(75, 47);
            this.North.TabIndex = 6;
            this.North.Text = "North";
            this.North.UseVisualStyleBackColor = true;
            this.North.Click += new System.EventHandler(this.North_Click);
            // 
            // South
            // 
            this.South.Location = new System.Drawing.Point(1175, 393);
            this.South.Name = "South";
            this.South.Size = new System.Drawing.Size(75, 45);
            this.South.TabIndex = 7;
            this.South.Text = "South";
            this.South.UseVisualStyleBackColor = true;
            this.South.Click += new System.EventHandler(this.South_Click);
            // 
            // East
            // 
            this.East.Location = new System.Drawing.Point(1256, 336);
            this.East.Name = "East";
            this.East.Size = new System.Drawing.Size(75, 52);
            this.East.TabIndex = 8;
            this.East.Text = "East";
            this.East.UseVisualStyleBackColor = true;
            this.East.Click += new System.EventHandler(this.East_Click);
            // 
            // West
            // 
            this.West.Location = new System.Drawing.Point(1094, 336);
            this.West.Name = "West";
            this.West.Size = new System.Drawing.Size(75, 52);
            this.West.TabIndex = 9;
            this.West.Text = "West";
            this.West.UseVisualStyleBackColor = true;
            this.West.Click += new System.EventHandler(this.West_Click);
            // 
            // LookAround
            // 
            this.LookAround.Location = new System.Drawing.Point(1148, 144);
            this.LookAround.Name = "LookAround";
            this.LookAround.Size = new System.Drawing.Size(122, 46);
            this.LookAround.TabIndex = 10;
            this.LookAround.Text = "Look around";
            this.LookAround.UseVisualStyleBackColor = true;
            this.LookAround.Click += new System.EventHandler(this.LookAround_Click);
            // 
            // Inventory
            // 
            this.Inventory.Location = new System.Drawing.Point(1148, 80);
            this.Inventory.Name = "Inventory";
            this.Inventory.Size = new System.Drawing.Size(122, 44);
            this.Inventory.TabIndex = 11;
            this.Inventory.Text = "Inventory";
            this.Inventory.UseVisualStyleBackColor = true;
            this.Inventory.Click += new System.EventHandler(this.Inventory_Click);
            // 
            // helpButton
            // 
            this.helpButton.Location = new System.Drawing.Point(1291, 552);
            this.helpButton.Name = "helpButton";
            this.helpButton.Size = new System.Drawing.Size(82, 37);
            this.helpButton.TabIndex = 12;
            this.helpButton.Text = "Help";
            this.helpButton.UseVisualStyleBackColor = true;
            this.helpButton.Click += new System.EventHandler(this.helpButton_Click);
            // 
            // textBox_chatMessage
            // 
            this.textBox_chatMessage.Location = new System.Drawing.Point(780, 356);
            this.textBox_chatMessage.Name = "textBox_chatMessage";
            this.textBox_chatMessage.Size = new System.Drawing.Size(256, 26);
            this.textBox_chatMessage.TabIndex = 13;
            this.textBox_chatMessage.Text = "chat";
            // 
            // sendChatButton
            // 
            this.sendChatButton.Location = new System.Drawing.Point(780, 403);
            this.sendChatButton.Name = "sendChatButton";
            this.sendChatButton.Size = new System.Drawing.Size(256, 35);
            this.sendChatButton.TabIndex = 14;
            this.sendChatButton.Text = "Send chat message";
            this.sendChatButton.UseVisualStyleBackColor = true;
            this.sendChatButton.Click += new System.EventHandler(this.sendChatButton_Click);
            // 
            // showStatsButton
            // 
            this.showStatsButton.Location = new System.Drawing.Point(1148, 210);
            this.showStatsButton.Name = "showStatsButton";
            this.showStatsButton.Size = new System.Drawing.Size(122, 46);
            this.showStatsButton.TabIndex = 15;
            this.showStatsButton.Text = "Stats";
            this.showStatsButton.UseVisualStyleBackColor = true;
            this.showStatsButton.Click += new System.EventHandler(this.showStatsButton_Click);
            // 
            // Form1
            // 
            this.AcceptButton = this.buttonSend;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1407, 613);
            this.Controls.Add(this.showStatsButton);
            this.Controls.Add(this.sendChatButton);
            this.Controls.Add(this.textBox_chatMessage);
            this.Controls.Add(this.helpButton);
            this.Controls.Add(this.Inventory);
            this.Controls.Add(this.LookAround);
            this.Controls.Add(this.West);
            this.Controls.Add(this.East);
            this.Controls.Add(this.South);
            this.Controls.Add(this.North);
            this.Controls.Add(this.listBox_ClientList);
            this.Controls.Add(this.textBox_ClientName);
            this.Controls.Add(this.textBox_Output);
            this.Controls.Add(this.textBox_Input);
            this.Controls.Add(this.buttonSend);
            this.Enabled = false;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Form1";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.TextBox textBox_Input;
        private System.Windows.Forms.TextBox textBox_Output;
        private System.Windows.Forms.TextBox textBox_ClientName;
        private System.Windows.Forms.ListBox listBox_ClientList;
        private System.Windows.Forms.Button North;
        private System.Windows.Forms.Button South;
        private System.Windows.Forms.Button East;
        private System.Windows.Forms.Button West;
        private System.Windows.Forms.Button LookAround;
        private System.Windows.Forms.Button Inventory;
        private System.Windows.Forms.Button helpButton;
        private System.Windows.Forms.TextBox textBox_chatMessage;
        private System.Windows.Forms.Button sendChatButton;
        private System.Windows.Forms.Button showStatsButton;
    }
}

