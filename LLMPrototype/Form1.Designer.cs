namespace LLMPrototype
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
            richTextBox1 = new RichTextBox();
            btnBrowse = new Button();
            webView21 = new Microsoft.Web.WebView2.WinForms.WebView2();
            richTextBoxQuestion = new RichTextBox();
            btnAsk = new Button();
            tabControl1 = new TabControl();
            Answer = new TabPage();
            Notes = new TabPage();
            richTextBoxAnswer = new RichTextBox();
            richTextBoxNotes = new RichTextBox();
            ((System.ComponentModel.ISupportInitialize)webView21).BeginInit();
            tabControl1.SuspendLayout();
            Answer.SuspendLayout();
            Notes.SuspendLayout();
            SuspendLayout();
            // 
            // richTextBox1
            // 
            richTextBox1.Location = new Point(782, 12);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(444, 567);
            richTextBox1.TabIndex = 0;
            richTextBox1.Text = "";
            richTextBox1.TextChanged += richTextBox1_TextChanged;
            // 
            // btnBrowse
            // 
            btnBrowse.Location = new Point(806, 602);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(75, 23);
            btnBrowse.TabIndex = 1;
            btnBrowse.Text = "Browse File";
            btnBrowse.UseVisualStyleBackColor = true;
            btnBrowse.Click += btnBrowse_Click;
            // 
            // webView21
            // 
            webView21.AllowExternalDrop = true;
            webView21.CreationProperties = null;
            webView21.DefaultBackgroundColor = Color.White;
            webView21.Location = new Point(763, 12);
            webView21.Name = "webView21";
            webView21.Size = new Size(485, 567);
            webView21.TabIndex = 2;
            webView21.ZoomFactor = 1D;
            // 
            // richTextBoxQuestion
            // 
            richTextBoxQuestion.Location = new Point(32, 555);
            richTextBoxQuestion.Name = "richTextBoxQuestion";
            richTextBoxQuestion.Size = new Size(622, 86);
            richTextBoxQuestion.TabIndex = 4;
            richTextBoxQuestion.Text = "";
            richTextBoxQuestion.TextChanged += richTextBoxQuestion_TextChanged;
            // 
            // btnAsk
            // 
            btnAsk.Location = new Point(305, 659);
            btnAsk.Name = "btnAsk";
            btnAsk.Size = new Size(75, 23);
            btnAsk.TabIndex = 5;
            btnAsk.Text = "Ask Agent";
            btnAsk.UseVisualStyleBackColor = true;
            btnAsk.Click += btnAsk_Click;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(Answer);
            tabControl1.Controls.Add(Notes);
            tabControl1.Location = new Point(33, 29);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(621, 520);
            tabControl1.TabIndex = 6;
            // 
            // Answer
            // 
            Answer.Controls.Add(richTextBoxAnswer);
            Answer.Location = new Point(4, 24);
            Answer.Name = "Answer";
            Answer.Padding = new Padding(3);
            Answer.Size = new Size(613, 492);
            Answer.TabIndex = 0;
            Answer.Text = "Answer";
            Answer.UseVisualStyleBackColor = true;
            // 
            // Notes
            // 
            Notes.Controls.Add(richTextBoxNotes);
            Notes.Location = new Point(4, 24);
            Notes.Name = "Notes";
            Notes.Padding = new Padding(3);
            Notes.Size = new Size(613, 492);
            Notes.TabIndex = 1;
            Notes.Text = "Notes";
            Notes.UseVisualStyleBackColor = true;
            // 
            // richTextBoxAnswer
            // 
            richTextBoxAnswer.Location = new Point(0, 0);
            richTextBoxAnswer.Name = "richTextBoxAnswer";
            richTextBoxAnswer.Size = new Size(613, 496);
            richTextBoxAnswer.TabIndex = 0;
            richTextBoxAnswer.Text = "";
            richTextBoxAnswer.TextChanged += richTextBox2_TextChanged;
            // 
            // richTextBoxNotes
            // 
            richTextBoxNotes.Location = new Point(0, 0);
            richTextBoxNotes.Name = "richTextBoxNotes";
            richTextBoxNotes.Size = new Size(613, 496);
            richTextBoxNotes.TabIndex = 0;
            richTextBoxNotes.Text = "";
            richTextBoxNotes.TextChanged += richTextBoxNotes_TextChanged;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1278, 741);
            Controls.Add(tabControl1);
            Controls.Add(btnAsk);
            Controls.Add(richTextBoxQuestion);
            Controls.Add(webView21);
            Controls.Add(btnBrowse);
            Controls.Add(richTextBox1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)webView21).EndInit();
            tabControl1.ResumeLayout(false);
            Answer.ResumeLayout(false);
            Notes.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private RichTextBox richTextBox1;
        private Button btnBrowse;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView21;
        private RichTextBox richTextBoxQuestion;
        private Button btnAsk;
        private TabControl tabControl1;
        private TabPage Answer;
        private TabPage Notes;
        private RichTextBox richTextBoxAnswer;
        private RichTextBox richTextBoxNotes;
    }
}
