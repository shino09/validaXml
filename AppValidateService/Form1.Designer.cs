namespace AppValidateService
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
     
            this.Btn_04002 = new System.Windows.Forms.Button();
         
            this.Txt_Rsp = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // Btn_04017
            // 
           
            // 
            // Btn_04002
            // 
            this.Btn_04002.Location = new System.Drawing.Point(12, 12);
            this.Btn_04002.Name = "Btn_04002";
            this.Btn_04002.Size = new System.Drawing.Size(75, 23);
            this.Btn_04002.TabIndex = 1;
            this.Btn_04002.Text = "04002";
            this.Btn_04002.UseVisualStyleBackColor = true;
            this.Btn_04002.Click += new System.EventHandler(this.Btn_04002_Click);
            
            // 
            // Txt_Rsp
            // 
            this.Txt_Rsp.Location = new System.Drawing.Point(94, 13);
            this.Txt_Rsp.Multiline = true;
            this.Txt_Rsp.Name = "Txt_Rsp";
            this.Txt_Rsp.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.Txt_Rsp.Size = new System.Drawing.Size(495, 364);
            this.Txt_Rsp.TabIndex = 7;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(599, 399);
            this.Controls.Add(this.Txt_Rsp);
          
            this.Controls.Add(this.Btn_04002);
          
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

     
        private System.Windows.Forms.Button Btn_04002;

        private System.Windows.Forms.TextBox Txt_Rsp;
    }
}