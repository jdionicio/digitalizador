# Digitalizador
Digitalizador VUCEM API
1.- Crear una cuenta en: https://927d-2806-1016-e-d5a-10df-6c41-21f6-2fae.ngrok-free.app
    *Requiere correo electronico
2.- Confirmar la cuenta
3.- Agregar saldo (Produccion)
4.- Obtener Token con usuario y password registrado

//Get Token
var user = new
{
    usuario = "usuario@dominio.com",
    password = "P@ssword23"
};

HttpClient client = new HttpClient();
var response = await client.PostAsync("https://927d-2806-1016-e-d5a-10df-6c41-21f6-2fae.ngrok-free.app/api/login", new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json"));
//Get Bearer Token
var result = await response.Content.ReadAsStringAsync();

5.- Enviar archivo para Digitalizar

//Upload file
HttpClient client = new HttpClient();
client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "YourToken");

using(var multipartFormContent = new MultipartFormDataContent())
{
    var fs = new StreamContent(File.OpenRead("C:\myarchivo.pdf"));
    fs.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");

    multipartFormContent.Add(fs, name:"file", fileName: Path.GetFileName("C:\myarchivo.pdf"));

    var response = await client.PostAsync("https://927d-2806-1016-e-d5a-10df-6c41-21f6-2fae.ngrok-free.app/api/digitalizador", multipartFormContent);
    if(response.IsSuccessStatusCode)
    {
        //Get
        var result = await response.Content.ReadAsStringAsync();
    }
}

6.- Consultar Archivo Digitalizado

HttpClient client = new HttpClient();
client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "YourToken");

//guid : identificador del archivo
var response = await client.GetAsync($"https://927d-2806-1016-e-d5a-10df-6c41-21f6-2fae.ngrok-free.app/api/digitalizador?guid=c123019d-0fc2-442f-8134-41431cb9d123c");
if(response.IsSuccessStatusCode)
{
    var result = await response.Content.ReadAsStringAsync();
    var fileResult = JsonSerializer.Deserialize<Digitalizado>(result);
    var data = Convert.FromBase64String(fileResult.digitalizado);
    File.WriteAllBytes("MyArchivoDigitalizado_vu.pdf")), data );
}

public class Digitalizado
{
    public bool error { get; set; }
    public string guid { get; set; }
    public DateTime updated { get; set; }
    public string filename { get;set; }
    public string message { get; set; }
    public string digitalizado { get;set; }
}
