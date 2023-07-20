# Digitalizador VUCEM API
Integra tu aplicación de Comercio Exterior, Aduanas , Facturacion , cumplimento, etc. para cumplir con las especificaciones técnicas de digitalización de documentos en la Ventanilla Digital Mexicana de Comercio Exterior (VUCEM) con nuetro API de una manera sencilla y facil.

## Creación de una cuenta
Antes de consumir el Digitalizador VUCEM API, es necesario crear una cuenta siguiendo los pasos siguientes:

1. Crear una cuenta
    * Requiere correo electronico.
    * Para el registro solicite el URL de prueba a soporte@quiana.app.
2. Confirmar la cuenta
3. Agregar saldo (Produccion)
4. Obtener Token con usuario y password registrado

## Solicitar Token
Para realizar la petición es necesario haber creado y confirmado la cuenta, para solicitar el token envie su usuario y password como se muestra en el siguiente ejemplo.
```charp
//Get Token
var user = new
{
    usuario = "usuario@dominio.com",
    password = "P@ssword123"
};

HttpClient client = new HttpClient();
var response = await client.PostAsync("https://qa.quiana.app/api/login", new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json"));
//Get Bearer Token
var result = await response.Content.ReadAsStringAsync();
```
### Response
```
{"token":"eyJhb..."}
```
## Enviar archivo para Digitalizar
Es necesario agregar el token en el encabezado antes de enviar cualquier archivo para su digitalización.
```charp
//Upload file
HttpClient client = new HttpClient();
//Add Token
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
### Response
```
{"guid":"6a5fae21-77d5-4a8d","created":"2022-04-02T08:13:35.6720701-06:00","message":"Digitalizacion de documento finalizado, recupere el documento con el identificador (guid)."}
```
## Recuperar o consultar Archivo Digitalizado
Utilice el identificador unico para recuperar el archivo digitalizado, el proceso de digitalizacion tardara dependiendo del tamaño y peso del documento.
```charp
HttpClient client = new HttpClient();
//Add Token
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
### Response
```
{"error":false,"guid":"6a5fae21-77d5-4a8d","updated":"2022-04-13T08:13:35.657","filename":"myarchivo.pdf","message":"Documento digitalizado exitosamente.","digitalizado":"base64String"}
```
## Model de respuesta
```charp
public class Digitalizado
{
    public bool error { get; set; }
    public string guid { get; set; }
    public DateTime created {get; set; }
    public DateTime updated { get; set; }
    public string filename { get;set; }
    public string message { get; set; }
    public string digitalizado { get;set; }
}
```
### QuianaApps
- [IDEA] (https://www.quiana.app/idea)
- [Digitalizador VUCEM] (https://digitalizador.quiana.app/) 
- [Convertidor PDF] (https://convertidor.quiana.app/)
<p align="center">
  <img src="https://convertidor.quiana.app/Content/images/digitalizadorVucem.png?raw=true" alt="DigitalizadorVUCEM"/>
</p>
