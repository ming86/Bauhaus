using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bauhaus.Helpers
{
    public static class ViewsHelper
    {
        /// <summary>
        /// Returns Descriptive String of order Status
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="status">Current order Status</param>
        /// <returns>Html String containing status description</returns>
        public static MvcHtmlString StatusResolver(this HtmlHelper helper, int status)
        {
            TagBuilder text = new TagBuilder("p");
            string translation;
            switch (status)
            {
                case 10:
                    translation = "Retained";
                    break;
                case 11:
                    translation = "Date Error";
                    break;
                case 17:
                    translation = "Manual Block";
                    break;
                case 20:
                    translation = "OK (Waiting for DSS)";
                    break;
                case 30:
                    translation = "DSS Assigned";
                    break;
                case 40:
                    translation = "Planned";
                    break;
                case 50:
                    translation = "Invoice Created";
                    break;
                case 99:
                    translation = "Dropped";
                    break;
                default:
                    translation = "Not Available";
                    break;
            }
            text.SetInnerText(translation);
            return new MvcHtmlString(text.ToString());
        }


        public static MvcHtmlString ResolveProgressBar(this HtmlHelper helper, int status)
        {
            TagBuilder div = new TagBuilder("div");
            string type = "";
            int value;
            
            switch (status)
            {
                case 10:
                    type = "danger";
                    value = 20;
                    break;
                case 11:
                    
                    type = "danger";
                    value = 20;
                    break;
                case 17:
                    
                    type = "danger";
                    value = 20;
                    break;
                case 20:
                    
                    type = "warning";
                    value = 30;
                    break;
                case 30:
                    type = "info";
                    value = 50;
                    break;
                case 40:
                    type = "info";
                    value = 60;
                    break;
                case 50:
                    type = "success";
                    value = 70;
                    break;
                case 99:
                    type = "danger";
                    value = 100;
                    break;
                default:
                    type = "info";
                    value = 0;
                    break;
            }

            div.Attributes.Add("class", "progress-bar progress-bar-"+type);
            div.Attributes.Add("role", "progressbar");
            div.Attributes.Add("aria-valuenow", value.ToString());
            div.Attributes.Add("aria-valuemin", "0");
            div.Attributes.Add("aria-valuemax", "100");
            div.Attributes.Add("style", "width: "+value+"%");
            
            return new MvcHtmlString(div.ToString());
             
        }

        public static MvcHtmlString ProgressBarComplex(this HtmlHelper helper, Bauhaus.Models.Order ord)
        {
            //Declares Divs
            TagBuilder divPro = new TagBuilder("div");
            TagBuilder divBarSuc = new TagBuilder("div");
            TagBuilder divBarDan = new TagBuilder("div");
            int suc,dan;

            // Switch Status Cond

            switch (ord.Status.Code)
            {
                case 10:
                    suc = 10;
                    dan = 10;
                    break;
                case 11:

                    suc = 10;
                    dan = 10;
                    break;
                case 17:

                    suc = 10;
                    dan = 10;
                    break;
                case 20:

                    suc = 20;
                    dan = 10;
                    break;
                case 30:

                    suc = 40;
                    dan = 10;
                    break;
                case 40:

                    suc = 50;
                    dan = 10;
                    break;
                case 50:

                    suc = 60;
                    dan = 10;
                    break;

                case 99:

                    suc = 100;
                    dan = 0;
                    break;
                default:

                    suc = 0;
                    dan = 0;
                    break;
            }



            //Format Divs

            divPro.Attributes.Add("class", "progress progress-striped active");

            divBarSuc.Attributes.Add("class", "progress-bar progress-bar-success");
            divBarSuc.Attributes.Add("style", "width: " + suc + "%");

            divBarDan.Attributes.Add("class", "progress-bar progress-bar-warning");
            divBarDan.Attributes.Add("style", "width: " + dan + "%");

            //Nesting

            divPro.InnerHtml = divBarSuc.ToString() + " " + divBarDan.ToString();
            
            return new MvcHtmlString(divPro.ToString());

        }

        public static MvcHtmlString ProgressBarBool(this HtmlHelper helper, bool state)
        {
            //Declares Divs
            TagBuilder divPro = new TagBuilder("div");
            TagBuilder divBarFill = new TagBuilder("div");
            string type;

            if (state)
                type = "success";
            else
                type = "danger";
            //Format Divs

            divPro.Attributes.Add("class", "progress progress-striped active");

            divBarFill.Attributes.Add("class", "progress-bar progress-bar-"+type);
            divBarFill.Attributes.Add("style", "width: 100%");

            //Nesting

            divPro.InnerHtml = divBarFill.ToString();

            return new MvcHtmlString(divPro.ToString());

        }

        /// <summary>
        /// Returns progressbar tag with selected state and Date if provided
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="state">1 = Success, 2 = Warning, 3 = Danger, Default = Info</param>
        /// <param name="date">Date to appear inside Bar</param>
        /// <returns>Set of Div tags that conform a progress bar</returns>
        public static MvcHtmlString ProgressBarState(this HtmlHelper helper, int state,String date=null)
        {
            //Declares Divs
            TagBuilder divPro = new TagBuilder("div");
            TagBuilder divBarFill = new TagBuilder("div");
            TagBuilder spanDate = new TagBuilder("span");

            string type = "info";

            switch (state)
            {
                case 1:
                    type = "success";
                    break;
                case 2:
                    type = "warning";
                    break;
                case 3:
                    type = "danger";
                    break;
                default:
                    type = "info";
                    break;
            }


            //Format Divs
            if (date!=null)
            {
                date.Trim();
                spanDate.InnerHtml = date;
                divBarFill.InnerHtml = spanDate.ToString();
            }
            
            divPro.Attributes.Add("class", "progress progress-striped active");

            divBarFill.Attributes.Add("class", "progress-bar progress-bar-" +type);
            divBarFill.Attributes.Add("style", "width: 100%");

            //Nesting
            if(state != 0)
                divPro.InnerHtml = divBarFill.ToString();

            return new MvcHtmlString(divPro.ToString());

        }
        
        /// <summary>
        /// Returns Descriptive status for a given reason  
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="stage">Order's Stage</param>
        /// <param name="state">Order's State</param>
        /// <param name="reason">Order's Reason</param>
        /// <returns></returns>
        public static MvcHtmlString ReasonResolver(this HtmlHelper helper, int stage, int state, int reason)
        {
            TagBuilder par = new TagBuilder("p");
            par.AddCssClass("text-center");
            string text="";

            switch (stage)
            {
                case 0:
                    if (state == 1)
                    {
                        switch (reason)
                        {
                            case 1:
                                text = "Bloqueada";
                                break;
                            case 2:
                                text = "Bloqueada por Credito";
                                break;
                            case 3:
                                text = "Bloqueada por Logistica";
                                break;
                            default:
                                text = "Unknow";
                                break;
                        }
                    }
                    else
                    {
                        text = "Ok";

                    }
                    break;
                case 1:
                    if (state == 1)
                    {
                        switch (reason)
                        {
                            case 1:
                                text = "Confirmación de Cita";
                                break;
                            case 2:
                                text = "Capacidad de Cliente";
                                break;
                            case 3:
                                text = "Pedido Pospuesto";
                                break;
                            case 4:
                                text = "Falta de Vehiculo";
                                break;
                            case 5:
                                text = "Falta de Inventario";
                                break;
                            case 6:
                                text = "Orden Mínima";
                                break;
                            case 7:
                                text = "Orden Mínima Makro";
                                break;
                            case 8:
                                text = "VFR";
                                break;
                            case 9:
                                text = "VFR Makro";
                                break;
                            case 10:
                                text = "Eliminar";
                                break;
                            case 11:
                                text = "Zsplit";
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        text = "OK";
                    }
                    break;
                case 2:
                    if (state == 1)
                    {
                        switch (reason)
                        {
                            case 1:
                                text = "Auditoria en BDC";
                                break;
                            case 2:
                                text = "Falta de paletas";
                                break;
                            case 3:
                                text = "Retraso en operación BDC";
                                break;
                            case 4:
                                text = "Atraso en recepción de rechazo";
                                break;
                            case 5:
                                text = "Problemas con SAP";
                                break;
                            case 6:
                                text = "Pedido Pospuesto";
                                break;
                            case 7:
                                text = "Disponibilidad de vehiculo";
                                break;
                            case 8:
                                text = "No show";
                                break;
                            case 9:
                                text = "Disponibilidad de Chuto";
                                break;
                            case 10:
                                text = "Vehículo Ponchado";
                                break;
                            case 11:
                                text = "Problemas con el chofer";
                                break;
                            case 12:
                                text = "Error en creacion del plan";
                                break;
                            case 13:
                                text = "Error en ruta/Redirección";
                                break;
                            case 14:
                                text = "Escolta";
                                break;
                            case 15:
                                text = "Pernocta por Adelanto de Carga";
                                break;
                            case 16:
                                text = "Pernocta por Falta de producto";
                                break;
                            case 17:
                                text = "Pernocta en BDC";
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        if (reason == 1)
                            text = "Confirmada";
                        else
                            text = "Ok";
                    }
                    break;
                case 3:
                    if (state == 1)
                    {
                        switch (reason)
                        {
                            case 1:
                                text = "Salida tarde de BDC";
                                break;
                            case 2:
                                text = "Problema en Ruta (Infraestructura)";
                                break;
                            case 3:
                                text = "Restricción de Tránsito por Feriado";
                                break;
                            case 4:
                                text = "Manifestación";
                                break;
                            case 5:
                                text = "Vehiculo accidentado";
                                break;
                            case 6:
                                text = "Re-envio Cliente sin capacidad";
                                break;
                            case 7:
                                text = "Re-envio Salida tarde del centro";
                                break;
                            case 8:
                                text = "Re-envio Cerrados por el Seniat";
                                break;
                            case 9:
                                text = "Re-envios Otros";
                                break;
                            case 10:
                                text = "Posee como destino varias sucursales";
                                break;
                            case 11:
                                text = "Viajes los fines de semana";
                                break;
                            case 12:
                                text = "Veh. cargados los sábados";
                                break;
                            case 13:
                                text = "Pernoctas Otros";
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        text = "Ok";
                    }
                    break;
                case 4:
                    if (state == 1)
                    {
                        switch (reason)
                        {
                            case 1:
                                text = "Rechazo Total";
                                break;
                            case 2:
                                text = "Rechazo Parcial";
                                break;
                            case 3:
                                text = "Rechazos Otros";
                                break;
                            case 4:
                                text = "Rechazos Enviados a Clover";
                                break;
                            case 5:
                                text = "Cliente sin capacidad de recepción";
                                break;
                            case 6:
                                text = "Falta de caleteros";
                                break;
                            case 7:
                                text = "Cliente en desacuerdo con # de caleteros";
                                break;
                            case 8:
                                text = "Cerrados por el Seniat";
                                break;
                            case 9:
                                text = "Cliente no quiere recibir";
                                break;
                            case 10:
                                text = "Vehículos de otras compañías por delante";
                                break;
                            case 11:
                                text = "Problemas logísticos del Cliente";
                                break;
                            case 12:
                                text = "Lluvia retrasa la descarga";
                                break;
                            case 13:
                                text = "Retraso en la descarga por calidad del producto";
                                break;
                            case 14:
                                text = "El cliente no quiere recibir. Negociación CBD/CSO";
                                break;
                            case 15:
                                text = "Lentitud en la descarga, mercancía a granel";
                                break;
                            case 16:
                                text = "Envío sin cita al cliente";
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        text = "Descargando";
                    }
                    break;
                case 5:
                    if (state == 1)
                    {
                        text = "Novedad";

                    }
                    else
                    {
                        text = "Ok";
                    }
                    break;
                default:
                    break;
            }

            par.SetInnerText(text);
            return new MvcHtmlString(par.ToString());
        }

        /// <summary>
        /// Return Descriptive Stage from a stage number
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="stage">Stage Number</param>
        /// <returns>String containing description</returns>
        public static MvcHtmlString StageResolver(this HtmlHelper helper,int stage)
        {
            TagBuilder par = new TagBuilder("p");
            par.AddCssClass("text-center");
            switch (stage)
            {
                case 0:
                    par.SetInnerText("SAP");
                    break;
                case 1:
                    par.SetInnerText("DSS");
                    break;
                case 2:
                    par.SetInnerText("Centro de Dist.");
                    break;
                case 3:
                    par.SetInnerText("En Transito");
                    break;
                case 4:
                    par.SetInnerText("En Cliente");
                    break;
                case 5:
                    par.SetInnerText("Entregada");
                    break;
            }
            return new MvcHtmlString(par.ToString());
        }
    }
}