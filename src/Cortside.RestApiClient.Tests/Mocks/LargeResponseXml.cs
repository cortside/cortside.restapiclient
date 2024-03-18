using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Cortside.RestApiClient.Tests.Mocks {
    // using System.Xml.Serialization;
    // XmlSerializer serializer = new XmlSerializer(typeof(Associates));
    // using (StringReader reader = new StringReader(xml))
    // {
    //    var test = (Associates)serializer.Deserialize(reader);
    // }

    [XmlRoot(ElementName = "company")]
    public class Company {

        [XmlAttribute(AttributeName = "code")]
        public string Code { get; set; }
    }

    [XmlRoot(ElementName = "division")]
    public class Division {

        [XmlAttribute(AttributeName = "code")]
        public string Code { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
    }

    [XmlRoot(ElementName = "group")]
    public class Group {

        [XmlAttribute(AttributeName = "code")]
        public string Code { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
    }

    [XmlRoot(ElementName = "department")]
    public class Department {

        [XmlAttribute(AttributeName = "code")]
        public string Code { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
    }

    [XmlRoot(ElementName = "costCenter")]
    public class CostCenter {

        [XmlAttribute(AttributeName = "code")]
        public string Code { get; set; }
    }

    [XmlRoot(ElementName = "job")]
    public class Job {

        [XmlAttribute(AttributeName = "code")]
        public string Code { get; set; }

        [XmlAttribute(AttributeName = "title")]
        public string Title { get; set; }
    }

    [XmlRoot(ElementName = "functional")]
    public class Functional {

        [XmlAttribute(AttributeName = "title")]
        public string Title { get; set; }
    }

    [XmlRoot(ElementName = "hierarchy")]
    public class Hierarchy {

        [XmlElement(ElementName = "company")]
        public Company Company { get; set; }

        [XmlElement(ElementName = "division")]
        public Division Division { get; set; }

        [XmlElement(ElementName = "group")]
        public Group Group { get; set; }

        [XmlElement(ElementName = "department")]
        public Department Department { get; set; }

        [XmlElement(ElementName = "costCenter")]
        public CostCenter CostCenter { get; set; }

        [XmlElement(ElementName = "job")]
        public Job Job { get; set; }

        [XmlElement(ElementName = "functional")]
        public Functional Functional { get; set; }
    }

    [XmlRoot(ElementName = "manager")]
    public class Manager {

        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "firstName")]
        public string FirstName { get; set; }

        [XmlAttribute(AttributeName = "lastName")]
        public string LastName { get; set; }
    }

    [XmlRoot(ElementName = "limit")]
    public class Limit {

        [XmlAttribute(AttributeName = "amount")]
        public string Amount { get; set; }

        [XmlAttribute(AttributeName = "calculation")]
        public string Calculation { get; set; }

        [XmlAttribute(AttributeName = "isDerived")]
        public bool IsDerived { get; set; }

        [XmlAttribute(AttributeName = "source")]
        public string Source { get; set; }

        [XmlAttribute(AttributeName = "category")]
        public string Category { get; set; }

        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }

        [XmlAttribute(AttributeName = "product")]
        public string Product { get; set; }

        [XmlAttribute(AttributeName = "system")]
        public string System { get; set; }
    }

    [XmlRoot(ElementName = "limits")]
    public class Limits {

        [XmlElement(ElementName = "limit")]
        public Limit Limit { get; set; }
    }

    [XmlRoot(ElementName = "associate")]
    public class Associate {

        [XmlElement(ElementName = "hierarchy")]
        public Hierarchy Hierarchy { get; set; }

        [XmlElement(ElementName = "manager")]
        public Manager Manager { get; set; }

        [XmlElement(ElementName = "limits")]
        public Limits Limits { get; set; }

        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "firstName")]
        public string FirstName { get; set; }

        [XmlAttribute(AttributeName = "lastName")]
        public string LastName { get; set; }

        [XmlAttribute(AttributeName = "exeCode")]
        public string ExeCode { get; set; }

        [XmlAttribute(AttributeName = "status")]
        public string Status { get; set; }

        [XmlAttribute(AttributeName = "email")]
        public string Email { get; set; }
    }

    [Serializable]
    [XmlRoot(ElementName = "associates")]
    public class Associates {
        [XmlElement(ElementName = "associate")]
        public List<Associate> Associate { get; set; }

        [XmlAttribute(AttributeName = "exportDate")]
        public DateTime ExportDate { get; set; }
    }
}
