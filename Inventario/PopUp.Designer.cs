namespace Inventario
{
    partial class PopUp
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
            this.txtNumCarton = new System.Windows.Forms.TextBox();
            this.lblNumeroCarton = new System.Windows.Forms.Label();
            this.btnAceptar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtNumCarton
            // 
            this.txtNumCarton.Location = new System.Drawing.Point(121, 29);
            this.txtNumCarton.Name = "txtNumCarton";
            this.txtNumCarton.Size = new System.Drawing.Size(137, 20);
            this.txtNumCarton.TabIndex = 0;
            // 
            // lblNumeroCarton
            // 
            this.lblNumeroCarton.AutoSize = true;
            this.lblNumeroCarton.Location = new System.Drawing.Point(34, 32);
            this.lblNumeroCarton.Name = "lblNumeroCarton";
            this.lblNumeroCarton.Size = new System.Drawing.Size(81, 13);
            this.lblNumeroCarton.TabIndex = 1;
            this.lblNumeroCarton.Text = "Numero Carton:";
            // 
            // btnAceptar
            // 
            this.btnAceptar.Location = new System.Drawing.Point(264, 27);
            this.btnAceptar.Name = "btnAceptar";
            this.btnAceptar.Size = new System.Drawing.Size(75, 23);
            this.btnAceptar.TabIndex = 2;
            this.btnAceptar.Text = "Guardar";
            this.btnAceptar.UseVisualStyleBackColor = true;
            this.btnAceptar.Click += new System.EventHandler(this.btnAceptar_Click);
            // 
            // PopUp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(398, 81);
            this.Controls.Add(this.btnAceptar);
            this.Controls.Add(this.lblNumeroCarton);
            this.Controls.Add(this.txtNumCarton);
            this.Name = "PopUp";
            this.Text = "PopUp";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtNumCarton;
        private System.Windows.Forms.Label lblNumeroCarton;
        private System.Windows.Forms.Button btnAceptar;
    }
}