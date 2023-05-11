# Digitalizador VUCEM API
Cumple con las especificaciones técnicas de digitalización de documentos para la Ventanilla Digital Mexicana de Comercio Exterior (VUCEM).
### QuianaApps
- [IDEA] (https://www.quiana.app/idea)
- [Digitalizador VUCEM] (https://digitalizador.quiana.app/) 
- [Convertidor PDF] (https://convertidor.quiana.app/)
## Creación de una cuenta
Antes de consumir el Digitalizador VUCEM API, es necesario crear una cuenta siguiendo los pasos siguientes:

1. Crear una cuenta
    * Requiere correo electronico.
    * El EndPoint (URL) de prueba es proporcionado por el proveedor.
2. Confirmar la cuenta
3. Agregar saldo (Produccion)
4. Obtener Token con usuario y password registrado

## Solicitar Token
```charp
//Get Token
var user = new
{
    usuario = "usuario@dominio.com",
    password = "P@ssword23"
};

HttpClient client = new HttpClient();
var response = await client.PostAsync("https://qa.quiana.app/api/login", new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json"));
//Get Bearer Token
var result = await response.Content.ReadAsStringAsync();
```
## Enviar archivo para Digitalizar
```charp
//Upload file
HttpClient client = new HttpClient();
client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "YourToken");

using(var multipartFormContent = new MultipartFormDataContent())
{
    var fs = new StreamContent(File.OpenRead("myarchivo.pdf"));
    fs.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");

    multipartFormContent.Add(fs, name:"file", fileName: Path.GetFileName("myarchivo.pdf"));

    var response = await client.PostAsync("https://qa.quiana.app/api/digitalizador", multipartFormContent);
    if(response.IsSuccessStatusCode)
    {
        //Get
        var result = await response.Content.ReadAsStringAsync();
    }
}
```
## Recuperar o consultar Archivo Digitalizado
```charp
HttpClient client = new HttpClient();
client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "YourToken");

//guid : identificador del archivo
var response = await client.GetAsync($"https://qa.quiana.app/api/digitalizador?guid=c123019d-0fc2-442f-8134-41431cb9d123c");
if(response.IsSuccessStatusCode)
{
    var result = await response.Content.ReadAsStringAsync();
    var fileResult = JsonSerializer.Deserialize<Digitalizado>(result);
    var data = Convert.FromBase64String(fileResult.digitalizado);
    File.WriteAllBytes("MyArchivoDigitalizado_vu.pdf")), data );
}
```
## Model de respuesta
```charp
public class Digitalizado
{
    public bool error { get; set; }
    public string guid { get; set; }
    public DateTime updated { get; set; }
    public string filename { get;set; }
    public string message { get; set; }
    public string digitalizado { get;set; }
}
```
