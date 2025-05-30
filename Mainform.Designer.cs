namespace Proyecto_Final_1
{
    partial class Mainform
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Mainform));
            label1 = new Label();
            cbmOrigen = new ComboBox();
            cbmDestino = new ComboBox();
            btnBuscar = new Button();
            gridHorarios = new DataGridView();
            txtRespuestaIA = new TextBox();
            label2 = new Label();
            btnBuscarHora = new Button();
            btnTraducir = new Button();
            btnEspanol = new Button();
            ((System.ComponentModel.ISupportInitialize)gridHorarios).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(30, 37);
            label1.Name = "label1";
            label1.Size = new Size(56, 15);
            label1.TabIndex = 0;
            label1.Text = "\"Origen:\"";
            label1.Click += label1_Click;
            // 
            // cbmOrigen
            // 
            cbmOrigen.DropDownStyle = ComboBoxStyle.DropDownList;
            cbmOrigen.FormattingEnabled = true;
            cbmOrigen.Location = new Point(101, 34);
            cbmOrigen.Name = "cbmOrigen";
            cbmOrigen.Size = new Size(97, 23);
            cbmOrigen.TabIndex = 2;
            // 
            // cbmDestino
            // 
            cbmDestino.DropDownStyle = ComboBoxStyle.DropDownList;
            cbmDestino.FormattingEnabled = true;
            cbmDestino.Location = new Point(338, 31);
            cbmDestino.Name = "cbmDestino";
            cbmDestino.Size = new Size(97, 23);
            cbmDestino.TabIndex = 3;
            // 
            // btnBuscar
            // 
            btnBuscar.Location = new Point(469, 23);
            btnBuscar.Name = "btnBuscar";
            btnBuscar.Size = new Size(123, 42);
            btnBuscar.TabIndex = 4;
            btnBuscar.Text = "\"Buscar rutas\"";
            btnBuscar.UseVisualStyleBackColor = true;
            btnBuscar.Click += btnBuscar_Click;
            // 
            // gridHorarios
            // 
            gridHorarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridHorarios.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridHorarios.Location = new Point(12, 71);
            gridHorarios.Name = "gridHorarios";
            gridHorarios.ReadOnly = true;
            gridHorarios.Size = new Size(644, 209);
            gridHorarios.TabIndex = 5;
            // 
            // txtRespuestaIA
            // 
            txtRespuestaIA.BackColor = Color.ForestGreen;
            txtRespuestaIA.Dock = DockStyle.Bottom;
            txtRespuestaIA.ForeColor = Color.Silver;
            txtRespuestaIA.Location = new Point(0, 298);
            txtRespuestaIA.Multiline = true;
            txtRespuestaIA.Name = "txtRespuestaIA";
            txtRespuestaIA.ReadOnly = true;
            txtRespuestaIA.ScrollBars = ScrollBars.Vertical;
            txtRespuestaIA.Size = new Size(1260, 234);
            txtRespuestaIA.TabIndex = 6;
            txtRespuestaIA.TextChanged += txtRespuestaIA_TextChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(246, 34);
            label2.Name = "label2";
            label2.Size = new Size(60, 15);
            label2.TabIndex = 1;
            label2.Text = "\"Destino:\"";
            // 
            // btnBuscarHora
            // 
            btnBuscarHora.BackColor = Color.FromArgb(192, 192, 255);
            btnBuscarHora.Location = new Point(712, 34);
            btnBuscarHora.Name = "btnBuscarHora";
            btnBuscarHora.Size = new Size(169, 59);
            btnBuscarHora.TabIndex = 7;
            btnBuscarHora.Text = "Informacion de horario seleccionado ";
            btnBuscarHora.UseVisualStyleBackColor = false;
            btnBuscarHora.Click += btnBuscarHora_Click;
            // 
            // btnTraducir
            // 
            btnTraducir.BackColor = SystemColors.ActiveCaptionText;
            btnTraducir.Font = new Font("Segoe UI", 13F);
            btnTraducir.ForeColor = Color.Gold;
            btnTraducir.Image = (Image)resources.GetObject("btnTraducir.Image");
            btnTraducir.ImageAlign = ContentAlignment.TopLeft;
            btnTraducir.Location = new Point(712, 99);
            btnTraducir.Name = "btnTraducir";
            btnTraducir.Size = new Size(169, 85);
            btnTraducir.TabIndex = 8;
            btnTraducir.Text = "If you speak english  click here";
            btnTraducir.UseVisualStyleBackColor = false;
            btnTraducir.Click += btnTraducir_Click;
            // 
            // btnEspanol
            // 
            btnEspanol.ForeColor = SystemColors.ControlText;
            btnEspanol.Image = (Image)resources.GetObject("btnEspanol.Image");
            btnEspanol.Location = new Point(712, 211);
            btnEspanol.Name = "btnEspanol";
            btnEspanol.Size = new Size(169, 69);
            btnEspanol.TabIndex = 9;
            btnEspanol.Text = "Volver al Español";
            btnEspanol.UseVisualStyleBackColor = true;
            btnEspanol.Click += btnEspanol_Click;
            // 
            // Mainform
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1260, 532);
            Controls.Add(btnEspanol);
            Controls.Add(btnTraducir);
            Controls.Add(btnBuscarHora);
            Controls.Add(txtRespuestaIA);
            Controls.Add(gridHorarios);
            Controls.Add(btnBuscar);
            Controls.Add(cbmDestino);
            Controls.Add(cbmOrigen);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "Mainform";
            Text = "Form1";
            Load += Mainform_Load;
            ((System.ComponentModel.ISupportInitialize)gridHorarios).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private Label label1;
        private ComboBox cbmOrigen;
        private ComboBox cbmDestino;
        private Button btnBuscar;
        private DataGridView gridHorarios;
        private TextBox txtRespuestaIA;
        private Label label2;
        private Button btnBuscarHora;
        private Button btnTraducir;
        private bool enIngles = false;
        private Button btnEspanol;
    }
}
