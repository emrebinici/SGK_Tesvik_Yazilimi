using System.Collections.Generic;

namespace TesvikProgrami.Classes
{
    public class MuhtasarOnaydaBekleyenlerResponse : BaseResponse
    {
        public Dictionary<Bildirge, string> HataliBildirgeler { get; set; } = new Dictionary<Bildirge, string>();
    }

}
