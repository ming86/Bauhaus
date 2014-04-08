using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bauhaus.Models
{
    public class ReportStructure
    {
        // ZVORF
        public string OrderNumber, Status, BlkInd, OrderDate, OrderType, ShipTo, CustomerPONumber, CustomerName,
            Plant, OrderQtyCS, OrderQtySU, RDDF, OrderWeight, OrderVolume,
            DeliveryNumber, DeliveryDate, DeliveryQtyCS, DeliveryQtySU, ShipmentNumber, VehicleType,
            InvoiceNumber, InvoiceDate, PODDate, CanceledorRejectedCases;

        // Clientes Activos
        public string Address, Region, City, SaleZone, Team,
               CBDRep, GU, TransportZone, PaymentTerm, PaymentTermDescription,
               ContactName, ContactTelephone, GeneralEmail, MainTelephone, MaxVehicle,
               MainOMCSR, BackupOMCSR, MainARCSR, BackupARCSR;

        // Data Monica
        public string  ReceptionContactName,
                ReceptionContactTelephone, PurchasesContactName,
                PurchasesContactTelephone, Remark, SENIATSpecialCondition,
                LeadTime, OrderingFrecuency, LoadingType, OAT, Payer,
                CSR;

        // Ordenes en Mano
        public string  Route, CarrierName,
            Staging, Priority, ShipmentStatus, DSSdays,
            Pallets, Note, Turn , NonDescStatus;

        // ZSKU
        public string ShipmentDate, CarrierNumber, OrderNetValue, OrderTotalPrice;

        // COnsolidado de Caletas
        public string CarryFee;


        public void ZVORF()
        {
            ////////////////////////////////////////
            //
            //  ZVORF
            //
            //  Assuming that worksheet Structure is:
            // SAPOrder, OrdStatus, BlkInd, DocDate, CustOrdTyp, ShipTo, CusPON, CustName,
            // Plant, OrderQtyCS, OrderQtySU, ReqDelivDateFrom, OrdNetWgh, OrdVolume,
            // Delivery, DlvCreaDte, DelvryQtyCS, DelvryQtySU, Shipment, VehicleTyp,
            // Invoice#, InvCreation, PODDate, CanceledorRejectedCases;
            //

            OrderNumber = "B"; 
            Status = "C";
            BlkInd = "E"; 
            OrderDate = "F";
            OrderType = "G"; 
            ShipTo = "H";
            CustomerPONumber = "I"; 
            CustomerName = "J";
            OrderQtyCS = "L";
            OrderQtySU = "M"; 
            RDDF = "N";
            OrderWeight = "O"; 
            OrderVolume = "P";
            DeliveryNumber = "Q"; 
            DeliveryDate = "R";
            DeliveryQtyCS = "S"; 
            DeliveryQtySU = "T";
            ShipmentNumber = "U"; 
            VehicleType = "V";
            InvoiceNumber = "W"; 
            InvoiceDate = "X";
            PODDate = "Y"; 
            CanceledorRejectedCases = "Z";
        }

        public void ZVCO()
        {

        }

        public void CustomerData()
        {
            ShipTo = "cliente"; 
            CustomerName = "nombrecomercial";
            Address = ""; 
            Region = "region";
            City = "ciudad"; 
            SaleZone = "zona";
            Team = "team"; 
            CBDRep = "cbdrep";
            GU = "gu"; 
            Route = "ruta";
            PaymentTerm = ""; 
            PaymentTermDescription = "";
            ContactName = "nombredepersonacontacto"; 
            ContactTelephone = "telefonodepersonacontacto";
            GeneralEmail = "emailgeneral"; 
            MainTelephone = "telefonoprincipal";
            MaxVehicle = "maximocamion";
            MainOMCSR = "csromtitular"; 
            BackupOMCSR = "csrombackup";
            MainARCSR = "csrartitular"; 
            BackupARCSR = "csrarbackup";
        }

        public void DataActualizada()
        {

            ShipTo = "A"; 
            CustomerName = "B";
            SaleZone = "C"; 
            Region = "D";
            City = "E"; 
            GeneralEmail = "F";
            MainTelephone = "G"; 
            ReceptionContactName = "H";
            ReceptionContactTelephone = "I"; 
            PurchasesContactName = "J";
            PurchasesContactTelephone = "K"; 
            Remark = "L";
            SENIATSpecialCondition = "M"; 
            LeadTime = "N";
            MaxVehicle = "O"; 
            OrderingFrecuency = "P";
            LoadingType = "Q"; 
            OAT = "R";
            Payer = "S";
            CSR = "T";

        }

        public void OrdernesEnMano()
        {
            OrderNumber = "saporder";
            ShipTo = "shipto";
            Route = "route";
            CarrierName = "transporte";
            LeadTime = "leadtime";
            DeliveryNumber = "delivery";
            ShipmentNumber = "shipment";
            Staging = "staging";
            Priority = "prioridad";
            ShipmentStatus="observacion";
            RDDF = "fecha";
            DeliveryQtyCS = "delvryqtycs";
            DeliveryQtySU = " delvryqtysu";
            DSSdays = "dssdays";
            Pallets = "paletas";
            CustomerName = "custname";
            Note = "nota";
            Turn = "turno";
            VehicleType = "type";
            NonDescStatus = "status";
        }

        public void ZSKU()
        {
            OrderNumber = "C";
            Status = "D";
            OrderDate = "E";
            OrderType = "F";
            ShipTo = "G";
            CustomerPONumber = "H";
            CustomerName = "I";
            OrderQtyCS = "K";
            OrderQtySU = "L";
            RDDF = "M";
            OrderWeight = "N";
            OrderVolume = "O";
            DeliveryNumber = "P";
            DeliveryDate = "Q";
            DeliveryQtyCS = "R";
            DeliveryQtySU = "S";
            ShipmentNumber = "T";
            ShipmentDate = "U";
            CarrierNumber = "V";
            InvoiceNumber = "W";
            InvoiceDate = "X";
            OrderNetValue = "Y";
            OrderTotalPrice = "Z";
            PODDate = "AA";
            PaymentTerm = "AB";
            
        }

        public void CarryFees()
        {
            CarrierNumber = "proveeedor";
            CarrierName = "nombreproveedor";
            Route = "ruta";
            VehicleType = "tipodevehiculo";
            CarryFee = "caleta"; 
        }

        internal void RoutesData()
        {
            Plant = "plant";
            Route = "route";
            LeadTime = "leadtime";
        }
    }
}