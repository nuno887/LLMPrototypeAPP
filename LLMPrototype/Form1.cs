using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Xceed.Words.NET;

namespace LLMPrototype;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();

        // Initially, only the richTextBox is shown
        richTextBox1.Visible = true;
        webView21.Visible = false;
    }

    private void Form1_Load(object sender, EventArgs e)
    {
    }

    private void richTextBox1_TextChanged(object sender, EventArgs e)
    {
    }

    private void btnBrowse_Click(object sender, EventArgs e)
    {
        using OpenFileDialog dialog = new OpenFileDialog();
        dialog.Filter = "Text files (*.txt)|*.txt|Word Documents (*.docx)|*.docx|PDF Files (*.pdf)|*.pdf|All files (*.*)|*.*";

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            string path = dialog.FileName;
            string ext = Path.GetExtension(path).ToLower();

            try
            {
                switch (ext)
                {
                    case ".txt":
                        ShowText(File.ReadAllText(path));
                        break;

                    case ".docx":
                        ShowText(ReadDocx(path));
                        break;

                    case ".pdf":
                        ShowPdfInWebView(path);
                        break;

                    default:
                        MessageBox.Show("Unsupported file type.");
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening file: " + ex.Message);
            }
        }
    }

    // Show text content in RichTextBox
    private void ShowText(string content)
    {
        richTextBox1.Visible = true;
        webView21.Visible = false;
        richTextBox1.Text = content;
    }

    // Show PDF file in WebView2
    private void ShowPdfInWebView(string path)
    {
        richTextBox1.Visible = false;
        webView21.Visible = true;

        string uri = "file:///" + path.Replace("\\", "/");
        webView21.Source = new Uri(uri);
    }

    // Read .docx files using Xceed.Words.NET
    private string ReadDocx(string path)
    {
        try
        {
            using var doc = DocX.Load(path);
            return doc.Text;
        }
        catch (Exception ex)
        {
            return $"Error reading .docx file: {ex.Message}";
        }
    }
}
