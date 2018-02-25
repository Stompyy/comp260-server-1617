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
            this.textBox_Output.TextChanged += new System.EventHandler(this.textBox_Output_TextChanged);
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
            this.listBox_ClientList.Location = new System.Drawing.Point(645, 88);
            this.listBox_ClientList.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.listBox_ClientList.Name = "listBox_ClientList";
            this.listBox_ClientList.Size = new System.Drawing.Size(104, 344);
            this.listBox_ClientList.TabIndex = 5;
            this.listBox_ClientList.SelectedIndexChanged += new System.EventHandler(this.listBox_ClientList_SelectedIndexChanged);
            // 
            // North
            // 
            this.North.Location = new System.Drawing.Point(893, 321);
            this.North.Name = "North";
            this.North.Size = new System.Drawing.Size(75, 47);
            this.North.TabIndex = 6;
            this.North.Text = "North";
            this.North.UseVisualStyleBackColor = true;
            this.North.Click += new System.EventHandler(this.North_Click);
            // 
            // South
            // 
            this.South.Location = new System.Drawing.Point(893, 427);
            this.South.Name = "South";
            this.South.Size = new System.Drawing.Size(75, 45);
            this.South.TabIndex = 7;
            this.South.Text = "South";
            this.South.UseVisualStyleBackColor = true;
            this.South.Click += new System.EventHandler(this.South_Click);
            // 
            // East
            // 
            this.East.Location = new System.Drawing.Point(974, 370);
            this.East.Name = "East";
            this.East.Size = new System.Drawing.Size(75, 52);
            this.East.TabIndex = 8;
            this.East.Text = "East";
            this.East.UseVisualStyleBackColor = true;
            this.East.Click += new System.EventHandler(this.East_Click);
            // 
            // West
            // 
            this.West.Location = new System.Drawing.Point(812, 370);
            this.West.Name = "West";
            this.West.Size = new System.Drawing.Size(75, 52);
            this.West.TabIndex = 9;
            this.West.Text = "West";
            this.West.UseVisualStyleBackColor = true;
            this.West.Click += new System.EventHandler(this.West_Click);
            // 
            // LookAround
            // 
            this.LookAround.Location = new System.Drawing.Point(869, 481);
            this.LookAround.Name = "LookAround";
            this.LookAround.Size = new System.Drawing.Size(122, 46);
            this.LookAround.TabIndex = 10;
            this.LookAround.Text = "LookAround";
            this.LookAround.UseVisualStyleBackColor = true;
            this.LookAround.Click += new System.EventHandler(this.LookAround_Click);
            // 
            // Inventory
            // 
            this.Inventory.Location = new System.Drawing.Point(869, 88);
            this.Inventory.Name = "Inventory";
            this.Inventory.Size = new System.Drawing.Size(122, 36);
            this.Inventory.TabIndex = 11;
            this.Inventory.Text = "Inventory";
            this.Inventory.UseVisualStyleBackColor = true;
            this.Inventory.Click += new System.EventHandler(this.Inventory_Click);
            // 
            // Form1
            // 
            this.AcceptButton = this.buttonSend;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1088, 577);
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
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Form1";
            this.Text = "Form1";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
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
    }
}

