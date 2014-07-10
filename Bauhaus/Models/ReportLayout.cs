using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Bauhaus.Helpers;

namespace Bauhaus.Models
{
    public class ReportLayout
    {

        /// <summary>
        /// Checks report mapping dictionary for missing columns. Returns true if report passed test.
        /// </summary>
        /// <param name="map">Dictionary with all report columns mapped</param>
        /// <returns>returns true if report passed test, false otherwise</returns>
        public String SelfCheck(Dictionary<String, String> map)
        {
            Type type = this.GetType();
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            String helper, result = "";
            foreach (PropertyInfo property in properties)
            {

                if (!map.TryGetValue(property.GetValue(this, null).ToString(), out helper))
                {
                    System.Diagnostics.Debug.WriteLine("Dictionary doesn't contain requested key: " + property.Name);
                    result += property.GetValue(this, null).ToString() + " ";
                }
            }

            return result;

        }
    }

    public class Customers : ReportLayout
    {
        public String ShipTo { get; set; }
        public String CustomerName { get; set; }
        public String Address { get; set; }
        public String Region { get; set; }
        public String City { get; set; }
        public String SaleZone { get; set; }
        public String Team { get; set; }
        public String CBDRep { get; set; }
        public String CBDRepTel { get; set; }
        public String GU { get; set; }
        public String GUTel { get; set; }
        public String Route { get; set; }
        //public String PaymentTerm { get; set; }
        //public String PaymentTermDescription { get; set; }
        public String ContactName { get; set; }
        public String ContactTelephone { get; set; }
        public String GeneralEmail { get; set; }
        public String MainTelephone { get; set; }
        public String MaxVehicle { get; set; }
        public String MainOMCSR { get; set; }
        public String BackupOMCSR { get; set; }
        public String MainARCSR { get; set; }
        public String BackupARCSR { get; set; }

        public Customers()
        {
            ShipTo = "cliente";
            CustomerName = "nombrecomercial";
            Address = "direccion";
            Region = "region";
            City = "ciudad";
            SaleZone = "zona";
            Team = "team";
            CBDRep = "cbdrep";
            CBDRepTel = "tlfcbdr";
            GU = "gu";
            GUTel = "tlfgu";
            Route = "ruta";
            //PaymentTerm = "";
            //PaymentTermDescription = "";
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
    }
    
    public class OEM : ReportLayout
    {
        public String OrderNumber { get; set; }
        public String ShipTo { get; set; }
        public String Route { get; set; }
        public String CarrierName { get; set; }
        public String ShipmentNumber { get; set; }
        public String ShipmentStatus { get; set; }
        public String CustomerName { get; set; }
        //public String Turn { get; set; }
        public String VehicleType { get; set; }

        public OEM(bool pendiente)
        {
            OrderNumber = "saporder";
            ShipTo = "shipto";
            Route = "route";
            CarrierName = "transporte";
            ShipmentNumber = "shipment";
            CustomerName = "custname";
            //Turn = "turno";
            VehicleType = "type";
            if(pendiente)
                ShipmentStatus = "observacion";
            else
                ShipmentStatus = "status";
        }
    }

    public class CarrierCodes: ReportLayout
    {
        public String CarrierNumber { get; set; }
        public String CarrierName { get; set; }

        public CarrierCodes()
        {
            CarrierNumber = "carrier";
            CarrierName = "carriername";
        }
    }

    public class CarryFees: CarrierCodes
    {
        public String Route { get; set; }
        public String VehicleType { get; set; }
        public String Fee { get; set; }

        public CarryFees():base()
        {
            Route = "route";
            VehicleType = "vehtype";
            Fee = "fee";
        }
    }

    public class Comments: ReportLayout
    {
        public String Customer { get; set; }
        public String Comment { get; set; }

        public Comments()
        {
            Customer = "customer";
            Comment = "comment";
        }
    }

    public class Contacts: ReportLayout
    {
        public String Customer { get; set; }
        public String Area { get; set; }
        public String Name { get; set; }
        public String Telephone { get; set; }
        public String Email { get; set; }

        public Contacts()
        {
            Customer = "customer";
            Area = "area";
            Name = "name";
            Telephone = "tel";
            Email = "email";
        }
    }

     
}