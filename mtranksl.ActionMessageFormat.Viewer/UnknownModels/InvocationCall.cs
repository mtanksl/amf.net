using mtanksl.ActionMessageFormat;

namespace mtranksl.ActionMessageFormat.Viewer.UnknownModels
{
    [TraitClass("org.granite.tide.invocation.InvocationCall")]
    public class InvocationCall
    {
        [TraitMember("listeners")] 
        public object Listeners { get; set; }

        [TraitMember("updates")]
        public object Updates { get; set; }

        [TraitMember("results")]
        public object Results { get; set; }
    }
}