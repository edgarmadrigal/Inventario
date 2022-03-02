
namespace Inventario
{
    partial class Principal
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.eSCANEOToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.eSCANEOToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.sALIRToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.eSCANEOToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1140, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // eSCANEOToolStripMenuItem
            // 
            this.eSCANEOToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.eSCANEOToolStripMenuItem1,
            this.sALIRToolStripMenuItem});
            this.eSCANEOToolStripMenuItem.Name = "eSCANEOToolStripMenuItem";
            this.eSCANEOToolStripMenuItem.Size = new System.Drawing.Size(70, 20);
            this.eSCANEOToolStripMenuItem.Text = "ARCHIVO";
            // 
            // eSCANEOToolStripMenuItem1
            // 
            this.eSCANEOToolStripMenuItem1.Name = "eSCANEOToolStripMenuItem1";
            this.eSCANEOToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.eSCANEOToolStripMenuItem1.Text = "ESCANEO";
            this.eSCANEOToolStripMenuItem1.Click += new System.EventHandler(this.eSCANEOToolStripMenuItem1_Click);
            // 
            // sALIRToolStripMenuItem
            // 
            this.sALIRToolStripMenuItem.Name = "sALIRToolStripMenuItem";
            this.sALIRToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.sALIRToolStripMenuItem.Text = "SALIR";
            this.sALIRToolStripMenuItem.Click += new System.EventHandler(this.sALIRToolStripMenuItem_Click);
            // 
            // Principal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1140, 776);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Principal";
            this.Text = "Principal";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem eSCANEOToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem eSCANEOToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem sALIRToolStripMenuItem;
    }
}