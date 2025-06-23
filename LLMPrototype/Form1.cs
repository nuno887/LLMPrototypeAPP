using MyAgent.Core;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Xceed.Words.NET;
namespace LLMPrototype
{
    public partial class Form1 : Form
    {

        private LLM_SQL _sqlLLM;
        private Agent _agent;

        public Form1()
        {
            InitializeComponent();

            richTextBox1.Visible = true;
            webView21.Visible = false;

            string dbPath = "C:\\Users\\nuno.ms.goncalves\\Desktop\\DataBases\\my_database.db";
            string sqlModel = "llama3";
            string ragApiUrl = "http://127.0.0.1:8000/query";
            string normalModel = "llama3";

            _agent = new Agent(dbPath, sqlModel, ragApiUrl, normalModel);
        }

        private void Form1_Load(object sender, EventArgs e) { }

        private void richTextBox1_TextChanged(object sender, EventArgs e) { }

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

        private void ShowText(string content)
        {
            richTextBox1.Visible = true;
            webView21.Visible = false;
            richTextBox1.Text = content;
        }

        private void ShowPdfInWebView(string path)
        {
            richTextBox1.Visible = false;
            webView21.Visible = true;
            string uri = "file:///" + path.Replace("\\", "/");
            webView21.Source = new Uri(uri);
        }

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

        private void richTextBoxResponse_TextChanged(object sender, EventArgs e) { }

        private void richTextBoxQuestion_TextChanged(object sender, EventArgs e) { }

        private async void btnAsk_Click(object sender, EventArgs e)
        {
            try
            {
                string question = richTextBoxQuestion.Text.Trim();

                if (string.IsNullOrEmpty(question))
                {
                    MessageBox.Show("Please enter a question.");
                    return;
                }

                richTextBoxResponse.Text = "Processing...";

                string response = await _agent.AskAsync(question);

                richTextBoxResponse.Text = response;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[Unhandled Error]\n{ex.Message}\n\n{ex.StackTrace}");
            }
        }


    }
}
