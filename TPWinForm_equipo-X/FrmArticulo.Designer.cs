using System.Windows.Forms;
namespace TPWinForm_equipo_X
{
    partial class FrmArticulo
    {
        private System.Windows.Forms.Label lblcodigo;
        private System.Windows.Forms.TextBox codigo;
        private System.Windows.Forms.Label lblnombre;
        private System.Windows.Forms.TextBox nombre;
        private System.Windows.Forms.Label lbldescripcion;
        private System.Windows.Forms.TextBox descripcion;
        private System.Windows.Forms.Label lblmarca;
        private System.Windows.Forms.ComboBox marca;
        private System.Windows.Forms.Label lblcategoria;
        private System.Windows.Forms.ComboBox categoria;
        private System.Windows.Forms.Label lblprecio;
        private System.Windows.Forms.NumericUpDown precio;
        private System.Windows.Forms.Label lblimagenes;
        private System.Windows.Forms.ListBox imagenes;
        private System.Windows.Forms.TextBox enlace;
        private System.Windows.Forms.Button nuevaMarca;
        private System.Windows.Forms.Button nuevaCategoria;
        private System.Windows.Forms.Button agregarEnlace;
        private System.Windows.Forms.Button agregarFotos;
        private System.Windows.Forms.Button verFotos;
        private System.Windows.Forms.Button quitar;
        private System.Windows.Forms.Button guardar;
        private System.Windows.Forms.Button cancelar;
        private System.Windows.Forms.Label error;

