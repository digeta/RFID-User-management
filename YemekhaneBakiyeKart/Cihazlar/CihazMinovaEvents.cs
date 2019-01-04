using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YemekhaneBakiyeKart.Cihazlar
{
    public delegate void SeriPortVeriGeldi(object source, SeriPortVeriGeldiArgs e);

    public class SeriPortVeriGeldiArgs : EventArgs
    {
        private CihazVeri _cihazVerisi;

        public SeriPortVeriGeldiArgs(CihazVeri cihazVerisi)
        {
            _cihazVerisi = cihazVerisi;
        }
        public CihazVeri CihazVerisi
        {
            get
            {
                return _cihazVerisi;
            }
        }
    }
}
