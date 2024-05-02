namespace ReforgerServerApp
{
    partial class AdminManager
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
            adminListView = new ListView();
            addBtn = new Button();
            removeBtn = new Button();
            closeBtn = new Button();
            adminTB = new TextBox();
            label1 = new Label();
            linkLblSteamIdIo = new LinkLabel();
            linkLblSteamIdFinder = new LinkLabel();
            SuspendLayout();
            // 
            // adminListView
            // 
            adminListView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            adminListView.FullRowSelect = true;
            adminListView.GridLines = true;
            adminListView.Location = new Point(12, 72);
            adminListView.MultiSelect = false;
            adminListView.Name = "adminListView";
            adminListView.Size = new Size(776, 331);
            adminListView.TabIndex = 0;
            adminListView.UseCompatibleStateImageBehavior = false;
            adminListView.View = View.List;
            // 
            // addBtn
            // 
            addBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            addBtn.Location = new Point(392, 415);
            addBtn.Name = "addBtn";
            addBtn.Size = new Size(75, 23);
            addBtn.TabIndex = 1;
            addBtn.Text = "Add";
            addBtn.UseVisualStyleBackColor = true;
            addBtn.Click += AddBtnClicked;
            // 
            // removeBtn
            // 
            removeBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            removeBtn.Location = new Point(473, 415);
            removeBtn.Name = "removeBtn";
            removeBtn.Size = new Size(75, 23);
            removeBtn.TabIndex = 2;
            removeBtn.Text = "Remove";
            removeBtn.UseVisualStyleBackColor = true;
            removeBtn.Click += RemoveBtnClicked;
            // 
            // closeBtn
            // 
            closeBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            closeBtn.Location = new Point(713, 415);
            closeBtn.Name = "closeBtn";
            closeBtn.Size = new Size(75, 23);
            closeBtn.TabIndex = 3;
            closeBtn.Text = "Close";
            closeBtn.UseVisualStyleBackColor = true;
            closeBtn.Click += CloseBtnClicked;
            // 
            // adminTB
            // 
            adminTB.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            adminTB.Location = new Point(12, 415);
            adminTB.Name = "adminTB";
            adminTB.Size = new Size(374, 23);
            adminTB.TabIndex = 4;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(359, 30);
            label1.TabIndex = 5;
            label1.Text = "Players can be made admins by adding their SteamID 64 to this list.\r\nYou can use services like steamid.io to get the user's SteamID 64.";
            // 
            // linkLblSteamIdIo
            // 
            linkLblSteamIdIo.AutoSize = true;
            linkLblSteamIdIo.Location = new Point(12, 39);
            linkLblSteamIdIo.Name = "linkLblSteamIdIo";
            linkLblSteamIdIo.Size = new Size(107, 15);
            linkLblSteamIdIo.TabIndex = 7;
            linkLblSteamIdIo.TabStop = true;
            linkLblSteamIdIo.Text = "https://steamid.io/";
            linkLblSteamIdIo.LinkClicked += linkLblSteamIdIo_LinkClicked;
            // 
            // linkLblSteamIdFinder
            // 
            linkLblSteamIdFinder.AutoSize = true;
            linkLblSteamIdFinder.Location = new Point(12, 54);
            linkLblSteamIdFinder.Name = "linkLblSteamIdFinder";
            linkLblSteamIdFinder.Size = new Size(182, 15);
            linkLblSteamIdFinder.TabIndex = 8;
            linkLblSteamIdFinder.TabStop = true;
            linkLblSteamIdFinder.Text = "https://www.steamidfinder.com/";
            linkLblSteamIdFinder.LinkClicked += linkLblSteamIdFinder_LinkClicked;
            // 
            // AdminManager
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(linkLblSteamIdFinder);
            Controls.Add(linkLblSteamIdIo);
            Controls.Add(label1);
            Controls.Add(adminTB);
            Controls.Add(closeBtn);
            Controls.Add(removeBtn);
            Controls.Add(addBtn);
            Controls.Add(adminListView);
            MinimumSize = new Size(640, 480);
            Name = "AdminManager";
            ShowIcon = false;
            Text = "Admin Manager";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListView adminListView;
        private Button addBtn;
        private Button removeBtn;
        private Button closeBtn;
        private TextBox adminTB;
        private Label label1;
        private LinkLabel linkLblSteamIdIo;
        private LinkLabel linkLblSteamIdFinder;
    }
}