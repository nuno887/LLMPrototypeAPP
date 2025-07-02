using MyAgent.Core;
using System;
using System.Data.SqlClient;
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

            string connectionString = "Server=localhost\\MSSQLSERVER02;Database=GovernmentDocs;Trusted_Connection=True;";
            string sqlModel = "magistral:latest";
            string normalModel = "magistral:latest";
            string ragModel = "magistral:latest";

            // Passing the same SQL Server connection string for both structured and vector data
            _agent = new Agent(connectionString, sqlModel, normalModel, connectionString, ragModel);




            btnImportJson.Enabled = false;
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
            btnAsk.Enabled = false;
            try
            {
                string question = richTextBoxQuestion.Text.Trim();

                if (string.IsNullOrEmpty(question))
                {
                    MessageBox.Show("Please enter a question.");
                    return;
                }

                richTextBoxAnswer.Text = "Processing...";
                richTextBoxNotes.Text = "";

                var result = await _agent.AskAsync(question);

                richTextBoxAnswer.Text = result.Answer;
                richTextBoxNotes.Text = result.Notes;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[Unhandled Error]\n{ex.Message}\n\n{ex.StackTrace}");
            }
            finally
            {
                btnAsk.Enabled = true;
            
            }
        }


        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void richTextBoxNotes_TextChanged(object sender, EventArgs e)
        {

        }


        private async void btnImportJson_Click(object sender, EventArgs e)
        {
            btnImportJson.Enabled = false; 

            string jsonFolder = @"C:\Users\nuno.ms.goncalves\Desktop\SpaCy_NET\DATA\JSON";
            string connectionString = "Server=localhost\\MSSQLSERVER02;Database=GovernmentDocs;Trusted_Connection=True;";

            var processor = new RelatorioProcessor(connectionString);

            try
            {
                string[] jsonFiles = Directory.GetFiles(jsonFolder, "*.json");

                if (jsonFiles.Length == 0)
                {
                    MessageBox.Show("No JSON files found in the directory.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                foreach (string jsonFilePath in jsonFiles)
                {
                    await processor.ProcessJsonFileAsync(jsonFilePath);
                }

                MessageBox.Show($"Import completed successfully for {jsonFiles.Length} files.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during import: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnImportJson.Enabled = true; // Re-enable the button even if there's an error
            }
        }


        private void btnTestConnection_Click(object sender, EventArgs e)
        {
            string connectionString = "Server=localhost\\MSSQLSERVER02;Database=GovernmentDocs;Trusted_Connection=True;";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    MessageBox.Show("Connection successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
