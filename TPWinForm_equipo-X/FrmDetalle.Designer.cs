using System.Windows.Forms;
namespace TPWinForm_equipo_X
{
    partial class FrmDetalle
    {
        private System.Windows.Forms.Label descripcionArticulo;
        private System.Windows.Forms.PictureBox vista;
        private System.Windows.Forms.Label estado;
        private System.Windows.Forms.Button anterior;
        private System.Windows.Forms.Label contador;
        private System.Windows.Forms.Button siguiente;

        private void InitializeComponent()
        {
            this.descripcionArticulo = new System.Windows.Forms.Label();
            this.vista = new System.Windows.Forms.PictureBox();
            this.estado = new System.Windows.Forms.Label();
            this.anterior = new System.Windows.Forms.Button();
            this.contador = new System.Windows.Forms.Label();
            this.siguiente = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.vista)).BeginInit();
            this.SuspendLayout();
            // descripcionArticulo
            this.descripcionArticulo.Name = "descripcionArticulo";
            this.descripcionArticulo.Location = new System.Drawing.Point(24, 24);
            this.descripcionArticulo.Size = new System.Drawing.Size(852, 100);
            this.descripcionArticulo.TabIndex = 0;
            // vista
            this.vista.Name = "vista";
            this.vista.Location = new System.Drawing.Point(24, 150);
            this.vista.Size = new System.Drawing.Size(852, 410);
            this.vista.TabIndex = 1;
            this.vista.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.vista.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            // estado
            this.estado.Name = "estado";
            this.estado.Location = new System.Drawing.Point(24, 566);
            this.estado.Size = new System.Drawing.Size(852, 45);
            this.estado.TabIndex = 2;
            // anterior
            this.anterior.Name = "anterior";
            this.anterior.Location = new System.Drawing.Point(24, 628);
            this.anterior.Size = new System.Drawing.Size(135, 32);
            this.anterior.TabIndex = 3;
            this.anterior.Text = "Anterior";
            this.anterior.UseVisualStyleBackColor = true;
            // contador
            this.contador.Name = "contador";
            this.contador.Location = new System.Drawing.Point(164, 628);
            this.contador.Size = new System.Drawing.Size(550, 32);
            this.contador.TabIndex = 4;
            this.contador.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // siguiente
            this.siguiente.Name = "siguiente";
            this.siguiente.Location = new System.Drawing.Point(741, 628);
            this.siguiente.Size = new System.Drawing.Size(135, 32);
            this.siguiente.TabIndex = 5;
            this.siguiente.Text = "Siguiente";
            this.siguiente.UseVisualStyleBackColor = true;
            this.ClientSize = new System.Drawing.Size(900, 680);
            this.Name = "FrmDetalle";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Controls.Add(this.descripcionArticulo);
            this.Controls.Add(this.vista);
            this.Controls.Add(this.estado);
            this.Controls.Add(this.anterior);
            this.Controls.Add(this.contador);
            this.Controls.Add(this.siguiente);
            ((System.ComponentModel.ISupportInitialize)(this.vista)).EndInit();
            this.anterior.Click += anterior_Click;
            this.siguiente.Click += siguiente_Click;
            this.Shown += FrmDetalle_Shown;
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
