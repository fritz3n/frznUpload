using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace frznUpload.Client
{
    partial class HotkeyConfigControl : GroupBox
    {
        public HotkeyConfigControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            this.Initialize();
            ResumeLayout(false);
        }


    }
}
