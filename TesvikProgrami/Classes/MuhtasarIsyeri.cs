using System;
using System.Collections.Generic;
using System.Data;
using System.Xml.Linq;

namespace TesvikProgrami.Classes
{
    public class MuhtasarIsyeri
    {
        public Isyerleri Isyeri { get; set; }
        public List<AphbSatir> kisiler { get; set; } = new List<AphbSatir>();
        public DataTable CariAphb { get; set; }
        public Dictionary<string, XDocument> xmller { get; set; } = new Dictionary<string, XDocument>();
        public Dictionary<string, List<string[]>> netsisBildirgeler { get; set; } = new Dictionary<string, List<string[]>>();
        public Dictionary<string, List<string[]>> netsisBildirgelerExcel { get; set; } = new Dictionary<string, List<string[]>>();
        public Dictionary<DataRow, XElement> SatirReferanslari { get; set; } = new Dictionary<DataRow, XElement>();
        public Dictionary<DataRow, NetsisSatir> SatirReferanslariNetsis { get; set; } = new Dictionary<DataRow, NetsisSatir>();
        public Dictionary<DataRow, NetsisSatir> SatirReferanslariNetsisExcel { get; set; } = new Dictionary<DataRow, NetsisSatir>();
        public Dictionary<DataRow, int> SatirReferanslariNetsisKanunSira { get; set; } = new Dictionary<DataRow, int>();
        public string Aphb { get; set; }
        public string BasvuruFormu { get; set; }
        public bool AphbGuncel { get; set; }
        public bool BfGuncel { get; set; }
        public int Yil { get; set; }
        public int Ay { get; set; }
        public List<string> hataliKisiler { get; set; }
    }
}
