using Fiddler;
using System.Windows.Forms;

[assembly: RequiredVersion("2.3.5.0")]

namespace mtanksl.ActionMessageFormat.FiddlerViewer
{
    public class AmfRequestInspector : Inspector2, IRequestInspector2
    {
        private AmfViewer control;

        public AmfRequestInspector()
        {
            control = new AmfViewer();
        }

        public HTTPRequestHeaders headers { get; set; }

        private byte[] _body;

        public byte[] body
        {
            get
            {
                return _body;
            }
            set
            {
                _body = value;

                control.Body = _body;
            }
        }

        public bool bDirty { get; }

        public bool bReadOnly { get; set; }

        public override void AddToTab(TabPage o)
        {
            o.Text = "AMF";

            control.Dock = DockStyle.Fill;

            o.Controls.Add(control);
        }

        public override int GetOrder()
        {
            return 120;
        }

        public void Clear()
        {
            
        }
    }

    public class AmfResponseInspector : Inspector2, IResponseInspector2
    {
        private AmfViewer control;

        public AmfResponseInspector()
        {
            control = new AmfViewer();
        }

        public HTTPResponseHeaders headers { get; set; }

        private byte[] _body;

        public byte[] body
        {
            get
            {
                return _body;
            }
            set
            {
                _body = value;

                control.Body = _body;
            }
        }

        public bool bDirty { get; }

        public bool bReadOnly { get; set; }

        public override void AddToTab(TabPage o)
        {
            o.Text = "AMF";

            control.Dock = DockStyle.Fill;

            o.Controls.Add(control);
        }

        public override int GetOrder()
        {
            return 120;
        }

        public void Clear()
        {

        }
    }
}