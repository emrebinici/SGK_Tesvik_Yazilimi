using System.Collections.Generic;

namespace TesvikProgrami.Classes
{
    public class BildirgeIcmaliResponse : BaseResponse
    {
        public Dictionary<string, List<BildirgeYuklemeIcmal>> Onaylilar { get; set; } = new Dictionary<string, List<BildirgeYuklemeIcmal>>();
        public Dictionary<string, List<BildirgeYuklemeIcmal>> Onaysizlar { get; set; } = new Dictionary<string, List<BildirgeYuklemeIcmal>>();
        public Dictionary<string, List<BildirgeYuklemeIcmal>> Tumu { get; set; } = new Dictionary<string, List<BildirgeYuklemeIcmal>>();
        public Dictionary<Bildirge, string> HataliBildirgeler { get; set; } = new Dictionary<Bildirge, string>();
    }

}
