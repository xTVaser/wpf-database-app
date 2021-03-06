﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateApp {

    public class EmployeeItem {

        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Email { get; set; }
        public String Occupation { get; set; }
        public String Username { get; set; }
        public int OfficeID { get; set; }

        public Decimal Salary { get; set; }
        public String PhoneNumber { get; set; }
        public float Commission { get; set; }
        public float? BrokerShare { get; set; }

        public String securityQuestion { get; set; }
        public String securityAnswer { get; set; }

        public String Type { get; set; }

        public EmployeeItem(String employeeType) {

            if (employeeType.Equals("A"))
                Occupation = "Sales Agent";
            else if (employeeType.Equals("S"))
                Occupation = "Administrative Staff";
            else
                Occupation = "Broker";

            this.Type = employeeType;
        }
    }
}
