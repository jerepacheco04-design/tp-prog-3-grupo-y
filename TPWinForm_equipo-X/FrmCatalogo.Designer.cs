using System.Windows.Forms;

namespace TPWinForm_equipo_X
{
    partial class FrmCatalogo
    {
        private TextBox txtBusqueda;

        private DataGridView dgvArticulos;

        private ComboBox cboMarca;

        private ComboBox cboCategoria;

        private Button btnBuscar;

        private Button btnLimpiar;

        private Button btnAgregar;

        private Button btnModificar;

        private Button btnEliminar;

        private Button btnDetalle;

        private Label lblBusqueda;

        private Label lblResultado;

        private void InitializeComponent()
        {
            this.txtBusqueda = new System.Windows.Forms.TextBox();
            this.dgvArticulos = new System.Windows.Forms.DataGridView();
            this.cboMarca = new System.Windows.Forms.ComboBox();
            this.cboCategoria = new System.Windows.Forms.ComboBox();
            this.btnBuscar = new System.Windows.Forms.Button();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.btnAgregar = new System.Windows.Forms.Button();
            this.btnModificar = new System.Windows.Forms.Button();
            this.btnEliminar = new System.Windows.Forms.Button();
            this.btnDetalle = new System.Windows.Forms.Button();
            this.lblBusqueda = new System.Windows.Forms.Label();
            this.lblResultado = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvArticulos)).BeginInit();
            this.SuspendLayout();
            // 
            // txtBusqueda
            // 
            this.txtBusqueda.Location = new System.Drawing.Point(24, 38);
            this.txtBusqueda.Name = "txtBusqueda";
            this.txtBusqueda.Size = new System.Drawing.Size(607, 20);
            this.txtBusqueda.TabIndex = 0;
            // 
            // dgvArticulos
            // 
            this.dgvArticulos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvArticulos.Location = new System.Drawing.Point(24, 153);
            this.dgvArticulos.Name = "dgvArticulos";
            this.dgvArticulos.Size = new System.Drawing.Size(822, 315);
            this.dgvArticulos.TabIndex = 1;
            // 
            // cboMarca
            // 
            this.cboMarca.FormattingEnabled = true;
            this.cboMarca.Location = new System.Drawing.Point(24, 98);
            this.cboMarca.Name = "cboMarca";
            this.cboMarca.Size = new System.Drawing.Size(247, 21);
            this.cboMarca.TabIndex = 2;
            // 
            // cboCategoria
            // 
            this.cboCategoria.FormattingEnabled = true;
            this.cboCategoria.Location = new System.Drawing.Point(308, 98);
            this.cboCategoria.Name = "cboCategoria";
            this.cboCategoria.Size = new System.Drawing.Size(247, 21);
            this.cboCategoria.TabIndex = 3;
            // 
            // btnBuscar
            // 
            this.btnBuscar.Location = new System.Drawing.Point(657, 38);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(75, 23);
            this.btnBuscar.TabIndex = 4;
            this.btnBuscar.Text = "Buscar";
            this.btnBuscar.UseVisualStyleBackColor = true;
            // 
            // btnLimpiar
            // 
            this.btnLimpiar.Location = new System.Drawing.Point(769, 38);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(75, 23);
            this.btnLimpiar.TabIndex = 5;
            this.btnLimpiar.Text = "Limpiar";
            this.btnLimpiar.UseVisualStyleBackColor = true;
            // 
            // btnAgregar
            // 
            this.btnAgregar.Location = new System.Drawing.Point(24, 499);
            this.btnAgregar.Name = "btnAgregar";
            this.btnAgregar.Size = new System.Drawing.Size(75, 23);
            this.btnAgregar.TabIndex = 6;
            this.btnAgregar.Text = "Agregar";
            this.btnAgregar.UseVisualStyleBackColor = true;
            // 
            // btnModificar
            // 
            this.btnModificar.Location = new System.Drawing.Point(132, 499);
            this.btnModificar.Name = "btnModificar";
            this.btnModificar.Size = new System.Drawing.Size(75, 23);
            this.btnModificar.TabIndex = 7;
            this.btnModificar.Text = "Modificar";
            this.btnModificar.UseVisualStyleBackColor = true;
            // 
            // btnEliminar
            // 
            this.btnEliminar.Location = new System.Drawing.Point(242, 499);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(75, 23);
            this.btnEliminar.TabIndex = 8;
            this.btnEliminar.Text = "Eliminar";
            this.btnEliminar.UseVisualStyleBackColor = true;
            // 
            // btnDetalle
            // 
            this.btnDetalle.Location = new System.Drawing.Point(350, 499);
            this.btnDetalle.Name = "btnDetalle";
            this.btnDetalle.Size = new System.Drawing.Size(108, 23);
            this.btnDetalle.TabIndex = 9;
            this.btnDetalle.Text = "Detalle / fotos";
            this.btnDetalle.UseVisualStyleBackColor = true;
            // 
            // lblBusqueda
            // 
            this.lblBusqueda.AutoSize = true;
            this.lblBusqueda.Location = new System.Drawing.Point(24, 14);
            this.lblBusqueda.Name = "lblBusqueda";
            this.lblBusqueda.Size = new System.Drawing.Size(287, 13);
            this.lblBusqueda.TabIndex = 10;
            this.lblBusqueda.Text = "Buscar por código, nombre, descripción, marca o categoría";
            // 
            // lblResultado
            // 
            this.lblResultado.AutoSize = true;
            this.lblResultado.Location = new System.Drawing.Point(24, 537);
            this.lblResultado.Name = "lblResultado";
            this.lblResultado.Size = new System.Drawing.Size(35, 13);
            this.lblResultado.TabIndex = 11;
            this.lblResultado.Text = "";
            // 
            // FrmCatalogo
            // 
            this.ClientSize = new System.Drawing.Size(873, 561);
            this.Controls.Add(this.lblResultado);
            this.Controls.Add(this.lblBusqueda);
            this.Controls.Add(this.btnDetalle);
            this.Controls.Add(this.btnEliminar);
            this.Controls.Add(this.btnModificar);
            this.Controls.Add(this.btnAgregar);
            this.Controls.Add(this.btnLimpiar);
            this.Controls.Add(this.btnBuscar);
            this.Controls.Add(this.cboCategoria);
            this.Controls.Add(this.cboMarca);
            this.Controls.Add(this.dgvArticulos);
            this.Controls.Add(this.txtBusqueda);
            this.Name = "FrmCatalogo";
            ((System.ComponentModel.ISupportInitialize)(this.dgvArticulos)).EndInit();
            this.dgvArticulos.SelectionChanged += dgvArticulos_SelectionChanged;
            this.dgvArticulos.CellDoubleClick += dgvArticulos_CellDoubleClick;
            this.btnBuscar.Click += btnBuscar_Click;
            this.btnLimpiar.Click += btnLimpiar_Click;
            this.btnAgregar.Click += btnAgregar_Click;
            this.btnModificar.Click += btnModificar_Click;
            this.btnEliminar.Click += btnEliminar_Click;
            this.btnDetalle.Click += btnDetalle_Click;
            this.Shown += FrmCatalogo_Shown;
            this.Text = "Catálogo de artículos";
            this.lblResultado.Text = "";
            this.txtBusqueda.MaxLength = 150;
            this.cboMarca.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboCategoria.DropDownStyle = ComboBoxStyle.DropDownList;
            this.dgvArticulos.ReadOnly = true;
            this.dgvArticulos.MultiSelect = false;
            this.dgvArticulos.AllowUserToAddRows = false;
            this.dgvArticulos.AllowUserToDeleteRows = false;
            this.dgvArticulos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvArticulos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.AcceptButton = btnBuscar;
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
