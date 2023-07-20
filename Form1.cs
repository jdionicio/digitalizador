using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SNComplete
{
    public partial class Form1 : Form
    {
        private string token = string.Empty;
        private string guid = string.Empty;
        public Form1()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Enviar archivo para la Digitalizacion
        /// 300 DPI y Escala de grises.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnConvertir_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtFile.Text))
            {
                MessageBox.Show("Proporcione un archivo.");
                return;
            }
            
            //Request
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            using(var multipartFormContent = new MultipartFormDataContent())
            {
                var fs = new StreamContent(File.OpenRead(txtFile.Text));
                fs.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");

                multipartFormContent.Add(fs, name:"file", fileName: Path.GetFileName(txtFile.Text));

                var response = await client.PostAsync("https://convertidor.quiana.app/api/digitalizador", multipartFormContent);
                if(response.IsSuccessStatusCode)
                {
                    //Get
                    var result = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JsonSerializer.Deserialize<Digitalizado>(result);
                    guid = jsonResponse.guid;
                }
            }
        }
        /// <summary>
        /// Solicita Token , es necesario tener registrado un usuario
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnGetToken_Click(object sender, EventArgs e)
        {
            //Get Token
            var user = new Auth
            {
                usuario = "youruser@mydomain.com",
                password = "MyP@ssword"
            };
            
            HttpClient client = new HttpClient();
            var response = await client.PostAsync("https://convertidor.quiana.app/api/login", new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json"));
            var result = await response.Content.ReadAsStringAsync();
            var jsonResult = JsonSerializer.Deserialize<Auth>(result);
            token = jsonResult.token;
        }
        /// <summary>
        /// Solicita archivo convertido o digitalizado
        /// Es necesario poner un Delay entre 5 a 15 segundos, dependiendo del tamaño del archivo
        /// y cantidad de paginas.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnGuardar_Click(object sender, EventArgs e)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            await Task.Delay(TimeSpan.FromSeconds(5));
            var response = await client.GetAsync($"https://convertidor.quiana.app/api/digitalizador?guid={guid}");
            if(response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var fileResult = JsonSerializer.Deserialize<Digitalizado>(result);
                var data = Convert.FromBase64String(fileResult.digitalizado);
                File.WriteAllBytes(Path.Combine(Path.GetDirectoryName(txtFile.Text),string.Concat(Path.GetFileNameWithoutExtension(txtFile.Text),"_vu.pdf")), data );
            }
        }
    }
    public class Auth
    {
        public string usuario { get; set; }
        public string password { get; set; }
        public string token { get; set; }
    }
    public class Digitalizado
    {
        public bool error { get; set; }
        public string guid { get; set; }
        public DateTime? created { get; set; }
        public DateTime? updated { get; set; }
        public string filename { get;set; }
        public string message { get; set; }
        public string digitalizado { get;set; }
    }
}
