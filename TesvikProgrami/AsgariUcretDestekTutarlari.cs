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

    public partial class AsgariUcretDestekTutarlari
    {
        public long ID { get; set; }
        public long IsyeriID { get; set; }
        public long DonemYil { get; set; }
        public long DonemAy { get; set; }
        public long HesaplananGun { get; set; }
    
        public virtual Isyerleri Isyerleri { get; set; }
    }
}