        private void InitializeComponent()
        {
            this.lblcodigo = new System.Windows.Forms.Label();
            this.codigo = new System.Windows.Forms.TextBox();
            this.lblnombre = new System.Windows.Forms.Label();
            this.nombre = new System.Windows.Forms.TextBox();
            this.lbldescripcion = new System.Windows.Forms.Label();
            this.descripcion = new System.Windows.Forms.TextBox();
            this.lblmarca = new System.Windows.Forms.Label();
            this.marca = new System.Windows.Forms.ComboBox();
            this.lblcategoria = new System.Windows.Forms.Label();
            this.categoria = new System.Windows.Forms.ComboBox();
            this.lblprecio = new System.Windows.Forms.Label();
            this.precio = new System.Windows.Forms.NumericUpDown();
            this.lblimagenes = new System.Windows.Forms.Label();
            this.imagenes = new System.Windows.Forms.ListBox();
            this.enlace = new System.Windows.Forms.TextBox();
            this.nuevaMarca = new System.Windows.Forms.Button();
            this.nuevaCategoria = new System.Windows.Forms.Button();
            this.agregarEnlace = new System.Windows.Forms.Button();
            this.agregarFotos = new System.Windows.Forms.Button();
            this.verFotos = new System.Windows.Forms.Button();
            this.quitar = new System.Windows.Forms.Button();
            this.guardar = new System.Windows.Forms.Button();
            this.cancelar = new System.Windows.Forms.Button();
            this.error = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.precio)).BeginInit();
            this.SuspendLayout();
            // 
            // lblcodigo
            // 
            this.lblcodigo.Location = new System.Drawing.Point(24, 71);
            this.lblcodigo.Name = "lblcodigo";
            this.lblcodigo.Size = new System.Drawing.Size(210, 20);
            this.lblcodigo.TabIndex = 0;
            this.lblcodigo.Text = "Código";
            // 
            // codigo
            // 
            this.codigo.Location = new System.Drawing.Point(24, 95);
            this.codigo.MaxLength = 50;
            this.codigo.Name = "codigo";
            this.codigo.Size = new System.Drawing.Size(210, 20);
            this.codigo.TabIndex = 1;
            // 
            // lblnombre
            // 
            this.lblnombre.Location = new System.Drawing.Point(254, 71);
            this.lblnombre.Name = "lblnombre";
            this.lblnombre.Size = new System.Drawing.Size(460, 20);
            this.lblnombre.TabIndex = 2;
            this.lblnombre.Text = "Nombre";
            // 
            // nombre
            // 
            this.nombre.Location = new System.Drawing.Point(254, 95);
            this.nombre.MaxLength = 50;
            this.nombre.Name = "nombre";
            this.nombre.Size = new System.Drawing.Size(460, 20);
            this.nombre.TabIndex = 3;
            // 
            // lbldescripcion
            // 
            this.lbldescripcion.Location = new System.Drawing.Point(24, 135);
            this.lbldescripcion.Name = "lbldescripcion";
            this.lbldescripcion.Size = new System.Drawing.Size(690, 20);
            this.lbldescripcion.TabIndex = 4;
            this.lbldescripcion.Text = "Descripción";
            // 
            // descripcion
            // 
            this.descripcion.Location = new System.Drawing.Point(24, 159);
            this.descripcion.MaxLength = 150;
            this.descripcion.Multiline = true;
            this.descripcion.Name = "descripcion";
            this.descripcion.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.descripcion.Size = new System.Drawing.Size(690, 72);
            this.descripcion.TabIndex = 5;
            // 
            // lblmarca
            // 
            this.lblmarca.Location = new System.Drawing.Point(24, 245);
            this.lblmarca.Name = "lblmarca";
            this.lblmarca.Size = new System.Drawing.Size(205, 20);
            this.lblmarca.TabIndex = 6;
            this.lblmarca.Text = "Marca";
            // 
            // marca
            // 
            this.marca.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.marca.Location = new System.Drawing.Point(24, 269);
            this.marca.Name = "marca";
            this.marca.Size = new System.Drawing.Size(205, 21);
            this.marca.TabIndex = 7;
            // 
            // lblcategoria
            // 
            this.lblcategoria.Location = new System.Drawing.Point(374, 245);
            this.lblcategoria.Name = "lblcategoria";
            this.lblcategoria.Size = new System.Drawing.Size(205, 20);
            this.lblcategoria.TabIndex = 8;
            this.lblcategoria.Text = "Categoría";
            // 
            // categoria
            // 
            this.categoria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.categoria.Location = new System.Drawing.Point(374, 269);
            this.categoria.Name = "categoria";
            this.categoria.Size = new System.Drawing.Size(205, 21);
            this.categoria.TabIndex = 9;
            // 
            // lblprecio
            // 
            this.lblprecio.Location = new System.Drawing.Point(24, 309);
            this.lblprecio.Name = "lblprecio";
            this.lblprecio.Size = new System.Drawing.Size(325, 20);
            this.lblprecio.TabIndex = 10;
            this.lblprecio.Text = "Precio";
            // 
            // precio
            // 
            this.precio.DecimalPlaces = 4;
            this.precio.Location = new System.Drawing.Point(24, 333);
            this.precio.Maximum = new decimal(new int[] {
            -1,
            2147483647,
            0,
            262144});
            this.precio.Name = "precio";
            this.precio.Size = new System.Drawing.Size(325, 20);
            this.precio.TabIndex = 11;
            this.precio.ThousandsSeparator = true;
            // 
            // lblimagenes
            // 
            this.lblimagenes.Location = new System.Drawing.Point(24, 373);
            this.lblimagenes.Name = "lblimagenes";
            this.lblimagenes.Size = new System.Drawing.Size(690, 20);
            this.lblimagenes.TabIndex = 12;
            this.lblimagenes.Text = "Imágenes";
            // 
            // imagenes
            // 
            this.imagenes.HorizontalScrollbar = true;
            this.imagenes.IntegralHeight = false;
            this.imagenes.Location = new System.Drawing.Point(24, 397);
            this.imagenes.Name = "imagenes";
            this.imagenes.Size = new System.Drawing.Size(690, 96);
            this.imagenes.TabIndex = 13;
            // 
            // enlace
            // 
            this.enlace.Location = new System.Drawing.Point(24, 503);
            this.enlace.MaxLength = 1000;
            this.enlace.Name = "enlace";
            this.enlace.Size = new System.Drawing.Size(525, 20);
            this.enlace.TabIndex = 14;
            // 
            // nuevaMarca
            // 
            this.nuevaMarca.Location = new System.Drawing.Point(235, 267);
            this.nuevaMarca.Name = "nuevaMarca";
            this.nuevaMarca.Size = new System.Drawing.Size(114, 32);
            this.nuevaMarca.TabIndex = 15;
            this.nuevaMarca.Text = "Nueva marca";
            this.nuevaMarca.UseVisualStyleBackColor = true;
            // 
            // nuevaCategoria
            // 
            this.nuevaCategoria.Location = new System.Drawing.Point(585, 267);
            this.nuevaCategoria.Name = "nuevaCategoria";
            this.nuevaCategoria.Size = new System.Drawing.Size(129, 32);
            this.nuevaCategoria.TabIndex = 16;
            this.nuevaCategoria.Text = "Nueva categoría";
            this.nuevaCategoria.UseVisualStyleBackColor = true;
            // 
            // agregarEnlace
            // 
            this.agregarEnlace.Location = new System.Drawing.Point(559, 501);
            this.agregarEnlace.Name = "agregarEnlace";
            this.agregarEnlace.Size = new System.Drawing.Size(155, 32);
            this.agregarEnlace.TabIndex = 17;
            this.agregarEnlace.Text = "Agregar URL";
            this.agregarEnlace.UseVisualStyleBackColor = true;
            // 
            // agregarFotos
            // 
            this.agregarFotos.Location = new System.Drawing.Point(24, 541);
            this.agregarFotos.Name = "agregarFotos";
            this.agregarFotos.Size = new System.Drawing.Size(228, 34);
            this.agregarFotos.TabIndex = 18;
            this.agregarFotos.Text = "Fotos de la PC";
            this.agregarFotos.UseVisualStyleBackColor = true;
            // 
            // verFotos
            // 
            this.verFotos.Location = new System.Drawing.Point(262, 541);
            this.verFotos.Name = "verFotos";
            this.verFotos.Size = new System.Drawing.Size(150, 34);
            this.verFotos.TabIndex = 19;
            this.verFotos.Text = "Ver foto";
            this.verFotos.UseVisualStyleBackColor = true;
            // 
            // quitar
            // 
            this.quitar.Location = new System.Drawing.Point(422, 541);
            this.quitar.Name = "quitar";
            this.quitar.Size = new System.Drawing.Size(155, 34);
            this.quitar.TabIndex = 20;
            this.quitar.Text = "Quitar";
            this.quitar.UseVisualStyleBackColor = true;
            // 
            // guardar
            // 
            this.guardar.Location = new System.Drawing.Point(394, 667);
            this.guardar.Name = "guardar";
            this.guardar.Size = new System.Drawing.Size(170, 34);
            this.guardar.TabIndex = 21;
            this.guardar.Text = "Guardar";
            this.guardar.UseVisualStyleBackColor = true;
            // 
            // cancelar
            // 
            this.cancelar.Location = new System.Drawing.Point(584, 667);
            this.cancelar.Name = "cancelar";
            this.cancelar.Size = new System.Drawing.Size(130, 34);
            this.cancelar.TabIndex = 22;
            this.cancelar.Text = "Cancelar";
            this.cancelar.UseVisualStyleBackColor = true;
            // 
            // error
            // 
            this.error.Location = new System.Drawing.Point(24, 585);
            this.error.Name = "error";
            this.error.Size = new System.Drawing.Size(690, 66);
            this.error.TabIndex = 23;
            // 
            // FrmArticulo
            // 
            this.ClientSize = new System.Drawing.Size(740, 720);
            this.Controls.Add(this.lblcodigo);
            this.Controls.Add(this.codigo);
            this.Controls.Add(this.lblnombre);
            this.Controls.Add(this.nombre);
            this.Controls.Add(this.lbldescripcion);
            this.Controls.Add(this.descripcion);
            this.Controls.Add(this.lblmarca);
            this.Controls.Add(this.marca);
            this.Controls.Add(this.lblcategoria);
            this.Controls.Add(this.categoria);
            this.Controls.Add(this.lblprecio);
            this.Controls.Add(this.precio);
            this.Controls.Add(this.lblimagenes);
            this.Controls.Add(this.imagenes);
            this.Controls.Add(this.enlace);
            this.Controls.Add(this.nuevaMarca);
            this.Controls.Add(this.nuevaCategoria);
            this.Controls.Add(this.agregarEnlace);
            this.Controls.Add(this.agregarFotos);
            this.Controls.Add(this.verFotos);
            this.Controls.Add(this.quitar);
            this.Controls.Add(this.guardar);
            this.Controls.Add(this.cancelar);
            this.Controls.Add(this.error);
            this.Name = "FrmArticulo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            ((System.ComponentModel.ISupportInitialize)(this.precio)).EndInit();
            this.nuevaMarca.Click += nuevaMarca_Click;
            this.nuevaCategoria.Click += nuevaCategoria_Click;
            this.agregarEnlace.Click += agregarEnlace_Click;
            this.agregarFotos.Click += agregarFotos_Click;
            this.verFotos.Click += verFotos_Click;
            this.imagenes.DoubleClick += imagenes_DoubleClick;
            this.quitar.Click += quitar_Click;
            this.guardar.Click += guardar_Click;
            this.FormClosing += FrmArticulo_FormClosing;
            this.cancelar.DialogResult = DialogResult.Cancel;
            this.CancelButton = cancelar;
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
