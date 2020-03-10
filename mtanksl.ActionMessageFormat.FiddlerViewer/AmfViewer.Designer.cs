namespace mtanksl.ActionMessageFormat.FiddlerViewer
{
    partial class AmfViewer
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.treeListViewOutput = new BrightIdeasSoftware.TreeListView();
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn3 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            ((System.ComponentModel.ISupportInitialize)(this.treeListViewOutput)).BeginInit();
            this.SuspendLayout();
            // 
            // treeListViewOutput
            // 
            this.treeListViewOutput.AllColumns.Add(this.olvColumn1);
            this.treeListViewOutput.AllColumns.Add(this.olvColumn2);
            this.treeListViewOutput.AllColumns.Add(this.olvColumn3);
            this.treeListViewOutput.CellEditUseWholeCell = false;
            this.treeListViewOutput.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn1,
            this.olvColumn2,
            this.olvColumn3});
            this.treeListViewOutput.Cursor = System.Windows.Forms.Cursors.Default;
            this.treeListViewOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeListViewOutput.HideSelection = false;
            this.treeListViewOutput.Location = new System.Drawing.Point(0, 0);
            this.treeListViewOutput.Name = "treeListViewOutput";
            this.treeListViewOutput.ShowGroups = false;
            this.treeListViewOutput.Size = new System.Drawing.Size(1200, 600);
            this.treeListViewOutput.TabIndex = 1;
            this.treeListViewOutput.UseCompatibleStateImageBehavior = false;
            this.treeListViewOutput.View = System.Windows.Forms.View.Details;
            this.treeListViewOutput.VirtualMode = true;
            // 
            // olvColumn1
            // 
            this.olvColumn1.AspectName = "Name";
            this.olvColumn1.Sortable = false;
            this.olvColumn1.Text = "Name";
            this.olvColumn1.Width = 350;
            // 
            // olvColumn2
            // 
            this.olvColumn2.AspectName = "Value";
            this.olvColumn2.Sortable = false;
            this.olvColumn2.Text = "Value";
            this.olvColumn2.Width = 250;
            // 
            // olvColumn3
            // 
            this.olvColumn3.AspectName = "Type";
            this.olvColumn3.Sortable = false;
            this.olvColumn3.Text = "Type";
            this.olvColumn3.Width = 250;
            // 
            // AmfViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeListViewOutput);
            this.Name = "AmfViewer";
            this.Size = new System.Drawing.Size(1200, 600);
            ((System.ComponentModel.ISupportInitialize)(this.treeListViewOutput)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private BrightIdeasSoftware.TreeListView treeListViewOutput;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private BrightIdeasSoftware.OLVColumn olvColumn2;
        private BrightIdeasSoftware.OLVColumn olvColumn3;
    }
}
