//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TesvikProgrami
{
    using System;
    using System.Collections.Generic;
    
    public partial class Sirketler
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Sirketler()
        {
            this.Isyerleri = new HashSet<Isyerleri>();
            this.Cari14857YapilanSirketler = new HashSet<Cari14857YapilanSirketler>();
        }
    
        public long SirketID { get; set; }
        public string SirketAdi { get; set; }
        public string VergiKimlikNo { get; set; }
        public long Aktif { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Isyerleri> Isyerleri { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Cari14857YapilanSirketler> Cari14857YapilanSirketler { get; set; }
    }
}