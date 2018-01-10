namespace HandHistories.Parser.WindowsTestApp
{
    partial class ParserTestForm
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
            this.richTextBoxHandText = new System.Windows.Forms.RichTextBox();
            this.buttonParse = new System.Windows.Forms.Button();
            this.listBoxSite = new System.Windows.Forms.ListBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBox_validateHands = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // richTextBoxHandText
            // 
            this.richTextBoxHandText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxHandText.Location = new System.Drawing.Point(3, 23);
            this.richTextBoxHandText.Name = "richTextBoxHandText";
            this.tableLayoutPanel1.SetRowSpan(this.richTextBoxHandText, 3);
            this.richTextBoxHandText.Size = new System.Drawing.Size(623, 651);
            this.richTextBoxHandText.TabIndex = 0;
            this.richTextBoxHandText.Text = "";
            // 
            // buttonParse
            // 
            this.buttonParse.Location = new System.Drawing.Point(632, 624);
            this.buttonParse.Name = "buttonParse";
            this.buttonParse.Size = new System.Drawing.Size(140, 49);
            this.buttonParse.TabIndex = 1;
            this.buttonParse.Text = "Parse";
            this.buttonParse.UseVisualStyleBackColor = true;
            this.buttonParse.Click += new System.EventHandler(this.buttonParse_Click);
            // 
            // listBoxSite
            // 
            this.listBoxSite.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxSite.FormattingEnabled = true;
            this.listBoxSite.Location = new System.Drawing.Point(632, 23);
            this.listBoxSite.Name = "listBoxSite";
            this.listBoxSite.Size = new System.Drawing.Size(141, 572);
            this.listBoxSite.TabIndex = 2;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 147F));
            this.tableLayoutPanel1.Controls.Add(this.label2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.richTextBoxHandText, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.listBoxSite, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.buttonParse, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.checkBox_validateHands, 1, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 56F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(776, 677);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(688, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Site";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(260, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Hand History Text";
            // 
            // checkBox_validateHands
            // 
            this.checkBox_validateHands.AutoSize = true;
            this.checkBox_validateHands.Location = new System.Drawing.Point(632, 601);
            this.checkBox_validateHands.Name = "checkBox_validateHands";
            this.checkBox_validateHands.Size = new System.Drawing.Size(96, 17);
            this.checkBox_validateHands.TabIndex = 5;
            this.checkBox_validateHands.Text = "Validate hands";
            this.checkBox_validateHands.UseVisualStyleBackColor = true;
            // 
            // ParserTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(776, 677);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ParserTestForm";
            this.Text = "Hand Parser Test App";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBoxHandText;
        private System.Windows.Forms.Button buttonParse;
        private System.Windows.Forms.ListBox listBoxSite;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBox_validateHands;
    }
}

