using ReforgerServerApp.Components;

namespace ReforgerServerApp
{
    partial class ListForm
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
      addBtn = new Button();
      removeBtn = new Button();
      closeBtn = new Button();
      itemTB = new TextBox();
      itemListView = new BoundListBox();
      SuspendLayout();
      // 
      // addBtn
      // 
      addBtn.Anchor =  AnchorStyles.Bottom | AnchorStyles.Right;
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
      removeBtn.Anchor =  AnchorStyles.Bottom | AnchorStyles.Right;
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
      closeBtn.Anchor =  AnchorStyles.Bottom | AnchorStyles.Right;
      closeBtn.Location = new Point(713, 415);
      closeBtn.Name = "closeBtn";
      closeBtn.Size = new Size(75, 23);
      closeBtn.TabIndex = 3;
      closeBtn.Text = "Close";
      closeBtn.UseVisualStyleBackColor = true;
      closeBtn.Click += CloseBtnClicked;
      // 
      // itemTB
      // 
      itemTB.Anchor =  AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      itemTB.Location = new Point(12, 415);
      itemTB.Name = "itemTB";
      itemTB.Size = new Size(374, 23);
      itemTB.TabIndex = 4;
      // 
      // itemListView
      // 
      itemListView.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      itemListView.ItemHeight = 15;
      itemListView.Location = new Point(12, 12);
      itemListView.Name = "itemListView";
      itemListView.Size = new Size(776, 379);
      itemListView.TabIndex = 0;
      // 
      // ListForm
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(800, 450);
      Controls.Add(itemTB);
      Controls.Add(closeBtn);
      Controls.Add(removeBtn);
      Controls.Add(addBtn);
      Controls.Add(itemListView);
      MinimumSize = new Size(640, 480);
      Name = "ListForm";
      ShowIcon = false;
      StartPosition = FormStartPosition.CenterParent;
      Text = "Longbow - ListForm";
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion
    private Button addBtn;
        private Button removeBtn;
        private Button closeBtn;
        private TextBox itemTB;
        private BoundListBox itemListView;
    }
}