using Fiddler;
using System.Windows.Forms;

[assembly: RequiredVersion("2.3.5.0")]

namespace mtanksl.ActionMessageFormat.FiddlerViewer
{
    public class AmfRequestInspector : Inspector2, IRequestInspector2
    {
        private AmfViewer control;

        private bool visible;

        public AmfRequestInspector()
        {
            control = new AmfViewer();
        }

        private HTTPRequestHeaders _headers;

        public HTTPRequestHeaders headers
        {
            get
            {
                return _headers;
            }
            set
            {
                _headers = value;

                visible = _headers.ExistsAndEquals("content-type", "application/x-amf");
            }
        }

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

                if (visible)
                {
                    control.Body = _body;
                }
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
            control.Dispose();
        }
    }

    public class AmfResponseInspector : Inspector2, IResponseInspector2
    {
        private AmfViewer control;

        private bool visible;

        public AmfResponseInspector()
        {
            control = new AmfViewer();
        }

        private HTTPResponseHeaders _headers;

        public HTTPResponseHeaders headers
        {
            get
            {
                return _headers;
            }
            set
            {
                _headers = value;

                visible = _headers.ExistsAndEquals("content-type", "application/x-amf;charset=UTF-8");
            }
        }

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

                if (visible)
                {
                    control.Body = _body;
                }
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
            control.Dispose();
        }
    }
}