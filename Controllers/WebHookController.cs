using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Dialogflow.V2;
using Google.Protobuf;
using System.IO;
using System.Text;
using Google.Protobuf.WellKnownTypes;


namespace WebhookApi.Controllers
{
    [Route("webhook")]
    public class WebhookController : Controller
    {
        private static readonly JsonParser jsonParser = new JsonParser(JsonParser.Settings.Default.WithIgnoreUnknownFields(true));
        private Struct pas;
        private StringBuilder sb; // Declaración global de StringBuilder
        public WebhookController() => sb = new StringBuilder();

        [HttpPost]
        public async Task<JsonResult> GetWebhookResponse()
        {
            #region Conexion DialogFlow
            WebhookRequest request;
            using (var reader = new StreamReader(Request.Body))
            {
                var bodyContent = await reader.ReadToEndAsync();
                request = jsonParser.Parse<WebhookRequest>(bodyContent);
            }

            string intentName = request.QueryResult.Intent.DisplayName;
            pas = request.QueryResult.Parameters;
            #endregion

            #region Definir Intent
            var response = new WebhookResponse();
            sb.Clear();
            switch (intentName)
            {
                case "mostrar_saldo":
                    MostrarSaldo();
                    break;
                case "enviar_dinero":
                    EnviarDinero();
                    break;
                case "Consultar_acuerdoPago":
                case "Solicitar_numeroID":
                    ConsultarSaldoAcuerdoPago();
                    break;
                case "Default Fallback Intent":
                    FallBack();
                    break;
                default:
                    sb.Append("No se reconoció el intent.");
                    break;
            }
            #endregion
            response.FulfillmentText = sb.ToString();
            var hola = sb.ToString().Trim();
            return Json(response);
        }
        #region Consultas
        protected void ConsultarSaldoAcuerdoPago()
        {
            //Parametros requeridos para hacer la transferencia
            string clienteID = pas.Fields.ContainsKey("NumeroID") ? pas.Fields["NumeroID"].ToString().Replace('\"', ' ').Trim().Replace("[", "").Replace("]", "") : "false";
            string tipoAcuerdo = pas.Fields.ContainsKey("TipoAcuerdoPago") ? pas.Fields["TipoAcuerdoPago"].ToString().Replace('\"', ' ').Trim().Replace("[", "").Replace("]", "") : "false";
            string clienteIDContext = pas.Fields.ContainsKey("NumeroID") ? pas.Fields["NumeroID"].ToString().Replace('\"', ' ').Trim() : "NoValido";

            if (clienteID != "false" && tipoAcuerdo != "false")
            {
                try
                {
                    // SIMULAR BD
                    List<string> clientes = new List<string>
                    {
                        "1014480124",
                        "37860255",
                        "3132952842"
                    };
                    // El cliente existe o la cc esta bien dígitada

                    if (clientes.Contains(clienteID))
                    {
                        if (tipoAcuerdo.Equals("tarjeta de credito", StringComparison.CurrentCultureIgnoreCase))
                        {
                            string saldo = new Random().Next(10000, 400000).ToString();
                            sb.Append($"El saldo de tu acuerdo de pago {tipoAcuerdo} es de: {saldo}");
                            sb.Append("¿En que más te puedo ayudar?");
                        }
                        else
                        {
                            double saldo = new Random().Next(100000, 6000000);
                            sb.Append($"El saldo de tu acuerdo de pago {tipoAcuerdo} es de: {saldo}");
                            sb.Append("\n¿En que más te puedo ayudar?");
                        }

                    }
                    else
                    {
                        sb.Append($"El número de identificación no se encuentra en nuestro sistema");
                        sb.Append($"Por favor, indícame tu número de cédula para consultar el acuerdo de pago");
                    }

                }
                catch
                {
                    sb.Append("Lo siento, ahora mismo la información no esta disponible");
                    sb.Append("¿En que más te puedo ayudar?");
                }
            }
            else
            {
                sb.Append("La solicitud no cumple con los parametros establecidos");
                sb.Append("Asegurate que los datos sean correctos");
            }
        }

        protected void MostrarSaldo()
        {
            string cuenta = pas.Fields.ContainsKey("Cuenta") ? pas.Fields["Cuenta"].ToString().Replace('\"', ' ').Trim().Replace("[", "").Replace("]", "") : "NoValido";
            if (cuenta != "NoValido")
            {
                //Consultar el saldo en la bd
                double saldo = new Random().Next(500000, 1000000);
                try
                {
                    sb.Append("Tienes un saldo de " + saldo + " en tu cuenta " + cuenta);
                }
                catch
                {
                    sb.Append("Lo siento, pero ahora mismo la información no esta disponible, intentelo más tarde");
                }
                sb.Append("\n¿En que más te puedo ayudar?");
            }
            else
            {
                sb.Append("La solicitud no cumple con los parametros establecidos");
                sb.Append("Asegurate que los datos sean correctos");
            }
        }

        #endregion
        protected void EnviarDinero()
        {
            //Parametros requeridos para hacer la transferencia
            string importe = pas.Fields.ContainsKey("Importe") ? pas.Fields["Importe"].ToString().Replace('\"', ' ').Trim().Replace("[", "").Replace("]", "") : "NoValido";
            string moneda = pas.Fields.ContainsKey("Moneda") ? pas.Fields["Moneda"].ToString().Replace('\"', ' ').Trim().Replace("[", "").Replace("]", "") : "NoValido";
            string destinatario = pas.Fields.ContainsKey("Destinatario") ? pas.Fields["Destinatario"].ToString().Replace('\"', ' ').Trim().Replace("[", "").Replace("]", "") : "NoValido";
            if (importe != "NoValido" && moneda != "NoValido" && destinatario != "NoValido")
            {
                try
                {
                    //LLAMAR A LA API CORRESPONDIENTE PARA REALIZAR LA TRANSFERENCIA
                    sb.Append($"Se tranfirió {importe} {moneda} al destinatario {destinatario} de manera exitosa");
                    sb.Append("\n¿En que más te puedo ayudar?");
                }
                catch
                {
                    sb.Append("Lo siento, ahora mismo la información no esta disponible");
                    sb.Append("¿En que más le puedo ayudar?");
                }
            }
            else
            {
                sb.Append("La solicitud no cumple con los parametros establecidos");
                sb.Append("Asegurate que los datos sean correctos");
            }
        }
        protected void FallBack()
        {
            sb.Append("Perdón, no he entendido");
            sb.Append("¿puede volver a formularlo?");
        }

    }
}
