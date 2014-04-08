using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bauhaus.Models;

namespace Bauhaus.Helpers
{
    public class CodeHelpers
    {
        /// <summary>
        /// Provides Descriptive status for Stage
        /// </summary>
        /// <param name="stage">Stage to be translated</param>
        /// <returns>String with Description</returns>
        public string StageResolver(int stage)
        {
            string message;

            switch (stage)
            {
                case 0:
                    message =  "SAP";
                    break;
                case 1:
                    message =  "DSS";
                    break;
                case 2:
                    message =  "Centro de Dist.";
                    break;
                case 3:
                    message =  "En Transito";
                    break;
                case 4:
                    message =  "En Cliente";
                    break;
                case 5:
                    message =  "Entregada";
                    break;
                default:
                    message =  "Unknown";
                    break;
            }
            return message;
        }


        /// <summary>
        /// Provides Descriptive Status for Reason
        /// </summary>
        /// <param name="stage">Current Stage</param>
        /// <param name="state">Current State</param>
        /// <param name="reason">Current Reason</param>
        /// <returns>Reason Description</returns>
        public string ReasonResolver(int stage, int state, int reason)
        {
            string text = "";
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
                                text ="Unknow";
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
                        if (reason == 0)
                            text = "OK";
                        else
                            text = "Confirmada";
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
                        text = "Confirmada";
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

            return text;
        }

        /// <summary>
        /// Calculates KPIs on Orders with POD
        /// </summary>
        /// <param name="order">Order to be Calculated</param>
        /// <returns>Return 0 if Normal Flow. 1 if Error encountered.</returns>
        public int CalculateIndicators(Order order)
        {
            // Order Needs POD for indicators to be calculated
            if (order.POD == null)
                return 1;
            else
            {
                if (order.POD.CSOT == null)
                {
                    order.POD.CSOT = new Indicator();
                    if (order.POD.Date <= order.RDDF.Original)   // Inside Time Window.
                    {
                        order.POD.CSOT.Value = true; // HIT
                        order.POD.CSOT.Confirmed = true;
                    }
                    else
                    {
                        order.POD.CSOT.Value = false; // MISS
                    }
                }

                if (order.POD.OT == null)
                {
                    order.POD.OT = new Indicator();
                    if (order.POD.Date <= order.RDDF.DSSDate || // Inside Time Window
                        order.Quantities.QtyCS < 200)
                    {
                        order.POD.OT.Value = true;
                        order.POD.OT.Confirmed = true;
                    }
                    else
                    {
                        order.POD.OT.Value = false;
                    }

                }

                return 0;
            }
        }

    }

    /// <summary>
    /// Datetime Extensions to handle business days
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Add Business Days to Datetime
        /// </summary>
        /// <param name="date">Base Date</param>
        /// <param name="days">Days to be added</param>
        /// <returns></returns>
        public static DateTime AddBusinessDays(this DateTime date, int days)
        {
            int direction = days < 0 ? -1 : 1;

            while (days != 0)
            {
                date = date.AddDays(direction);
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    days -= direction;
                }
            }

            return date;
        }
    }
}

