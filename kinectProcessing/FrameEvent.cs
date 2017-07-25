using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace KINECTmania.kinectProcessing
{
    public class BitmapGenerated : EventArgs
    {
        private WriteableBitmap wb;
        public WriteableBitmap Bitmap
        {
            get { return this.wb; }
            set { wb = value; }
        }

        public BitmapGenerated(WriteableBitmap wb)
        {
            this.wb = wb;
        }


    }

    public interface BitmapGenerator
    {
        event EventHandler<BitmapGenerated> RaiseBitmapGenerated;

        void OnRaiseBitmapGenerated(BitmapGenerated b);

    }
}
