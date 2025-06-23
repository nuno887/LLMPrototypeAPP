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
            richTextBoxResponse = new RichTextBox();
            richTextBoxQuestion = new RichTextBox();
            btnAsk = new Button();
            ((System.ComponentModel.ISupportInitialize)webView21).BeginInit();
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
            // richTextBoxResponse
            // 
            richTextBoxResponse.Location = new Point(32, 12);
            richTextBoxResponse.Name = "richTextBoxResponse";
            richTextBoxResponse.ReadOnly = true;
            richTextBoxResponse.Size = new Size(622, 509);
            richTextBoxResponse.TabIndex = 3;
            richTextBoxResponse.Text = "";
            richTextBoxResponse.TextChanged += richTextBoxResponse_TextChanged;
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
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1278, 741);
            Controls.Add(btnAsk);
            Controls.Add(richTextBoxQuestion);
            Controls.Add(richTextBoxResponse);
            Controls.Add(webView21);
            Controls.Add(btnBrowse);
            Controls.Add(richTextBox1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)webView21).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private RichTextBox richTextBox1;
        private Button btnBrowse;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView21;
        private RichTextBox richTextBoxResponse;
        private RichTextBox richTextBoxQuestion;
        private Button btnAsk;
    }
}
