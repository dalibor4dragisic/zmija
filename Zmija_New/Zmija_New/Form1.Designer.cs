namespace Zmija_New
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Text = "Form1";


            this.panelContent = new System.Windows.Forms.Panel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.novaIgraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pokreniNovuIgruToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pomocToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rezimToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rucnoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.astarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.automatskoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hamiltonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hibridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelContent
            // 
            this.panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContent.Location = new System.Drawing.Point(0, 24);
            this.panelContent.Name = "panelContent";
            this.panelContent.Size = new System.Drawing.Size(631, 621);
            this.panelContent.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.novaIgraToolStripMenuItem,
            this.rezimToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(631, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // novaIgraToolStripMenuItem
            // 
            this.novaIgraToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pokreniNovuIgruToolStripMenuItem,
            this.pomocToolStripMenuItem});
            this.novaIgraToolStripMenuItem.Name = "novaIgraToolStripMenuItem";
            this.novaIgraToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
            this.novaIgraToolStripMenuItem.Text = "Opcije";
            // 
            // pokreniNovuIgruToolStripMenuItem
            // 
            this.pokreniNovuIgruToolStripMenuItem.Name = "pokreniNovuIgruToolStripMenuItem";
            this.pokreniNovuIgruToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.pokreniNovuIgruToolStripMenuItem.Text = "Pokreni novu igru";
            this.pokreniNovuIgruToolStripMenuItem.Click += new System.EventHandler(this.pokreniNovuIgruToolStripMenuItem_Click);
            // 
            // pomocToolStripMenuItem
            // 
            this.pomocToolStripMenuItem.Name = "pomocToolStripMenuItem";
            this.pomocToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.pomocToolStripMenuItem.Text = "Uputstvo";
            this.pomocToolStripMenuItem.Click += new System.EventHandler(this.pomocToolStripMenuItem_Click);
            // 
            // rezimToolStripMenuItem
            // 
            this.rezimToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rucnoToolStripMenuItem,
            this.astarToolStripMenuItem,
            this.automatskoToolStripMenuItem,
            this.hamiltonToolStripMenuItem,
            this.hibridToolStripMenuItem});
            this.rezimToolStripMenuItem.Name = "rezimToolStripMenuItem";
            this.rezimToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.rezimToolStripMenuItem.Text = "Režim";
            // 
            // rucnoToolStripMenuItem
            // 
            this.rucnoToolStripMenuItem.Name = "rucnoToolStripMenuItem";
            this.rucnoToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.rucnoToolStripMenuItem.Text = "Ručno igranje";
            this.rucnoToolStripMenuItem.Click += new System.EventHandler(this.rezimToolStripMenuItem_Click);
            // 
            // astarToolStripMenuItem
            // 
            this.astarToolStripMenuItem.Name = "astarToolStripMenuItem";
            this.astarToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.astarToolStripMenuItem.Text = "Automatsko - A*";
            this.astarToolStripMenuItem.Click += new System.EventHandler(this.rezimToolStripMenuItem_Click);
            // 
            // automatskoToolStripMenuItem
            // 
            this.automatskoToolStripMenuItem.Name = "automatskoToolStripMenuItem";
            this.automatskoToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.automatskoToolStripMenuItem.Text = "Automatsko";
            this.automatskoToolStripMenuItem.Click += new System.EventHandler(this.rezimToolStripMenuItem_Click);
            // 
            // hamiltonToolStripMenuItem
            // 
            this.hamiltonToolStripMenuItem.Name = "hamiltonToolStripMenuItem";
            this.hamiltonToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.hamiltonToolStripMenuItem.Text = "Automatsko - Hamiltonov ciklus";
            this.hamiltonToolStripMenuItem.Click += new System.EventHandler(this.rezimToolStripMenuItem_Click);
            // 
            // hibridToolStripMenuItem
            // 
            this.hibridToolStripMenuItem.Name = "hibridToolStripMenuItem";
            this.hibridToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.hibridToolStripMenuItem.Text = "Automatsko - hibrid";
            this.hibridToolStripMenuItem.Click += new System.EventHandler(this.rezimToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(631, 645);
            this.Controls.Add(this.panelContent);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Zmija";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Panel panelContent;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem novaIgraToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pokreniNovuIgruToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pomocToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rezimToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rucnoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem astarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem automatskoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hamiltonToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hibridToolStripMenuItem;
    }
}
