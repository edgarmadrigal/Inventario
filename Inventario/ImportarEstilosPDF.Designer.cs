namespace Inventario
{
    partial class ImportarEstilosPDF
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
            this.lblTotalFilas = new System.Windows.Forms.Label();
            this.dgvPreview = new System.Windows.Forms.DataGridView();
            this.button1 = new System.Windows.Forms.Button();
            this.btnGuardarTallas = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTotalFilas
            // 
            this.lblTotalFilas.AutoSize = true;
            this.lblTotalFilas.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalFilas.Location = new System.Drawing.Point(23, 469);
            this.lblTotalFilas.Name = "lblTotalFilas";
            this.lblTotalFilas.Size = new System.Drawing.Size(0, 29);
            this.lblTotalFilas.TabIndex = 4;
            // 
            // dgvPreview
            // 
            this.dgvPreview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPreview.Location = new System.Drawing.Point(18, 69);
            this.dgvPreview.Name = "dgvPreview";
            this.dgvPreview.Size = new System.Drawing.Size(978, 380);
            this.dgvPreview.TabIndex = 6;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(821, 16);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(175, 51);
            this.button1.TabIndex = 8;
            this.button1.Text = "CargarPDF";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnCargarPDF_Click);
            // 
            // btnGuardarTallas
            // 
            this.btnGuardarTallas.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGuardarTallas.Location = new System.Drawing.Point(821, 455);
            this.btnGuardarTallas.Name = "btnGuardarTallas";
            this.btnGuardarTallas.Size = new System.Drawing.Size(175, 51);
            this.btnGuardarTallas.TabIndex = 11;
            this.btnGuardarTallas.Text = "Guardar";
            this.btnGuardarTallas.UseVisualStyleBackColor = true;
            this.btnGuardarTallas.Click += new System.EventHandler(this.btnGuardarTallas_Click);
            // 
            // ImportarEstilosPDF
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1060, 596);
            this.Controls.Add(this.btnGuardarTallas);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dgvPreview);
            this.Controls.Add(this.lblTotalFilas);
            this.Name = "ImportarEstilosPDF";
            this.Text = "ImportarEstilosPDF";
            ((System.ComponentModel.ISupportInitialize)(this.dgvPreview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblTotalFilas;
        private System.Windows.Forms.DataGridView dgvPreview;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnGuardarTallas;



    }
}